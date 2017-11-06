using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Web.eBado.Helpers;
using Web.eBado.IoC;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;

namespace Web.eBado.Controllers
{
    public class ManageController : Controller
    {
        private const string successResponse = "OK";
        private readonly IUnitOfWork unitOfWork;
        SessionHelper sessionHelper;

        public ManageController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            sessionHelper = new SessionHelper();
        }
        public ActionResult SetLanguage(string language)
        {
            var supportedLang = ConfigurationManager.AppSettings.Get(ConfigurationKeys.SupportedLanguagesKey);
            if (supportedLang.Contains(language))
            {
                if (!(language == Request.Cookies["lang"].Value))
                {
                    CultureInfo ci = new CultureInfo(language);
                    Thread.CurrentThread.CurrentCulture = ci;
                    Thread.CurrentThread.CurrentUICulture = ci;
                    var requestCookie = Request.Cookies["lang"];
                    requestCookie.Value = language;
                    Response.SetCookie(requestCookie);
                    return new HttpStatusCodeResult(HttpStatusCode.Redirect);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult SetAccount(string accountName)
        {
            var currentSession = Session["User"] as SessionModel;
            bool isUserAccount = currentSession.Name == accountName;
            SessionModel newSession = null;

            if (isUserAccount)
            {
                newSession = sessionHelper.SetUserSession(currentSession.Id, unitOfWork);

            }
            else
            {
                newSession = sessionHelper.SetCompanySession(accountName, currentSession, unitOfWork);

            }
            Session.Remove("User");
            Session["User"] = newSession;
            return new HttpStatusCodeResult(HttpStatusCode.Redirect);
        }

        [HttpPost]
        public JsonResult DeleteCategory(string category)
        {
            var session = Session["User"] as SessionModel;
            bool deleted = false;

            return deleted ? Json("Deleted", JsonRequestBehavior.AllowGet) : Json("Error", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult AddMemberToCompany(string email, string selectedRole)
        {
            int companyId = GetCompanyId();
            string response = successResponse;
            var userDetails = GetUserByEmail(email);
            if (userDetails != null)
            {
                var companyDetail = unitOfWork.CompanyDetailsRepository.FindById(companyId);

                var userExistInCompany = unitOfWork.CompanyDetails2UserDetailsRepository
                    .AnyActive(cd => cd.UserDetailsId == userDetails.Id && cd.CompanyDetailsId == companyId);
                if (userExistInCompany)
                {
                    response = "User already exist!";
                }
                else
                {
                    var roleId = GetRoleIdByName(selectedRole, companyId);
                    if(roleId == null)
                    {
                        // TODO: add custom jsonResulType
                    }
                    companyDetail.CompanyDetails2UserDetails.Add(new CompanyDetails2UserDetailsDbo
                    {
                        CompanyDetailsId = companyId,
                        UserDetailsId = userDetails.Id,
                        CompanyRoleId = roleId.Value
                    });
                    unitOfWork.Commit();
                }
            }
            else
            {
                response = "Wrong email!";
            }
            return new JsonNetResult(response);
        }

        [HttpPost]
        public JsonResult DeleteMember(string email)
        {
            int companyId = GetCompanyId();
            string response = successResponse;

            var company2UserDbo = unitOfWork.CompanyDetails2UserDetailsRepository
                .FindFirstOrDefault(cd => cd.CompanyDetailsId == companyId && cd.UserDetail.Email == email);

            if (company2UserDbo == null)
            {
                // TODO: add custom jsonResulType
            }
            else
            {
                company2UserDbo.IsActive = false;
                unitOfWork.Commit();
            }
            return new JsonNetResult(response);
        }

       

        [HttpPost]
        public JsonResult ChangeMemberRole(string user, string role)
        {
            int companyId = GetCompanyId();
            string response = successResponse;

            var companyRole = unitOfWork.CompanyRoleRepository.FindFirstOrDefault(cr => cr.Name == role);
            var user2Company = unitOfWork.CompanyDetails2UserDetailsRepository
                .FindFirstOrDefault(cd => cd.UserDetail.Email == user && cd.CompanyDetailsId == companyId);
            user2Company.CompanyRoleId = companyRole.Id;

            if (companyRole == null || user2Company == null)
            {
                // TODO: add custom jsonResulType
            }

            unitOfWork.Commit();
            return new JsonNetResult(response);
        }

        [HttpPost]
        public JsonResult AddCustomRoleToCompany(string roleName, List<string> permissions)
        {
            int companyId = GetCompanyId();
            string response = successResponse;

            var companyRole = unitOfWork.CompanyRoleRepository.FindWhere(cr => cr.CreatedByCompId == companyId);
            bool roleNameExist = companyRole.Any(cr => cr.Name == roleName && cr.IsActive);
            if (roleNameExist)
            {
                response = "Role name already exist!";
            }
            else
            {
                var newCompanyRole = new CompanyRoleDbo
                {
                    Name = roleName,
                    CreatedByCompId = companyId
                };

                unitOfWork.CompanyRoleRepository.Add(newCompanyRole);

                var allPermissions = unitOfWork.CompanyPermissionsRepository.FindAll().ToList();
                foreach(var permission in permissions.Where(p=> !string.IsNullOrEmpty(p)))
                {
                    var permissionDbo = allPermissions.FirstOrDefault(ap => ap.Name == permission);
                    if(permissionDbo == null)
                    {
                        // TODO: add custom jsonResulType
                    }
                    var role2PermissionDbo = new CompanyRole2CompanyPermissionDbo {
                        CompanyPermission = permissionDbo,
                        CompanyRole = newCompanyRole
                    };
                    newCompanyRole.CompanyRole2CompanyPermission.Add(role2PermissionDbo);

                }
                unitOfWork.Commit();
            }

            return new JsonNetResult(response);
        }

        [HttpGet]
        public JsonResult GetPostalCodes(string prefix)
        {
            var location = new object();
            using (var uow = NinjectResolver.GetInstance<IUnitOfWork>())
            {
                location = uow.LocationRepository.FindWhere(x => x.PostalCode.StartsWith(prefix)
                    || x.PostalCode.Replace(" ", "").StartsWith(prefix.Replace(" ", ""))
                    || x.City.StartsWith(prefix)).Take(10).AsEnumerable()
                    .Select(loc => new
                    {
                        val = loc.PostalCode,
                        label = $"{loc.PostalCode} - {loc.District} - {loc.City}"
                    }).ToList();
            }

            return Json(location, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void GetCategories()
        {
            List<string> categoriesList = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                var text = $"myText{i}";
                categoriesList.Add(text);
            }

            // TempData["Categories"] = categoriesList;
            ViewBag.MultiselectCountry = new MultiSelectList(categoriesList);
        }

        private static string RemoveDiacritics(string city)
        {
            var normalizedString = city.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        private UserDetailDbo GetUserByEmail(string email)
        {
            return unitOfWork.UserDetailsRepository.FindFirstOrDefault(ud => ud.Email == email);
        }

        private int? GetRoleIdByName(string role, int companyId)
        {
            return unitOfWork.CompanyRoleRepository
                .FindFirstOrDefault(cr => cr.Name == role.ToString() && ( cr.CreatedByCompId == null || cr.CreatedByCompId == companyId))?.Id;
        }

        private int GetCompanyId()
        {
            var session = Session["User"] as SessionModel;
            int companyId = session.Companies.FirstOrDefault(c => c.IsActive).Id;
            return companyId;
        }
    }
}