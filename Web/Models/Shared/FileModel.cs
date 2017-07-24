using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Shared
{
    public class FileModel
    {
        public string Name { get; set; }

        public int Size { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public string Url { get; set; }

        public string DeleteUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string DeleteType { get; set; }
    }
}