using System;
using System.Web.UI;

namespace OlyMath
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect logged-in users to their dashboard
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    Response.Redirect("/Admin/Index.aspx");
                else if (User.IsInRole("Trainer"))
                    Response.Redirect("/Trainer/Index.aspx");
                else
                    Response.Redirect("/Trainee/Index.aspx");
            }
        }
    }
}
