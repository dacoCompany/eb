using Infrastructure.Common.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.eBado.Models.Company;

namespace Web.eBado.Helpers
{
    public class CompanyHelper
    {
        SharedHelper sharedHelper;
        public CompanyHelper(IUnitOfWork unitOfWork)
        {
            sharedHelper = new SharedHelper(unitOfWork);
        }
        public string GetCategoryBySelectedItem(string selectedItem)
        {
            if (selectedItem == null)
            {
                return null;
            }
            var allCategories = sharedHelper.GetCategoriesWithSubCategories();
            var category = allCategories.FirstOrDefault(ac => ac.Category.Contains(selectedItem));
            if (category != null)
            {
                return selectedItem;
            }
            else
            {
                category = allCategories.FirstOrDefault(ac => ac.SubCategories.Contains(selectedItem));
                return category.Category;
            }
        }
    }
}