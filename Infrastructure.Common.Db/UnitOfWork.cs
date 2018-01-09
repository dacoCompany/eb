using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace Infrastructure.Common.DB
{
    /// <summary>
    /// Implementation of Unit of Work
    /// </summary>
    /// <seealso cref="Infrastructure.Common.DB.IUnitOfWork" />
    public class UnitOfWork : IUnitOfWork
    {

        private DbContext context = new EBADOEntitiesTest(@"metadata=res://*/EBADOModel.csdl|res://*/EBADOModel.ssdl|res://*/EBADOModel.msl;provider=System.Data.SqlClient;provider connection string='data source=ebadodbsrv.database.windows.net;initial catalog=testDB;persist security info=True;user id=ebadoadmin;password=ebado.159;MultipleActiveResultSets=True;App=EntityFramework'");
        //private DbContext context = new EBADOEntitiesTest(@"metadata=res://*/EBADOModel.csdl|res://*/EBADOModel.ssdl|res://*/EBADOModel.msl;provider=System.Data.SqlClient;provider connection string='data source=.;initial catalog=testDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework'");
        private bool disposed = false;
        private Repository<CompanyTypeDbo> companyTypeRepository;
        private Repository<AddressDbo> addressRepository;
        private Repository<CategoryDbo> categoryRepository;
        private Repository<LocationDbo> locationRepository;
        private Repository<SubCategoryDbo> subCategoryRepository;
        private Repository<UserDetailDbo> userDetailsRepository;
        private Repository<UserRoleDbo> userRoleRepository;
        private Repository<BatchAttachmentDbo> batchAttachmentRepository;
        private Repository<AttachmentDbo> attachmentRepository;
        private Repository<CompanyDetailDbo> companyDetailsRespository;
        private Repository<CompanyDetails2UserDetailsDbo> companyDetails2UserDetailsRespository;
        private Repository<Category2CompanyDetailsDbo> category2CompanyDetailsRespository;
        private Repository<CompanyPermissionDbo> companyPermissionsRespository;
        private Repository<CompanyRoleDbo> companyRoleRespository;
        private Repository<CompanyRole2CompanyPermissionDbo> companyRole2CompanyPermissionsRespository;
        private Repository<SubCategory2CompanyDetailsDbo> subCategory2CompanyDetailsRespository;
        private Repository<UserPermissionDbo> userPermissionsRespository;
        private Repository<UserRole2UserPermissionDbo> userRole2UserPermissionsRespository;
        private Repository<LanguageDbo> languageRespository;
        private Repository<CompanyDetails2LanguagesDbo> companyDetails2LanguagesRespository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        public UnitOfWork()
        {
        }

        /// <summary>
        /// Gets the account type repository.
        /// </summary>
        /// <value>
        /// The account type repository.
        /// </value>
        public IRepository<CompanyTypeDbo> CompanyTypeRepository =>
            companyTypeRepository ?? (companyTypeRepository = new Repository<CompanyTypeDbo>(context));

        /// <summary>
        /// Gets the address repository.
        /// </summary>
        /// <value>
        /// The address repository.
        /// </value>
        public IRepository<AddressDbo> AddressRepository =>
            addressRepository ?? (addressRepository = new Repository<AddressDbo>(context));

        /// <summary>
        /// Gets the category repository.
        /// </summary>
        /// <value>
        /// The category repository.
        /// </value>
        public IRepository<CategoryDbo> CategoryRepository =>
            categoryRepository ?? (categoryRepository = new Repository<CategoryDbo>(context));

        /// <summary>
        /// Gets the location repository.
        /// </summary>
        /// <value>
        /// The location repository.
        /// </value>
        public IRepository<LocationDbo> LocationRepository =>
            locationRepository ?? (locationRepository = new Repository<LocationDbo>(context));

        /// <summary>
        /// Gets the sub category repository.
        /// </summary>
        /// <value>
        /// The sub category repository.
        /// </value>
        public IRepository<SubCategoryDbo> SubCategoryRepository =>
            subCategoryRepository ?? (subCategoryRepository = new Repository<SubCategoryDbo>(context));

        /// <summary>
        /// Gets the user account repository.
        /// </summary>
        /// <value>
        /// The user account repository.
        /// </value>
        public IRepository<UserDetailDbo> UserDetailsRepository =>
            userDetailsRepository ?? (userDetailsRepository = new Repository<UserDetailDbo>(context));

        /// <summary>
        /// Gets the user role repository.
        /// </summary>
        /// <value>
        /// The user role repository.
        /// </value>
        public IRepository<UserRoleDbo> UserRoleRepository =>
            userRoleRepository ?? (userRoleRepository = new Repository<UserRoleDbo>(context));

        /// <summary>
        /// Gets the batch attachment repository.
        /// </summary>
        public IRepository<BatchAttachmentDbo> BatchAttachmentRepository =>
            batchAttachmentRepository ?? (batchAttachmentRepository = new Repository<BatchAttachmentDbo>(context));

        /// <summary>
        /// Gets the attachment repository.
        /// </summary>
        public IRepository<AttachmentDbo> AttachmentRepository =>
            attachmentRepository ?? (attachmentRepository = new Repository<AttachmentDbo>(context));

        /// <summary>
        /// Gets or sets the company details repository.
        /// </summary>
        public IRepository<CompanyDetailDbo> CompanyDetailsRepository =>
            companyDetailsRespository ?? (companyDetailsRespository = new Repository<CompanyDetailDbo>(context));

        /// <summary>
        /// Gets or sets the company details to user details repository.
        /// </summary>
        public IRepository<CompanyDetails2UserDetailsDbo> CompanyDetails2UserDetailsRepository =>
            companyDetails2UserDetailsRespository ?? (companyDetails2UserDetailsRespository = new Repository<CompanyDetails2UserDetailsDbo>(context));

        public IRepository<Category2CompanyDetailsDbo> Category2CompanyDetailsRepository =>
            category2CompanyDetailsRespository ?? (category2CompanyDetailsRespository = new Repository<Category2CompanyDetailsDbo>(context));

        public IRepository<CompanyRoleDbo> CompanyRoleRepository =>
            companyRoleRespository ?? (companyRoleRespository = new Repository<CompanyRoleDbo>(context));

        public IRepository<CompanyRole2CompanyPermissionDbo> CompanyRole2CompanyPermissionsRepository =>
            companyRole2CompanyPermissionsRespository ?? (companyRole2CompanyPermissionsRespository = new Repository<CompanyRole2CompanyPermissionDbo>(context));

        public IRepository<SubCategory2CompanyDetailsDbo> SubCategory2CompanyDetailsRepository =>
            subCategory2CompanyDetailsRespository ?? (subCategory2CompanyDetailsRespository = new Repository<SubCategory2CompanyDetailsDbo>(context));

        public IRepository<CompanyPermissionDbo> CompanyPermissionsRepository =>
            companyPermissionsRespository ?? (companyPermissionsRespository = new Repository<CompanyPermissionDbo>(context));

        public IRepository<UserPermissionDbo> UserPermissionsRepository =>
            userPermissionsRespository ?? (userPermissionsRespository = new Repository<UserPermissionDbo>(context));

        public IRepository<UserRole2UserPermissionDbo> UserRole2UserPermissionsRepository =>
            userRole2UserPermissionsRespository ?? (userRole2UserPermissionsRespository = new Repository<UserRole2UserPermissionDbo>(context));

        public IRepository<LanguageDbo> LanguageRepository =>
           languageRespository ?? (languageRespository = new Repository<LanguageDbo>(context));

        public IRepository<CompanyDetails2LanguagesDbo> CompanyDetails2LanguagesRepository =>
           companyDetails2LanguagesRespository ?? (companyDetails2LanguagesRespository = new Repository<CompanyDetails2LanguagesDbo>(context));


        /// <summary>
        /// Implementation of commit method
        /// </summary>
        public void Commit()
        {
            try
            {
                var ctx = ((IObjectContextAdapter)context).ObjectContext;

                // detect changes in db context and filter modified and added entities
                ctx.DetectChanges();
                var modifiedEntries = ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified);
                var addedEntries = ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added);
                var timestamp = DateTime.Now;

                // set DateModified for modified entities
                foreach (var entry in modifiedEntries)
                {
                    var entity = entry.Entity as IEntity;

                    if (entity != null)
                    {
                        entity.DateModified = timestamp;
                    }
                }

                // set DateCreated and IsActive for new entities
                foreach (var entry in addedEntries)
                {
                    var entity = entry.Entity as IEntity;

                    if (entity != null)
                    {
                        entity.DateCreated = timestamp;
                        entity.IsActive = true;
                    }
                }

                ctx.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                //StringBuilder sb = new StringBuilder();

                //foreach (var failure in ex.EntityValidationErrors)
                //{
                //    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());

                //    foreach (var error in failure.ValidationErrors)
                //    {
                //        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                //        sb.AppendLine();
                //    }
                //}

                //throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex); // Add the original exception as the innerException
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
