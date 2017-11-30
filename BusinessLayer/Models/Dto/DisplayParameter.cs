using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class DisplayParameter
    {
        public string DisplayName { get; set; }
        public int DisplayOrder { get; set; }
        public string AttributeName { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
    }
}
