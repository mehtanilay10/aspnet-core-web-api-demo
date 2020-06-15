using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DemoApp.DataAccess.Services;
using DemoApp.EntityFramework.IdentityModels;
using DemoApp.Localization.Localizers.SharedLocalizer;
using DemoApp.Models.AppSettings;
using DemoApp.Models.Constants;
using DemoApp.Models.Enums;
using DemoApp.Models.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DemoApp.Business.Services
{
    #region Interface

    public interface IServiceBase
    {
        string GetCurrentUserId();
        Task<AppUser> GetCurrentUser();
        Task<Roles> GetCurrentUserRole();
    }

    #endregion

    /// <summary>
    ///   Base class for Business Logic
    /// </summary>
    public class ServiceBase<TService> : IServiceBase where TService : class
    {
        #region Fields & Ctor

        protected readonly ILogger<TService> _logger;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMapper _mapper;

        protected readonly AppSettings _appSettings;
        protected readonly UserManager<AppUser> _userManager;

        protected readonly IStringLocalizerFactory _localizerFactory;
        protected readonly IStringLocalizer _sharedLocalizer;

        protected ServiceBase(IServiceProvider serviceProvider)
        {
            _logger = (ILogger<TService>)serviceProvider.GetService(typeof(ILogger<TService>));
            _unitOfWork = (IUnitOfWork)serviceProvider.GetService(typeof(IUnitOfWork));
            _httpContextAccessor = (IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor));
            _mapper = (IMapper)serviceProvider.GetService(typeof(IMapper));

            _appSettings = (AppSettings)serviceProvider.GetService(typeof(AppSettings));
            _userManager = (UserManager<AppUser>)serviceProvider.GetService(typeof(UserManager<AppUser>));

            _localizerFactory = (IStringLocalizerFactory)serviceProvider.GetService(typeof(IStringLocalizerFactory));
            _sharedLocalizer = _localizerFactory.Create(typeof(SharedLocalizer));
        }

        #endregion

        #region Current User related

        /// <summary>
        /// Obtain UserId from token
        /// </summary>
        /// <returns>UserId</returns>
        public string GetCurrentUserId()
        {
            ClaimsIdentity identity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            string userId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId ?? string.Empty;
        }

        /// <summary>
        /// Obtain AccountUser instance from JWT token that we get
        /// </summary>
        /// <returns>AccountUser for logged in user</returns>
        public async Task<AppUser> GetCurrentUser()
        {
            // Obtain MailId from token
            ClaimsIdentity identity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            var userMailId = identity?.FindFirst(AppConstants.JWT_SUB)?.Value;

            // Obtain user from token
            AppUser user = null;
            if (!string.IsNullOrEmpty(userMailId))
            {
                user = await _userManager.FindByEmailAsync(userMailId);
            }

            return user;
        }

        /// <summary>
        /// Return role of current user
        /// </summary>
        /// <returns>Role of logged in user</returns>
        public async Task<Roles> GetCurrentUserRole()
        {
            AppUser currentUser = await GetCurrentUser();
            IList<string> existingRole = await _userManager.GetRolesAsync(currentUser);
            string roleName = existingRole?.FirstOrDefault();
            Roles currentUserRole = default;

            if (!string.IsNullOrEmpty(roleName))
            {
                currentUserRole = EnumHelpers<Roles>.Parse(roleName);
            }
            return currentUserRole;
        }

        #endregion
    }
}
