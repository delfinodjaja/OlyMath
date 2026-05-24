using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Admin
{
    public partial class ManageUsers : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Admin" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
                ResetForm();
            }
        }

        private void LoadUsers()
        {
            lblError.Visible = false;
            try
            {
                string sql = "SELECT * FROM Users ORDER BY Role ASC, FullName ASC";
                DataTable dt = DbHelper.ExecuteQuery(sql);
                rptUsers.DataSource = dt;
                rptUsers.DataBind();
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load user directories: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void ResetForm()
        {
            hfEditUserId.Value = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtPassword.Text = "";
            txtLocation.Text = "";
            ddlRole.SelectedIndex = 0;

            litFormTitle.Text = "Create New User Account";
            spanPasswordHint.Visible = false;
            btnCancel.Visible = false;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
            lblError.Visible = false;
            lblMessage.Visible = false;
        }

        protected void rptUsers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int userId = Convert.ToInt32(e.CommandArgument);
            lblError.Visible = false;
            lblMessage.Visible = false;

            if (e.CommandName == "EditUser")
            {
                try
                {
                    string sql = "SELECT * FROM Users WHERE Id = @userId";
                    DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@userId", userId));

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        hfEditUserId.Value = row["Id"].ToString();
                        txtName.Text = row["FullName"].ToString();
                        txtEmail.Text = row["Email"].ToString();
                        txtLocation.Text = row["CityCountry"]?.ToString() ?? "";
                        ddlRole.SelectedValue = row["Role"].ToString();

                        litFormTitle.Text = "Edit User Account: " + row["FullName"].ToString();
                        spanPasswordHint.Visible = true;
                        btnCancel.Visible = true;
                        
                        txtPassword.Text = ""; // Clear password box
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Failed to load user info: " + ex.Message;
                    lblError.Visible = true;
                }
            }
            else if (e.CommandName == "DeleteUser")
            {
                // Safety: check that admin is not deleting themselves
                if (userId == CurrentUserId)
                {
                    lblError.Text = "You cannot delete your own admin account while logged in.";
                    lblError.Visible = true;
                    return;
                }

                try
                {
                    string deleteSql = "DELETE FROM Users WHERE Id = @userId";
                    int rows = DbHelper.ExecuteNonQuery(deleteSql, new SqlParameter("@userId", userId));

                    if (rows > 0)
                    {
                        lblMessage.Text = "User membership deleted successfully.";
                        lblMessage.Visible = true;
                        ResetForm();
                        LoadUsers();
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Failed to delete user: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            lblMessage.Visible = false;

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string role = ddlRole.SelectedValue;
            string location = txtLocation.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
            {
                lblError.Text = "Full Name and Email Address are required fields.";
                lblError.Visible = true;
                return;
            }

            string editIdStr = hfEditUserId.Value;

            try
            {
                if (!string.IsNullOrEmpty(editIdStr))
                {
                    // Edit Mode
                    int editId = Convert.ToInt32(editIdStr);

                    if (string.IsNullOrEmpty(password))
                    {
                        // Update without changing password
                        string sql = @"
                            UPDATE Users 
                            SET FullName = @name, Email = @email, Role = @role, CityCountry = @loc 
                            WHERE Id = @id";
                        DbHelper.ExecuteNonQuery(sql,
                            new SqlParameter("@name", name),
                            new SqlParameter("@email", email),
                            new SqlParameter("@role", role),
                            new SqlParameter("@loc", location),
                            new SqlParameter("@id", editId)
                        );
                    }
                    else
                    {
                        // Update WITH password hash reset
                        if (password.Length < 6)
                        {
                            lblError.Text = "New password must be at least 6 characters long.";
                            lblError.Visible = true;
                            return;
                        }

                        string sql = @"
                            UPDATE Users 
                            SET FullName = @name, Email = @email, Role = @role, CityCountry = @loc, PasswordHash = @hash 
                            WHERE Id = @id";
                        DbHelper.ExecuteNonQuery(sql,
                            new SqlParameter("@name", name),
                            new SqlParameter("@email", email),
                            new SqlParameter("@role", role),
                            new SqlParameter("@loc", location),
                            new SqlParameter("@hash", DbHelper.HashPassword(password)),
                            new SqlParameter("@id", editId)
                        );
                    }

                    lblMessage.Text = $"User '{name}' updated successfully.";
                    lblMessage.Visible = true;
                }
                else
                {
                    // Create Mode
                    if (string.IsNullOrEmpty(password))
                    {
                        lblError.Text = "Password is required when creating a new user account.";
                        lblError.Visible = true;
                        return;
                    }

                    if (password.Length < 6)
                    {
                        lblError.Text = "Password must be at least 6 characters long.";
                        lblError.Visible = true;
                        return;
                    }

                    // Check duplicate email
                    string checkSql = "SELECT COUNT(*) FROM Users WHERE Email = @email";
                    long count = (long)DbHelper.ExecuteScalar(checkSql, new SqlParameter("@email", email));
                    if (count > 0)
                    {
                        lblError.Text = "This email address is already in use.";
                        lblError.Visible = true;
                        return;
                    }

                    string insertSql = @"
                        INSERT INTO Users (FullName, Email, PasswordHash, Role, CityCountry)
                        VALUES (@name, @email, @hash, @role, @loc)";
                    DbHelper.ExecuteNonQuery(insertSql,
                        new SqlParameter("@name", name),
                        new SqlParameter("@email", email),
                        new SqlParameter("@hash", DbHelper.HashPassword(password)),
                        new SqlParameter("@role", role),
                        new SqlParameter("@loc", location)
                    );

                    lblMessage.Text = $"New {role} account '{name}' created successfully.";
                    lblMessage.Visible = true;
                }

                ResetForm();
                LoadUsers();
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred while saving: " + ex.Message;
                lblError.Visible = true;
            }
        }
    }
}
