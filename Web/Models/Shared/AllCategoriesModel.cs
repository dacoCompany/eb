using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Shared
{
    public class AllCategoriesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
    }
}