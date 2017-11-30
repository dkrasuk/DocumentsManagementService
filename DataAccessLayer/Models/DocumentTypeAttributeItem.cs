using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class DocumentTypeAttributeItem : BaseDictionaryItem
    {
        public int? AllowedFileMaxSize { get; set; }
        public List<string> AllowedMimeTypes { get; set; }
        public IEnumerable<DataSourceObject> DataSourceObjects { get; set; }
        public IEnumerable<AttributeParameter> Attributes { get; set; }
        public IEnumerable<SearchAttribute> SearchAttributes { get; set; }
        public IEnumerable<DisplayParameter> DisplayParameters { get; set; }
    }
}