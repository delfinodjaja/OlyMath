using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;

namespace OlyMath.Trainee
{
    public partial class Assessment : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee" };

        public int ModuleId
        {
            get
            {
                int id = 0;
                int.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ModuleId <= 0)
            {
                Response.Redirect("BrowseModules.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Verify enrollment
                string checkSql = "SELECT COUNT(*) FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
                long count = (long)DbHelper.ExecuteScalar(checkSql, 
                    new SQLiteParameter("@userId", CurrentUserId),
                    new SQLiteParameter("@moduleId", ModuleId)
                );

                if (count == 0)
                {
                    Response.Redirect($"ModuleDetail.aspx?id={ModuleId}");
                    return;
                }

                LoadModuleInfo();
                LoadQuestions();
            }
        }

        private void LoadModuleInfo()
        {
            string sql = "SELECT Title FROM Modules WHERE Id = @moduleId";
            object title = DbHelper.ExecuteScalar(sql, new SQLiteParameter("@moduleId", ModuleId));
            litModuleTitle.Text = title?.ToString() ?? "";
        }

        private void LoadQuestions()
        {
            string sql = "SELECT * FROM Questions WHERE ModuleId = @moduleId";
            DataTable dt = DbHelper.ExecuteQuery(sql, new SQLiteParameter("@moduleId", ModuleId));

            if (dt.Rows.Count > 0)
            {
                rptQuestions.DataSource = dt;
                rptQuestions.DataBind();
                phQuiz.Visible = true;
                phNoQuestions.Visible = false;
            }
            else
            {
                phQuiz.Visible = false;
                phNoQuestions.Visible = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            // Load correct options
            string sql = "SELECT Id, CorrectOption FROM Questions WHERE ModuleId = @moduleId";
            DataTable dt = DbHelper.ExecuteQuery(sql, new SQLiteParameter("@moduleId", ModuleId));

            if (dt.Rows.Count == 0)
            {
                lblError.Text = "Unable to grade assessment: No questions found.";
                lblError.Visible = true;
                return;
            }

            int totalQuestions = dt.Rows.Count;
            int correctAnswers = 0;
            int answeredCount = 0;

            foreach (DataRow row in dt.Rows)
            {
                string qId = row["Id"].ToString();
                string submittedAnswer = Request.Form["Q_" + qId];
                string correctOption = row["CorrectOption"].ToString();

                if (!string.IsNullOrEmpty(submittedAnswer))
                {
                    answeredCount++;
                    if (string.Equals(submittedAnswer, correctOption, StringComparison.OrdinalIgnoreCase))
                    {
                        correctAnswers++;
                    }
                }
            }

            // Calculate percentage score
            int scorePercent = (int)Math.Round((double)(correctAnswers * 100) / totalQuestions);

            int userId = CurrentUserId;

            if (scorePercent >= 60)
            {
                // Pass! Generate Certificate ID
                // Format: OM-[Year]-[MonthDay]-[Random 4-digits]
                string certId = $"OM-{DateTime.Now.ToString("yyyy-MMdd")}-{new Random().Next(1000, 9999)}";

                // Save passed assessment
                string insertSql = @"
                    INSERT INTO UserAssessments (UserId, ModuleId, Score, MaxScore, CertificateId)
                    VALUES (@userId, @moduleId, @score, 100, @certId)";
                
                DbHelper.ExecuteNonQuery(insertSql,
                    new SQLiteParameter("@userId", userId),
                    new SQLiteParameter("@moduleId", ModuleId),
                    new SQLiteParameter("@score", scorePercent),
                    new SQLiteParameter("@certId", certId)
                );

                // Set course progress to 100% since assessment is passed
                string updateProgressSql = @"
                    UPDATE UserProgress 
                    SET ProgressPercentage = 100, LastAccessed = CURRENT_TIMESTAMP
                    WHERE UserId = @userId AND ModuleId = @moduleId";
                
                DbHelper.ExecuteNonQuery(updateProgressSql,
                    new SQLiteParameter("@userId", userId),
                    new SQLiteParameter("@moduleId", ModuleId)
                );

                // Mark all study materials for this module as done for completion integrity
                string markAllDoneSql = @"
                    INSERT OR REPLACE INTO UserMaterialsStatus (UserId, MaterialId, IsDone)
                    SELECT @userId, Id, 1 FROM StudyMaterials WHERE ModuleId = @moduleId";
                
                DbHelper.ExecuteNonQuery(markAllDoneSql,
                    new SQLiteParameter("@userId", userId),
                    new SQLiteParameter("@moduleId", ModuleId)
                );

                // Redirect to certificate display page
                Response.Redirect($"Certificate.aspx?certId={certId}");
            }
            else
            {
                // Fail! Save attempt with no certificate
                string insertSql = @"
                    INSERT INTO UserAssessments (UserId, ModuleId, Score, MaxScore, CertificateId)
                    VALUES (@userId, @moduleId, @score, 100, NULL)";
                
                DbHelper.ExecuteNonQuery(insertSql,
                    new SQLiteParameter("@userId", userId),
                    new SQLiteParameter("@moduleId", ModuleId),
                    new SQLiteParameter("@score", scorePercent)
                );

                // Update UI error warning
                lblError.Text = $"You scored {scorePercent}% ({correctAnswers} of {totalQuestions} correct). You need at least 60% to pass and earn your certificate. Review the study materials and try again!";
                lblError.Visible = true;
            }
        }
    }
}
