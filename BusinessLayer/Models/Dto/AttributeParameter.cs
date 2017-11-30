using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class AttributeParameter
    {
        public string AttributeName { get; set; }
        public string DataSourceObjectName { get; set; }
        public string DataSourceProperty { get; set; }
        public string IdentificationGroup { get; set; }
        public string DataSourceValue { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
    }
}
