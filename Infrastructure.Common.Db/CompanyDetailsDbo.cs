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

    public partial class CompanyDetailsDbo : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanyDetailsDbo()
        {
            this.Addresses = new HashSet<AddressDbo>();
            this.BatchAttachments = new HashSet<BatchAttachmentDbo>();
            this.CompanyDetails2UserDetails = new HashSet<CompanyDetails2UserDetailsDbo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> PhoneNumber { get; set; }
        public Nullable<int> AdditionalPhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public int CompanyTypeId { get; set; }
        public int SubCategoryId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AddressDbo> Addresses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BatchAttachmentDbo> BatchAttachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyDetails2UserDetailsDbo> CompanyDetails2UserDetails { get; set; }
        public virtual CompanyTypeDbo CompanyType { get; set; }
        public virtual SubCategoryDbo SubCategory { get; set; }
    }
}
