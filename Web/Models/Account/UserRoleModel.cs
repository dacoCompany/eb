using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class UserRoleModel
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        public int SelectedRoleId { get; set; }
    }
}