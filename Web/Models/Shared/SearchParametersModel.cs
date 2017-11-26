using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Shared
{
    public class SearchParametersModel
    {
        public string SelectedCategory { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public int Radius { get; set; }
        public bool SearchInSK { get; set; }
        public bool SearchInCZ { get; set; }
        public bool SearchInHU { get; set; }
    }
}