using System;
using System.Web;
using System.Web.UI;

namespace OlyMath
{
    public class BasePage : Page
    {
        // Override in specific page to restrict access (e.g., AllowedRoles = new string[] { "Trainer" })
        public virtual string[] AllowedRoles { get; set; } = null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Prevent caching of sensitive pages
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            // 1. Session check
            if (Session["UserId"] == null)
            {
                string returnUrl = Request.RawUrl;
                Response.Redirect("~/Login.aspx?returnUrl=" + Server.UrlEncode(returnUrl));
                return;
            }

            // 2. Role check
            if (AllowedRoles != null && AllowedRoles.Length > 0)
            {
                string userRole = Session["UserRole"] as string;
                bool hasAccess = false;
                foreach (string role in AllowedRoles)
                {
                    if (string.Equals(role, userRole, StringComparison.OrdinalIgnoreCase))
                    {
                        hasAccess = true;
                        break;
                    }
                }

                if (!hasAccess)
                {
                    // Access denied, redirect to appropriate default page for their role
                    RedirectToRoleDashboard(userRole);
                }
            }
        }

        public void RedirectToRoleDashboard(string role)
        {
            if (string.Equals(role, "Trainee", StringComparison.OrdinalIgnoreCase))
                Response.Redirect("~/Trainee/Dashboard.aspx");
            else if (string.Equals(role, "Trainer", StringComparison.OrdinalIgnoreCase))
                Response.Redirect("~/Trainer/Dashboard.aspx");
            else if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                Response.Redirect("~/Admin/Dashboard.aspx");
            else
                Response.Redirect("~/Default.aspx");
        }

        // Shortcut properties for page code-behind access
        public int CurrentUserId => Convert.ToInt32(Session["UserId"]);
        public string CurrentUserFullName => Session["FullName"] as string;
        public string CurrentUserEmail => Session["UserEmail"] as string;
        public string CurrentUserRole => Session["UserRole"] as string;
    }
}
