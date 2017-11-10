using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class CompanyPermissionsModel
    {
        public bool AddMember { get; set; }
        public bool RemoveMember { get; set; }
        public bool AddGallery { get; set; }
        public bool RemoveGallery { get; set; }
        public bool AddAttachments { get; set; }
        public bool RemoveAttachments { get; set; }
        public bool Comment { get; set; }
        public bool CreateDemand { get; set; }
        public bool EditDemand { get; set; }
        public bool DeleteDemand { get; set; }
        public bool ChangeSettings { get; set; }
        public bool ReadOnly { get; set; }
    }
}