using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Web.eBado.Models.Account
{
    public class NotificationModel
    {
        public NotificationModel()
        {
            MembersNotifications = new Collection<CompanyMemberNotificationModel>();
        }

        public bool NotifyCommentOnContribution { get; set; }
        public bool NotifyCommentOnAccount { get; set; }
        public bool NotifyAllMember { get; set; }
        public ICollection<CompanyMemberNotificationModel> MembersNotifications { get; set; }
    }
}