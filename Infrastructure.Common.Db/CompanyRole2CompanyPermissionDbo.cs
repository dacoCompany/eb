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

    public partial class CompanyRole2CompanyPermissionDbo : IEntity
    {
        public int Id { get; set; }
        public Nullable<int> CompanyRoleId { get; set; }
        public Nullable<int> CompanyPermissionId { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }

        public virtual CompanyPermissionDbo CompanyPermission { get; set; }
        public virtual CompanyRoleDbo CompanyRole { get; set; }
    }
}
