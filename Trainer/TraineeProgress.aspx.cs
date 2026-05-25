using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OlyMath.Trainer
{
    public partial class TraineeProgress : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainer" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTraineeProgress();
            }
        }

        private void LoadTraineeProgress()
        {
            lblError.Visible = false;

            try
            {
                // Query all users progress for modules created by this trainer
                string sql = @"
                    SELECT u.FullName AS TraineeName, m.Title AS ModuleTitle, up.ProgressPercentage, up.LastAccessed
                    FROM UserProgress up
                    INNER JOIN Users u ON up.UserId = u.Id
                    INNER JOIN Modules m ON up.ModuleId = m.Id
                    WHERE m.CreatedByUserId = @trainerId
                    ORDER BY up.LastAccessed DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@trainerId", CurrentUserId));

                if (dt.Rows.Count > 0)
                {
                    rptTraineeProgress.DataSource = dt;
                    rptTraineeProgress.DataBind();
                    phNoProgress.Visible = false;
                }
                else
                {
                    rptTraineeProgress.DataSource = null;
                    rptTraineeProgress.DataBind();
                    phNoProgress.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load trainee progress: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected string GetStatusBadge(int progress)
        {
            if (progress >= 100)
            {
                return "<span class=\"badge green\" style=\"background:#EEF0FF; color:#5B60F0;\">Complete</span>";
            }
            else if (progress >= 50)
            {
                return "<span class=\"badge green\">On Track</span>";
            }
            else
            {
                return "<span class=\"badge orange\">Behind</span>";
            }
        }
    }
}
