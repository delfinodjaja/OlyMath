using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OlyMath.Models;

namespace OlyMath
{
    /// <summary>
    /// Call SeedData.Initialize() from Global.asax Application_Start
    /// (replaces the async await block in Program.cs)
    /// </summary>
    public static class SeedData
    {
        public static void Initialize()
        {
            using (var context = ApplicationDbContext.Create())
            {
                var roleStore   = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore   = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                // Seed roles
                string[] roles = { "Admin", "Trainer", "Trainee" };
                foreach (var role in roles)
                {
                    if (!roleManager.RoleExists(role))
                        roleManager.Create(new IdentityRole(role));
                }

                // Seed default Admin account
                const string adminEmail = "admin@olymath.com";
                if (userManager.FindByEmail(adminEmail) == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName       = adminEmail,
                        Email          = adminEmail,
                        Name           = "OlyMath Admin",
                        EmailConfirmed = true
                    };
                    var result = userManager.Create(admin, "Admin@123");
                    if (result.Succeeded)
                        userManager.AddToRole(admin.Id, "Admin");
                }
            }
        }
    }
}
