using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Common;
using Infrastructure.Resources;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace Web.eBado.Models.Account
{
    public class BatchGalleryModel
    {
        public BatchGalleryModel()
        {
            Batch = new Collection<BatchModel>();
        }

        [StringLengthValidator(1, RangeBoundaryType.Inclusive, 100, RangeBoundaryType.Ignore, MessageTemplateResourceType = typeof(Resources), MessageTemplateResourceName = ErrorMessages.RequiredFieldResources, Ruleset = "CreateBatch")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool HasError { get; set; }

        public Collection<BatchModel> Batch { get; set; }
    }
}