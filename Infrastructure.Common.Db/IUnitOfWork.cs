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
        IRepository<UserDetailsDbo> UserDetailsRepository { get; }
        IRepository<UserRoleDbo> UserRoleRepository { get; }
        IRepository<BatchAttachmentDbo> BatchAttachmentRepository { get; }
        IRepository<AttachmentDbo> AttachmentRepository { get; }
        IRepository<CompanyDetailsDbo> CompanyDetailsRepositry { get; }

        void Commit();
    }
}
