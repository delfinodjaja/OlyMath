using System;
using System.Web.UI;

namespace OlyMath
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("Trainer") || User.IsInRole("Admin"))
                {
                    Response.Redirect("/TrainerDashboard.aspx");
                }
                else
                {
                    Response.Redirect("/AssessmentList.aspx");
                }
            }
        }
    }
}
