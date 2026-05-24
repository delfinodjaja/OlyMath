using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace OlyMath
{
    public partial class QuestionList : System.Web.UI.Page
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

            int assessmentId = Convert.ToInt32(Request.QueryString["id"]);
            hfAssessmentID.Value = assessmentId.ToString();

            if (!IsPostBack)
            {
                LoadAssessmentTitle(assessmentId);
                LoadQuestions(assessmentId);

                hlAddQuestion.NavigateUrl = "AddQuestion.aspx?aid=" + assessmentId;

                if (Request.QueryString["msg"] == "created")
                    ShowSuccess("Assessment created. Now add your questions.");

                if (Request.QueryString["msg"] == "added")
                    ShowSuccess("Question added successfully.");

                if (Request.QueryString["msg"] == "updated")
                    ShowSuccess("Question updated successfully.");
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
                litAssessmentTitle.Text = result != null ? result.ToString() : "Unknown";
            }
        }

        private void LoadQuestions(int assessmentId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT QuestionID, AssessmentID, QuestionText, " +
                    "       OptionA, OptionB, OptionC, OptionD, " +
                    "       CorrectAnswer, DisplayOrder " +
                    "FROM   AssessmentQuestions " +
                    "WHERE  AssessmentID = @ID " +
                    "ORDER  BY DisplayOrder, QuestionID";

                cmd.Parameters.AddWithValue("@ID", assessmentId);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }

            gvQuestions.DataSource = dt;
            gvQuestions.DataBind();
        }

        protected void gvQuestions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteQuestion")
            {
                int questionId   = Convert.ToInt32(e.CommandArgument);
                int assessmentId = Convert.ToInt32(hfAssessmentID.Value);

                DeleteQuestion(questionId, assessmentId);
            }
        }

        private void DeleteQuestion(int questionId, int assessmentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "DELETE FROM AssessmentQuestions WHERE QuestionID = @ID";

                    cmd.Parameters.AddWithValue("@ID", questionId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ShowSuccess("Question deleted.");
                LoadQuestions(assessmentId);
            }
            catch (Exception ex)
            {
                ShowError("Could not delete question: " + ex.Message);
            }
        }

        private void ShowSuccess(string message)
        {
            litSuccess.Text = message;
            pnlSuccess.Visible = true;
        }

        private void ShowError(string message)
        {
            litError.Text = message;
            pnlError.Visible = true;
        }
    }
}
