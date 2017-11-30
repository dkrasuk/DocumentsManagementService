using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class DataSourceObject
    {
        public string Name { get; set; }
        public IEnumerable<DataSourceObjectRequiredParameters> RequiredParameters { get; set; }
    }
}
