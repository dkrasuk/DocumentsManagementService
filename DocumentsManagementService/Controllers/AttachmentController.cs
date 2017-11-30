using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DocumentsManagementService.Controllers
{
    public class AttachmentController : Controller
    {
        /// <summary>
        /// The dictionary service
        /// </summary>
        private static IDictionaryService _dictionaryService { get; set; }


        private static IAgreementService _agreementService { get; set; }
        private static ClientManager _clientManager;

        public AttachmentController(IDictionaryService dictionaryService, IAgreementService agreementService)
        {
            _dictionaryService = dictionaryService;
            _agreementService = agreementService;
            _clientManager = new ClientManager();
        }

        [HttpGet]
        [ApplicationLog]
        [Route("{collateralId}")]
        public async Task<ActionResult> GetAttachments(string collateralId)
        {
            return PartialView("_AttachmentsCollateral");
        }

        [HttpPost]
        [ApplicationLog]
        [Route("upload")]
        public JsonResult UploadPhoto()
        {
            string __filepath = Server.MapPath("~/uploads");
            int __maxSize = 2 * 1024 * 1024;
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result
            {
                Files = new List<string>()
            };

            if (Request.Files.Count > 0)
            {
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];
                    if (file.ContentLength > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 2 Мб";
                        break;
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        break;
                    }

                    // Сохранить файл и вернуть URL
                    if (Directory.Exists(__filepath))
                    {
                        Guid guid = Guid.NewGuid();
                        file.SaveAs($@"{__filepath}\{guid}.{file.FileName}");
                        result.Files.Add($"/uploads/{guid}.{file.FileName}");
                    }
                }
            }
            return Json(result);
        }
    }
}