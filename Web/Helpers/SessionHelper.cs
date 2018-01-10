using Infrastructure.Common.DB;
using System.Data.Entity;
using System.Linq;
using Web.eBado.Models.Shared;

namespace Web.eBado.Helpers
{
    public class SessionHelper
    {
        private readonly IUnitOfWork unitOfWork;

        public SessionHelper(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public SessionModel SetCompanySession(string accountName, SessionModel currentSession)
        {
            int companyId = currentSession.Companies.FirstOrDefault(c => c.Name == accountName).Id;
            var companyDetailDbo = unitOfWork.CompanyDetailsRepository.FindById(companyId);
            var userDetailDbo = companyDetailDbo.CompanyDetails2UserDetails.FirstOrDefault(cd => cd.UserDetailsId == currentSession.Id).UserDetail;

            var newSession = new SessionModel
            {
                Id = userDetailDbo.Id,
                Email = userDetailDbo.Email,
                HasCompany = userDetailDbo.CompanyDetails2UserDetails.Any(cd => cd.IsActive),
                Name = userDetailDbo.DisplayName,
                UserRole = userDetailDbo.UserRole.Name,
                ProfileUrl = userDetailDbo.ProfilePictureUrlSmall,
                UserPermissions = userDetailDbo.UserRole.UserRole2UserPermission.Select(ur => ur.UserPermission.Name)
            };
            CompanySessionModel companySession = null;
            foreach (var company in userDetailDbo.CompanyDetails2UserDetails.Where(cd => cd.IsActive && cd.CompanyDetail != companyDetailDbo))
            {
                var companyDetail = company.CompanyDetail;
                var companyRole = company.CompanyRole;
                companySession = new CompanySessionModel()
                {
                    Id = companyDetail.Id,
                    Name = companyDetail.Name,
                    CompanyRole = companyRole.Name,
                    ProfileUrl = companyDetail.ProfilePictureUrlSmall,
                    CompanyPermissions = companyRole.CompanyRole2CompanyPermission.Select(cr => cr.CompanyPermission.Name)
                };
                newSession.Companies.Add(companySession);
            }
            var companyRoleDbo = companyDetailDbo.CompanyDetails2UserDetails
                .FirstOrDefault(cd => cd.IsActive && cd.UserDetail == userDetailDbo && cd.CompanyDetail == companyDetailDbo).CompanyRole;

            companySession = new CompanySessionModel()
            {
                Id = companyDetailDbo.Id,
                IsActive = true,
                Name = companyDetailDbo.Name,
                CompanyRole = companyRoleDbo.Name,
                ProfileUrl = companyDetailDbo.ProfilePictureUrlSmall,
                CompanyPermissions = companyRoleDbo.CompanyRole2CompanyPermission.Select(cr => cr.CompanyPermission.Name)
            };
            newSession.Companies.Add(companySession);
            return newSession;
        }

        public SessionModel SetUserSession(int userId)
        {
            var userDetailDbo = unitOfWork.UserDetailsRepository.FindWhere(ud => ud.Id == userId)
                 .Include(ud => ud.UserRole.UserRole2UserPermission.Select(ur => ur.UserPermission))
                 .Include(ud => ud.CompanyDetails2UserDetails.Select(cd => cd.CompanyDetail))
                 .Include(ud => ud.CompanyDetails2UserDetails.Select(cd => cd.CompanyRole.CompanyRole2CompanyPermission
                 .Select(cr => cr.CompanyPermission))).FirstOrDefault();

            var userRole = userDetailDbo.UserRole;
            var newSession = new SessionModel
            {
                Id = userDetailDbo.Id,
                Email = userDetailDbo.Email,
                HasCompany = userDetailDbo.CompanyDetails2UserDetails.Any(cd => cd.IsActive),
                IsActive = true,
                Name = userDetailDbo.DisplayName,
                UserRole = userRole.Name,
                ProfileUrl = userDetailDbo.ProfilePictureUrlSmall,
                UserPermissions = userRole.UserRole2UserPermission.Select(ur => ur.UserPermission.Name)
            };
            foreach (var company in userDetailDbo.CompanyDetails2UserDetails.Where(cd => cd.IsActive))
            {
                var companyDetail = company.CompanyDetail;
                var companyRole = company.CompanyRole;
                CompanySessionModel companySession = new CompanySessionModel()
                {
                    Id = companyDetail.Id,
                    Name = companyDetail.Name,
                    CompanyRole = companyRole.Name,
                    ProfileUrl = companyDetail.ProfilePictureUrlSmall,
                    CompanyPermissions = companyRole.CompanyRole2CompanyPermission.Select(cr => cr.CompanyPermission.Name)
                };
                newSession.Companies.Add(companySession);
            }

            return newSession;
        }
    }
}