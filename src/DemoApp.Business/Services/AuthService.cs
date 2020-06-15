using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoApp.DataAccess.Services;
using DemoApp.EntityFramework.Entities;
using DemoApp.EntityFramework.IdentityModels;
using DemoApp.Localization.Localizers.AuthLocalizer;
using DemoApp.Models.ApiModels.Auth;
using DemoApp.Models.Enums;
using DemoApp.Models.ServiceModels;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DemoApp.Business.Services
{
    #region Interface

    [AspNetCore.ServiceRegistration.Dynamic.Attributes.TransientService]
    public interface IAuthService : IServiceBase
    {
        Task<ServiceResultModel> RegisterUserAsync(RegisterUserDto model);
        Task<ServiceResultModel> ResendConfirmMailAsync(string emailId);
        Task<ServiceResultModel> ValidateConfirmMailTokenAsync(ValidateConfirmUserDto model);
        Task<ServiceResultModel> ConfirmUserAsync(ConfirmUserDto model);

        List<AllSecurityQuestionsDto> GetAllSecurityQuestions();
        Task<ServiceResultModel> GetSecurityQuestionsAsync(string mailId);
        Task<ServiceResultModel> VerifySecurityQuestionsAsync(UpdateSecurityQuestionDto model);
        Task<ServiceResultModel> UpdateSecurityQuestionsAsync(UpdateSecurityQuestionDto model);
        Task<bool> ForgotPasswordAsync(string mailId);
        Task<ServiceResultModel> ResetPasswordAsync(ResetPasswordDto model);
        Task<ServiceResultModel> ResetPasswordOnNextLoginAsync(ResetPasswordOnNextLoginDto model);

        Task<UserDetailsDto> GetUserDetailsAsync(AppUser user);
    }

    #endregion

    public class AuthService : ServiceBase<AuthService>, IAuthService
    {
        #region Private Properties & Constructor

        private readonly IEmailService _emailService;
        private readonly IDataRepository<MstSecurityQuestions> _securityQuestionRepo;
        private readonly IStringLocalizer _authLocalizer;

        public AuthService(IEmailService emailService, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _emailService = emailService;
            _securityQuestionRepo = _unitOfWork.GetRepository<MstSecurityQuestions>();
            _authLocalizer = _localizerFactory.Create(typeof(AuthLocalizer));
        }

        #endregion

        #region Implementation

        #region Create account related

        public async Task<ServiceResultModel> RegisterUserAsync(RegisterUserDto model)
        {
            AppUser newUser = _mapper.Map<AppUser>(model);

            // Create User
            var createUserResult = await _userManager.CreateAsync(newUser, model.Password);
            if (createUserResult.Succeeded)
            {
                // Send Confirmation link
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                await _emailService.SendConfirmationEmailAsync(model.Email, newUser.Id, code);

                // Add user in Role
                const string userRole = nameof(Roles.User);
                await _userManager.AddToRoleAsync(newUser, userRole);
                _logger.LogInformation($"User with MailId: {newUser.UserName} created successfully. And added in {userRole}");
                return new ServiceResultModel(true, newUser.Id);
            }
            return new ServiceResultModel(createUserResult);
        }

        public async Task<ServiceResultModel> ResendConfirmMailAsync(string emailId)
        {
            // Retrive User with given user id
            var user = await _userManager.FindByEmailAsync(emailId);
            if (user == null)
            {
                _logger.LogInformation($"Not found user with given Mail Id. MailId: {emailId}");
                return new ServiceResultModel { IsNotFound = true };
            }

            // Send Confirmation link
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            bool isSuccess = await _emailService.SendConfirmationEmailAsync(user.Email, user.Id, code);
            if (isSuccess)
            {
                _logger.LogInformation($"Resent confirmation link for: {user.Email}.");
                return new ServiceResultModel(true, _authLocalizer["ConfirmationMailSendSuccessfully"]);
            }
            else
            {
                _logger.LogError($"An error occurred to Resent confirmation link for: {user.Email}.");
                return new ServiceResultModel(false, _authLocalizer["ErrorOccurredToResendConfirmationMail"]);
            }
        }

        public async Task<ServiceResultModel> ValidateConfirmMailTokenAsync(ValidateConfirmUserDto model)
        {
            // Retrive User with given user id
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogInformation($"Not found user with given UserId. UserId: {model.UserId}");
                return new ServiceResultModel { IsNotFound = true };
            }

            // Check user is verified mail id or not
            if (user.EmailConfirmed)
            {
                _logger.LogDebug($"User already verified MailId. UserId: {model.UserId}");
                return new ServiceResultModel(false, _authLocalizer["AlreadyVerifiedMailId"]);
            }

            // Confirm based on code
            var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (confirmEmailResult.Succeeded)
            {
                user.EmailConfirmed = false;
                await _userManager.UpdateAsync(user);
                return new ServiceResultModel(true, _authLocalizer["SetSecurityQuetionsAndPasswordToConfirmYourAccount"]);
            }
            else
            {
                // If not match the return errors
                return new ServiceResultModel(confirmEmailResult);
            }
        }

        public async Task<ServiceResultModel> ConfirmUserAsync(ConfirmUserDto model)
        {
            // Retrive User with given user id
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogInformation($"Not found user with given UserId. UserId: {model.UserId}");
                return new ServiceResultModel { IsNotFound = true };
            }

            // Check user is verified mail id or not
            if (user.EmailConfirmed)
            {
                _logger.LogDebug($"User already verified MailId. UserId: {model.UserId}");
                return new ServiceResultModel(false, _authLocalizer["AlreadyVerifiedMailId"]);
            }

            // Confirm based on code
            var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (confirmEmailResult.Succeeded)
            {
                // Update Security questions
                var updateSecurityQuestionDto = _mapper.Map<UpdateSecurityQuestionDto>(model.SecurityQuestions);
                updateSecurityQuestionDto.Email = user.Email;

                var updateSecurityQuestionsResult = await UpdateSecurityQuestionsAsync(updateSecurityQuestionDto);
                if (updateSecurityQuestionsResult.Success)
                {
                    await _emailService.SendAccountCreationEmailAsync(user.Email);
                    return new ServiceResultModel(true, _authLocalizer["AccountVerifiedSuccessfully"]);
                }
                else
                {
                    // If not updated security questions then return error details
                    return updateSecurityQuestionsResult;
                }
            }
            else
            {
                // If not match the return errors
                return new ServiceResultModel(confirmEmailResult);
            }
        }

        #endregion

        #region  Password Releated

        public List<AllSecurityQuestionsDto> GetAllSecurityQuestions()
        {
            return _securityQuestionRepo.GetAll().Select(x => new AllSecurityQuestionsDto
            {
                Id = x.Id,
                Question = x.Question
            }).ToList();
        }

        public async Task<ServiceResultModel> GetSecurityQuestionsAsync(string mailId)
        {
            AppUser appUser = await _userManager.FindByEmailAsync(mailId);
            if (appUser == null)
                return new ServiceResultModel { IsNotFound = true };
            else if (!appUser.HasSecurityQuestions
                || !appUser.SecurityQuestion1.HasValue
                || appUser.SecurityQuestion1 == 0
                || !appUser.SecurityQuestion2.HasValue
                || appUser.SecurityQuestion2 == 0)
            {
                _logger.LogInformation($"User does not added security questions. UserId: {appUser.Id}");
                return new ServiceResultModel(false, _authLocalizer["NotSettingUpSecurityQuestions"]);
            }
            else
            {
                var questions = new List<AllSecurityQuestionsDto> {
                    new AllSecurityQuestionsDto
                    {
                        Id = appUser.SecurityQuestion1.Value,
                        Question = _securityQuestionRepo.GetById(appUser.SecurityQuestion1.Value)?.Question
                    },
                    new AllSecurityQuestionsDto
                    {
                        Id = appUser.SecurityQuestion2.Value,
                        Question = _securityQuestionRepo.GetById(appUser.SecurityQuestion2.Value)?.Question
                    },
                };
                return new ServiceResultModel(questions);
            }
        }

        public async Task<ServiceResultModel> VerifySecurityQuestionsAsync(UpdateSecurityQuestionDto model)
        {
            // Obtain User
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogDebug($"Not found user for verifying security questions. MailId: {model.Email}");
                return new ServiceResultModel { IsNotFound = true };
            }

            // Verifying password
            if (user.SecurityAnswer1.Trim().Equals(model.SecurityAnswer1.Trim(), StringComparison.InvariantCultureIgnoreCase)
                && user.SecurityAnswer2.Trim().Equals(model.SecurityAnswer2.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogInformation($"Security questions verified successfully. UserId: {user.Id}");
                return new ServiceResultModel(true, user.Id);
            }
            else
            {
                return new ServiceResultModel(false, _authLocalizer["OneOrMoreAnswerDoesNotMatch"]);
            }
        }

        public async Task<ServiceResultModel> UpdateSecurityQuestionsAsync(UpdateSecurityQuestionDto model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new ServiceResultModel { IsNotFound = true };
            else
            {
                user.SecurityQuestion1 = model.SecurityQuestion1;
                user.SecurityAnswer1 = model.SecurityAnswer1;
                user.SecurityQuestion2 = model.SecurityQuestion2;
                user.SecurityAnswer2 = model.SecurityAnswer2;
                user.HasSecurityQuestions = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    _logger.LogInformation($"Security questions updated successfully. UserId: {user.Id}");
                    return new ServiceResultModel(true);
                }
                else
                {
                    return new ServiceResultModel(updateResult);
                }
            }
        }

        public async Task<bool> ForgotPasswordAsync(string mailId)
        {
            var user = await _userManager.FindByEmailAsync(mailId);
            if (user == null)
            {
                _logger.LogInformation($"Not found User with Given MailId. MailId: {mailId}");
                return false;
            }

            // Generate Reset Token & Send mail
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordResetEmailAsync(mailId, user.Id, passwordResetToken);
            _logger.LogDebug($"Reset password link sent. MailId: {mailId}");
            return true;
        }

        public async Task<ServiceResultModel> ResetPasswordAsync(ResetPasswordDto model)
        {
            // Obtain User
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogDebug($"Not found user for reseting password. UserId: {model.UserId}");
                return new ServiceResultModel { IsNotFound = true };
            }

            // Resetting Password
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
            if (resetPasswordResult.Succeeded)
            {
                _logger.LogInformation($"Reset Password successfully. UserId: {model.UserId}");
                return new ServiceResultModel(true);
            }
            else
            {
                return new ServiceResultModel(resetPasswordResult);
            }
        }

        public async Task<ServiceResultModel> ResetPasswordOnNextLoginAsync(ResetPasswordOnNextLoginDto model)
        {
            // Obtain User
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                _logger.LogDebug($"Not found user for reseting password. UserId: {model.UserId}");
                return new ServiceResultModel { IsNotFound = true };
            }

            // Resetting Password
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, passwordResetToken, model.NewPassword);
            if (resetPasswordResult.Succeeded)
            {
                _logger.LogInformation($"Password changed successfully. UserId: {model.UserId}");

                // Update flag in DB
                user.ResetPasswordOnNextLogin = false;
                var updateResult = await _userManager.UpdateAsync(user);

                return new ServiceResultModel(true);
            }
            else
            {
                return new ServiceResultModel(resetPasswordResult);
            }
        }

        #endregion

        #region Misc

        public async Task<UserDetailsDto> GetUserDetailsAsync(AppUser user)
        {
            if (user != null)
            {
                var userDetails = _mapper.Map<UserDetailsDto>(user);

                var currentUserRoles = await _userManager.GetRolesAsync(user);
                userDetails.Role = currentUserRoles.FirstOrDefault() ?? string.Empty;
                return userDetails;
            }

            return null;
        }

        #endregion

        #endregion
    }
}
