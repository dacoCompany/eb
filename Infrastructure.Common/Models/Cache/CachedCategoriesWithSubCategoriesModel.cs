using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models.Cache
{
    public class CachedCategoriesWithSubCategoriesModel
    {
        public string Category { get; set; }
        public IEnumerable<string> SubCategories { get; set; }
    }
}
