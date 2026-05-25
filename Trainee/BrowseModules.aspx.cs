using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OlyMath.Trainee
{
    public partial class BrowseModules : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadModules();
            }
        }

        private void LoadModules()
        {
            string sql = @"
                SELECT m.Id, m.Title, m.Topic,
                (SELECT COUNT(*) FROM StudyMaterials sm WHERE sm.ModuleId = m.Id) AS MaterialCount,
                up.ProgressPercentage
                FROM Modules m
                LEFT JOIN UserProgress up ON m.Id = up.ModuleId AND up.UserId = @userId
                WHERE m.Status = 'Approved'
                ORDER BY m.Topic, m.Title";

            DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@userId", CurrentUserId));
            rptAllModules.DataSource = dt;
            rptAllModules.DataBind();
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

        public string GetEnrollmentStatus(object progressVal)
        {
            if (progressVal == null || progressVal == DBNull.Value)
            {
                return "Not Enrolled";
            }
            int prog = Convert.ToInt32(progressVal);
            if (prog == 100) return "Completed";
            return prog + "%";
        }
    }
}
