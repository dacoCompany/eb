using Infrastructure.Common.DB;
using Infrastructure.Common.Enums;
using Infrastructure.Configuration;
using Messaging.Email.Models;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Messaging.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private MailMessage message;
        private SmtpClient client;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
        /// </summary>
        public SmtpEmailSender()
        {
            client = new SmtpClient();
            Configure();
        }

        /// <summary>
        /// Creates and configure email message instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="userAccount">The user account.</param>
        public void CreateMessage(MailMessageType type, UserAccountDbo userAccount)
        {
            message = new MailMessage();
            BuildMessage(type, userAccount);
        }

        /// <summary>
        /// Sends email message synchronously
        /// </summary>
        public void Send()
        {
            client.Send(message);
        }

        /// <summary>
        /// Sends email message asynchronously
        /// </summary>
        public void SendAsync()
        {
            client.SendAsync(message, null);
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
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        /// <summary>
        /// Configures smtp client instance
        /// </summary>
        private void Configure()
        {
            client.Host = EbadoConfiguration.AppSettings.Get(ConfigurationKeys.EmailServer);
            client.Port = EbadoConfiguration.AppSettings.Get<int>(ConfigurationKeys.EmailPort);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential
            {
                UserName = EbadoConfiguration.AppSettings.Get(ConfigurationKeys.EmailLogin),
                Password = EbadoConfiguration.AppSettings.Get(ConfigurationKeys.EmailPassword)
            };
            client.EnableSsl = EbadoConfiguration.AppSettings.Get<bool>(ConfigurationKeys.EmailUseSsl);
        }

        /// <summary>
        /// Builds the message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="userAccount">The user account.</param>
        /// TODO Need to create additional message types
        private void BuildMessage(MailMessageType messageType, UserAccountDbo userAccount, params object[] additionalParams)
        {
            string addressFrom = EbadoConfiguration.AppSettings.Get(ConfigurationKeys.EmailFrom);
            string displayName = EbadoConfiguration.AppSettings.Get(ConfigurationKeys.EmailDisplayName);

            message.From = new MailAddress(addressFrom, displayName);
            message.To.Add(new MailAddress(userAccount.Email));
            message.IsBodyHtml = true;

            switch (messageType)
            {
                case MailMessageType.ForgotPassword:
                    var model = CreateForgotPasswordModel(userAccount);
                    message.Body = GetHtmlBodyString(messageType, model);
                    break;
                case MailMessageType.RegisterConfirmation:

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

        private ForgotPasswordModel CreateForgotPasswordModel(UserAccountDbo userAccount)
        {
            return new ForgotPasswordModel
            {
                Name = "DummyName"
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
