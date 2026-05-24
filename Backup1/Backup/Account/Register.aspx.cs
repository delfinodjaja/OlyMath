using System;
using System.Text;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using OlyMath.Models;
using System.Web;
using Microsoft.Owin;

namespace OlyMath.Account
{
    public partial class Register : Page
    {
        protected global::System.Web.UI.WebControls.TextBox EmailInput;
        protected global::System.Web.UI.WebControls.TextBox NameInput;
        protected global::System.Web.UI.WebControls.TextBox PasswordInput;
        protected global::System.Web.UI.WebControls.DropDownList RoleDropDown;
        protected global::System.Web.UI.WebControls.Panel ErrorPanel;
        protected global::System.Web.UI.WebControls.Literal ErrorMessages;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
                Response.Redirect("/Default.aspx");
        }

        protected void RegisterButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            var userManager   = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();

            var user = new ApplicationUser
            {
                UserName       = EmailInput.Text.Trim(),
                Email          = EmailInput.Text.Trim(),
                Name           = NameInput.Text.Trim(),
                EmailConfirmed = true
            };

            var result = userManager.Create(user, PasswordInput.Text);

            if (result.Succeeded)
            {
                var role = RoleDropDown.SelectedValue; // "Trainee" or "Trainer"
                userManager.AddToRole(user.Id, role);
                signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);

                if (role == "Trainer")
                    Response.Redirect("/Trainer/Index.aspx");
                else
                    Response.Redirect("/Trainee/Index.aspx");
            }
            else
            {
                var errors = new StringBuilder();
                foreach (var error in result.Errors)
                    errors.AppendFormat("<li>{0}</li>", error);

                ErrorPanel.Visible    = true;
                ErrorMessages.Text    = errors.ToString();
            }
        }
    }
}
