using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace OlyMath
{
    public partial class AttemptAssessment : Page
    {
        protected global::System.Web.UI.WebControls.HiddenField hfAssessmentID;
        protected global::System.Web.UI.WebControls.Literal litTitle;
        protected global::System.Web.UI.WebControls.Literal litDescription;
        protected global::System.Web.UI.WebControls.Literal litQuestionCount;
        protected global::System.Web.UI.WebControls.Literal litPassingScore;
        protected global::System.Web.UI.WebControls.Panel pnlError;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.PlaceHolder phQuestions;
        protected global::System.Web.UI.WebControls.Panel pnlSubmit;
        protected global::System.Web.UI.WebControls.Panel pnlNoQuestions;

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // Keeps the question data accessible across Page_Load and btnSubmit_Click
        private DataTable _questions = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
            }

            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Response.Redirect("AssessmentList.aspx");
            }

            int assessmentId = Convert.ToInt32(Request.QueryString["id"]);
            hfAssessmentID.Value = assessmentId.ToString();

            LoadAssessmentInfo(assessmentId);

            _questions = LoadQuestions(assessmentId);

            if (_questions.Rows.Count == 0)
            {
                pnlNoQuestions.Visible = true;
                pnlSubmit.Visible = false;
            }
            else
            {
                BuildQuestionUI(_questions);
                pnlSubmit.Visible = true;
            }
        }

        private void LoadAssessmentInfo(int assessmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT Title, Description, PassingScore " +
                    "FROM   Assessments " +
                    "WHERE  AssessmentID = @ID AND IsActive = 1";

                cmd.Parameters.AddWithValue("@ID", assessmentId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        litTitle.Text        = reader["Title"].ToString();
                        litDescription.Text  = reader["Description"] == DBNull.Value
                                                ? string.Empty
                                                : reader["Description"].ToString();
                        litPassingScore.Text = reader["PassingScore"].ToString();
                    }
                    else
                    {
                        Response.Redirect("AssessmentList.aspx");
                    }
                }
            }
        }

        private DataTable LoadQuestions(int assessmentId)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT QuestionID, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectAnswer " +
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

            litQuestionCount.Text = dt.Rows.Count.ToString();
            return dt;
        }

        // Builds one card per question with radio options A–D
        private void BuildQuestionUI(DataTable questions)
        {
            int questionNumber = 1;

            foreach (DataRow row in questions.Rows)
            {
                string questionId = row["QuestionID"].ToString();

                // Outer card div
                HtmlGenericControl card = new HtmlGenericControl("div");
                card.Attributes["class"] = "question-card";

                // Question number label
                HtmlGenericControl numLabel = new HtmlGenericControl("div");
                numLabel.Attributes["class"] = "question-num";
                numLabel.InnerText = "Question " + questionNumber;
                card.Controls.Add(numLabel);

                // Question text
                HtmlGenericControl questionText = new HtmlGenericControl("div");
                questionText.Attributes["class"] = "question-text";
                questionText.InnerText = row["QuestionText"].ToString();
                card.Controls.Add(questionText);

                // Options list
                HtmlGenericControl ul = new HtmlGenericControl("ul");
                ul.Attributes["class"] = "options-list";

                string[] letters  = { "A", "B", "C", "D" };
                string[] options  = {
                    row["OptionA"].ToString(),
                    row["OptionB"].ToString(),
                    row["OptionC"].ToString(),
                    row["OptionD"].ToString()
                };

                for (int i = 0; i < 4; i++)
                {
                    string letter     = letters[i];
                    string optionText = options[i];
                    string radioId    = "q_" + questionId + "_" + letter;

                    HtmlGenericControl li    = new HtmlGenericControl("li");
                    HtmlGenericControl label = new HtmlGenericControl("label");
                    label.Attributes["class"] = "option-radio";
                    label.Attributes["for"]   = radioId;

                    HtmlInputRadioButton radio = new HtmlInputRadioButton();
                    radio.ID      = radioId;
                    radio.Name    = "answer_" + questionId;   // group by question
                    radio.Value   = letter;
                    radio.Attributes["style"] = "display:none";

                    HtmlGenericControl letterSpan = new HtmlGenericControl("span");
                    letterSpan.Attributes["class"] = "option-letter";
                    letterSpan.InnerText = letter;

                    HtmlGenericControl textSpan = new HtmlGenericControl("span");
                    textSpan.InnerText = optionText;

                    label.Controls.Add(radio);
                    label.Controls.Add(letterSpan);
                    label.Controls.Add(textSpan);
                    li.Controls.Add(label);
                    ul.Controls.Add(li);
                }

                card.Controls.Add(ul);
                phQuestions.Controls.Add(card);

                questionNumber++;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int assessmentId = Convert.ToInt32(hfAssessmentID.Value);
            DataTable questions = LoadQuestions(assessmentId);

            if (questions.Rows.Count == 0)
            {
                litError.Text = "No questions found.";
                pnlError.Visible = true;
                return;
            }

            // Score the answers
            int correctCount = 0;

            foreach (DataRow row in questions.Rows)
            {
                string questionId   = row["QuestionID"].ToString();
                string correctAnswer = row["CorrectAnswer"].ToString();

                // Read the submitted radio button value from the form
                string submitted = Request.Form["answer_" + questionId];

                if (!string.IsNullOrEmpty(submitted) &&
                    submitted.Trim().ToUpper() == correctAnswer.Trim().ToUpper())
                {
                    correctCount++;
                }
            }

            int totalQuestions = questions.Rows.Count;
            int scorePercent   = (int)Math.Round((double)correctCount / totalQuestions * 100);

            // Get passing score for this assessment
            int passingScore = GetPassingScore(assessmentId);
            bool isPassed    = scorePercent >= passingScore;

            // Save the attempt and get the new AttemptID
            int attemptId = SaveAttempt(assessmentId, scorePercent, isPassed);

            // If passed, generate a certificate record
            if (isPassed)
            {
                IssueCertificate(attemptId, assessmentId, scorePercent);
            }

            // Redirect to result page
            Response.Redirect(
                "AssessmentResult.aspx?attemptId=" + attemptId);
        }

        private int GetPassingScore(int assessmentId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT PassingScore FROM Assessments WHERE AssessmentID = @ID";

                cmd.Parameters.AddWithValue("@ID", assessmentId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 60;
            }
        }

        private int SaveAttempt(int assessmentId, int score, bool isPassed)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "INSERT INTO AssessmentAttempts " +
                    "    (AssessmentID, TraineeUsername, Score, IsPassed, AttemptDate) " +
                    "VALUES " +
                    "    (@AssessmentID, @Username, @Score, @IsPassed, GETDATE()); " +
                    "SELECT SCOPE_IDENTITY();";

                cmd.Parameters.AddWithValue("@AssessmentID", assessmentId);
                cmd.Parameters.AddWithValue("@Username",     User.Identity.Name);
                cmd.Parameters.AddWithValue("@Score",        score);
                cmd.Parameters.AddWithValue("@IsPassed",     isPassed ? 1 : 0);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private void IssueCertificate(int attemptId, int assessmentId, int score)
        {
            // Fetch assessment title
            string assessmentTitle = string.Empty;

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT Title FROM Assessments WHERE AssessmentID = @ID";

                cmd.Parameters.AddWithValue("@ID", assessmentId);

                conn.Open();
                object result = cmd.ExecuteScalar();
                assessmentTitle = result != null ? result.ToString() : string.Empty;
            }

            // Insert certificate record
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "INSERT INTO Certificates " +
                    "    (AttemptID, AssessmentID, TraineeName, AssessmentTitle, Score, IssueDate) " +
                    "VALUES " +
                    "    (@AttemptID, @AssessmentID, @TraineeName, @AssessmentTitle, @Score, GETDATE())";

                cmd.Parameters.AddWithValue("@AttemptID",       attemptId);
                cmd.Parameters.AddWithValue("@AssessmentID",    assessmentId);
                cmd.Parameters.AddWithValue("@TraineeName",     User.Identity.Name);
                cmd.Parameters.AddWithValue("@AssessmentTitle", assessmentTitle);
                cmd.Parameters.AddWithValue("@Score",           score);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
