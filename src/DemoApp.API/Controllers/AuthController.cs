using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DemoApp.API.ApiModels;
using DemoApp.Business.Services;
using DemoApp.EntityFramework.IdentityModels;
using DemoApp.Localization.Localizers.AuthLocalizer;
using DemoApp.Models.ApiModels.Auth;
using DemoApp.Models.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace DemoApp.API.Controllers
{
    public class AuthController : AppBaseController<AuthController>
    {
        #region Private members

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IStringLocalizer _authLocalizer;

        #endregion

        #region Constructor

        public AuthController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager,
            SignInManager<AppUser> signInManager, IAuthService authService, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

            _authService = authService;
            _authLocalizer = _localizerFactory.Create(typeof(AuthLocalizer));
        }

        #endregion

        #region Actions

        #region Login & Register

        /// <summary>
        /// Add new User details in DB,
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserIdDto), Status200OK)]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
        {
            ServiceResultModel createUserResult = await _authService.RegisterUserAsync(model);

            if (createUserResult.Success)
                return Ok(new UserIdDto { UserId = createUserResult.Messages?.FirstOrDefault() });
            else
                return UnprocessableEntity(_authLocalizer["UnableToCreateUser"], createUserResult.Messages);
        }

        /// <summary>
        /// Generate token based on mail/password
        /// </summary>
        /// <returns>Token & Expire time for Token</returns>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JwtTokenDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserDto model)
        {
            // Check user exist in system or not
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(_authLocalizer["LoginFailure"], _authLocalizer["InvalidLoginAttempt"]);
            }

            // Check wherther user confirmed mail id or not
            if (!user.EmailConfirmed)
            {
                return Forbidden(_authLocalizer["LoginFailure"], _authLocalizer["YouNeedToConfirmMailId"]);
            }

            if (!user.IsActive)
            {
                return Forbidden(_authLocalizer["LoginFailure"], _authLocalizer["YourAccountIsDeactivated"]);
            }

            // Perform login operation
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if (signInResult.Succeeded)
            {
                // Obtain token
                JwtTokenDto token = await GetJwtSecurityTokenAsync(user);
                return Ok(token);
            }
            else if (signInResult.IsLockedOut)
            {
                return Forbidden(_authLocalizer["LoginFailure"], _authLocalizer["YourAccountIsLocked"]);
            }
            else
            {
                return Forbidden(_authLocalizer["LoginFailure"], _authLocalizer["InvalidLoginAttempt"]);
            }
        }

        /// <summary>
        /// Create a new token for logged in user
        /// </summary>
        /// <returns>New token details</returns>
        [HttpGet]
        [ProducesResponseType(typeof(JwtTokenDto), Status200OK)]
        public async Task<IActionResult> RefreshToken()
        {
            // Obtain logged in user
            var currentUser = await _authService.GetCurrentUser();
            if (currentUser == null)
                return Unauthorized();

            // Generate new token
            JwtTokenDto refreshedToken = await GetJwtSecurityTokenAsync(currentUser);
            if (refreshedToken == null)
                return Unauthorized();

            return Ok(refreshedToken);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CommonResultModel), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> ResendConfirmMailAsync([FromBody] UserMailIdDto model)
        {
            ServiceResultModel resendMailResult = await _authService.ResendConfirmMailAsync(model.Email);

            if (resendMailResult.IsNotFound)
                return NotFound(_sharedLocalizer["SomethingWentWrong"], _authLocalizer["UnableToResnedConfirmationLink"]);
            else if (resendMailResult.Success)
                return Ok(resendMailResult.Messages.FirstOrDefault());
            else
                return UnprocessableEntity(_sharedLocalizer["AnErrorOccurred"], resendMailResult.Messages);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CommonResultModel), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> ValidateConfirmUserToken([FromBody] ValidateConfirmUserDto model)
        {
            ServiceResultModel confirmMailResult = await _authService.ValidateConfirmMailTokenAsync(model);

            if (confirmMailResult.IsNotFound)
                return NotFound(_authLocalizer["UnableToConfirmMailId"], _authLocalizer["InvalidToken"]);
            else if (confirmMailResult.Success)
                return Ok(confirmMailResult.Messages.FirstOrDefault());
            else
                return UnprocessableEntity(_authLocalizer["UnableToConfirmMailId"], confirmMailResult.Messages);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CommonResultModel), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> ConfirmUser([FromBody] ConfirmUserDto model)
        {
            ServiceResultModel confirmMailResult = await _authService.ConfirmUserAsync(model);

            if (confirmMailResult.IsNotFound)
                return NotFound(_authLocalizer["UnableToConfirmMailId"], _authLocalizer["InvalidToken"]);
            else if (confirmMailResult.Success)
                return Ok(confirmMailResult.Messages.FirstOrDefault());
            else
                return UnprocessableEntity(_authLocalizer["UnableToConfirmMailId"], confirmMailResult.Messages);
        }

        #endregion

        #region Password Related

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(typeof(List<AllSecurityQuestionsDto>), Status200OK)]
        public IActionResult GetAllSecurityQuestions()
        {
            List<AllSecurityQuestionsDto> securityQuestions = _authService.GetAllSecurityQuestions();

            if (securityQuestions.Any())
                return Ok(securityQuestions);
            else
                return NotFound(_authLocalizer["NotFoundSecurityQuestions"], _authLocalizer["NotFoundSecurityQuestionsContactSupport"]);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(typeof(List<AllSecurityQuestionsDto>), Status200OK)]
        public async Task<IActionResult> GetSecurityQuestions([FromBody] ForgotPasswordDto model)
        {
            ServiceResultModel securityQuestionResult = await _authService.GetSecurityQuestionsAsync(model.Email);

            if (securityQuestionResult.IsNotFound)
                return NotFound(_authLocalizer["NotFoundSecurityQuestions"], _authLocalizer["NotSettingUpSecurityQuestions"]);
            else if (securityQuestionResult.Success)
                return Ok(securityQuestionResult.Data);
            else
                return UnprocessableEntity(_authLocalizer["AnErrorOccurred"], securityQuestionResult.Messages);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(typeof(UserIdDto), Status200OK)]
        public async Task<IActionResult> VerifySecurityQuestions([FromBody] UpdateSecurityQuestionDto model)
        {
            ServiceResultModel securityQuestionResult = await _authService.VerifySecurityQuestionsAsync(model);

            if (securityQuestionResult.IsNotFound)
                return NotFound(_authLocalizer["ErrorOccurredToVerifyAnswer"], _authLocalizer["NotFoundSecurityQuestions"]);
            else if (securityQuestionResult.Success)
                return Ok(new UserIdDto { UserId = securityQuestionResult.Messages.FirstOrDefault() });
            else
                return UnprocessableEntity(_authLocalizer["ErrorOccurredToVerifyAnswer"], securityQuestionResult.Messages);
        }

        [HttpPost]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(typeof(List<string>), Status200OK)]
        public async Task<IActionResult> UpdateSecurityQuestions([FromBody] UpdateSecurityQuestionDto model)
        {
            ServiceResultModel securityQuestionResult = await _authService.UpdateSecurityQuestionsAsync(model);

            if (securityQuestionResult.IsNotFound)
                return NotFound(_sharedLocalizer["SomethingWentWrong"], _authLocalizer["ErrorOccurredToUpdateSecurityQuestions"]);
            else if (securityQuestionResult.Success)
                return Ok();
            else
                return UnprocessableEntity(_authLocalizer["ErrorOccurredToUpdateSecurityQuestions"], securityQuestionResult.Messages);
        }

        /// <summary>
        /// Send Reset password link for give user
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(Status200OK)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                // Validate Model
                return UnprocessableEntity(_sharedLocalizer["AnErrorOccurred"], _authLocalizer["EmailIdIsRequired"]);
            }
            else
            {
                bool userExist = await _authService.ForgotPasswordAsync(model.Email);
                if (userExist)
                    return Ok(_sharedLocalizer["Done"], _authLocalizer["ResetLinkSendSuccefully"]);
                return NotFound(_sharedLocalizer["AnErrorOccurred"], "SomethingWentWrong");
            }
        }

        /// <summary>
        /// Reset new password based on token
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CommonResultModel), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            ServiceResultModel resetPasswordResult = await _authService.ResetPasswordAsync(model);

            if (resetPasswordResult.IsNotFound)
                return NotFound(_authLocalizer["ErrorOccurredToResetPassword"], _sharedLocalizer["SomethingWentWrong"]);
            else if (resetPasswordResult.Success)
                return Ok(_authLocalizer["ResetPasswordSuccessfully"]);
            else
                return UnprocessableEntity(_authLocalizer["ErrorOccurredToResetPassword"], resetPasswordResult.Messages);
        }

        /// <summary>
        /// Reset password when ResetPasswordOnNextLogin flag is set true
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CommonResultModel), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> ResetPasswordOnNextLogin([FromBody] ResetPasswordOnNextLoginDto model)
        {
            ServiceResultModel resetPasswordResult = await _authService.ResetPasswordOnNextLoginAsync(model);

            if (resetPasswordResult.IsNotFound)
                return NotFound(_authLocalizer["ErrorOccurredToUpdatePassword"], _authLocalizer["SomethingWentWrong"]);
            else if (resetPasswordResult.Success)
                return Ok(_authLocalizer["PasswordUpdatedSuccessfully"]);
            else
                return UnprocessableEntity(_authLocalizer["ErrorOccurredToUpdatePassword"], resetPasswordResult.Messages);
        }

        #endregion

        #endregion

        #region Private Methods

        /// <summary>
        /// Generate JWT token for user
        /// </summary>
        /// <param name="user">User entity for which token will be generate</param>
        /// <returns>Generated token details including expire data</returns>
        private async Task<JwtTokenDto> GetJwtSecurityTokenAsync(AppUser user)
        {
            var jwtOptions = _appSettings.JwtIssuerOptions;

            // Obtain existing claims, Here we will obtain last 4 JTI claims only
            // As We only maintain login for 5 maximum sessions, So need to remove other from that
            var allClaims = await _userManager.GetClaimsAsync(user);
            var toRemoveClaims = new List<Claim>();
            var allJtiClaims = allClaims.Where(claim => claim.Type.Equals(JwtRegisteredClaimNames.Jti)).ToList();
            if (allJtiClaims.Count > 4)
            {
                toRemoveClaims = allJtiClaims.SkipLast(4).ToList();
                allJtiClaims = allJtiClaims.TakeLast(4).ToList();
            }

            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey)), SecurityAlgorithms.HmacSha256);

            DateTime tokenExpireOn = DateTime.Now.AddHours(3);
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                // If its development then set 3 years as token expiry for testing purpose
                tokenExpireOn = DateTime.Now.AddYears(3);
            }

            string roles = string.Join("; ", await _userManager.GetRolesAsync(user));

            // Obtain Role of User
            IList<string> rolesOfUser = await _userManager.GetRolesAsync(user);

            // Add new claims
            List<Claim> tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, rolesOfUser.FirstOrDefault()),
            };

            // Make JWT token
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: tokenClaims.Union(allJtiClaims),
                expires: tokenExpireOn,
                signingCredentials: credentials
            );
            _logger.LogDebug($"Token generated. UserId: {user.Email}");

            // Set current user details for busines & common library
            var currentUser = await _userManager.FindByEmailAsync(user.Email);

            // Update claim details
            await _userManager.RemoveClaimsAsync(currentUser, toRemoveClaims);
            await _userManager.AddClaimsAsync(currentUser, tokenClaims);

            // Return it
            JwtTokenDto generatedToken = new JwtTokenDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpireOn = tokenExpireOn,
                UserDetails = await _authService.GetUserDetailsAsync(currentUser)
            };

            return generatedToken;
        }

        #endregion
    }
}
