using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models.Response
{
    public class PermissionsResponse
    {
        public bool DownloadAttachment { get; set; }
        public bool UploadAttachment { get; set; }
        public bool DeleteAttachment { get; set; }        
        public bool ViewAttachment { get; set; }
    }
}
