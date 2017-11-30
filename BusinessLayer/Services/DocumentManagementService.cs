using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using Logger;
using DataAccessLayer.Interfaces;
using AutoMapper;
using BusinessLayer.Models;
using System.Web;
using BusinessLayer.Models.Response;

namespace BusinessLayer.Services
{
    public class DocumentManagementService : IDocumentManagementService
    {
        IDocumentManagementDataService _service;
        ILogger _logger;
        IDictionaryService _dictionary;
        Dictionary<string, object> _dataObjects;
        Dictionary<string, object> _documentTypeAttributes;

        public DocumentManagementService(IDocumentManagementDataService service, ILogger logger, IDictionaryService dictionary)
        {
            _service = service;
            _logger = logger;
            _dictionary = dictionary;
        }

        public async Task<bool> ValidateFileParameters(string documentType, HttpPostedFileBase file, List<UploadResult> result)
        {
            bool res = true;
            int? _maxSizeMb = await GetAllowedDocumentMaxSizeAsync(documentType);
            int? _maxSize = _maxSizeMb * 1024 * 1024 ?? (4 * 1024 * 1024);

            List<string> allowedMimes = await GetAllowedMimeTypesAsync(documentType);

            if (file.ContentLength > _maxSize)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Размер файла ");
                error.Append(file.FileName);
                error.Append(" не должен превышать ");
                error.Append(_maxSizeMb);
                error.Append(" Мб");

                result.Add(new UploadResult() { Error = error.ToString(), FileName = file.FileName });
                res = false;
            }


            if (!allowedMimes.Contains(file.ContentType))
            {
                StringBuilder error = new StringBuilder();
                error.Append("Недопустимый формат '");
                error.Append(file.ContentType);
                error.Append("' файла  ");
                error.Append(file.FileName);

                result.Add(new UploadResult() { Error = error.ToString(), FileName = file.FileName });

                res = false;
            }

            return res;
        }

        public async Task<bool> ValidateParametersAsync(string documentType, List<Models.AdditionalParametersRequest> parameters)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                throw new ArgumentNullException($"documentType is null");
            }
            if (parameters == null)
            {
                throw new ArgumentNullException($"Argument parameters is null");
            }
            var attr = await GetDocumentMetadataAsync(documentType);
            if (attr == null)
            {
                throw new ArgumentNullException($"GetDocumentMetadataAsync({documentType}) returned null");
            }

            foreach (var param in attr.DataSourceObjects?.Select(x => x.RequiredParameters))
            {

                foreach (var d in param?.Select(y => y.InputParameterName))
                {
                    if (!parameters.Any(x => x != null && x.Name == d && !string.IsNullOrWhiteSpace(x.Value)))
                    {
                        return false;
                    }

                }
            }
            foreach (var param in attr.Attributes.Where(x => x.DataSourceObjectName == "additionalParameters")?.Select(x => x.DataSourceProperty))
            {
                if (!parameters.Any(x => x != null && x.Name == param && !string.IsNullOrWhiteSpace(x.Value)))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task PostDocument(string documentType, HttpPostedFileBase file, List<AdditionalParametersRequest> requestParameters)
        {
            var requiredAttributesList = await getRequiredDocumentAttributeListAsync(documentType);
            var dtoParameters = Mapper.Map<List<AdditionalParameters>>(requestParameters);
            var docType = await GetDocumentMetadataAsync(documentType);

            dtoParameters.FirstOrDefault(x => x.Name == "FileName").Value = file.FileName;

            prepareDataSourceObjects(documentType, docType, dtoParameters);

            var requiredAttributes = getDocumentRequestParameters(docType, requiredAttributesList, dtoParameters);

            if (requiredAttributes == null)
                throw new NullReferenceException("requiredAttributes is empty");

            System.IO.BinaryReader reader = new System.IO.BinaryReader(file.InputStream);
            byte[] fileByteArray = reader.ReadBytes(((int)file.InputStream.Length));

            var res = await _service.registerDocumentAsync(requiredAttributes.ToArray(), fileByteArray, documentType);
        }

        public async Task<List<AttachmentResponse>> FindDocuments(string documentType, List<Models.AdditionalParametersRequest> filterConditions)
        {
            var res = new List<AttachmentResponse>();
            DataAccessLayer.DocumentManagement2SoapService.AttributeTypeWithAnyAttribute[] documentTypeAttributes = await getDocumentTypeAttributesAsync(documentType);



            DocumentTypeAttributeItem docMetadata = await GetDocumentMetadataAsync(documentType);
            var searchParams = docMetadata.SearchAttributes?.Select(x => x.DataSourceProperty); 

            var filter = getFilters(filterConditions.Where(x => searchParams.Contains(x.Name)).ToList(), docMetadata.SearchAttributes.ToList());

            DataAccessLayer.DocumentManagement2SoapService.document[] docs = await _service.FindDocument(filter);
            
            docs.ToList().ForEach(doc => 
            {
                var attach = new AttachmentResponse();

                docMetadata.DisplayParameters.ToList().ForEach(displayParameter =>
                {
                    var destAttribute = documentTypeAttributes.FirstOrDefault(x => x.name == displayParameter.AttributeName);
                    var dataType = displayParameter.Type??destAttribute?.AnyAttr.First(z => z.Name == "propertyType")?.Value;
                    attach.AttachmentsAttributes.Add(
                       new AttachmentsAttribute() {
                           Name = displayParameter.AttributeName,
                           Value = ChangePropertyValueFormat(doc.AnyAttr.First(z => z.Name == displayParameter.AttributeName).Value, dataType, displayParameter.Format),
                           DisplayName = displayParameter.DisplayName,
                           DisplayOrder = displayParameter.DisplayOrder
                       }
                    );                    
                });
                res.Add(attach); 
            });

            return res;
        }

        private DataAccessLayer.DocumentManagement2SoapService.criteriaAttribute[] getFilters(List<AdditionalParametersRequest> filterConditions, List<SearchAttribute> searchAttr)
        {
            var res = new List<DataAccessLayer.DocumentManagement2SoapService.criteriaAttribute>();

            filterConditions.ForEach(x =>
            {
                res.Add(new DataAccessLayer.DocumentManagement2SoapService.criteriaAttribute()
                {
                    name = searchAttr.FirstOrDefault(y=>y.DataSourceProperty == x.Name).AttributeName,
                    value = x.Value
                });
            });            

            return res.ToArray();
        }

        public async Task<FileResponse> getDocument(string documentId)
        {
            try
            {
                var doc = await _service.getDocumentAsync(documentId);

                if (doc == null)
                    throw new Exception($"Document not found");

                var res = new FileResponse();

                res.FileByteArray = doc.documentBase64;
                res.FileName = doc.fileName;
                res.MimeType = doc.mimeType;

                return res;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "getDisplayParameters");
            }

            return null;
        }

        public async Task<List<DisplayParameter>> getDisplayParameters(string documentType)
        {
            try
            {
                var doc = await GetDocumentMetadataAsync(documentType);

                return doc.DisplayParameters.ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "getDisplayParameters");
            }

            return null;
        }

        public async Task<string> deleteDocumentAsync(string documentId)
        {
            try
            {
                var res = await _service.deleteDocumentAsync(documentId);

                return res;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "deleteDocumentAsync");
            }

            return null;
        }

        private async Task<DataAccessLayer.DocumentManagement2SoapService.AttributeTypeWithAnyAttribute[]> getDocumentTypeAttributesAsync(string documentType)
        {
            try
            {
                var documentTypeList = await _service.getDocumentTypeInfoAsync(documentType);

                if (documentTypeList == null)
                {
                    StringBuilder errorMessage = new StringBuilder();
                    errorMessage.Append("Document type ").Append(documentType).Append(" is not found");

                    throw new KeyNotFoundException(errorMessage.ToString());
                }

                var attributeList = documentTypeList.First().attributes;

                return attributeList;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "getDocumentAttributes");
            }

            return null;
        }

        private async Task<List<DataAccessLayer.DocumentManagement2SoapService.AttributeTypeWithAnyAttribute>> getRequiredDocumentAttributeListAsync(string documentType)
        {
            var attributeList = await getDocumentTypeAttributesAsync(documentType);//?.Where(c => c.isRequired == true));
            return attributeList.ToList();
        }
       
        private void prepareDataSourceObjects(string documentType, DocumentTypeAttributeItem docType, List<Models.AdditionalParameters> additionalParameters)
        {
            try
            {
                foreach (var source in docType.DataSourceObjects)
                {
                    if (_dataObjects == null)
                        _dataObjects = new Dictionary<string, object>();

                    if (!_dataObjects.ContainsKey(source.Name))
                    {
                        var myObj = Type.GetType("BusinessLayer.Models.DataSource." + source.Name);
                        var handle = Activator.CreateInstance(myObj);

                        foreach (var param in source.RequiredParameters)
                        {
                            if (!additionalParameters.Exists(x => x.Name == param.InputParameterName))
                                throw new MissingFieldException(string.Format("Input additional property doesn't contains item with name {0}", param.InputParameterName));

                            if (handle.GetType().GetProperty(param.PropertyName) != null)
                            {
                                handle.GetType().GetProperty(param.PropertyName).SetValue(handle, additionalParameters.First(x => x.Name == param.InputParameterName).Value, null);
                            }
                            else
                                throw new MissingFieldException(string.Format("Data source object doesn't has property {0}", param.PropertyName));
                        }

                        _dataObjects.Add(source.Name, handle);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "prepareDataSourceObjects");
            }
        }

        private List<DataAccessLayer.DocumentManagement2SoapService.AttributeTypeAttribute> getDocumentRequestParameters(DocumentTypeAttributeItem docType, List<DataAccessLayer.DocumentManagement2SoapService.AttributeTypeWithAnyAttribute> attributes, List<Models.AdditionalParameters> additionalParameters)
        {
            List<DataAccessLayer.DocumentManagement2SoapService.AttributeTypeAttribute> _requiredAttributes = new List<DataAccessLayer.DocumentManagement2SoapService.AttributeTypeAttribute>();

            try
            {
                foreach (var attribute in docType.Attributes)
                {
                    var destAttribute = attributes.FirstOrDefault(x => x.name == attribute.AttributeName);
                    var attr = new DataAccessLayer.DocumentManagement2SoapService.AttributeTypeAttribute();
                    attr.name = attribute.AttributeName;
                    attr.identificationGroup = attribute.IdentificationGroup;
                    attr.isRequired = destAttribute.isRequired;
                    
                    var dataType = destAttribute?.AnyAttr.First(z => z.Name == "propertyType")?.Value;

                    switch (attribute.DataSourceObjectName)
                    {
                        case "additionalParameters":
                            var propertyValue = additionalParameters.First(x => x.Name == attribute.DataSourceProperty)?.Value;
                            attr.Value = ChangePropertyValueFormat(propertyValue, dataType, attribute.Format);
                            break;
                        case "static":
                            attr.Value = attribute.DataSourceValue;
                            break;
                        default:
                            var dataObject = _dataObjects[attribute.DataSourceObjectName];
                            var attributeValue = dataObject.GetType().GetProperty(attribute.DataSourceProperty).GetValue(dataObject);
                            attr.Value = ChangePropertyValueFormat(attributeValue, dataType, attribute.Format);
                            break;

                    }

                    _requiredAttributes.Add(attr);
                }

                return _requiredAttributes;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "getDocumentAttributes");
            }

            return null;
        }

        private string ChangePropertyValueFormat(object propertyValue, string dataType, string format)
        {
            string res;

            if (propertyValue == null)
                return null;

            switch (dataType)
            {
                case "datetime":
                    DateTime date;
                    //res = DateTime.TryParse(propertyValue.ToString(), out date) ? date.ToString("yyyy-MM-dd") : propertyValue.ToString();
                    res = DateTime.TryParse(propertyValue.ToString(), out date) ? date.ToString(format) : propertyValue.ToString();
                    break;
                case "boolean":
                    bool boolVal;
                    res = bool.TryParse(propertyValue.ToString(), out boolVal) ? boolVal.ToString() : propertyValue.ToString();
                    break;
                default:
                    res = propertyValue.ToString();
                    break;
            }

            return res;
        }

        private async Task<int?> GetAllowedDocumentMaxSizeAsync(string documentType)
        {
            var attr = await GetDocumentMetadataAsync(documentType);

            return attr.AllowedFileMaxSize;
        }

        private async Task<List<string>> GetAllowedMimeTypesAsync(string documentType)
        {
            var attr = await GetDocumentMetadataAsync(documentType);

            return attr.AllowedMimeTypes;
        }

        private async Task<Models.DocumentTypeAttributeItem> GetDocumentMetadataAsync(string documentType)
        {
            Models.DocumentTypeAttributeItem _attr = new Models.DocumentTypeAttributeItem();

            if (_documentTypeAttributes == null)
            {
                _documentTypeAttributes = new Dictionary<string, object>();
            }

            if (_documentTypeAttributes.ContainsKey(documentType))
            {
                object obj = new object();
                _documentTypeAttributes.TryGetValue(documentType, out obj);
                _attr = (Models.DocumentTypeAttributeItem)obj;
            }
            else
            {
                _attr = await _dictionary.getDocumentTypeAttributesAsync(documentType);

                if (_attr == null)
                {
                    StringBuilder errorMessage = new StringBuilder();
                    errorMessage.Append("Document type attribute info for ").Append(documentType).Append(" is not found");

                    throw new KeyNotFoundException(errorMessage.ToString());
                }
                else
                {
                    _documentTypeAttributes.Add(documentType, _attr);
                }
            }

            return _attr;
        }


    }
}
