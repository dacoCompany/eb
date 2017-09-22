using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Shared
{
    public class AttachmentModel
    {
        public string name { get; set; }
        public string size { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
    }
}