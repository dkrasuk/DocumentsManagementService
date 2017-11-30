using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IDictionaryDataService
    {
        Task<IEnumerable<Models.BaseDictionaryItem>> getDictionaryByNameAsync(string name, string version);
        Task<Models.DocumentTypeAttributeItem> getDictionaryItemByNameAsync(string dictionaryName, string version, string itemName);
    }
}
