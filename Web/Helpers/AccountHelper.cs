using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Web.eBado.Models.Account;

namespace Web.eBado.Helpers
{
    public class AccountHelper
    {
        #region Constants
        const string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
        #endregion

        #region Public Methods
        public void RegisterCompany(RegisterCompanyModel model, IUnitOfWork uow)
        {
            var userRole = uow.UserRoleRepository.FirstOrDefault(r => r.Code == UserRole.StandardUser.ToString());
            var companyType = uow.CompanyTypeRepository.FirstOrDefault(ct => ct.Code == model.CompanyType.ToString());
            var companyRoleId = uow.CompanyRoleRepository.FirstOrDefault(cr => cr.Code == CompanyRole.Owner.ToString()).Id;
            var location = uow.LocationRepository.FirstOrDefault(l => l.PostalCode == model.PostalCode);
            var companyLocation = uow.LocationRepository.FirstOrDefault(l => l.PostalCode == model.CompanyPostalCode);
            string salt = GenerateSalt();

            var userDetails = new UserDetailDbo
            {
                Email = model.Email,
                Password = EncodePassword(model.Password, salt),
                Salt = salt,
                Title = model.Title,
                FirstName = model.FirstName,
                Surname = model.Surname,
                PhoneNumber = model.PhoneNumber,
                AdditionalPhoneNumber = model.AdditionalPhoneNumber,
                DisplayName = string.Format("{0} {1}", model.FirstName, model.Surname),
                UserRoleId = userRole.Id,
            };
            userDetails.Addresses.Add(new AddressDbo
            {
                Street = model.Street,
                Number = model.StreetNumber,
                IsBillingAddress = true,
                LocationId = location.Id
            });

            var companyDetails = new CompanyDetailDbo
            {
                Name = model.CompanyName,
                PhoneNumber = model.CompanyPhoneNumber,
                AdditionalPhoneNumber = model.CompanyAdditionalPhoneNumber,
                Ico = model.Ico,
                Dic = model.Dic,
                CompanyTypeId = companyType.Id,
            };
            companyDetails.Addresses.Add(new AddressDbo
            {
                Street = model.CompanyStreet,
                Number = model.CompanyStreetNumber,
                IsBillingAddress = true,
                LocationId = companyLocation.Id
            });
            var company2User = new CompanyDetails2UserDetailsDbo
            {
                CompanyDetail = companyDetails,
                UserDetail = userDetails,
                CompanyRoleId = companyRoleId
            };
            uow.Commit();
        }

        public void RegisterUser(RegisterUserModel model, IUnitOfWork uow)
        {
            var userRole = uow.UserRoleRepository.FirstOrDefault(r => r.Code == UserRole.StandardUser.ToString());
            var location = uow.LocationRepository.FirstOrDefault(l => l.PostalCode == model.PostalCode);
            string salt = GenerateSalt();

            var userDetails = new UserDetailDbo
            {
                Email = model.Email,
                Password = EncodePassword(model.Password, salt),
                Salt = salt,
                Title = model.Title,
                FirstName = model.FirstName,
                Surname = model.Surname,
                PhoneNumber = model.PhoneNumber,
                AdditionalPhoneNumber = model.AdditionalPhoneNumber,
                DisplayName = string.Format("{0} {1}", model.FirstName, model.Surname),
                UserRoleId = userRole.Id,
            };
            userDetails.Addresses.Add(new AddressDbo
            {
                Street = model.Street,
                Number = model.StreetNumber,
                IsBillingAddress = true,
                LocationId = location.Id
            });
            uow.Commit();
        }



        public void InitializeAllCategories(RegisterCompanyModel model)
        {
            CategoriesModel cat = new CategoriesModel();
            cat.AllCategories = GetAllCategories();
            model.Categories = cat;
        }
        #endregion
        #region Private Methods
        private static string GenerateSalt()
        {
            int length = 10;

            var randNum = new Random();
            var chars = new char[length];
            var allowedCharCount = allowedChars.Length;
            for (var i = 0; i <= length - 1; i++)
            {
                chars[i] = allowedChars[Convert.ToInt32((allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        public static string EncodePassword(string pass, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(pass);
            byte[] src = Encoding.Unicode.GetBytes(salt);
            byte[] dst = new byte[src.Length + bytes.Length];
            System.Buffer.BlockCopy(src, 0, dst, 0, src.Length);
            System.Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
            HashAlgorithm algorithm = HashAlgorithm.Create("SHA1");
            byte[] inArray = algorithm.ComputeHash(dst);

            return Convert.ToBase64String(inArray);
        }

        private bool CheckIfEmailExist(string email, UnitOfWork uow)
        {
            var userEmail = uow.UserDetailsRepository.FindWhere(ua => ua.Email == email).FirstOrDefault();
            return userEmail == null ? true : false;
        }

        private IEnumerable<SelectListItem> GetAllCategories()
        {
            List<SelectListItem> allCars = new List<SelectListItem>();
            //Add a few cars to make a list of cars
            for (var i = 0; i < 50; i++)
            {
                allCars.Add(new SelectListItem { Value = $"Tag Text{i}", Text = $"tag Text{i}" });

            }

            return allCars.AsEnumerable();
        }

        #endregion
    }
}