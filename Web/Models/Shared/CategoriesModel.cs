using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Web.eBado.Models.Shared
{
    public class CategoriesModel
    {
        public CategoriesModel()
        {
            SubCategories = new Collection<int>();
        }
        public int Id { get; set; }

        public ICollection<int> SubCategories { get; private set; }
    }
}