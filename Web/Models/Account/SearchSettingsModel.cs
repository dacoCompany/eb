using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class SearchSettingsModel
    {
        public int SearchRadius { get; set; }
        public bool SearchInSK { get; set; }
        public bool SearchInCZ { get; set; }
        public bool SearchInHU { get; set; }

    }
}