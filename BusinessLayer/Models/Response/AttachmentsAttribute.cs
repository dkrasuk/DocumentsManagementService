using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models.Response
{
    public class AttachmentsAttribute
    {
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
