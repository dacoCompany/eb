using Infrastructure.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.eBado.Models.Account
{
    public class CompanyModel
    {
        public CompanyModel()
        {
            Categories = new CategoriesModel();
        }
        public CompanyType CompanyType { get; set; }

        public Countries CompanyLocation { get; set; }

        public string CompanyName { get; set; }

        public int? CompanyIco { get; set; }

        public int? CompanyDic { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string CompanyPhoneNumber { get; set; }

        public string CompanyAdditionalPhoneNumber { get; set; }

        public string CompanyStreet { get; set; }

        public string CompanyStreetNumber { get; set; }

        public string CompanyPostalCode { get; set; }

        public CategoriesModel Categories { get; set; }
    }
}