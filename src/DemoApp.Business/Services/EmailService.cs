using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApp.Business.Helpers;

namespace DemoApp.Business.Services
{
    #region Interface

    [AspNetCore.ServiceRegistration.Dynamic.Attributes.TransientService]
    public interface IEmailService : IServiceBase
    {
        Task<bool> SendPasswordResetEmailAsync(string mailId, string userId, string code);
        Task<bool> SendConfirmationEmailAsync(string mailId, string userId, string code);
        Task<bool> SendAccountCreationEmailAsync(string mailId);
    }

    #endregion

    /// <summary>
    /// This class is used by the application to send Email
    /// For Confirming mail id, reset password etc.
    /// </summary>
    public class EmailService : ServiceBase<EmailService>, IEmailService
    {
        #region Private Properties & Constructor

        private readonly IMailHelper _mailHelper;

        public EmailService(IMailHelper mailHelper, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _mailHelper = mailHelper;
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Send mail containing confirm link on signup
        /// </summary>
        public Task<bool> SendConfirmationEmailAsync(string mailId, string userId, string code)
        {
            List<string> to = new List<string> { mailId };
            string subject = "Confirm Mail Id";
            string templateName = "ConfirmMail";
            string dashboadHome = _appSettings.WebsiteDetails.DashboadHome;
            string encodedCode = System.Web.HttpUtility.UrlEncode(code);
            string confirmationLink = $"{dashboadHome}confirmation/{userId}/{encodedCode}";
            Dictionary<string, string> placeHolders = new Dictionary<string, string>
            {
                { "ConfirmationLink", confirmationLink },
                { "WebsiteTitle", _appSettings.WebsiteDetails.Title },
                { "EmailId", mailId }
            };

            return _mailHelper.SendEmailAsync(to, subject, templateName, placeHolders);
        }

        /// <summary>
        /// Send Reset password link when user forgot password
        /// </summary>
        public Task<bool> SendPasswordResetEmailAsync(string mailId, string userId, string code)
        {
            List<string> to = new List<string> { mailId };
            string subject = "Reset Password";
            string templateName = "ResetPassword";
            string dashboadHome = _appSettings.WebsiteDetails.DashboadHome;
            string encodedCode = System.Web.HttpUtility.UrlEncode(code);
            string resetPasswordLink = $"{dashboadHome}reset-password/{userId}/{encodedCode}";
            Dictionary<string, string> placeHolders = new Dictionary<string, string>
            {
                { "ResetPasswordLink", resetPasswordLink },
                { "WebsiteTitle", _appSettings.WebsiteDetails.Title },
                { "EmailId", mailId }
            };

            return _mailHelper.SendEmailAsync(to, subject, templateName, placeHolders);
        }

        /// <summary>
        /// Send Account created mail
        /// </summary>
        public Task<bool> SendAccountCreationEmailAsync(string mailId)
        {
            List<string> to = new List<string> { mailId };
            string subject = "Account Confirmed!";
            string templateName = "AccountConfirmed";
            string dashboadHome = _appSettings.WebsiteDetails.DashboadHome;
            string loginLink = $"{dashboadHome}";
            Dictionary<string, string> placeHolders = new Dictionary<string, string>
            {
                { "EmailId", mailId },
                { "LoginLink", loginLink }
            };

            return _mailHelper.SendEmailAsync(to, subject, templateName, placeHolders);
        }

        #endregion
    }
}
