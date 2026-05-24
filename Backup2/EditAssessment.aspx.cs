using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace OlyMath
{
    public partial class EditAssessment : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("Trainer") && !User.IsInRole("Admin"))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            if (!IsPostBack)
            {
                LoadAssessment();
            }
        }

        private void LoadAssessment()
        {
            int assessmentId = Convert.ToInt32(Request.QueryString["id"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT AssessmentID, Title, Description, PassingScore " +
                    "FROM   Assessments " +
                    "WHERE  AssessmentID = @ID AND IsActive = 1";

                cmd.Parameters.AddWithValue("@ID", assessmentId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hfAssessmentID.Value   = reader["AssessmentID"].ToString();
                        txtTitle.Text          = reader["Title"].ToString();
                        txtDescription.Text    = reader["Description"] == DBNull.Value
                                                    ? string.Empty
                                                    : reader["Description"].ToString();
                        txtPassingScore.Text   = reader["PassingScore"].ToString();
                    }
                    else
                    {
                        // Assessment not found; go back to list
                        Response.Redirect("AssessmentList.aspx");
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int    assessmentId = Convert.ToInt32(hfAssessmentID.Value);
            string title        = txtTitle.Text.Trim();
            string description  = txtDescription.Text.Trim();
            int    passingScore = Convert.ToInt32(txtPassingScore.Text.Trim());

            try
            {
                UpdateAssessment(assessmentId, title, description, passingScore);
                Response.Redirect("AssessmentList.aspx?msg=updated");
            }
            catch (Exception ex)
            {
                litError.Text = "Error saving changes: " + ex.Message;
                pnlError.Visible = true;
            }
        }

        private void UpdateAssessment(
            int assessmentId, string title, string description, int passingScore)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "UPDATE Assessments " +
                    "SET    Title        = @Title, " +
                    "       Description  = @Description, " +
                    "       PassingScore = @PassingScore " +
                    "WHERE  AssessmentID = @ID";

                cmd.Parameters.AddWithValue("@Title",        title);
                cmd.Parameters.AddWithValue("@Description",  string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                cmd.Parameters.AddWithValue("@PassingScore", passingScore);
                cmd.Parameters.AddWithValue("@ID",           assessmentId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
