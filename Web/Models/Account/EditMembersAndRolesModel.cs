using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.eBado.Models.Account
{
    public class EditMembersAndRolesModel
    {
        public string MemberEmail { get; set; }
        public int SelectedRoleId { get; set; }
        public IEnumerable<SelectListItem> AllRoles { get; set; }
        public string RoleName { get; set; }
        public CompanyPermissionsModel Permissions { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }


    }
}