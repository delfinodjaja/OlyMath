using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace OlyMath
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfigureLayout();
            }
        }

        private void ConfigureLayout()
        {
            if (Session["UserId"] != null)
            {
                // User is authenticated
                phAnonymousNav.Visible = false;
                phAuthenticatedNav.Visible = true;
                litUserName.Text = HttpUtility.HtmlEncode(Session["FullName"]?.ToString() ?? "");

                string role = Session["UserRole"]?.ToString() ?? "";
                lblUserBadge.Text = role;

                // Adjust badge class
                if (string.Equals(role, "Trainee", StringComparison.OrdinalIgnoreCase))
                    lblUserBadge.CssClass = "nav-badge";
                else if (string.Equals(role, "Trainer", StringComparison.OrdinalIgnoreCase))
                    lblUserBadge.CssClass = "nav-badge trainer";
                else if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                    lblUserBadge.CssClass = "nav-badge admin";

                // Setup dashboard layout classes
                divMainLayout.Attributes["class"] = "dashboard-layout";
                divContentWrapper.Attributes["class"] = "main-content";
                asideSidebar.Visible = true;

                // Toggle Sidebars based on Role
                pnlTraineeSidebar.Visible = string.Equals(role, "Trainee", StringComparison.OrdinalIgnoreCase);
                pnlTrainerSidebar.Visible = string.Equals(role, "Trainer", StringComparison.OrdinalIgnoreCase);
                pnlAdminSidebar.Visible = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

                // Highlight active items
                HighlightActiveMenuItem();
            }
            else
            {
                // Anonymous User
                phAnonymousNav.Visible = true;
                phAuthenticatedNav.Visible = false;

                // Neutral full-page layout
                divMainLayout.Attributes["class"] = "";
                divContentWrapper.Attributes["class"] = "page-body";
                asideSidebar.Visible = false;
            }
        }

        private void HighlightActiveMenuItem()
        {
            string absolutePath = Request.Url.AbsolutePath.ToLower();

            // Reset classes
            string baseClass = "sidebar-item";
            lnkTraineeDash.Attributes["class"] = baseClass;
            lnkTraineeBrowse.Attributes["class"] = baseClass;
            lnkTraineeAchievements.Attributes["class"] = baseClass;
            lnkTraineeModuleDetail.Attributes["class"] = baseClass;
            lnkTraineeStudy.Attributes["class"] = baseClass;
            lnkTraineeAssessment.Attributes["class"] = baseClass;
            lnkTraineeCertificate.Attributes["class"] = baseClass;
            lnkTraineeDiscussion.Attributes["class"] = baseClass;

            lnkTrainerDash.Attributes["class"] = baseClass;
            lnkTrainerMyModules.Attributes["class"] = baseClass;
            lnkTrainerCreate.Attributes["class"] = baseClass;
            lnkTrainerManageMaterials.Attributes["class"] = baseClass;
            lnkTrainerProgress.Attributes["class"] = baseClass;
            lnkTrainerDiscussion.Attributes["class"] = baseClass;

            lnkAdminDash.Attributes["class"] = baseClass;
            lnkAdminUsers.Attributes["class"] = baseClass;
            lnkAdminModules.Attributes["class"] = baseClass;
            lnkAdminEnrollments.Attributes["class"] = baseClass;
            lnkAdminDiscussion.Attributes["class"] = baseClass;

            // Apply active class
            if (absolutePath.Contains("/trainee/dashboard.aspx"))
            {
                lnkTraineeDash.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainee/browsemodules.aspx"))
            {
                lnkTraineeBrowse.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainee/achievements.aspx"))
            {
                lnkTraineeAchievements.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainee/moduledetail.aspx"))
            {
                lnkTraineeModuleDetail.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainee/study.aspx"))
            {
                lnkTraineeStudy.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainee/assessment.aspx"))
            {
                lnkTraineeAssessment.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainee/certificate.aspx"))
            {
                lnkTraineeCertificate.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainee/discussion.aspx"))
            {
                lnkTraineeDiscussion.Attributes["class"] = baseClass + " active";
                lnkTrainerDiscussion.Attributes["class"] = baseClass + " active";
                lnkAdminDiscussion.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainer/dashboard.aspx"))
            {
                lnkTrainerDash.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainer/mymodules.aspx"))
            {
                lnkTrainerMyModules.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainer/editmodule.aspx"))
            {
                lnkTrainerCreate.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainer/managematerials.aspx") || absolutePath.Contains("/trainer/managequestions.aspx"))
            {
                lnkTrainerManageMaterials.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/trainer/traineeprogress.aspx"))
            {
                lnkTrainerProgress.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/admin/dashboard.aspx"))
            {
                lnkAdminDash.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/admin/manageusers.aspx"))
            {
                lnkAdminUsers.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/admin/moduleoversight.aspx"))
            {
                lnkAdminModules.Attributes["class"] = baseClass + " active";
            }
            else if (absolutePath.Contains("/admin/enrollmentmanagement.aspx"))
            {
                lnkAdminEnrollments.Attributes["class"] = baseClass + " active";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}
