﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models.Response
{
    public class FileResponse
    {
        public byte[] FileByteArray { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
    }
}
