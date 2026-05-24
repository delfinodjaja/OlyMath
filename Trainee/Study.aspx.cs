using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Trainee
{
    public partial class Study : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee" };

        public int ModuleId
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
            if (ModuleId <= 0)
            {
                Response.Redirect("BrowseModules.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Verify enrollment
                string checkSql = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
                long count = (long)DbHelper.ExecuteScalar(checkSql, 
                    new SqlParameter("@userId", CurrentUserId),
                    new SqlParameter("@moduleId", ModuleId)
                );

                if (count == 0)
                {
                    Response.Redirect($"ModuleDetail.aspx?id={ModuleId}");
                    return;
                }

                LoadModuleInfo();
                LoadMaterials();
            }
        }

        private void LoadModuleInfo()
        {
            string sql = "SELECT Title FROM Modules WHERE Id = @moduleId";
            object title = DbHelper.ExecuteScalar(sql, new SqlParameter("@moduleId", ModuleId));
            litModuleTitle.Text = title?.ToString() ?? "";
        }

        private void LoadMaterials()
        {
            string sql = @"
                SELECT sm.*, ums.IsDone
                FROM StudyMaterials sm
                LEFT JOIN UserMaterialsStatus ums ON sm.Id = ums.MaterialId AND ums.UserId = @userId
                WHERE sm.ModuleId = @moduleId
                ORDER BY sm.OrderIndex ASC";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                new SqlParameter("@userId", CurrentUserId),
                new SqlParameter("@moduleId", ModuleId)
            );

            rptStudyMaterials.DataSource = dt;
            rptStudyMaterials.DataBind();
        }

        protected void rptStudyMaterials_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "StudyMaterial")
            {
                int materialId = Convert.ToInt32(e.CommandArgument);
                int userId = CurrentUserId;

                // 1. Fetch material info
                string fetchSql = "SELECT * FROM StudyMaterials WHERE Id = @materialId";
                DataTable dtMat = DbHelper.ExecuteQuery(fetchSql, new SqlParameter("@materialId", materialId));
                if (dtMat.Rows.Count == 0) return;

                DataRow row = dtMat.Rows[0];
                string type = row["Type"].ToString();
                string url = row["ContentUrl"]?.ToString() ?? "";

                // 2. Mark material as Done
                string markSql = @"
                    IF NOT EXISTS (SELECT 1 FROM UserMaterialsStatus WHERE UserId = @userId AND MaterialId = @materialId)
                    BEGIN
                        INSERT INTO UserMaterialsStatus (UserId, MaterialId, IsDone) VALUES (@userId, @materialId, 1);
                    END
                    ELSE
                    BEGIN
                        UPDATE UserMaterialsStatus SET IsDone = 1 WHERE UserId = @userId AND MaterialId = @materialId;
                    END";
                DbHelper.ExecuteNonQuery(markSql, 
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@materialId", materialId)
                );

                // 3. Recalculate progress
                string countAllSql = "SELECT COUNT(*) FROM StudyMaterials WHERE ModuleId = @moduleId";
                long total = (long)DbHelper.ExecuteScalar(countAllSql, new SqlParameter("@moduleId", ModuleId));

                string countDoneSql = @"
                    SELECT COUNT(*) FROM UserMaterialsStatus ums
                    INNER JOIN StudyMaterials sm ON ums.MaterialId = sm.Id
                    WHERE ums.UserId = @userId AND sm.ModuleId = @moduleId AND ums.IsDone = 1";
                long completed = (long)DbHelper.ExecuteScalar(countDoneSql, 
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@moduleId", ModuleId)
                );

                int progress = 0;
                if (total > 0)
                {
                    progress = (int)((completed * 100) / total);
                }

                // Update progress table
                string updateSql = "UPDATE UserProgress SET ProgressPercentage = @progress, LastAccessed = CURRENT_TIMESTAMP WHERE UserId = @userId AND ModuleId = @moduleId";
                DbHelper.ExecuteNonQuery(updateSql, 
                    new SqlParameter("@progress", progress),
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@moduleId", ModuleId)
                );

                // Unlock next materials if progress is high (e.g. >= 75%)
                if (progress >= 75)
                {
                    string unlockSql = "UPDATE StudyMaterials SET IsLocked = 0 WHERE ModuleId = @moduleId AND IsLocked = 1";
                    DbHelper.ExecuteNonQuery(unlockSql, new SqlParameter("@moduleId", ModuleId));
                }

                // 4. Action behavior
                if (type == "QZ")
                {
                    Response.Redirect($"Assessment.aspx?id={ModuleId}");
                }
                else
                {
                    lblMessage.Text = $"Material '{row["Title"]}' marked as completed!";
                    lblMessage.Visible = true;

                    // Refresh materials view
                    LoadMaterials();

                    // Open link in new tab if valid
                    if (!string.IsNullOrEmpty(url))
                    {
                        string cleanUrl = url;
                        if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            cleanUrl = ResolveUrl("~/App_Data/" + url);
                        }
                        
                        // Register startup script to open URL in a new window/tab safely
                        string script = $"window.open('{cleanUrl}', '_blank');";
                        ScriptManager.RegisterStartupScript(this, GetType(), "OpenStudyLink", script, true);
                    }
                }
            }
        }

        protected void btnProceed_Click(object sender, EventArgs e)
        {
            Response.Redirect($"Assessment.aspx?id={ModuleId}");
        }

        // View formatting helpers
        public string GetIconClass(string type)
        {
            switch (type?.ToUpper())
            {
                case "PDF": return "orange";
                case "VID": return "teal";
                default: return "";
            }
        }

        public string GetButtonText(string type, object isDone, object isLocked)
        {
            if (isLocked != null && isLocked.ToString() == "1") return "Locked";
            
            if (isDone != null && isDone != DBNull.Value && Convert.ToInt32(isDone) == 1)
            {
                if (type == "QZ") return "Retake";
                return "Re-open";
            }
            
            switch (type?.ToUpper())
            {
                case "PDF": return "Open";
                case "VID": return "Watch";
                case "QZ": return "Start";
                default: return "View";
            }
        }

        public string GetButtonClass(object isDone, object isLocked)
        {
            if (isLocked != null && isLocked.ToString() == "1") return "list-item-badge";
            
            if (isDone != null && isDone != DBNull.Value && Convert.ToInt32(isDone) == 1)
            {
                return "list-item-badge active";
            }
            return "list-item-badge pending";
        }
    }
}
