using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common.Models
{
    public class CompanyDetailsModel
    {
        public string Ico { get; set; }

        public string Name { get; set; }

        public string PostCode { get; set; }

        public string Location { get; set; }

        public string CompanyType { get; set; }

        public IEnumerable<string> Activities { get; set; }

        public IEnumerable<string> Executives { get; set; }
    }
}
