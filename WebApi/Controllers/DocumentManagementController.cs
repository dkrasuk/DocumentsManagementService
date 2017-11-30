using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLayer;
using BusinessLayer.Interfaces;
using System.Threading.Tasks;
using System.Text;
using WebApi.Filters;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Logger;
using BusinessLayer.Models.Response;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AuthenticationFilter]
    [RoutePrefix("dmservice")]
    [AllowCrossSite]    
    [WebApiExceptionFilter]
    public class DocumentManagementController : Controller
    {
        Logger.ILogger _logger;
        IDocumentManagementService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public DocumentManagementController(IDocumentManagementService service)
        {
            _service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("form")]
        [HttpGet]
        public PartialViewResult GetForm()
        {           
            return PartialView("_DocumentAddForm");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("ui")]
        [HttpGet]
        public ActionResult GetUiScript()
        {
            var file = System.Web.Hosting.HostingEnvironment.MapPath("~/Scripts/ui/attachment.form.js");
            return File(System.IO.File.ReadAllBytes(file), "text/javascript");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Permissions(BusinessLayer.Models.Permissions.UploadAttachment)]
        [Route("upload")]
        [HttpPost]
        public async Task<JsonResult> Post(List<BusinessLayer.Models.AdditionalParametersRequest> parameters)
        {
            var result = new List<BusinessLayer.Models.UploadResult>();
            try
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException("parameters are missing");
                }
                var documentType = parameters.FirstOrDefault(x => x.Name == "documentType").Value;

                if (documentType == null)
                    throw new ArgumentNullException("Document Type parameter is missing");

                if (!await _service.ValidateParametersAsync(documentType, parameters))
                {
                    throw new ArgumentNullException("Not all parameters are valid");
                }

                if (Request.Files.Count > 0)
                {
                    foreach (string f in Request.Files)
                    {

                        HttpPostedFileBase file = Request.Files[f];                       
                        if (await _service.ValidateFileParameters(documentType, file, result))
                        {
                            await _service.PostDocument(documentType, file, parameters);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message,ex, "upload");
                result.Add(new BusinessLayer.Models.UploadResult() {Error = ex.Message, FileName="" });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Permissions(BusinessLayer.Models.Permissions.DownloadAttachment)]
        [Route("download/{id}")]
        [HttpGet]
        public async Task<FileResult> DownloadAttachment(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("Id is missing");

            var doc = await _service.getDocument(id);

            return File(doc.FileByteArray, doc.MimeType, doc.FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Permissions(BusinessLayer.Models.Permissions.ViewAttachment)]
        [HttpPost]
        [Route("attachments")]
        public async Task<ActionResult> GetAttachments(List<BusinessLayer.Models.AdditionalParametersRequest> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters are missing");
            }
            var documentType = parameters.FirstOrDefault(x => x.Name == "documentType")?.Value;

            if (documentType == null)
                throw new ArgumentNullException("Document Type parameter is missing");

            var docs = await _service.FindDocuments(documentType, parameters);

            return PartialView("_Attachments", docs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Permissions(BusinessLayer.Models.Permissions.ViewAttachment)]
        [HttpPost]
        [Route("attachmentlist")]
        public async Task<JsonResult> GetAttachmentList(List<BusinessLayer.Models.AdditionalParametersRequest> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters are missing");
            }
            var documentType = parameters.FirstOrDefault(x => x.Name == "documentType")?.Value;

            if (documentType == null)
                throw new ArgumentNullException("Document Type parameter is missing");

            List<AttachmentResponse> docs = await _service.FindDocuments(documentType, parameters);

            if (docs.Count() > 0)
            {
                var title = docs.FirstOrDefault().AttachmentsAttributes?.Select(x => new { DisplayName = x.DisplayName, DisplayOrder = x.DisplayOrder }).Distinct().OrderBy(y => y.DisplayOrder);

                var res = new
                {
                    Metadata = title,
                    Attachments = docs
                };

                return Json((object)res, JsonRequestBehavior.AllowGet);
            }
            else
                return null;             
        }
    }
}