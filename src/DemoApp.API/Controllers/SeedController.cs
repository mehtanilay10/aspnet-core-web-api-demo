using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoApp.DataAccess.Services;
using DemoApp.EntityFramework;
using DemoApp.EntityFramework.Entities;
using DemoApp.EntityFramework.IdentityModels;
using DemoApp.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace DemoApp.API.Controllers
{
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), Status200OK)]
    public class SeedController : AppBaseController<SeedController>
    {
        #region Private Fields & Constructors

        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork<AppDBContext> _unitOfWorkWithContext;
        private readonly string _wwwRootDirPath;

        public SeedController(IUnitOfWork<AppDBContext> dbContext, IWebHostEnvironment currentEnvironment,
            UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWorkWithContext = dbContext;
            _wwwRootDirPath = currentEnvironment.WebRootPath;
        }

        #endregion

        #region Database Migration

        [HttpPost]
        public async Task<string> MigrateDatabase()
        {
            var context = _unitOfWorkWithContext.GetContext();
            // Obtain last Migration name
            var fromMigration = (await context.Database.GetAppliedMigrationsAsync()).LastOrDefault() ?? "N/A";

            // Migrate database
            try
            {
                context.Database.Migrate();
                var toMigration = (await context.Database.GetAppliedMigrationsAsync()).LastOrDefault() ?? "N/A";
                if (fromMigration.Equals(toMigration))
                    return $"Database already Migrated to: {toMigration}.";
                else
                    return $"Database Migrated from: {fromMigration} to {toMigration}";
            }
            catch (Exception ex)
            {
                return $"An error occurred to perform Migration: {ex}";
            }
        }

        #endregion

        #region Actions for Seeding data

        #region Seed Master tables

        [HttpPost]
        public async Task<string> SeedAllMasterTables()
        {
            try
            {
                await SeedRoles();
                SeedMstSecurityQuestions();
                return "Seeding completed for all master tables. Note: If table already have data then it will not seed with new data.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ex.ToString();
            }
        }

        [HttpPost]
        public async Task<string> SeedRoles()
        {
            try
            {
                var roleDbSet = base._unitOfWork.GetRepository<AppRole>().GetDbSet();

                if (!roleDbSet.Any())
                {
                    _logger.LogInformation("Seeding Roles");

                    foreach (var roleName in Enum.GetNames(typeof(Roles)))
                    {
                        if (!await _roleManager.RoleExistsAsync(roleName))
                            await _roleManager.CreateAsync(new AppRole(roleName));
                    }
                    return "Role data seeded successfully";
                }
                else
                {
                    return "Roles are already seeded!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ex.ToString();
            }
        }

        [HttpPost]
        public string SeedMstSecurityQuestions()
        {
            try
            {
                var securityQuestionsRepo = base._unitOfWork.GetRepository<MstSecurityQuestions>();
                var securityQuestionsDbSet = securityQuestionsRepo.GetDbSet();

                if (!securityQuestionsDbSet.Any())
                {
                    _logger.LogInformation("Seeding MstSecurityQuestions");
                    string jsongData = System.IO.File.ReadAllText($@"{_wwwRootDirPath}/SeedData/MasterData/MstSecurityQuestions.json");
                    List<MstSecurityQuestions> questions = JsonConvert.DeserializeObject<List<MstSecurityQuestions>>(jsongData);
                    securityQuestionsRepo.Add(questions);
                    return "Successfully seeded MstSecurityQuestions";
                }
                else
                {
                    return "Unable to seed security questions, as some data already exist in table.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ex.ToString();
            }
        }

        #endregion

        #region Seed Dummy data

        [HttpPost]
        public async Task<string> SeedUsersOfAllRoles()
        {
            try
            {
                _logger.LogInformation("Seeding Users Of All Roles");

                foreach (var roleName in Enum.GetNames(typeof(Roles)))
                {
                    var user = new AppUser
                    {
                        FirstName = roleName,
                        LastName = "Dummy User",
                        UserName = $"{roleName}@dummy-domain.com",
                        Email = $"{roleName}@dummy-domain.com",
                        EmailConfirmed = true,
                        HasSecurityQuestions = true,
                        Culture = (int)SupportedCulture.en,
                        IsActive = true,
                        JoinedOn = DateTime.Now,
                        SecurityQuestion1 = 1,  // Set dummy securiy questions
                        SecurityQuestion2 = 2,
                        SecurityAnswer1 = "1",
                        SecurityAnswer2 = "2",
                    };
                    var createUserResult = await _userManager.CreateAsync(user, "Password");    // Set "Password" as default password
                    var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
                }

                return "Added Users for given Roles successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return ex.ToString();
            }
        }

        #endregion

        #endregion
    }
}
