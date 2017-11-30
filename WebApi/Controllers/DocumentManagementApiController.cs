using BusinessLayer.Interfaces;
using BusinessLayer.Models.Request;
using BusinessLayer.Models.Response;
using Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using WebApi.Filters;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AuthenticationFilter]
    //Accept, Origin, 
    [EnableCors(origins: "*", headers: "*", methods:"*", SupportsCredentials = true)]
    //[AllowCrossSite]
    [RoutePrefix("api/dmservice")]
    [WebApiExceptionFilter]
    public class DocumentManagementApiController : ApiController
    {
        private IDictionaryService _service;
        private IDocumentManagementService _dmService;
        private IAccountService _accountService;
        private ILogger _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="dmService"></param>
        /// <param name="accountService"></param>
        /// /// <param name="logger"></param>
        public DocumentManagementApiController(IDictionaryService service, IDocumentManagementService dmService, IAccountService accountService, ILogger logger)
        {
            _service = service;
            _dmService = dmService;
            _accountService = accountService;
            _logger = logger;
        }


        /// <summary>
        /// GetDocumentType action returns info for selected document
        /// </summary>
        /// <param name="documentTypeName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("documenttype/{documentTypeName}")]        
        public async Task<JsonResult<List<BusinessLayer.Models.DictionaryItem>>> GetDocumentType(string documentTypeName)
        {
            List<BusinessLayer.Models.DictionaryItem> _docList = new List<BusinessLayer.Models.DictionaryItem>();
            if (!string.IsNullOrEmpty(documentTypeName))
            {
                _docList.Add(await _service.getDocumentTypeAsync(documentTypeName));
            }
            else
                throw new KeyNotFoundException("documentTypeName input parameter is missing");    

            return Json(_docList);
        }

        /// <summary>
        /// GetDocumentType action returns list of supported documents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("documenttypelist")]
        public async Task<JsonResult<List<BusinessLayer.Models.DictionaryItem>>> GetDocumentType()
        {
            _logger.Info("");
            List<BusinessLayer.Models.DictionaryItem> _docList = new List<BusinessLayer.Models.DictionaryItem>();
            _docList.AddRange(await _service.getDocumentTypeListAsync());
            return Json(_docList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("requiredattributes")]
        public async Task<JsonResult<List<BusinessLayer.Models.AdditionalParametersRequest>>> GetRequiredAttributes(string documentType)
        {
            if (string.IsNullOrEmpty(documentType))
                throw new KeyNotFoundException("documentType parameter is missing in DocumentManagementApi Controller GetRequiredAttributes action");

            var res = await _service.getDocumentTypeAttributesAsync(documentType);

            List<BusinessLayer.Models.AdditionalParametersRequest> parameters = new List<BusinessLayer.Models.AdditionalParametersRequest>();

            parameters.AddRange(res.Attributes.Where(x=>x.DataSourceObjectName == "additionalParameters").Select(x => new BusinessLayer.Models.AdditionalParametersRequest { Name = x.DataSourceProperty }).ToList());

            foreach (var param in res.DataSourceObjects.Select(x => x.RequiredParameters))
            {
                parameters.AddRange(param.Select(x => new BusinessLayer.Models.AdditionalParametersRequest { Name = x.InputParameterName }).ToList());
            }

            return Json(parameters);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
       // [Permissions(Common.Services.Permissions.ViewAttachment)]
        [HttpPost]
        [Route("attachments")]
        public async Task<JsonResult<object>> GetAttachments(List<BusinessLayer.Models.AdditionalParametersRequest> parameters )
        {
            // List<BusinessLayer.Models.AdditionalParametersRequest>
            //var s = JsonConvert.DeserializeObject<List<BusinessLayer.Models.AdditionalParametersRequest>>(parameters);
            return null;

            /*if (parameters == null)
            {
                throw new ArgumentNullException("parameters are missing");
            }
            var documentType = parameters.FirstOrDefault(x => x.Name == "documentType")?.Value;

            if (documentType == null)
                throw new ArgumentNullException("Document Type parameter is missing");

            var docs = await _dmService.FindDocuments(documentType, parameters);

            var title = docs.FirstOrDefault().AttachmentsAttributes?.Select(x => new { DisplayName = x.DisplayName, DisplayOrder = x.DisplayOrder }).Distinct().OrderBy(y => y.DisplayOrder);

            var res = new
            {
                Metadata = title,
                Attachments = docs
            };

            return Json((object)res);*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("permissions")]
        public JsonResult<PermissionsResponse> GetUserPermissions([FromBody] PermissionsRequest request)
        {
            var user = User.Identity.Name.ToLower().Replace("alfa\\", "");

            PermissionsResponse _response = _accountService.GetUserPermissions(user, request);

            return Json(_response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentid"></param>
        /// <returns></returns>
        [Permissions(BusinessLayer.Models.Permissions.DeleteAttachment)]
        [HttpDelete]
        [Route("deletedocument/{documentId}")]
        public async Task<JsonResult<object>> DeleteDocument(string documentid)
        {
            try
            {
                var delRes = await _dmService.deleteDocumentAsync(documentid);

                if (string.IsNullOrEmpty(delRes))
                    throw new Exception("Error has ocurred while deleting a document");
                
                var res = new { Message="Document successfuly deleted", StatusCode = 200 };

                return Json( (object)res );
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "DeleteDocument");
                var res = new { ErrorMessage = "Error has ocurred while deleting a document", StatusCode = 500 };

                return Json( (object)res );
            }
        }
    }
}
