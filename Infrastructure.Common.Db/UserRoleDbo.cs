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
    
    public partial class UserRoleDbo : IEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserRoleDbo()
        {
            this.UserRole2UserPermission = new HashSet<UserRole2UserPermissionDbo>();
            this.UserDetails = new HashSet<UserDetailsDbo>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRole2UserPermissionDbo> UserRole2UserPermission { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDetailsDbo> UserDetails { get; set; }
    }
}
