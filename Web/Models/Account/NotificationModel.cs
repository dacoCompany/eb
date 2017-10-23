using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.eBado.Models.Account
{
    public class NotificationModel
    {
        public bool NotifyCommentOnContribution { get; set; }
        public bool NotifyCommentOnAccount { get; set; }
        public bool NotifyAllMember { get; set; }
        public string[] SelectedMembers { get; set; }
    }
}