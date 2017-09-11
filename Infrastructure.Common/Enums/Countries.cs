using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Enums
{
    public enum Countries
    {
        [Display(ResourceType = typeof(Resources.Resources), Name = "CountrySelect")]
        Select = 0,
        [Display(ResourceType = typeof(Resources.Resources), Name = "CountryCZ")]
        Czechia = 1,
        [Display(ResourceType = typeof(Resources.Resources), Name = "CountryHU")]
        Hungary = 2,
        [Display(ResourceType = typeof(Resources.Resources), Name = "CountrySK")]
        Slovakia = 3,

    }
}
