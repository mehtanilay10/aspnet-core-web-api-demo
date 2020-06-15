using DemoApp.EntityFramework.Entities;
using DemoApp.EntityFramework.IdentityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.EntityFramework
{
    public class AppDBContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDBContext(DbContextOptions options)
               : base(options)
        {
        }

        public DbSet<MstSecurityQuestions> MstSecurityQuestions { get; set; }
    }
}
