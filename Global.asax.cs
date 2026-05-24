using System;
using System.Web;
using System.Web.Routing;

namespace OlyMath
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            System.Data.Entity.Database.SetInitializer<OlyMath.Models.ApplicationDbContext>(null);
            RegisterRoutes.Register(RouteTable.Routes);
            SeedData.Initialize();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            /*Exception ex = Server.GetLastError();

            if (Request.RawUrl.ToLower().Contains("error.aspx"))
            {
                return; 
            }

            if (ex != null)
            {
                
                Server.ClearError();
                Response.Redirect("~/Error.aspx");
            }*/
        }
    }
}
