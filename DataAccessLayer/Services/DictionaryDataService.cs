using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Interfaces;
using Dictionaries;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Dictionaries.Models;

namespace DataAccessLayer.Services
{
    public class DictionaryDataService : IDictionaryDataService
    {
        IDictionaryOperations _dictionaryOperations;

        public DictionaryDataService(IDictionaryOperations dictionaryOperations)
        {
            _dictionaryOperations = dictionaryOperations;
        }

        public async Task<IEnumerable<Models.BaseDictionaryItem>> getDictionaryByNameAsync(string name, string version)
        {
            Dictionary dictionary;
            try
            {
                dictionary = await _dictionaryOperations.GetDictionaryByNameAndVersionAsync(name, version);

            }
            catch (Exception ex)
            {

                throw ex;
            }
              return dictionary.Items.Select(i => (i.Value as JObject).ToObject<Models.BaseDictionaryItem>()).ToList();
        }

        public async Task<Models.DocumentTypeAttributeItem> getDictionaryItemByNameAsync(string dictionaryName, string version, string itemName)
        {
            Dictionary dictionary;
            try
            {
                dictionary = await _dictionaryOperations.GetDictionaryByNameAndVersionAsync(dictionaryName, version);

            }
            catch (Exception ex)
            {

                throw ex;
            }            
            //var a = dictionary.Items.Select(i => (i.Value as JObject).ToObject<Models.DocumentTypeAttributeItem>()).ToList().First(x => x.Id == itemName).ToString();
            return dictionary.Items.Select(i => (i.Value as JObject).ToObject<Models.DocumentTypeAttributeItem>()).ToList().First(x=>x.Id == itemName);
        }
        
    }
}
