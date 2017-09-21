using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Shared
{
    public class AttachmentGalleryModel
    {
        public AttachmentGalleryModel()
        {
            Attachments = new Collection<AttachmentModel>();
        }

        public string Name { get; set; }
        public string Guid { get; set; }
        public string Description { get; set; }
        public ICollection<AttachmentModel> Attachments { get; private set; }
    }
}