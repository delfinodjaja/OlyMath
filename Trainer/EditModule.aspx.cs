using System;
using System.Data;
using System.Data.SqlClient;
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
                    new SqlParameter("@moduleId", ModuleId),
                    new SqlParameter("@userId", CurrentUserId)
                );

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtTitle.Text = row["Title"].ToString();
                    ddlTopic.SelectedValue = row["Topic"].ToString();
                    txtTime.Text = row["EstimatedTime"].ToString();
                    txtDescription.Text = row["Description"].ToString();
                    if (row["Difficulty"] != DBNull.Value)
                    {
                        ddlDifficulty.SelectedValue = row["Difficulty"].ToString();
                    }
                    
                    litPageTitle.Text = "Edit Module";
                }
                else
                {
                    // Unauthorized or missing module
                    Response.Redirect("MyModules.aspx");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load module details: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveModule("Draft", false);
        }

        protected void btnSaveAddMaterials_Click(object sender, EventArgs e)
        {
            SaveModule("Approved", true);
        }

        private void SaveModule(string status, bool goToAddMaterials)
        {
            lblError.Visible = false;

            string title = txtTitle.Text.Trim();
            string topic = ddlTopic.SelectedValue;
            string time = txtTime.Text.Trim();
            string description = txtDescription.Text.Trim();
            string difficulty = ddlDifficulty.SelectedValue;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(description))
            {
                lblError.Text = "All fields are required. Please complete the form.";
                lblError.Visible = true;
                return;
            }

            try
            {
                int savedId = ModuleId;
                if (ModuleId > 0)
                {
                    // Edit Module
                    string sql = @"
                        UPDATE Modules 
                        SET Title = @title, Topic = @topic, EstimatedTime = @time, Description = @desc, Status = @status, Difficulty = @difficulty 
                        WHERE Id = @moduleId AND CreatedByUserId = @userId";

                    int rows = DbHelper.ExecuteNonQuery(sql,
                        new SqlParameter("@title", title),
                        new SqlParameter("@topic", topic),
                        new SqlParameter("@time", time),
                        new SqlParameter("@desc", description),
                        new SqlParameter("@status", status),
                        new SqlParameter("@difficulty", difficulty),
                        new SqlParameter("@moduleId", ModuleId),
                        new SqlParameter("@userId", CurrentUserId)
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
                        INSERT INTO Modules (Title, Topic, EstimatedTime, Description, CreatedByUserId, Status, Difficulty)
                        OUTPUT INSERTED.Id
                        VALUES (@title, @topic, @time, @desc, @userId, @status, @difficulty)";

                    savedId = Convert.ToInt32(DbHelper.ExecuteScalar(sql,
                        new SqlParameter("@title", title),
                        new SqlParameter("@topic", topic),
                        new SqlParameter("@time", time),
                        new SqlParameter("@desc", description),
                        new SqlParameter("@userId", CurrentUserId),
                        new SqlParameter("@status", status),
                        new SqlParameter("@difficulty", difficulty)
                    ));
                }

                if (goToAddMaterials)
                {
                    Response.Redirect($"ManageMaterials.aspx?id={savedId}");
                }
                else
                {
                    Response.Redirect("MyModules.aspx");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred while saving: " + ex.Message;
                lblError.Visible = true;
            }
        }
    }
}
