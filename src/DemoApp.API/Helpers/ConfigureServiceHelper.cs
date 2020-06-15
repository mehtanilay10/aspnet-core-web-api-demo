using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using AspNetCore.ServiceRegistration.Dynamic.Attributes;
using AspNetCore.ServiceRegistration.Dynamic.Extensions;
using AspNetCore.ServiceRegistration.Dynamic.Interfaces;
using DemoApp.DataAccess.ExtensionMethods;
using DemoApp.EntityFramework;
using DemoApp.EntityFramework.IdentityModels;
using DemoApp.Localization.Localizers.ValidationLocalizer;
using DemoApp.Models.AppSettings;
using DemoApp.Models.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DemoApp.API.Helpers
{
    public static class ConfigureServiceHelper
    {
        /// <summary>
        /// Swagger - Enable this line and the related lines in Configure method to enable swagger UI
        /// </summary>
        public static void AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                var apiInfo = new OpenApiInfo
                {
                    Version = configuration["SwaggerDetails:ApiVersion"],
                    Title = configuration["SwaggerDetails:Title"],
                    Description = configuration["SwaggerDetails:Description"],
                    Contact = new OpenApiContact
                    {
                        Name = configuration["SwaggerDetails:Contact:Name"],
                        Email = configuration["SwaggerDetails:Contact:Email"],
                        Url = new Uri(configuration["SwaggerDetails:Contact:Url"]),
                    }
                };

                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "bearerAuth"
                                }
                            },
                            new string[] {}
                    }
                };

                options.SwaggerDoc(configuration["SwaggerDetails:ApiVersion"], apiInfo);
                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
                options.AddSecurityDefinition("bearerAuth", securityScheme);
                options.AddSecurityRequirement(securityRequirement);
            });
        }

        /// <summary>
        /// Suppress Error Handling from ApiController
        /// </summary>
        public static void AddModelErrorHandler(this IServiceCollection services)
        {
            // Enable error for Models
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressInferBindingSourcesForParameters = true;
                options.SuppressConsumesConstraintForFormFileParameters = true;
                options.SuppressMapClientErrors = true;
            });

            // Specify localization
            services.Configure<MvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.DataAnnotationLocalizerProvider = (_, factory)
                    => factory.Create(typeof(ValidationLocalizer));
            });
        }

        /// <summary>
        /// Enable CORS
        /// </summary>
        public static void AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .SetIsOriginAllowed(_ => true)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
            });
        }

        /// <summary>
        /// Add support for SQL server & Unit of work
        /// </summary>
        public static void AddSqlServerUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDBContext>(option =>
            {
                option.UseSqlServer(configuration["ConnectionStrings:SQLServer"]);
            })
            .AddUnitOfWork<AppDBContext>();
        }

        /// <summary>
        /// Generate AppSettings
        /// </summary>
        public static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            AppSettings appSettings = configuration.Get<AppSettings>();
            services.TryAddSingleton(appSettings);
        }

        /// <summary>
        /// Adding Support for Identity
        /// </summary>
        public static void AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AppDBContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password configs
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;

                // ApplicationUser settings
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
            });
        }

        /// <summary>
        /// Add Localization & configure it
        /// </summary>
        public static void AddCustomLocalization(this IServiceCollection services)
        {
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>();
                foreach (var culture in Enum.GetValues(typeof(Models.Enums.SupportedCulture)))
                {
                    supportedCultures.Add(new CultureInfo(culture.ToString()));
                }

                var defaultCulture = AppConstants.DEFAULT_CULTURE.ToString();
                opts.DefaultRequestCulture = new RequestCulture(defaultCulture, defaultCulture);
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
            });
        }

        /// <summary>
        /// Adding Auth scheme & JWT configuration
        /// </summary>
        public static void AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration["JwtIssuerOptions:SecretKey"]));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtIssuerOptions:Issuer"],
                    ValidAudience = configuration["JwtIssuerOptions:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = signingKey,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        /// <summary>
        /// Add Custom/Business service dependency
        /// </summary>
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddServicesOfType<IScopedService>();
            services.AddServicesWithAttributeOfType<ScopedServiceAttribute>();
            services.AddServicesOfType<ITransientService>();
            services.AddServicesWithAttributeOfType<TransientServiceAttribute>();
            services.AddServicesOfType<ISingletonService>();
            services.AddServicesWithAttributeOfType<SingletonServiceAttribute>();
        }
    }
}
