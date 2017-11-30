using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    /// <summary>
    /// Base Dictionary Item class for common dictioanries
    /// </summary>
    public class BaseDictionaryItem
    {
        /// <summary>
        /// Gets or sets dictionary item identifier.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets dictionary item Name.
        /// </summary>
        public string Name { get; set; }
    }
}
