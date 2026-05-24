using System;
using System.Configuration;
using System.Data.SqlClient;

namespace OlyMath
{
    public partial class CreateAssessment : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Only Trainer and Admin can access this page
            if (!User.IsInRole("Trainer") && !User.IsInRole("Admin"))
            {
                Response.Redirect("AssessmentList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            string title       = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            int passingScore   = Convert.ToInt32(txtPassingScore.Text.Trim());
            string createdBy   = User.Identity.Name;

            try
            {
                int newId = InsertAssessment(title, description, passingScore, createdBy);

                // Redirect to question list so trainer can add questions right away
                Response.Redirect("QuestionList.aspx?id=" + newId + "&msg=created");
            }
            catch (Exception ex)
            {
                litError.Text = "Error creating assessment: " + ex.Message;
                pnlError.Visible = true;
            }
        }

        private int InsertAssessment(
            string title, string description, int passingScore, string createdBy)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "INSERT INTO Assessments (Title, Description, PassingScore, CreatedBy, CreatedDate, IsActive) " +
                    "VALUES (@Title, @Description, @PassingScore, @CreatedBy, GETDATE(), 1); " +
                    "SELECT SCOPE_IDENTITY();";

                cmd.Parameters.AddWithValue("@Title",        title);
                cmd.Parameters.AddWithValue("@Description",  string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);
                cmd.Parameters.AddWithValue("@PassingScore", passingScore);
                cmd.Parameters.AddWithValue("@CreatedBy",    createdBy);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }
    }
}
