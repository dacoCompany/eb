using BackgroundProcessing.Common;
using Hangfire;
using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Configuration;
using Messaging.Email.Models;
using RazorEngine.Templating;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Messaging.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private MailMessage message;
        private SmtpClient client;
        private IJobClient jobClient;

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
        /// </summary>
        public SmtpEmailSender(IJobClient jobClient)
        {
            this.jobClient = jobClient;

            client = new SmtpClient();
            message = new MailMessage();
            Configure();
        }

        /// <summary>
        /// Sends email message
        /// </summary>
        public void Send(MailMessageType messageType, UserDetailDbo userDetail)
        {
            jobClient.Enqueue(() => SendToQueue(messageType, userDetail));
        }

        /// <summary>
        /// Sends to queue.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="userDetail">The user detail.</param>
        [Queue("messaging")]
        public void SendToQueue(MailMessageType messageType, UserDetailDbo userDetail)
        {
            CreateMessage(messageType, userDetail);
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
        private void CreateMessage(MailMessageType messageType, UserDetailDbo userAccount)
        {
            string displayName = ConfigurationManager.AppSettings.Get(ConfigurationKeys.EmailDisplayName);
            string from = ConfigurationManager.AppSettings.Get(ConfigurationKeys.EmailFrom);

            MailAddress sender = new MailAddress(from, displayName);
            MailAddress recepient = new MailAddress(userAccount.Email);

            message.From = sender;
            message.To.Add(recepient);
            message.IsBodyHtml = true;

            switch (messageType)
            {
                case MailMessageType.ForgotPassword:
                    break;
                case MailMessageType.Registration:
                    var model = CreateRegistrationCompleteModel(userAccount);
                    message.Body = GetHtmlBodyString(messageType, model);
                    message.Subject = "Registracia";

                    break;
            }
        }

        /// <summary>
        /// Creates the forgot password model.
        /// </summary>
        /// <param name="userAccount">The user account.</param>
        /// <returns>
        /// Model
        /// </returns>

        private RegisterEmailModel CreateRegistrationCompleteModel(UserDetailDbo userAccount)
        {
            return new RegisterEmailModel
            {
                CompanyName = userAccount.ToString(),
                FirstName = userAccount.FirstName,
                LastName = userAccount.Surname,
                Login = userAccount.Email,
                Password = userAccount.Password,
                Title = userAccount.Title
            };
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
        private string GetHtmlBodyString<T>(MailMessageType messageType, T model) where T : new()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Views\Email", string.Concat(messageType.ToString(), ".cshtml"));
            var templateContent = File.ReadAllText(filePath);
            return RazorEngine.Engine.Razor.RunCompile(templateContent, messageType.ToString(), typeof(T), model);
        }
    }
}
