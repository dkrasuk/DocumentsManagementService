using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models.Request
{
    public class PermissionsRequest
    {
        public bool DownloadAttachment { get; set; }
        public bool UploadAttachment { get; set; }
        public bool DeleteAttachment { get; set; }
        public bool ViewAttachment { get; set; }
    }
}
