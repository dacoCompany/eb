using Infrastructure.Common.Models.Cache;
using System.Collections.Generic;

namespace Infrastructure.Common.Models
{
    public class CachedAllCategoriesModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<SubCategoryModel> SubCategories { get; set; }
    }
}