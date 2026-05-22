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
            // Already logged in — redirect to appropriate dashboard
            if (Request.IsAuthenticated)
                RedirectByRole();
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var email    = EmailInput.Text.Trim();
            var password = PasswordInput.Text;

            var userManager  = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();

            var result = signInManager.PasswordSignIn(email, password, isPersistent: false, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    RedirectByRole();
                    break;

                default:
                    ShowError("Invalid email or password.");
                    break;
            }
        }

        private void RedirectByRole()
        {
            if (User.IsInRole("Admin"))
                Response.Redirect("/Admin/Index.aspx");
            else if (User.IsInRole("Trainer"))
                Response.Redirect("/Trainer/Index.aspx");
            else
                Response.Redirect("/Trainee/Index.aspx");
        }

        private void ShowError(string message)
        {
            ErrorPanel.Visible = true;
            ErrorMessage.Text  = message;
        }
    }
}
