using System;
using System.Web.UI;

namespace OlyMath
{
    public partial class Dashboard : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect unauthenticated users to login
            if (!Request.IsAuthenticated)
                Response.Redirect("/Account/Login.aspx");
        }

        // Returns " active" CSS class if the current page matches the given path
        protected string IsActive(string path)
        {
            string currentPath = Request.AppRelativeCurrentExecutionFilePath
                                        .TrimStart('~').ToLower();
            return currentPath == path.ToLower() ? " active" : string.Empty;
        }
    }
}