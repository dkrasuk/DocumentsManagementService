using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using System;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer.Services
{
    public class DocumentManagementDataService : IDocumentManagementDataService
    {
        DocumentManagement2SoapService.DocumentManagement2PortClient _service;

        public DocumentManagementDataService()
        {
            _service = new DocumentManagement2SoapService.DocumentManagement2PortClient();
            
            _service.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["DocumentManagementServiceClientLogin"];
            _service.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["DocumentManagementServiceClientPassword"];

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
        }

        public async Task<DocumentManagement2SoapService.DocumentType[]> getDocumentTypeInfoAsync(string documentTypeCode)
        {
            var request = new DocumentManagement2SoapService.getDocumentTypesRequest();
            request.documentType = documentTypeCode;

            var res = await _service.getDocumentTypesAsync(request);
            return res.getDocumentTypesResponse1;
        }

        public async Task<string> registerDocumentAsync(DocumentManagement2SoapService.AttributeTypeAttribute[] attributes, byte[] documentBase, string documentType)
        {
            var request = new DocumentManagement2SoapService.registerDocumentRequest();
            request.attributes = attributes;
            request.documentBase64 = documentBase;
            request.documentType = documentType;
            request.emptyContent = false;
            request.emptyContentSpecified = true;
            DocumentManagement2SoapService.registerDocumentResponse1 res;
            try
            {
                res = await _service.registerDocumentAsync(request);
            }
            catch (Exception ex)
            {

                throw ex;
            }         

            return res.registerDocumentResponse.documentId;
        }

        public async Task<DocumentManagement2SoapService.getDocumentListResponse> GetDocumentsListAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException($"GetDocumentsListAsync(string id is null) ");
            }
            DocumentManagement2SoapService.getDocumentListResponse1 res;
            var request = new DocumentManagement2SoapService.getDocumentListRequest() {
                documentId = id
            };
            try
            {
                res = await _service.getDocumentListAsync(request);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return res.getDocumentListResponse;

        }

        public async Task<DocumentManagement2SoapService.getDocumentListResponse> GetDocumentsListAsync(DocumentManagement2SoapService.AttributeTypeAttribute[] attributes, string documentType,string documentVersion=nameof(DocumentVersion.all))
        {
            if (string.IsNullOrWhiteSpace(documentType)|| string.IsNullOrWhiteSpace(documentVersion))
            {
                throw new ArgumentNullException($"GetDocumentsListAsync(documentType/documentVersion is null) ");
            }
            DocumentManagement2SoapService.getDocumentListResponse1 res;
            var request = new DocumentManagement2SoapService.getDocumentListRequest()
            {
                attributes = attributes,
                documentType= documentType,
                documentVersion= documentVersion

            };

            res = await _service.getDocumentListAsync(request);
            return res.getDocumentListResponse;
        }

        public async Task<DocumentManagement2SoapService.document[]> FindDocument(DocumentManagement2SoapService.criteriaAttribute[] filters)
        {
            if (filters.Length == 0)
            {
                throw new ArgumentNullException("Filters parameter is empty");
            }

            var request = new DocumentManagement2SoapService.findDocumentsRequest()
            {
                searchAllVersions = true,
                criteria = filters,
                searchAllVersionsSpecified = true
            };
            try
            {
                var response = await _service.findDocumentsAsync(request);
                return response.findDocumentsResponse.documents;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        public async Task<DocumentManagement2SoapService.getDocumentResponse> getDocumentAsync(string documentUniqueId)
        {

            var request = new DocumentManagement2SoapService.getDocumentRequest()
            {
                documentId = documentUniqueId,
                returnLikeAttachment = false,
                returnLikeAttachmentSpecified = false
            };

            var res = await _service.getDocumentAsync(request);

            return res.getDocumentResponse;
        }

        public async Task<string> deleteDocumentAsync(string documentUniqueId)
        {
            var request = new DocumentManagement2SoapService.deleteObjectRequest()
            {
                allVersions = false,
                allVersionsSpecified = true,
                objectId = documentUniqueId
            };

            var res = await _service.deleteObjectAsync(request);

            return res.deleteObjectResponse1;
        }
    }
}
