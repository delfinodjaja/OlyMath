using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OlyMath.Trainee
{
    public partial class ModuleDetail : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee" };

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
            if (ModuleId <= 0)
            {
                Response.Redirect("BrowseModules.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadModuleDetails();
                LoadMaterials();
            }
        }

        private void LoadModuleDetails()
        {
            string sql = "SELECT * FROM Modules WHERE Id = @moduleId";
            DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@moduleId", ModuleId));

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("BrowseModules.aspx");
                return;
            }

            DataRow row = dt.Rows[0];
            litTitle.Text = row["Title"].ToString();
            litDescription.Text = row["Description"].ToString();

            // Load counts
            string countSql = "SELECT COUNT(*) FROM StudyMaterials WHERE ModuleId = @moduleId";
            long matCount = (long)DbHelper.ExecuteScalar(countSql, new SqlParameter("@moduleId", ModuleId));
            litMetaStats.Text = $"{matCount} materials &nbsp;|&nbsp; {row["EstimatedTime"]} &nbsp;|&nbsp; {row["Topic"]}";

            // Check enrollment
            string enrollSql = "SELECT * FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
            DataTable dtEnroll = DbHelper.ExecuteQuery(enrollSql, 
                new SqlParameter("@userId", CurrentUserId),
                new SqlParameter("@moduleId", ModuleId)
            );

            if (dtEnroll.Rows.Count > 0)
            {
                // Enrolled
                lblEnrollBadge.Text = "Enrolled";
                lblEnrollBadge.CssClass = "badge green";

                btnEnroll.Visible = false;
                btnStudyTop.Visible = true;
                divProgressCard.Visible = true;
                divFooterActions.Visible = true;

                int progress = Convert.ToInt32(dtEnroll.Rows[0]["ProgressPercentage"]);
                litProgressText.Text = progress.ToString();
                divProgressBar.Style["width"] = progress + "%";
            }
            else
            {
                // Not enrolled
                lblEnrollBadge.Text = "Not Enrolled";
                lblEnrollBadge.CssClass = "badge grey";

                btnEnroll.Visible = true;
                btnStudyTop.Visible = false;
                divProgressCard.Visible = false;
                divFooterActions.Visible = false;
            }
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

            if (dt.Rows.Count > 0)
            {
                rptMaterials.DataSource = dt;
                rptMaterials.DataBind();
                phNoMaterials.Visible = false;
            }
            else
            {
                phNoMaterials.Visible = true;
            }
        }

        protected void btnEnroll_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = @"
                    IF NOT EXISTS (SELECT 1 FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId)
                    BEGIN
                        INSERT INTO UserProgress (UserId, ModuleId, ProgressPercentage) VALUES (@userId, @moduleId, 0);
                    END";
                DbHelper.ExecuteNonQuery(sql, 
                    new SqlParameter("@userId", CurrentUserId),
                    new SqlParameter("@moduleId", ModuleId)
                );
                
                // Refresh
                Response.Redirect($"ModuleDetail.aspx?id={ModuleId}");
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to enroll: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnStudy_Click(object sender, EventArgs e)
        {
            Response.Redirect($"Study.aspx?id={ModuleId}");
        }

        protected void btnAssessment_Click(object sender, EventArgs e)
        {
            Response.Redirect($"Assessment.aspx?id={ModuleId}");
        }

        public string GetMaterialIconClass(string type)
        {
            switch (type?.ToUpper())
            {
                case "PDF": return "mat-pdf";
                case "VID": return "mat-vid";
                case "QZ": return "mat-qz";
                default: return "mat-pdf";
            }
        }

        public string GetMaterialMeta(string type, string size, string isLocked)
        {
            if (isLocked == "1") return "Locked";
            string typeName = "Reading";
            if (type == "VID") typeName = "Video";
            if (type == "QZ") typeName = "Quiz";
            return $"{typeName} · {size}";
        }

        public string GetStatusText(object isDoneVal, object isLockedVal)
        {
            if (isLockedVal != null && isLockedVal.ToString() == "1") return "Locked";
            
            // Check if enrolled
            string checkSql = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
            long count = (long)DbHelper.ExecuteScalar(checkSql, 
                new SqlParameter("@userId", CurrentUserId), 
                new SqlParameter("@moduleId", ModuleId)
            );
            
            if (count == 0) return "Pending"; // Show pending if not enrolled
            
            if (isDoneVal != null && isDoneVal != DBNull.Value && Convert.ToInt32(isDoneVal) == 1)
            {
                return "Done";
            }
            return "Pending";
        }

        public string GetStatusBadgeClass(object isDoneVal, object isLockedVal)
        {
            if (isLockedVal != null && isLockedVal.ToString() == "1") return "grey";
            
            // Check if enrolled
            string checkSql = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
            long count = (long)DbHelper.ExecuteScalar(checkSql, 
                new SqlParameter("@userId", CurrentUserId), 
                new SqlParameter("@moduleId", ModuleId)
            );
            if (count == 0) return "orange";

            if (isDoneVal != null && isDoneVal != DBNull.Value && Convert.ToInt32(isDoneVal) == 1)
            {
                return "green";
            }
            return "orange";
        }
    }
}
