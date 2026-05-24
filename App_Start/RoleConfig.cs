using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OlyMath.Models;

namespace OlyMath.App_Start
{
    public class RoleConfig
    {
        public static void RegisterRoles()
        {
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!roleManager.RoleExists("Admin"))
                roleManager.Create(new IdentityRole("Admin"));

            if (!roleManager.RoleExists("Trainer"))
                roleManager.Create(new IdentityRole("Trainer"));

            if (!roleManager.RoleExists("Trainee"))
                roleManager.Create(new IdentityRole("Trainee"));
        }
    }
}