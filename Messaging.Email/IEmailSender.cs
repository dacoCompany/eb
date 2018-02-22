using Infrastructure.Common.Enums;
using Messaging.Email.Models;
using System;

namespace Messaging.Email
{
    public interface IEmailSender : IDisposable
    {
        /// <summary>
        /// Sends email message synchronosly
        /// </summary>
        void Send<T>(MailMessageType messageType, T model) where T : BaseEmailModel;
    }
}
