using System;

namespace Infrastructure.Common.DB
{
    /// <summary>
    /// Interface of unit of work pattern
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWork : IDisposable
    {
        IRepository<CompanyTypeDbo> CompanyTypeRepository { get; }
        IRepository<AddressDbo> AddressRepository { get; }
        IRepository<CategoryDbo> CategoryRepository { get; }
        IRepository<LocationDbo> LocationRepository { get; }
        IRepository<SubCategoryDbo> SubCategoryRepository { get; }
        IRepository<UserDetailDbo> UserDetailsRepository { get; }
        IRepository<UserRoleDbo> UserRoleRepository { get; }
        IRepository<BatchAttachmentDbo> BatchAttachmentRepository { get; }
        IRepository<AttachmentDbo> AttachmentRepository { get; }
        IRepository<CompanyDetailDbo> CompanyDetailsRepository { get; }
        IRepository<CompanyDetails2UserDetailsDbo> CompanyDetails2UserDetailsRepository { get; }
        IRepository<CompanyPermissionDbo> CompanyPermissionsRepository { get; }
        IRepository<CompanyRoleDbo> CompanyRoleRepository { get; }
        IRepository<CompanyRole2CompanyPermissionDbo> CompanyRole2CompanyPermissionsRepository { get; }
        IRepository<SubCategory2CompanyDetailsDbo> SubCategory2CompanyDetailsRepository{ get; }
        IRepository<UserPermissionDbo> UserPermissionsRepository { get; }
        IRepository<UserRole2UserPermissionDbo> UserRole2UserPermissionsRepository { get; }
        IRepository<LanguageDbo> LanguageRepository { get; }
        IRepository<CompanyDetails2LanguagesDbo> CompanyDetails2LanguagesRepository { get; }

        void Commit();
    }
}
