using AutoMapper;
using DemoApp.API.Helpers;
using DemoApp.API.Middlewares;
using DemoApp.AutoMapper;
using DemoApp.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoApp.API
{
    public class Startup
    {
        #region Props & Ctor

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomSwagger(Configuration);
            services.AddCustomCors();
            services.AddCustomIdentity();
            services.AddCustomLocalization();
            services.AddAppSettings(Configuration);
            services.AddCustomServices();

            services.AddModelErrorHandler();
            services.AddJwtAuth(Configuration);
            services.AddSqlServerUnitOfWork(Configuration);

            services.AddAutoMapper(typeof(AuthProfile).Assembly);
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDBContext appDBContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /// Uncomment below line if you want to perform Auto Migration,
            /// personally, I prefer to use controller for same
            // appDBContext.Database.Migrate();

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();
            app.UseErrorLogging();
            app.UseSwaggerCustom(Configuration);
            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
