using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApp.Models.AppSettings;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DemoApp.Business.Helpers
{
    #region Interface

    [AspNetCore.ServiceRegistration.Dynamic.Attributes.TransientService]
    public interface IMailHelper
    {
        Task<bool> SendEmailAsync(List<string> recipients, string subject, string templateName, Dictionary<string, string> placeHolders = null, string plainTextBody = null);
    }

    #endregion

    public class MailHelper : IMailHelper
    {
        #region Private Properties & Constructor

        private readonly ILogger<MailHelper> _logger;
        private readonly AppSettings _appSettings;

        public MailHelper()
        {
        }

        public MailHelper(ILogger<MailHelper> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Send Email using SMTP configs
        /// </summary>
        /// <param name = "recipients" > List of email ids for recipient</param>
        /// <param name = "subject" > Mail subject text</param>
        /// <param name = "templateName" > Template name to be used for sending mail</param>
        /// <param name = "placeHolders" > Optional placeholders used in template</param>
        /// <param name = "plainTextBody" > Optional plain text body for mail</param>
        /// <returns>True if Status of email is 200</returns>
        public async Task<bool> SendEmailAsync(List<string> recipients, string subject, string templateName, Dictionary<string, string> placeHolders = null, string plainTextBody = null)
        {
            try
            {
                // Add From & Subject
                var configs = _appSettings.SendGridOptions;
                SendGridClient client = new SendGridClient(configs.ApiKey);
                EmailAddress from = new EmailAddress(configs.FromMailId, configs.FromDisplayName);

                // Add all recipients
                List<EmailAddress> tos = new List<EmailAddress>();
                foreach (var r in recipients)
                {
                    tos.Add(new EmailAddress(r));
                }

                // Generate Template
                string htmlBody = System.IO.File.ReadAllText($"wwwroot/MailTemplates/{templateName}.html");
                if (placeHolders != null)
                {
                    foreach (var item in placeHolders)
                    {
                        htmlBody = htmlBody.Replace($"{{{{ {item.Key} }}}}", item.Value);
                    }
                }
                if (string.IsNullOrEmpty(plainTextBody))
                {
                    // If plaintext is not pass then obtain it by striping html text from html body
                    plainTextBody = System.Text.RegularExpressions.Regex.Replace(htmlBody, "<.*?>", String.Empty);
                }

                // Sending Mail
                _logger.LogDebug($"Sending Mail For: {subject} to {string.Join(", ", recipients)}");
                var msg = SendGrid.Helpers.Mail.MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextBody, htmlBody);
                msg.SetReplyTo(new EmailAddress(configs.ReplyMailId));
                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    return true;
                else
                    _logger.LogError($"Error occurred to Send Mail For: {subject} to {string.Join(", ", recipients)} with HTTP status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return false;
        }

        #endregion
    }
}
