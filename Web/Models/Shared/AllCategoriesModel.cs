﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Shared
{
    public class AllCategoriesModel
    {
        public string Category { get; set; }
        public IEnumerable<string> SubCategories { get; set; }
    }
}