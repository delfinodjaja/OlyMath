using System;
using System.Configuration;
using System.Data.SqlClient;

namespace OlyMath
{
    public partial class AssessmentResult : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.Literal litScore;
        protected global::System.Web.UI.WebControls.Literal litPassingScore;
        protected global::System.Web.UI.WebControls.Literal litAssessmentTitle;
        protected global::System.Web.UI.WebControls.Literal litAttemptDate;
        protected global::System.Web.UI.WebControls.Literal litIcon;
        protected global::System.Web.UI.WebControls.Literal litVerdict;
        protected global::System.Web.UI.WebControls.Panel pnlPassBanner;
        protected global::System.Web.UI.WebControls.Panel pnlCertBtn;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor certLink;
        protected global::System.Web.UI.WebControls.Panel pnlFailBanner;

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
            LoadResult(attemptId);
        }

        private void LoadResult(int attemptId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT  aa.AttemptID, aa.Score, aa.IsPassed, aa.AttemptDate, " +
                    "        a.Title, a.PassingScore, a.AssessmentID " +
                    "FROM    AssessmentAttempts aa " +
                    "INNER JOIN Assessments a ON a.AssessmentID = aa.AssessmentID " +
                    "WHERE   aa.AttemptID = @AttemptID";

                cmd.Parameters.AddWithValue("@AttemptID", attemptId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Response.Redirect("AssessmentList.aspx");
                        return;
                    }

                    bool isPassed    = Convert.ToBoolean(reader["IsPassed"]);
                    int  score       = Convert.ToInt32(reader["Score"]);
                    int  passingScore = Convert.ToInt32(reader["PassingScore"]);
                    int  assessmentId = Convert.ToInt32(reader["AssessmentID"]);

                    litScore.Text           = score.ToString();
                    litPassingScore.Text    = passingScore.ToString();
                    litAssessmentTitle.Text = reader["Title"].ToString();
                    litAttemptDate.Text     = Convert.ToDateTime(reader["AttemptDate"])
                                                .ToString("dd MMM yyyy, hh:mm tt");

                    if (isPassed)
                    {
                        litIcon.Text            = "🎉";
                        litVerdict.Text         = "<span class=\"passed\">Passed!</span>";
                        pnlPassBanner.Visible   = true;
                        pnlCertBtn.Visible      = true;

                        // Build certificate link using the attemptId
                        certLink.HRef = "Certificate.aspx?attemptId=" + attemptId;
                    }
                    else
                    {
                        litIcon.Text          = "📝";
                        litVerdict.Text       = "<span class=\"failed\">Not Passed</span>";
                        pnlFailBanner.Visible = true;
                    }
                }
            }
        }
    }
}
