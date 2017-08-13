using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Resources;

namespace Infrastructure.Common.Enums
{
    public enum CompanyType
    {
        [Display(ResourceType = typeof(Resources.Resources), Name = "AccountTypeContractor")]
        PartTime = 1,

        [Display(ResourceType = typeof(Resources.Resources), Name = "AccountTypeSelfEmployed")]
        SelfEmployed = 2,

        [Display(ResourceType = typeof(Resources.Resources), Name = "AccountTypeCompany")]
        Company = 3
    }
}
