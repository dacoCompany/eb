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
    
    public partial class CompanySettingDbo : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanySettingDbo()
        {
            this.CompanyDetails = new HashSet<CompanyDetailDbo>();
        }
    
        public int Id { get; set; }
        public Nullable<int> SearchRadius { get; set; }
        public bool SearchInSK { get; set; }
        public bool SearchInCZ { get; set; }
        public bool SearchInHU { get; set; }
        public bool NotifyCommentOnContribution { get; set; }
        public bool NotifyCommentOnAccount { get; set; }
        public bool NotifyAllMember { get; set; }
        public string NotificationEmail { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public string Language { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyDetailDbo> CompanyDetails { get; set; }
    }
}
