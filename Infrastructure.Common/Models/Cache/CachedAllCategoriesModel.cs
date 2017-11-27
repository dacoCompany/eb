using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure.Common.Models
{
    public class CachedAllCategoriesModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
    }
}