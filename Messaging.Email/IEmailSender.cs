using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using System;

namespace Messaging.Email
{
    public interface IEmailSender : IDisposable
    {
        /// <summary>
        /// Creates and configure email message instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="user">The user.</param>
        void CreateMessage(MailMessageType type, UserAccountDbo user);
        /// <summary>
        /// Sends email message synchronosly
        /// </summary>
        void Send();
        /// <summary>
        /// Sends email message asynchronously
        /// </summary>
        void SendAsync();
    }
}
