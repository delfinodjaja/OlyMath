using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace OlyMath.Trainee
{
    public partial class Dashboard : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litTraineeName.Text = CurrentUserFullName;
                LoadStats();
                LoadEnrolledModules();
                LoadLatestDiscussion();
            }
        }

        private void LoadStats()
        {
            int userId = CurrentUserId;

            // 1. Active Modules Count
            string sqlActive = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ProgressPercentage < 100";
            long activeCount = (long)DbHelper.ExecuteScalar(sqlActive, new SQLiteParameter("@userId", userId));
            litActiveModulesCount.Text = activeCount.ToString();

            // 2. Average Progress
            string sqlAvg = "SELECT AVG(ProgressPercentage) FROM UserProgress WHERE UserId = @userId";
            object avgResult = DbHelper.ExecuteScalar(sqlAvg, new SQLiteParameter("@userId", userId));
            int avgProgress = 0;
            if (avgResult != null && avgResult != DBNull.Value)
            {
                avgProgress = Convert.ToInt32(Math.Round(Convert.ToDouble(avgResult)));
            }
            litAvgProgress.Text = avgProgress.ToString();

            // 3. Certificates Earned
            string sqlCerts = "SELECT COUNT(*) FROM UserAssessments WHERE UserId = @userId AND CertificateId IS NOT NULL";
            long certsCount = (long)DbHelper.ExecuteScalar(sqlCerts, new SQLiteParameter("@userId", userId));
            litCertsCount.Text = certsCount.ToString();

            // 4. Assessments Done
            string sqlAssess = "SELECT COUNT(*) FROM UserAssessments WHERE UserId = @userId";
            long assessCount = (long)DbHelper.ExecuteScalar(sqlAssess, new SQLiteParameter("@userId", userId));
            litAssessmentsCount.Text = assessCount.ToString();
        }

        private void LoadEnrolledModules()
        {
            string sql = @"
                SELECT m.Id, m.Title, m.Topic, m.EstimatedTime, up.ProgressPercentage,
                (SELECT COUNT(*) FROM StudyMaterials sm WHERE sm.ModuleId = m.Id) AS MaterialCount
                FROM Modules m
                INNER JOIN UserProgress up ON m.Id = up.ModuleId
                WHERE up.UserId = @userId
                ORDER BY up.LastAccessed DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql, new SQLiteParameter("@userId", CurrentUserId));
            if (dt.Rows.Count > 0)
            {
                rptEnrolledModules.DataSource = dt;
                rptEnrolledModules.DataBind();
                phNoModules.Visible = false;
            }
            else
            {
                phNoModules.Visible = true;
            }
        }

        private void LoadLatestDiscussion()
        {
            string sql = @"
                SELECT d.Id, d.Title, d.Content, d.Topic, d.LikesCount, d.CreatedAt, u.FullName, u.CityCountry,
                (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount
                FROM Discussions d
                INNER JOIN Users u ON d.UserId = u.Id
                ORDER BY d.CreatedAt DESC LIMIT 1";

            DataTable dt = DbHelper.ExecuteQuery(sql);
            rptLatestDiscussions.DataSource = dt;
            rptLatestDiscussions.DataBind();
        }

        public string GetTopicClass(string topic)
        {
            if (string.Equals(topic, "Combinatorics", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(topic, "Algebra", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(topic, "Logic", StringComparison.OrdinalIgnoreCase))
            {
                return "teal";
            }
            return "";
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

            // Handle potential timezone skew (negative timespan)
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
