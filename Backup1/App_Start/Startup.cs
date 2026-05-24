using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using OlyMath.Models;

[assembly: OwinStartup(typeof(OlyMath.Startup))]

namespace OlyMath
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Cookie-based authentication (equivalent to ASP.NET Core's AddDefaultIdentity cookie)
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath          = new PathString("/Account/Login.aspx"),
                Provider           = new CookieAuthenticationProvider(),
                ExpireTimeSpan     = TimeSpan.FromMinutes(30),
                SlidingExpiration  = true
            });
        }
    }
}
