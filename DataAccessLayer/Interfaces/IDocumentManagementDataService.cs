using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IDocumentManagementDataService
    {
        Task<DocumentManagement2SoapService.DocumentType[]> getDocumentTypeInfoAsync(string documentTypeCode);
        Task<string> registerDocumentAsync(DocumentManagement2SoapService.AttributeTypeAttribute[] attributes, byte[] documentBase, string documentType);
        Task<DocumentManagement2SoapService.getDocumentListResponse> GetDocumentsListAsync(string id);
        Task<DocumentManagement2SoapService.getDocumentListResponse> GetDocumentsListAsync(DocumentManagement2SoapService.AttributeTypeAttribute[] attributes, string documentType, string documentVersion);
        Task<DocumentManagement2SoapService.document[]> FindDocument(DocumentManagement2SoapService.criteriaAttribute[] filters);
        Task<DocumentManagement2SoapService.getDocumentResponse> getDocumentAsync(string documentUniqueId);
        Task<string> deleteDocumentAsync(string documentUniqueId);
    }
}
