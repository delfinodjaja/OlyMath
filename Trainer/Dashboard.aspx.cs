using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OlyMath.Trainer
{
    public partial class Dashboard : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainer" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litTrainerName.Text = CurrentUserFullName;
                LoadStats();
                LoadRecentActivity();
                LoadLatestDiscussions();
            }
        }

        private void LoadStats()
        {
            int userId = CurrentUserId;

            // 1. Active Modules Count (created by trainer, status Approved/Published or not Draft)
            string sqlModules = "SELECT COUNT(*) FROM Modules WHERE CreatedByUserId = @userId AND Status != 'Draft'";
            long activeCount = Convert.ToInt64(DbHelper.ExecuteScalar(sqlModules, new SqlParameter("@userId", userId)));
            litActiveModulesCount.Text = activeCount.ToString();

            // 2. Enrolled Trainees (distinct trainees in trainer's modules)
            string sqlTrainees = @"
                SELECT COUNT(DISTINCT up.UserId) 
                FROM UserProgress up 
                INNER JOIN Modules m ON up.ModuleId = m.Id 
                WHERE m.CreatedByUserId = @userId";
            long traineesCount = Convert.ToInt64(DbHelper.ExecuteScalar(sqlTrainees, new SqlParameter("@userId", userId)));
            litEnrolledTraineesCount.Text = traineesCount.ToString();

            // 3. Assessments Submitted
            string sqlAssess = @"
                SELECT COUNT(*) 
                FROM UserAssessments ua 
                INNER JOIN Modules m ON ua.ModuleId = m.Id 
                WHERE m.CreatedByUserId = @userId";
            long assessCount = Convert.ToInt64(DbHelper.ExecuteScalar(sqlAssess, new SqlParameter("@userId", userId)));
            litAssessmentsCount.Text = assessCount.ToString();

            // 4. Pending Reviews (assessments submitted with score < 60)
            string sqlPending = @"
                SELECT COUNT(*) 
                FROM UserAssessments ua 
                INNER JOIN Modules m ON ua.ModuleId = m.Id 
                WHERE m.CreatedByUserId = @userId AND ua.Score < 60";
            long pendingCount = Convert.ToInt64(DbHelper.ExecuteScalar(sqlPending, new SqlParameter("@userId", userId)));
            litPendingReviewsCount.Text = pendingCount.ToString();
        }

        private void LoadRecentActivity()
        {
            try
            {
                string sql = @"
                    SELECT TOP 6 * FROM (
                        SELECT 'Submission' AS ActivityType, u.FullName, m.Title AS ModuleTitle, ua.CompletedAt AS ActivityDate, ua.Score
                        FROM UserAssessments ua
                        INNER JOIN Users u ON ua.UserId = u.Id
                        INNER JOIN Modules m ON ua.ModuleId = m.Id
                        WHERE m.CreatedByUserId = @userId
                        UNION ALL
                        SELECT 'Enrollment' AS ActivityType, u.FullName, m.Title AS ModuleTitle, up.LastAccessed AS ActivityDate, 0 AS Score
                        FROM UserProgress up
                        INNER JOIN Users u ON up.UserId = u.Id
                        INNER JOIN Modules m ON up.ModuleId = m.Id
                        WHERE m.CreatedByUserId = @userId
                    ) AS CombinedActivities
                    ORDER BY ActivityDate DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@userId", CurrentUserId));

                if (dt.Rows.Count > 0)
                {
                    rptRecentActivity.DataSource = dt;
                    rptRecentActivity.DataBind();
                    phNoActivity.Visible = false;
                }
                else
                {
                    rptRecentActivity.DataSource = null;
                    rptRecentActivity.DataBind();
                    phNoActivity.Visible = true;
                }
            }
            catch
            {
                phNoActivity.Visible = true;
            }
        }

        private void LoadLatestDiscussions()
        {
            try
            {
                string sql = @"
                    SELECT TOP 3 d.Id, d.Title, d.Content, d.Topic, d.CreatedAt, u.FullName
                    FROM Discussions d
                    INNER JOIN Users u ON d.UserId = u.Id
                    ORDER BY d.CreatedAt DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql);

                if (dt.Rows.Count > 0)
                {
                    rptLatestDiscussions.DataSource = dt;
                    rptLatestDiscussions.DataBind();
                    phNoDiscussions.Visible = false;
                }
                else
                {
                    rptLatestDiscussions.DataSource = null;
                    rptLatestDiscussions.DataBind();
                    phNoDiscussions.Visible = true;
                }
            }
            catch
            {
                phNoDiscussions.Visible = true;
            }
        }

        public string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "OM";
            string[] parts = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        public string GetTimeAgo(object dateObj)
        {
            if (dateObj == null || dateObj == DBNull.Value) return "just now";
            DateTime dt = Convert.ToDateTime(dateObj);
            TimeSpan span = DateTime.Now - dt;

            if (span.TotalSeconds < 0) return "just now";
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return $"{Math.Floor(span.TotalMinutes)} min ago";
            if (span.TotalHours < 24) return $"{Math.Floor(span.TotalHours)} hours ago";
            if (span.TotalDays < 2) return "yesterday";
            return dt.ToString("MMM dd, yyyy");
        }

        public string GetTopicLabel(string topicCode)
        {
            switch (topicCode?.ToLower())
            {
                case "nt": return "Number Theory";
                case "co": return "Combinatorics";
                case "ge": return "Geometry";
                case "al": return "Algebra";
                case "iq": return "Inequalities";
                case "lg": return "Logic";
                default: return "General";
            }
        }
    }
}
