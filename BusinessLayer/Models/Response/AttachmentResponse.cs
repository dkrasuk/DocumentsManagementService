using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models.Response
{
    public class AttachmentResponse
    {
        public List<AttachmentsAttribute> AttachmentsAttributes { get; set; }

        public AttachmentResponse()
        {
            AttachmentsAttributes = new List<AttachmentsAttribute>();
        }
    }
}
