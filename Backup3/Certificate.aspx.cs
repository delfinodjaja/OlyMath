using System;
using System.Configuration;
using System.Data.SqlClient;

namespace OlyMath
{
    public partial class Certificate : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.Literal litTraineeName;
        protected global::System.Web.UI.WebControls.Literal litAssessmentTitle;
        protected global::System.Web.UI.WebControls.Literal litScore;
        protected global::System.Web.UI.WebControls.Literal litIssueDate;
        protected global::System.Web.UI.WebControls.Literal litCertID;

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (string.IsNullOrEmpty(Request.QueryString["attemptId"]))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            int attemptId = Convert.ToInt32(Request.QueryString["attemptId"]);
            LoadCertificate(attemptId);
        }

        private void LoadCertificate(int attemptId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT CertificateID, TraineeName, AssessmentTitle, Score, IssueDate " +
                    "FROM   Certificates " +
                    "WHERE  AttemptID = @AttemptID";

                cmd.Parameters.AddWithValue("@AttemptID", attemptId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        litTraineeName.Text    = reader["TraineeName"].ToString();
                        litAssessmentTitle.Text = reader["AssessmentTitle"].ToString();
                        litScore.Text          = reader["Score"].ToString();
                        litIssueDate.Text      = Convert.ToDateTime(reader["IssueDate"])
                                                     .ToString("dd MMM yyyy");
                        litCertID.Text         = "CERT-" + reader["CertificateID"].ToString().PadLeft(6, '0');
                    }
                    else
                    {
                        // No certificate found for this attempt
                        Response.Redirect("AssessmentList.aspx");
                    }
                }
            }
        }
    }
}
