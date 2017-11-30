using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public class DataSourceObject
    {
        public string Name { get; set; }
        public IEnumerable<DataSourceObjectRequiredParameters> RequiredParameters { get; set; }
    }
}
