using System;
using Microsoft.AspNet.Identity;
using OlyMath.Services;

namespace OlyMath.Controls
{
    public partial class EnrollmentWidget : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEnrollments();
            }
        }

        private void LoadEnrollments()
        {
            // Get logged in user's ID using Identity
            string userId = Context.User.Identity.GetUserId();

            EnrollmentService service = new EnrollmentService();
            var enrollments = service.GetTraineeEnrollments(userId);

            if (enrollments != null && enrollments.Count > 0)
            {
                rptEnrollments.DataSource = enrollments;
                rptEnrollments.DataBind();
            }
            else
            {
                rptEnrollments.Visible = false;
                lblNoModules.Visible = true;
            }
        }
    }
}