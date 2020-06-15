using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace DemoApp.API.Helpers
{
    public static class ApplicationBuilderHelper
    {
        public static void UseSwaggerCustom(this IApplicationBuilder app, IConfiguration configuration)
        {
            var apiVersion = configuration["SwaggerDetails:ApiVersion"];

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = configuration["SwaggerDetails:Template"]; });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.EnableFilter();
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.DisplayRequestDuration();
                options.RoutePrefix = configuration["SwaggerDetails:RoutePrefix"];

                options.SwaggerEndpoint(
                    configuration["SwaggerDetails:Endpoints:API:Url"],
                    configuration["SwaggerDetails:Endpoints:API:Name"]);
            });
        }
    }
}
