using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OlyMath.Admin
{
    public partial class Dashboard : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Admin" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSystemMetrics();
                LoadRecentRegistrations();
            }
        }

        private void LoadSystemMetrics()
        {
            try
            {
                // 1. Total Users
                string sqlUsers = "SELECT COUNT(*) FROM Users";
                long totalUsers = Convert.ToInt64(DbHelper.ExecuteScalar(sqlUsers));
                litTotalUsers.Text = totalUsers.ToString();

                // 2. Trainers
                string sqlTrainers = "SELECT COUNT(*) FROM Users WHERE Role = 'Trainer'";
                long totalTrainers = Convert.ToInt64(DbHelper.ExecuteScalar(sqlTrainers));
                litTotalTrainers.Text = totalTrainers.ToString();

                // 3. Active Modules
                string sqlModules = "SELECT COUNT(*) FROM Modules WHERE Status = 'Approved'";
                long activeModules = Convert.ToInt64(DbHelper.ExecuteScalar(sqlModules));
                litActiveModules.Text = activeModules.ToString();

                // 4. Enrollments
                string sqlEnrollments = "SELECT COUNT(*) FROM UserProgress";
                long totalEnrollments = Convert.ToInt64(DbHelper.ExecuteScalar(sqlEnrollments));
                litTotalEnrollments.Text = totalEnrollments.ToString();
            }
            catch (Exception)
            {
                litTotalUsers.Text = "0";
                litTotalTrainers.Text = "0";
                litActiveModules.Text = "0";
                litTotalEnrollments.Text = "0";
            }
        }

        private void LoadRecentRegistrations()
        {
            try
            {
                string sql = "SELECT TOP 5 FullName, Email, Role, CreatedAt, Status FROM Users ORDER BY CreatedAt DESC";
                DataTable dt = DbHelper.ExecuteQuery(sql);

                if (dt.Rows.Count > 0)
                {
                    rptRecentUsers.DataSource = dt;
                    rptRecentUsers.DataBind();
                    phNoRecent.Visible = false;
                }
                else
                {
                    rptRecentUsers.DataSource = null;
                    rptRecentUsers.DataBind();
                    phNoRecent.Visible = true;
                }
            }
            catch (Exception)
            {
                phNoRecent.Visible = true;
            }
        }
    }
}

