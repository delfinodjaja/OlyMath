using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace OlyMath.Trainer
{
    public partial class EditModule : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainer" };

        private int ModuleId
        {
            get
            {
                int id = 0;
                int.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ModuleId > 0)
                {
                    LoadModule();
                }
            }
        }

        private void LoadModule()
        {
            lblError.Visible = false;

            try
            {
                string sql = "SELECT * FROM Modules WHERE Id = @moduleId AND CreatedByUserId = @userId";
                DataTable dt = DbHelper.ExecuteQuery(sql, 
                    new SQLiteParameter("@moduleId", ModuleId),
                    new SQLiteParameter("@userId", CurrentUserId)
                );

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtTitle.Text = row["Title"].ToString();
                    ddlTopic.SelectedValue = row["Topic"].ToString();
                    txtTime.Text = row["EstimatedTime"].ToString();
                    txtDescription.Text = row["Description"].ToString();
                    
                    litPageTitle.Text = "Edit Module";
                }
                else
                {
                    // Unauthorized or missing module
                    Response.Redirect("Dashboard.aspx");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load module details: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            string title = txtTitle.Text.Trim();
            string topic = ddlTopic.SelectedValue;
            string time = txtTime.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(description))
            {
                lblError.Text = "All fields are required. Please complete the form.";
                lblError.Visible = true;
                return;
            }

            try
            {
                if (ModuleId > 0)
                {
                    // Edit Module
                    string sql = @"
                        UPDATE Modules 
                        SET Title = @title, Topic = @topic, EstimatedTime = @time, Description = @desc 
                        WHERE Id = @moduleId AND CreatedByUserId = @userId";

                    int rows = DbHelper.ExecuteNonQuery(sql,
                        new SQLiteParameter("@title", title),
                        new SQLiteParameter("@topic", topic),
                        new SQLiteParameter("@time", time),
                        new SQLiteParameter("@desc", description),
                        new SQLiteParameter("@moduleId", ModuleId),
                        new SQLiteParameter("@userId", CurrentUserId)
                    );

                    if (rows == 0)
                    {
                        lblError.Text = "Failed to update module. Verify ownership permissions.";
                        lblError.Visible = true;
                        return;
                    }
                }
                else
                {
                    // Create Module
                    string sql = @"
                        INSERT INTO Modules (Title, Topic, EstimatedTime, Description, CreatedByUserId)
                        VALUES (@title, @topic, @time, @desc, @userId)";

                    DbHelper.ExecuteNonQuery(sql,
                        new SQLiteParameter("@title", title),
                        new SQLiteParameter("@topic", topic),
                        new SQLiteParameter("@time", time),
                        new SQLiteParameter("@desc", description),
                        new SQLiteParameter("@userId", CurrentUserId)
                    );
                }

                Response.Redirect("Dashboard.aspx");
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred while saving: " + ex.Message;
                lblError.Visible = true;
            }
        }
    }
}
