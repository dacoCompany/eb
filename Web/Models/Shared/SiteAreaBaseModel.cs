using Infrastructure.Common.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Web.eBado.Models.Shared
{
    public class SiteAreaBaseModel<T> where T: SiteItemModel
    {
        public SiteAreaBaseModel()
        {
            SiteItems = new Collection<T>();
        }

        public SiteArea SelectedArea { get; set; }
        public string SearchString { get; set; }
        public IEnumerable<T> SiteItems { get; set; }

    }
}