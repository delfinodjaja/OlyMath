using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace OlyMath
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // If already logged in, redirect directly to dashboard
                if (Session["UserId"] != null)
                {
                    RedirectToDashboard(Session["UserRole"]?.ToString());
                    return;
                }

                // Check if starting in registration mode
                string mode = Request.QueryString["mode"];
                if (string.Equals(mode, "register", StringComparison.OrdinalIgnoreCase))
                {
                    hfActiveTab.Value = "register";
                }
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            lblSuccess.Visible = false;

            string email = txtLoginEmail.Text.Trim();
            string password = txtLoginPassword.Text;
            string role = ddlLoginRole.SelectedValue;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter both your email and password.";
                lblError.Visible = true;
                return;
            }

            try
            {
                string sql = "SELECT * FROM Users WHERE Email = @email AND Role = @role";
                DataTable dt = DbHelper.ExecuteQuery(sql, 
                    new SqlParameter("@email", email),
                    new SqlParameter("@role", role)
                );

                if (dt.Rows.Count > 0)
                {
                    string dbHash = dt.Rows[0]["PasswordHash"].ToString();
                    string inputHash = DbHelper.HashPassword(password);

                    if (string.Equals(dbHash, inputHash, StringComparison.OrdinalIgnoreCase))
                    {
                        // Set Session variables
                        Session["UserId"] = dt.Rows[0]["Id"];
                        Session["FullName"] = dt.Rows[0]["FullName"];
                        Session["UserEmail"] = dt.Rows[0]["Email"];
                        Session["UserRole"] = dt.Rows[0]["Role"];

                        // Set forms auth cookie
                        FormsAuthentication.SetAuthCookie(email, false);

                        // Redirect to return url or dashboard
                        string returnUrl = Request.QueryString["returnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            Response.Redirect(returnUrl);
                        }
                        else
                        {
                            RedirectToDashboard(role);
                        }
                        return;
                    }
                }

                lblError.Text = "Invalid email, password, or role choice.";
                lblError.Visible = true;
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred during sign in: " + HttpUtility.HtmlEncode(ex.Message);
                lblError.Visible = true;
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            lblSuccess.Visible = false;

            string name = txtRegName.Text.Trim();
            string email = txtRegEmail.Text.Trim();
            string password = txtRegPassword.Text;
            string role = ddlRegRole.SelectedValue;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "All fields are required for registration.";
                lblError.Visible = true;
                return;
            }

            if (password.Length < 6)
            {
                lblError.Text = "Password must be at least 6 characters long.";
                lblError.Visible = true;
                return;
            }

            try
            {
                // Check if email already registered
                string checkSql = "SELECT COUNT(*) FROM Users WHERE Email = @email";
                long count = (long)DbHelper.ExecuteScalar(checkSql, new SqlParameter("@email", email));

                if (count > 0)
                {
                    lblError.Text = "This email is already registered.";
                    lblError.Visible = true;
                    return;
                }

                // Insert new user
                string insertSql = "INSERT INTO Users (FullName, Email, PasswordHash, Role) VALUES (@name, @email, @hash, @role)";
                DbHelper.ExecuteNonQuery(insertSql,
                    new SqlParameter("@name", name),
                    new SqlParameter("@email", email),
                    new SqlParameter("@hash", DbHelper.HashPassword(password)),
                    new SqlParameter("@role", role)
                );

                lblSuccess.Text = "Registration successful! You can now sign in using the Login tab.";
                lblSuccess.Visible = true;

                // Reset forms
                txtRegName.Text = "";
                txtRegEmail.Text = "";
                txtRegPassword.Text = "";
                
                // Show login tab
                hfActiveTab.Value = "login";
            }
            catch (Exception ex)
            {
                lblError.Text = "Registration failed: " + HttpUtility.HtmlEncode(ex.Message);
                lblError.Visible = true;
            }
        }

        private void RedirectToDashboard(string role)
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
    }
}
