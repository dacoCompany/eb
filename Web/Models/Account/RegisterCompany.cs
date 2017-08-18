using Infrastructure.Common.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Resources;
using System.Web.Mvc;

namespace Web.eBado.Models.Account
{
    public class RegisterCompany : RegisterUser
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public CompanyType CompanyType { get; set; }

        public string CompanyName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public string Ico { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        [DataType(DataType.PhoneNumber)]
        public string CompanyPhoneNumber { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string CompanyAdditionalPhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public string CompanyStreet { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public string CompanyStreetNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public int CompanyPostalCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RequiredField")]
        public CategoriesModel Categories { get; set; }
    }
}