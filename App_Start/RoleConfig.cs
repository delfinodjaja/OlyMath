using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WAPP_Assignment_Module.Models;

namespace WAPP_Assignment_Module.App_Start
{
    public class RoleConfig
    {
        public static void RegisterRoles()
        {
            var context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            // Create roles if they don't exist
            if (!roleManager.RoleExists("Admin"))
                roleManager.Create(new IdentityRole("Admin"));

            if (!roleManager.RoleExists("Trainer"))
                roleManager.Create(new IdentityRole("Trainer"));

            if (!roleManager.RoleExists("Trainee"))
                roleManager.Create(new IdentityRole("Trainee"));
        }
    }
}