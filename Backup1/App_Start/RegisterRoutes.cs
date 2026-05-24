using System;
using System.Web.Routing;

namespace OlyMath
{
    public static class RegisterRoutes
    {
        public static void Register(RouteCollection routes)
        {
            routes.MapPageRoute("Login", "Account/Login", "~/Account/Login.aspx");
            routes.MapPageRoute("Register", "Account/Register", "~/Account/Register.aspx");
            routes.MapPageRoute("Logout", "Account/Logout", "~/Account/Logout.aspx");
        }
    }
}