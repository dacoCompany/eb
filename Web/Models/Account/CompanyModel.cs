using Infrastructure.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class CompanyModel
    {
        public CompanyModel()
        {
            Categories = new CategoriesModel();
            Languages = new LanguagesModel();
        }
        public CompanyType CompanyType { get; set; }

        public Countries CompanyLocation { get; set; }

        public string CompanyName { get; set; }

        [DataType(DataType.MultilineText)]
        public string CompanyDescription { get; set; }

        public string CompanyEmail { get; set; }

        public int? CompanyIco { get; set; }

        public int? CompanyDic { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string CompanyPhoneNumber { get; set; }

        public string CompanyAdditionalPhoneNumber { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyStreetNumber { get; set; }

        public string CompanyPostalCode { get; set; }

        public CategoriesModel Categories { get; set; }

        public LanguagesModel Languages { get; set; }

        public string ProfileUrl { get; set; }
    }
}