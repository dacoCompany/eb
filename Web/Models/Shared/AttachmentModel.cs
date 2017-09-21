using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Shared
{
    public class AttachmentModel
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}