using System;
using System.Configuration;
using System.Data.SqlClient;

namespace OlyMath
{
    public partial class AddQuestion : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("Trainer") && !User.IsInRole("Admin"))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            if (string.IsNullOrEmpty(Request.QueryString["aid"]))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            int assessmentId = Convert.ToInt32(Request.QueryString["aid"]);
            hfAssessmentID.Value = assessmentId.ToString();

            // Set the back link to this assessment's question list
            backLink.HRef = "QuestionList.aspx?id=" + assessmentId;

            if (!IsPostBack)
            {
                LoadAssessmentTitle(assessmentId);
            }
        }

        private void LoadAssessmentTitle(int assessmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT Title FROM Assessments WHERE AssessmentID = @ID";

                cmd.Parameters.AddWithValue("@ID", assessmentId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                litAssessmentTitle.Text = result != null ? result.ToString() : string.Empty;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int assessmentId = Convert.ToInt32(hfAssessmentID.Value);

            try
            {
                SaveQuestion(assessmentId);
                Response.Redirect("QuestionList.aspx?id=" + assessmentId + "&msg=added");
            }
            catch (Exception ex)
            {
                litError.Text = "Error saving question: " + ex.Message;
                pnlError.Visible = true;
            }
        }

        protected void btnSaveAnother_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int assessmentId = Convert.ToInt32(hfAssessmentID.Value);

            try
            {
                SaveQuestion(assessmentId);

                // Clear fields so trainer can enter the next question
                txtQuestion.Text     = string.Empty;
                txtOptionA.Text      = string.Empty;
                txtOptionB.Text      = string.Empty;
                txtOptionC.Text      = string.Empty;
                txtOptionD.Text      = string.Empty;
                txtDisplayOrder.Text = string.Empty;
                rblCorrectAnswer.SelectedValue = "A";

                // Show inline success confirmation
                litError.Text      = string.Empty;
                pnlError.Visible   = false;

                // Reload title in case of postback
                LoadAssessmentTitle(assessmentId);
            }
            catch (Exception ex)
            {
                litError.Text = "Error saving question: " + ex.Message;
                pnlError.Visible = true;
            }
        }

        private void SaveQuestion(int assessmentId)
        {
            string questionText  = txtQuestion.Text.Trim();
            string optionA       = txtOptionA.Text.Trim();
            string optionB       = txtOptionB.Text.Trim();
            string optionC       = txtOptionC.Text.Trim();
            string optionD       = txtOptionD.Text.Trim();
            string correctAnswer = rblCorrectAnswer.SelectedValue;

            int displayOrder = 0;
            if (!string.IsNullOrEmpty(txtDisplayOrder.Text.Trim()))
                displayOrder = Convert.ToInt32(txtDisplayOrder.Text.Trim());

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "INSERT INTO AssessmentQuestions " +
                    "    (AssessmentID, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer, DisplayOrder) " +
                    "VALUES " +
                    "    (@AssessmentID, @QuestionText, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer, @DisplayOrder)";

                cmd.Parameters.AddWithValue("@AssessmentID",  assessmentId);
                cmd.Parameters.AddWithValue("@QuestionText",  questionText);
                cmd.Parameters.AddWithValue("@OptionA",       optionA);
                cmd.Parameters.AddWithValue("@OptionB",       optionB);
                cmd.Parameters.AddWithValue("@OptionC",       optionC);
                cmd.Parameters.AddWithValue("@OptionD",       optionD);
                cmd.Parameters.AddWithValue("@CorrectAnswer", correctAnswer);
                cmd.Parameters.AddWithValue("@DisplayOrder",  displayOrder);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
