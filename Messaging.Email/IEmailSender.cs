using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using System;
using System.Net.Mail;

namespace Messaging.Email
{
    public interface IEmailSender : IDisposable
    {
        /// <summary>
        /// Sends email message synchronosly
        /// </summary>
        void Send(MailMessageType messageType, UserDetailDbo userDetail);
    }
}
