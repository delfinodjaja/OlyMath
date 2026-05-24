using System;
using System.Web.UI;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security; 
using Microsoft.AspNet.Identity;
using System.Web;

namespace OlyMath.Account
{
    public partial class Logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var authManager = Context.GetOwinContext().Authentication;
            authManager.SignOut();
            Response.Redirect("/Default.aspx");
        }
    }
}
