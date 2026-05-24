using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Trainer
{
    public partial class Dashboard : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainer" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTrainerModules();
            }
        }

        private void LoadTrainerModules()
        {
            lblError.Visible = false;
            lblMessage.Visible = false;

            try
            {
                string sql = @"
                    SELECT m.Id, m.Title, m.Topic,
                    (SELECT COUNT(*) FROM StudyMaterials sm WHERE sm.ModuleId = m.Id) AS MaterialCount,
                    (SELECT COUNT(*) FROM Questions q WHERE q.ModuleId = m.Id) AS QuestionCount
                    FROM Modules m
                    WHERE m.CreatedByUserId = @userId
                    ORDER BY m.CreatedAt DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@userId", CurrentUserId));

                if (dt.Rows.Count > 0)
                {
                    rptModules.DataSource = dt;
                    rptModules.DataBind();
                    phNoModules.Visible = false;
                }
                else
                {
                    rptModules.DataSource = null;
                    rptModules.DataBind();
                    phNoModules.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load modules: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteModule")
            {
                int moduleId = Convert.ToInt32(e.CommandArgument);
                int userId = CurrentUserId;

                try
                {
                    // SQLite handles cascade deletes automatically since we declared foreign keys with ON DELETE CASCADE
                    string deleteSql = "DELETE FROM Modules WHERE Id = @moduleId AND CreatedByUserId = @userId";
                    int rows = DbHelper.ExecuteNonQuery(deleteSql,
                        new SqlParameter("@moduleId", moduleId),
                        new SqlParameter("@userId", userId)
                    );

                    if (rows > 0)
                    {
                        lblMessage.Text = "Module and all associated materials/quizzes deleted successfully.";
                        lblMessage.Visible = true;
                    }
                    else
                    {
                        lblError.Text = "Failed to delete module. Verify ownership permissions.";
                        lblError.Visible = true;
                    }

                    LoadTrainerModules();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Deletion failed: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }
    }
}
