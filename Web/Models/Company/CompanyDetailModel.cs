using System.Collections.Generic;
using System.Collections.ObjectModel;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;

namespace Web.eBado.Models.Company
{
    public class CompanyDetailModel
    {
        public CompanyDetailModel()
        {
            Attachments = new Collection<AllCompanyAttachmentsModel>();
        }
        public CompanyModel CompanyModel { get; set; }
        public ICollection<AllCompanyAttachmentsModel> Attachments { get; set; }
        public IList<string> Languages { get; set; }
        public IList<string> Categories { get; set; }
    }
}