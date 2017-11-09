using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Infrastructure.Common.DB;
using WebApi.Authorization.Models;

namespace WebApi.Authorization.Controllers
{
    public class OAuthController : ApiController
    {
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetLoginToken(string appId, int userRoleId, int companyRoleId)
        {
            var tokenModel = new TokenValues();
            tokenModel.Issuer = GetConfigValue("issuer", "http://localhost");
            tokenModel.Audience = GetConfigValue("audience", "audience");
            tokenModel.ExpiredDate = DateTime.Now.AddDays(GetConfigValue("expiration", 7));
            tokenModel.Secret = GetConfigValue("secret", "This is a secret.");
            
            ICollection<string> permissions = new Collection<string>();

            using (var uow = new UnitOfWork())
            {
                if (userRoleId != 0)
                {
                    var role = uow.UserRoleRepository.FindWhere(ur => ur.Id == userRoleId).Include(ur => ur.UserRole2UserPermission.Select(r2p => r2p.UserPermission)).FirstOrDefault();
                    permissions = role.UserRole2UserPermission.Select(r2p => r2p.UserPermission.Name).ToList();
                }
                else if (companyRoleId != 0)
                {
                    var role = uow. CompanyRoleRepository.FindWhere(cr => cr.Id == companyRoleId).Include(ur => ur.CompanyRole2CompanyPermission.Select(r2p => r2p.CompanyPermission)).FirstOrDefault();
                    permissions = role.CompanyRole2CompanyPermission.Select(r2p => r2p.CompanyPermission.Name).ToList();
                }
            }

            if (permissions.Count == 0)
            {
                return Unauthorized();
            }

            foreach (string permission in permissions)
            {
                tokenModel.Claims.Add(new Claim("role", permission));
            }

            string jwtToken = await Generate(tokenModel);

            return Ok(jwtToken);
        }

        public Task<string> Generate(TokenValues values)
        {
            string tokenValue = null;

            return Task.Run(() =>
            {
                var credentials = new SigningCredentials(new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(values.Secret)),
                        SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
                
                JwtSecurityToken token = new JwtSecurityToken(values.Issuer, values.Audience, values.Claims, DateTime.Now, values.ExpiredDate, credentials);

                tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            }).ContinueWith(x => tokenValue);
        }

        private string GetConfigValue(string key, string defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];

            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        private int GetConfigValue(string key, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];

            return int.TryParse(value, out int result) ? result : defaultValue;
        }
    }
}
