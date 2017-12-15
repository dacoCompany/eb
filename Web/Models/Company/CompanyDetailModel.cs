using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
        [DataType(DataType.EmailAddress)]
        public string CustomerEmail { get; set; }
        public string Subject { get; set; }
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }
        public string MapUrl { get; set; }
        public int ImagesCount { get; set; }
        public int VideosCount { get; set; }
        public CompanyModel CompanyModel { get; set; }
        public ICollection<AllCompanyAttachmentsModel> Attachments { get; set; }
        public IList<string> Languages { get; set; }
        public IList<string> Categories { get; set; }
    }
}