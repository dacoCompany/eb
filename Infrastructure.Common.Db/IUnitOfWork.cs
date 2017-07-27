using System;

namespace Infrastructure.Common.DB
{
    /// <summary>
    /// Interface of unit of work pattern
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IUnitOfWork : IDisposable
    {
        IRepository<AccountTypeDbo> AccountTypeRepository { get; }
        IRepository<AddressDbo> AddressRepository { get; }
        IRepository<CategoryDbo> CategoryRepository { get; }
        IRepository<LocationDbo> LocationRepository { get; }
        IRepository<MainCategoryDbo> MainCategoryRepository { get; }
        IRepository<SubCategoryDbo> SubCategoryRepository { get; }
        IRepository<UserAccountDbo> UserAccountRepository { get; }
        IRepository<UserRoleDbo> UserRoleRepository { get; }
        IRepository<BatchAttachmentDbo> BatchAttachmentRepository { get; }
        IRepository<AttachmentDbo> AttachmentRepository { get; }

        void Commit();
    }
}
