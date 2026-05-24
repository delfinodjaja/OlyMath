using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OlyMath.Trainee
{
    public partial class Achievements : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadHistory();
                LoadCertificates();
            }
        }

        private void LoadHistory()
        {
            int userId = CurrentUserId;

            // 1. Load assessment attempts
            string sqlHistory = @"
                SELECT ua.Score, ua.MaxScore, ua.CompletedAt, ua.CertificateId, m.Title AS ModuleTitle
                FROM UserAssessments ua
                INNER JOIN Modules m ON ua.ModuleId = m.Id
                WHERE ua.UserId = @userId
                ORDER BY ua.CompletedAt DESC";
            DataTable dtHistory = DbHelper.ExecuteQuery(sqlHistory, new SqlParameter("@userId", userId));
            rptAssessmentHistory.DataSource = dtHistory;
            rptAssessmentHistory.DataBind();

            // 2. Load in-progress modules (not completed and no assessment attempt)
            string sqlInProgress = @"
                SELECT m.Title
                FROM UserProgress up
                INNER JOIN Modules m ON up.ModuleId = m.Id
                WHERE up.UserId = @userId AND up.ProgressPercentage < 100
                AND m.Id NOT IN (SELECT ModuleId FROM UserAssessments WHERE UserId = @userId)";
            DataTable dtInProgress = DbHelper.ExecuteQuery(sqlInProgress, new SqlParameter("@userId", userId));
            rptInProgressModules.DataSource = dtInProgress;
            rptInProgressModules.DataBind();

            // Check if entirely empty
            if (dtHistory.Rows.Count == 0 && dtInProgress.Rows.Count == 0)
            {
                phNoHistory.Visible = true;
            }
            else
            {
                phNoHistory.Visible = false;
            }
        }

        private void LoadCertificates()
        {
            string sqlCerts = @"
                SELECT ua.CertificateId, ua.CompletedAt, m.Title AS ModuleTitle
                FROM UserAssessments ua
                INNER JOIN Modules m ON ua.ModuleId = m.Id
                WHERE ua.UserId = @userId AND ua.CertificateId IS NOT NULL
                ORDER BY ua.CompletedAt DESC";

            DataTable dtCerts = DbHelper.ExecuteQuery(sqlCerts, new SqlParameter("@userId", CurrentUserId));
            if (dtCerts.Rows.Count > 0)
            {
                rptCertificates.DataSource = dtCerts;
                rptCertificates.DataBind();
                phNoCerts.Visible = false;
            }
            else
            {
                phNoCerts.Visible = true;
            }
        }
    }
}
