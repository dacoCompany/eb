using System;
using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.eBado.Helpers;
using Web.eBado.IoC;
using Web.eBado.Models.Account;
using Web.eBado.Models.Shared;
using WebAPIFactory.Configuration.Core;
using WebAPIFactory.Caching.Core;
using Infrastructure.Common;

namespace Web.eBado.Controllers
{
    [RoutePrefix("Manage")]
    public class ManageController : Controller
    {        
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        SessionHelper sessionHelper;

        public ManageController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            sessionHelper = new SessionHelper();
        }

        [AllowAnonymous]
        [Route("SetLanguage")]
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

        [System.Web.Http.Authorize]
        [Route("SetAccount")]
        public async Task<ActionResult> SetAccount(string accountName)
        {
            var currentSession = Session["User"] as SessionModel;
            bool isUserAccount = currentSession.Name == accountName;
            SessionModel newSession = null;

            if (isUserAccount)
            {
                newSession = sessionHelper.SetUserSession(currentSession.Id, unitOfWork);

                int userRoleId = 0;

                userRoleId = unitOfWork.UserDetailsRepository.FindById(newSession.Id).UserRoleId;

                bool result = await GetToken(userRoleId, 0);

                if (!result)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }

            }
            else
            {
                newSession = sessionHelper.SetCompanySession(accountName, currentSession, unitOfWork);

                int companyRoleId = 0;

                string companyRole = newSession.Companies.First(c => c.IsActive).CompanyRole;
                companyRoleId = unitOfWork.CompanyRoleRepository.FindFirstOrDefault(cr => cr.Name == companyRole).Id;

                bool result = await GetToken(0, companyRoleId);

                if (!result)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
                }

            }
            Session.Remove("User");
            Session["User"] = newSession;
            return new HttpStatusCodeResult(HttpStatusCode.Redirect);
        }

        [HttpPost]
        [System.Web.Http.Authorize(Roles = "ChangeSettings")]
        [Route("DeleteCategory")]
        public JsonResult DeleteCategory(string category)
        {
            category = category.Replace("_", " ");
            string response = ErrorMessages.SuccessResponse;
            var session = Session["User"] as SessionModel;
            int companyId = session.Companies.FirstOrDefault(c => c.IsActive).Id;
            var companyDetails = unitOfWork.CompanyDetailsRepository.FindFirstOrDefault(cd => cd.Id == companyId);
            var categoryDbo = companyDetails.Category2CompanyDetails.FirstOrDefault(c => c.IsActive && c.Category.Name == category);

            if (categoryDbo != null)
            {
                categoryDbo.IsActive = false;
            }
            else
            {
                var subCategoryDbo = companyDetails.SubCategory2CompanyDetails.FirstOrDefault(c => c.SubCategory.Name == category);
                if (subCategoryDbo == null)
                {
                    response = ErrorMessages.CategoryNotExists;
                }
                else
                {
                    subCategoryDbo.IsActive = false;
                }
            }
            unitOfWork.Commit();
            return new JsonNetResult(response);
        }

        [HttpPost]
        [System.Web.Http.Authorize(Roles = "ChangeSettings")]
        [Route("DeleteLanguage")]
        public JsonResult DeleteLanguage(string code)
        {
            string response = ErrorMessages.SuccessResponse;
            var session = Session["User"] as SessionModel;
            int companyId = session.Companies.FirstOrDefault(c => c.IsActive).Id;
            var companyDetails = unitOfWork.CompanyDetailsRepository.FindFirstOrDefault(cd => cd.Id == companyId);
            var languageDbo = companyDetails.CompanyDetails2Languages.FirstOrDefault(c => c.IsActive && c.Language.Code == code);

            if (languageDbo != null)
            {
                languageDbo.IsActive = false;
            }
            else
            {
                response = ErrorMessages.SomethingWrong;
            }
            unitOfWork.Commit();
            return new JsonNetResult(response);
        }

        [HttpGet]
        [System.Web.Http.Authorize(Roles = "AddMember")]
        [Route("AddMemberToCompany")]
        public JsonResult AddMemberToCompany(string email, string selectedRole)
        {
            int companyId = GetCompanyId();
            string response = ErrorMessages.SuccessResponse;
            var userDetails = GetUserByEmail(email);
            if (userDetails != null)
            {
                var companyDetail = unitOfWork.CompanyDetailsRepository.FindById(companyId);

                var userExistInCompany = unitOfWork.CompanyDetails2UserDetailsRepository
                    .AnyActive(cd => cd.UserDetailsId == userDetails.Id && cd.CompanyDetailsId == companyId);
                if (userExistInCompany)
                {
                    response = ErrorMessages.UserExists;
                }
                else
                {
                    var roleId = GetRoleIdByName(selectedRole, companyId);
                    if (roleId == null)
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
                response = ErrorMessages.WrongEmail;
            }
            return new JsonNetResult(response);
        }

        [HttpPost]
        [System.Web.Http.Authorize(Roles = "RemoveMember")]
        [Route("DeleteMember")]
        public JsonResult DeleteMember(string email)
        {
            int companyId = GetCompanyId();
            string response = ErrorMessages.SuccessResponse;

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
        [System.Web.Http.Authorize(Roles = "AddMember, RemoveMember")]
        [Route("ChangeMemberRole")]
        public JsonResult ChangeMemberRole(string user, string role)
        {
            int companyId = GetCompanyId();
            string response = ErrorMessages.SuccessResponse;

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
        [System.Web.Http.Authorize(Roles = "AddMember, RemoveMember")]
        [Route("AddCustomRoleToCompany")]
        public JsonResult AddCustomRoleToCompany(string roleName, List<string> permissions)
        {
            int companyId = GetCompanyId();
            string response = ErrorMessages.SuccessResponse;

            var companyRole = unitOfWork.CompanyRoleRepository.FindWhere(cr => cr.CreatedByCompId == companyId);
            bool roleNameExist = companyRole.Any(cr => cr.Name == roleName && cr.IsActive);
            if (roleNameExist)
            {
                response = ErrorMessages.RoleNameExists;
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
                foreach (var permission in permissions.Where(p => !string.IsNullOrEmpty(p)))
                {
                    var permissionDbo = allPermissions.FirstOrDefault(ap => ap.Name == permission);
                    if (permissionDbo == null)
                    {
                        // TODO: add custom jsonResulType
                    }
                    var role2PermissionDbo = new CompanyRole2CompanyPermissionDbo
                    {
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
        [AllowAnonymous]
        [Route("GetPostalCodes")]
        public JsonResult GetPostalCodes(string prefix)
        {
            var cache = NinjectResolver.GetInstance<ICache>();
            var cachedPostalCodes = cache.GetData<List<LocationDbo>>(CacheKeys.PostalCodeKey);

            if (cachedPostalCodes == null)
            {
                var cacheSettings = new CacheSettings("cacheDurationKey", "cacheExpirationKey");
                cachedPostalCodes = unitOfWork.LocationRepository.FindAll().ToList();
                cache.Insert(CacheKeys.PostalCodeKey, cachedPostalCodes, null, cacheSettings);
            }

            var location = cachedPostalCodes.Where(x => x.PostalCode.StartsWith(prefix)
                || x.PostalCode.Replace(" ", "").StartsWith(prefix.Replace(" ", "")) || x.City.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                || x.CityAlias.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || x.DistrictAlias.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Take(10).AsEnumerable()
                .Select(loc => new
                {
                    val = loc.PostalCode,
                    label = $"{loc.PostalCode} - {loc.District} - {loc.City}"
                }).ToList();

            return Json(location, JsonRequestBehavior.AllowGet);
        }

        private async Task<bool> GetToken(int userRoleId = 0, int companyRoleId = 0)
        {
            var client = new HttpClient();
            string authServerBaseUri = configuration.GetValueByKey("AuthServerBaseUri");
            client.BaseAddress = new Uri(authServerBaseUri);

            var response = await client.GetAsync($"api/OAuth/GetLoginToken?appId=123&userRoleId={userRoleId}&companyRoleId={companyRoleId}");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var authCookie = new HttpCookie("tokenCookie", content) { HttpOnly = true };
                HttpContext.Response.AppendCookie(authCookie);

                return true;
            }

            return false;
        }

        private UserDetailDbo GetUserByEmail(string email)
        {
            return unitOfWork.UserDetailsRepository.FindFirstOrDefault(ud => ud.Email == email);
        }

        private int? GetRoleIdByName(string role, int companyId)
        {
            return unitOfWork.CompanyRoleRepository
                .FindFirstOrDefault(cr => cr.Name == role.ToString() && (cr.CreatedByCompId == null || cr.CreatedByCompId == companyId))?.Id;
        }

        private int GetCompanyId()
        {
            var session = Session["User"] as SessionModel;
            int companyId = session.Companies.FirstOrDefault(c => c.IsActive).Id;
            return companyId;
        }
    }
}