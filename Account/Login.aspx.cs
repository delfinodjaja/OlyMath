using System;
using System.Web.Security;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using OlyMath.Models;
using System.Web;
using Microsoft.Owin;


namespace OlyMath.Account
{
    public partial class Login : Page
    {

        protected global::System.Web.UI.WebControls.Panel ErrorPanel;
        protected global::System.Web.UI.WebControls.Literal ErrorMessage;
        protected global::System.Web.UI.WebControls.TextBox EmailInput;
        protected global::System.Web.UI.WebControls.TextBox PasswordInput;
        protected global::System.Web.UI.WebControls.Button LoginButton;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
                RedirectByRole(Context.GetOwinContext().GetUserManager<ApplicationUserManager>(), User.Identity.Name);
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var email    = EmailInput.Text.Trim();
            var password = PasswordInput.Text;

            var userManager   = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();

            var result = signInManager.PasswordSignIn(email, password, isPersistent: false, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    RedirectByRole(userManager, email);
                    break;

                default:
                    ShowError("Invalid email or password.");
                    break;
            }
        }

        private void RedirectByRole(ApplicationUserManager userManager, string email)
        {
            var user = userManager.FindByEmail(email);
            if (user != null && userManager.IsInRole(user.Id, "Admin"))
            {
                Response.Redirect("/TrainerDashboard.aspx");
            }
            else if (user != null && userManager.IsInRole(user.Id, "Trainer"))
            {
                Response.Redirect("/TrainerDashboard.aspx");
            }
            else
            {
                Response.Redirect("/AssessmentList.aspx");
            }
        }

        private void ShowError(string message)
        {
            ErrorPanel.Visible = true;
            ErrorMessage.Text  = message;
        }
    }
}

