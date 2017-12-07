using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Web.eBado.Models.Shared
{
    public class AttachmentGalleryModel
    {
        public AttachmentGalleryModel()
        {
            Attachments = new Collection<AttachmentModel>();
        }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "guid")]
        public string Guid { get; set; }
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "videourl")]
        public string VideoUrl { get; set; }
        [JsonProperty(PropertyName = "attachments")]
        public ICollection<AttachmentModel> Attachments { get; private set; }
    }
}