using System;
using System.Web.UI;
using System.Web.Security;

namespace OlyMath
{
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void LogoutButton_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Redirect("/Account/Login.aspx");
        }
    }
}