using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IDictionaryService
    {
        Task<List<DictionaryItem>> getDocumentTypeListAsync();
        Task<DictionaryItem> getDocumentTypeAsync(string documentType);
        Task<DocumentTypeAttributeItem> getDocumentTypeAttributesAsync(string documentType);
    }
}
