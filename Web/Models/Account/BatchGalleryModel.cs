using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class BatchGalleryModel
    {
        public BatchGalleryModel()
        {
            Batch = new Collection<BatchModel>();
        }

        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public Collection<BatchModel> Batch { get; set; }
    }
}