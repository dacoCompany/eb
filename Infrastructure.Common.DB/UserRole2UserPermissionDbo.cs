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
    
    public partial class UserRole2UserPermissionDbo : IEntity
    {
        public int Id { get; set; }
        public Nullable<int> UserRoleId { get; set; }
        public Nullable<int> UserPermissionId { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
    
        public virtual UserPermissionDbo UserPermission { get; set; }
        public virtual UserRoleDbo UserRole { get; set; }
    }
}
