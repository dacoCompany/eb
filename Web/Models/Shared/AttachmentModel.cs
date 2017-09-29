using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Web.eBado.Models.Shared
{
    public class AttachmentModel
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "size")]
        public string Size { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
        [JsonProperty(PropertyName = "batch")]
        public string Batch { get; set; }
        [JsonProperty(PropertyName = "attachmentType")]
        public string AttachmentType { get; set; }
    }
}