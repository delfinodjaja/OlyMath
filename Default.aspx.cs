using System;
using System.Web;
using System.Web.UI;

namespace OlyMath
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] != null)
                {
                    phGuestActions.Visible = false;
                    phMemberActions.Visible = true;

                    string role = Session["UserRole"]?.ToString() ?? "";
                    if (string.Equals(role, "Trainee", StringComparison.OrdinalIgnoreCase))
                    {
                        lnkDashboard.NavigateUrl = "~/Trainee/Dashboard.aspx";
                    }
                    else if (string.Equals(role, "Trainer", StringComparison.OrdinalIgnoreCase))
                    {
                        lnkDashboard.NavigateUrl = "~/Trainer/Dashboard.aspx";
                    }
                    else if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        lnkDashboard.NavigateUrl = "~/Admin/Dashboard.aspx";
                    }
                }
            }
        }
    }
}
