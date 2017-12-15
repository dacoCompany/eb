using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Web.eBado.Models.Shared
{
    public class AllCompanyAttachmentsModel
    {
        public AllCompanyAttachmentsModel()
        {
            Attachment = new Collection<AttachmentModel>();
        }
        public string BatchName { get; set; }

        public string BatchDescription { get; set; }       

        public ICollection<AttachmentModel> Attachment { get; set; }
    }
}