using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using DataAccessLayer.Interfaces;
using BusinessLayer.Models;
using AutoMapper;
using Logger;

namespace BusinessLayer.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DictionaryService : IDictionaryService
    {
        IDictionaryDataService _dictionary;
        ILogger _logger;

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="logger"></param>
        public DictionaryService(IDictionaryDataService dictionary, ILogger logger)
        {
            _dictionary = dictionary;
            _logger = logger;
        }

        /// <summary>
        /// Getting Document Types 
        /// </summary>
        /// <returns></returns>
        public async Task<List<DictionaryItem>> getDocumentTypeListAsync()
        {
            try
            {
                var res = await _dictionary.getDictionaryByNameAsync("DM.DocumentType", "2");
                var dto = Mapper.Map<List<DictionaryItem>>(res);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "getDocumentTypeList");
            }

            return null;
        }

        public async Task<DictionaryItem> getDocumentTypeAsync(string documentType)
        {
            try
            {
                var res = await _dictionary.getDictionaryItemByNameAsync("DM.DocumentType", "2", documentType);
                var dto = Mapper.Map<DictionaryItem>(res);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "getDocumentTypeAsync");
            }

            return null;
        }

        public async Task<DocumentTypeAttributeItem> getDocumentTypeAttributesAsync(string documentType)
        {
            try
            {
                var res = await _dictionary.getDictionaryItemByNameAsync("DM.DocumentType", "2", documentType);
                var dto = Mapper.Map<DocumentTypeAttributeItem>(res);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex, "getDocumentTypeList");
            }

            return null;
        }
    }
}
