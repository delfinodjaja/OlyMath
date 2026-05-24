using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace OlyMath
{
    public partial class AssessmentDashboardWidget : System.Web.UI.UserControl
    {
        protected global::System.Web.UI.WebControls.Label lblTotalAssessments;
        protected global::System.Web.UI.WebControls.Label lblTotalAttempts;
        protected global::System.Web.UI.WebControls.Label lblPassedAttempts;
        protected global::System.Web.UI.WebControls.Label lblMessage;
        protected global::System.Web.UI.WebControls.GridView gvRecentAttempts;

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSummary();
                LoadRecentAttempts();
            }
        }

        private void LoadSummary()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    lblTotalAssessments.Text = GetCount(
                        con,
                        "SELECT COUNT(*) FROM Assessments"
                    ).ToString();

                    lblTotalAttempts.Text = GetCount(
                        con,
                        "SELECT COUNT(*) FROM AssessmentAttempts"
                    ).ToString();

                    lblPassedAttempts.Text = GetCount(
                        con,
                        "SELECT COUNT(*) FROM AssessmentAttempts WHERE IsPassed = 1"
                    ).ToString();
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Unable to load assessment summary: " + ex.Message;
            }
        }

        private int GetCount(SqlConnection con, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void LoadRecentAttempts()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 5
                            a.Title,
                            aa.UserName,
                            aa.Score,
                            aa.IsPassed,
                            aa.AttemptDate
                        FROM AssessmentAttempts aa
                        INNER JOIN Assessments a
                            ON aa.AssessmentId = a.AssessmentId
                        ORDER BY aa.AttemptDate DESC";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, con))
                    {
                        DataTable table = new DataTable();

                        da.Fill(table);

                        gvRecentAttempts.DataSource = table;
                        gvRecentAttempts.DataBind();

                        if (table.Rows.Count == 0)
                        {
                            lblMessage.Text = "No assessment attempts yet.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Unable to load recent assessment attempts: " + ex.Message;
            }
        }
    }
}