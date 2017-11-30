using BusinessLayer.Models;
using BusinessLayer.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusinessLayer.Interfaces
{
    public interface IDocumentManagementService
    {
        Task PostDocument(string documentType, HttpPostedFileBase file, List<Models.AdditionalParametersRequest> requestParameters);
        Task<bool> ValidateParametersAsync(string documentType, List<Models.AdditionalParametersRequest> parameters);
        Task<bool> ValidateFileParameters(string documentType, HttpPostedFileBase file, List<Models.UploadResult> result);
        Task<List<DisplayParameter>> getDisplayParameters(string documentType);
        Task<List<AttachmentResponse>> FindDocuments(string documentType, List<Models.AdditionalParametersRequest> filterConditions);
        Task<FileResponse> getDocument(string documentId);
        Task<string> deleteDocumentAsync(string documentId);
    }
}
