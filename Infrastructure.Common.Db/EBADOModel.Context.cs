﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infrastructure.Common.DB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EBADOEntitiesTest : DbContext
    {
        public EBADOEntitiesTest(string connectionString) : base (connectionString)
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AddressDbo> AddressDboes { get; set; }
        public virtual DbSet<AttachmentDbo> AttachmentDboes { get; set; }
        public virtual DbSet<BatchAttachmentDbo> BatchAttachmentDboes { get; set; }
        public virtual DbSet<CategoryDbo> CategoryDboes { get; set; }
        public virtual DbSet<Category2CompanyDetailsDbo> Category2CompanyDetailsDbo { get; set; }
        public virtual DbSet<CompanyDetailDbo> CompanyDetailDboes { get; set; }
        public virtual DbSet<CompanyDetails2UserDetailsDbo> CompanyDetails2UserDetailsDbo { get; set; }
        public virtual DbSet<CompanyPermissionDbo> CompanyPermissionDboes { get; set; }
        public virtual DbSet<CompanyRoleDbo> CompanyRoleDboes { get; set; }
        public virtual DbSet<CompanyRole2CompanyPermissionDbo> CompanyRole2CompanyPermissionDbo { get; set; }
        public virtual DbSet<CompanySettingDbo> CompanySettingDboes { get; set; }
        public virtual DbSet<CompanyTypeDbo> CompanyTypeDboes { get; set; }
        public virtual DbSet<SubCategoryDbo> SubCategoryDboes { get; set; }
        public virtual DbSet<SubCategory2CompanyDetailsDbo> SubCategory2CompanyDetailsDbo { get; set; }
        public virtual DbSet<UserDetailDbo> UserDetailDboes { get; set; }
        public virtual DbSet<UserPermissionDbo> UserPermissionDboes { get; set; }
        public virtual DbSet<UserRoleDbo> UserRoleDboes { get; set; }
        public virtual DbSet<UserRole2UserPermissionDbo> UserRole2UserPermissionDbo { get; set; }
        public virtual DbSet<UserSettingDbo> UserSettingDboes { get; set; }
        public virtual DbSet<AllDeviceDbo> AllDeviceDboes { get; set; }
        public virtual DbSet<LocationDbo> LocationDboes { get; set; }
    }
}
