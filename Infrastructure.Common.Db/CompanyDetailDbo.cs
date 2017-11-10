//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class CompanyDetailDbo : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanyDetailDbo()
        {
            this.Addresses = new HashSet<AddressDbo>();
            this.BatchAttachments = new HashSet<BatchAttachmentDbo>();
            this.Category2CompanyDetails = new HashSet<Category2CompanyDetailsDbo>();
            this.CompanyDetails2UserDetails = new HashSet<CompanyDetails2UserDetailsDbo>();
            this.SubCategory2CompanyDetails = new HashSet<SubCategory2CompanyDetailsDbo>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string Email { get; set; }
        public Nullable<int> Ico { get; set; }
        public Nullable<int> Dic { get; set; }
        public bool IsCompanyVerified { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public int CompanyTypeId { get; set; }
        public int CompanySettingId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AddressDbo> Addresses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchAttachmentDbo> BatchAttachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category2CompanyDetailsDbo> Category2CompanyDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyDetails2UserDetailsDbo> CompanyDetails2UserDetails { get; set; }
        public virtual CompanySettingDbo CompanySetting { get; set; }
        public virtual CompanyTypeDbo CompanyType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubCategory2CompanyDetailsDbo> SubCategory2CompanyDetails { get; set; }
    }
}
