using Microsoft.AspNetCore.Identity;

namespace DemoApp.EntityFramework.IdentityModels
{
    public class AppRole : IdentityRole
    {
        public AppRole()
        {
        }

        public AppRole(string roleName)
            : base(roleName)
        {
        }
    }
}
