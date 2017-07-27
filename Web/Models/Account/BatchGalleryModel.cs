using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class BatchGalleryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseThumbUrl { get; set; }
        public int AttachmentsCount { get; set; }
    }
}