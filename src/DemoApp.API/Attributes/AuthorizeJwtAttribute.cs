using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace DemoApp.API.Attributes
{
    public class AuthorizeJwtAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AuthorizeJwtAttribute()
        {
            // Add the JWT bearer authentication scheme
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
