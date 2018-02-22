using BackgroundProcessing.Common;
using Hangfire;
using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Configuration;
using Messaging.Email.Models;
using RazorEngine.Templating;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Messaging.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private MailMessage message;
        private SmtpClient client;
        private IJobClient jobClient;
        private IUnitOfWork uow;

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
        /// </summary>
        public SmtpEmailSender(IJobClient jobClient, IUnitOfWork uow)
        {
            this.jobClient = jobClient;
            this.uow = uow;

            client = new SmtpClient();
            message = new MailMessage();
            Configure();
        }

        /// <summary>
        /// Sends email message
        /// </summary>
        public void Send<T>(MailMessageType messageType, T model) where T : BaseEmailModel
        {
            jobClient.Enqueue(() => ProcessInQueue(messageType, model));
        }

        /// <summary>
        /// Sends to queue.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="userDetail">The user detail.</param>
        [Queue("messaging")]
        public void ProcessInQueue<T>(MailMessageType messageType, T model)
        {
            CreateMessage(messageType, model);
            client.Send(message);
        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                message.Dispose();
                client.Dispose();
                jobClient = null;
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        /// <summary>
        /// Configures smtp client instance
        /// </summary>
        private void Configure()
        {
            client.Host = ConfigurationManager.AppSettings.Get(ConfigurationKeys.EmailServer);
            client.Port = ConfigurationManager.AppSettings.Get<int>(ConfigurationKeys.EmailPort);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential
            {
                UserName = ConfigurationManager.AppSettings.Get(ConfigurationKeys.EmailLogin),
                Password = ConfigurationManager.AppSettings.Get(ConfigurationKeys.EmailPassword)
            };
            client.EnableSsl = ConfigurationManager.AppSettings.Get<bool>(ConfigurationKeys.EmailUseSsl);
        }

        /// <summary>
        /// Creates and configure email message instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="userAccount">The user account.</param>
        private void CreateMessage<T>(MailMessageType messageType, T model)
        {
            var contactInfo = model as BaseEmailModel;
            string displayName = ConfigurationManager.AppSettings.Get(ConfigurationKeys.EmailDisplayName);
            string from = ConfigurationManager.AppSettings.Get(ConfigurationKeys.EmailFrom);

            MailAddress sender = new MailAddress(from, displayName);
            MailAddress recepient = new MailAddress(contactInfo.Login);

            message.From = sender;
            message.To.Add(recepient);
            message.IsBodyHtml = true;

            var userCulture = uow.UserDetailsRepository.FindFirstOrDefault(user => user.Email == contactInfo.Login).UserSetting.Language;
            var template = uow.EmailTemplateRepository.FindFirstOrDefault(temp => temp.Type == messageType.ToString() && temp.Language == userCulture);

            switch (messageType)
            {
                case MailMessageType.ForgotPassword:
                    break;
                case MailMessageType.Registration:
                    var templateModel = (model as RegisterEmailModel) ?? throw new InvalidCastException("Model does not equals to specified message type");
                    message.Body = GetHtmlBodyString(messageType, template.Body, templateModel);
                    message.Subject = template.Subject;

                    break;
            }
        }

        /// <summary>
        /// Gets the HTML body string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="model">The model.</param>
        /// <returns>
        /// HTML body string
        /// </returns>
        private string GetHtmlBodyString<T>(MailMessageType messageType, string template, T model) where T : new()
        {
            return RazorEngine.Engine.Razor.RunCompile(template, messageType.ToString(), typeof(T), model);
        }
    }
}
