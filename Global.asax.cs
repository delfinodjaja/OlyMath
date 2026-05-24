using System;
using System.IO;
using System.Web;

namespace OlyMath
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Resolve physical path of App_Data and map it for connection string |DataDirectory| token
            string appDataPath = HttpContext.Current.Server.MapPath("~/App_Data");
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);

            // Initialize SQLite DB
            DbHelper.InitializeDatabase();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Session initialization if needed
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}
