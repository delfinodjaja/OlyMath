using System;
using System.Configuration;
using System.Data.SqlClient;

namespace OlyMath
{
    public partial class EditQuestion : System.Web.UI.Page
    {
        protected global::System.Web.UI.WebControls.HiddenField hfQuestionID;
        protected global::System.Web.UI.WebControls.HiddenField hfAssessmentID;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor backLink;
        protected global::System.Web.UI.HtmlControls.HtmlAnchor cancelLink;
        protected global::System.Web.UI.WebControls.Literal litAssessmentTitle;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.TextBox txtQuestion;
        protected global::System.Web.UI.WebControls.TextBox txtOptionA;
        protected global::System.Web.UI.WebControls.TextBox txtOptionB;
        protected global::System.Web.UI.WebControls.TextBox txtOptionC;
        protected global::System.Web.UI.WebControls.TextBox txtOptionD;
        protected global::System.Web.UI.WebControls.TextBox txtDisplayOrder;
        protected global::System.Web.UI.WebControls.RadioButtonList rblCorrectAnswer;

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("Trainer") && !User.IsInRole("Admin"))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            if (string.IsNullOrEmpty(Request.QueryString["qid"]) ||
                string.IsNullOrEmpty(Request.QueryString["aid"]))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            int questionId   = Convert.ToInt32(Request.QueryString["qid"]);
            int assessmentId = Convert.ToInt32(Request.QueryString["aid"]);

            hfQuestionID.Value   = questionId.ToString();
            hfAssessmentID.Value = assessmentId.ToString();

            string backUrl     = "QuestionList.aspx?id=" + assessmentId;
            backLink.HRef      = backUrl;
            cancelLink.HRef    = backUrl;

            if (!IsPostBack)
            {
                LoadAssessmentTitle(assessmentId);
                LoadQuestion(questionId);
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

        private void LoadQuestion(int questionId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT QuestionText, OptionA, OptionB, OptionC, OptionD, " +
                    "       CorrectAnswer, DisplayOrder " +
                    "FROM   AssessmentQuestions " +
                    "WHERE  QuestionID = @ID";

                cmd.Parameters.AddWithValue("@ID", questionId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtQuestion.Text          = reader["QuestionText"].ToString();
                        txtOptionA.Text           = reader["OptionA"].ToString();
                        txtOptionB.Text           = reader["OptionB"].ToString();
                        txtOptionC.Text           = reader["OptionC"].ToString();
                        txtOptionD.Text           = reader["OptionD"].ToString();
                        txtDisplayOrder.Text      = reader["DisplayOrder"].ToString();
                        rblCorrectAnswer.SelectedValue = reader["CorrectAnswer"].ToString();
                    }
                    else
                    {
                        int assessmentId = Convert.ToInt32(hfAssessmentID.Value);
                        Response.Redirect("QuestionList.aspx?id=" + assessmentId);
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int    questionId    = Convert.ToInt32(hfQuestionID.Value);
            int    assessmentId  = Convert.ToInt32(hfAssessmentID.Value);
            string questionText  = txtQuestion.Text.Trim();
            string optionA       = txtOptionA.Text.Trim();
            string optionB       = txtOptionB.Text.Trim();
            string optionC       = txtOptionC.Text.Trim();
            string optionD       = txtOptionD.Text.Trim();
            string correctAnswer = rblCorrectAnswer.SelectedValue;

            int displayOrder = 0;
            if (!string.IsNullOrEmpty(txtDisplayOrder.Text.Trim()))
                displayOrder = Convert.ToInt32(txtDisplayOrder.Text.Trim());

            try
            {
                UpdateQuestion(questionId, questionText, optionA, optionB,
                               optionC, optionD, correctAnswer, displayOrder);

                Response.Redirect("QuestionList.aspx?id=" + assessmentId + "&msg=updated");
            }
            catch (Exception ex)
            {
                litError.Text = "Error saving question: " + ex.Message;
                pnlError.Visible = true;
            }
        }

        private void UpdateQuestion(
            int questionId, string questionText,
            string optionA, string optionB, string optionC, string optionD,
            string correctAnswer, int displayOrder)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "UPDATE AssessmentQuestions " +
                    "SET    QuestionText  = @QuestionText, " +
                    "       OptionA       = @OptionA, " +
                    "       OptionB       = @OptionB, " +
                    "       OptionC       = @OptionC, " +
                    "       OptionD       = @OptionD, " +
                    "       CorrectAnswer = @CorrectAnswer, " +
                    "       DisplayOrder  = @DisplayOrder " +
                    "WHERE  QuestionID = @ID";

                cmd.Parameters.AddWithValue("@QuestionText",  questionText);
                cmd.Parameters.AddWithValue("@OptionA",       optionA);
                cmd.Parameters.AddWithValue("@OptionB",       optionB);
                cmd.Parameters.AddWithValue("@OptionC",       optionC);
                cmd.Parameters.AddWithValue("@OptionD",       optionD);
                cmd.Parameters.AddWithValue("@CorrectAnswer", correctAnswer);
                cmd.Parameters.AddWithValue("@DisplayOrder",  displayOrder);
                cmd.Parameters.AddWithValue("@ID",            questionId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
