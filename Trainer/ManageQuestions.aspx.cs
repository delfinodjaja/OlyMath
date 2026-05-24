using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Trainer
{
    public partial class ManageQuestions : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainer" };

        private int ModuleId
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
                Response.Redirect("Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                VerifyModuleOwnership();
                LoadQuestions();
            }
        }

        private void VerifyModuleOwnership()
        {
            try
            {
                string sql = "SELECT Title FROM Modules WHERE Id = @moduleId AND CreatedByUserId = @userId";
                object title = DbHelper.ExecuteScalar(sql, 
                    new SqlParameter("@moduleId", ModuleId),
                    new SqlParameter("@userId", CurrentUserId)
                );

                if (title == null)
                {
                    Response.Redirect("Dashboard.aspx");
                    return;
                }

                litModuleTitle.Text = title.ToString();
            }
            catch
            {
                Response.Redirect("Dashboard.aspx");
            }
        }

        private void LoadQuestions()
        {
            lblError.Visible = false;

            try
            {
                string sql = "SELECT * FROM Questions WHERE ModuleId = @moduleId ORDER BY Id ASC";
                DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@moduleId", ModuleId));

                if (dt.Rows.Count > 0)
                {
                    rptQuestions.DataSource = dt;
                    rptQuestions.DataBind();
                    phNoQuestions.Visible = false;
                }
                else
                {
                    rptQuestions.DataSource = null;
                    rptQuestions.DataBind();
                    phNoQuestions.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load questions list: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            lblMessage.Visible = false;

            string qText = txtQuestionText.Text.Trim();
            string optA = txtOptionA.Text.Trim();
            string optB = txtOptionB.Text.Trim();
            string optC = txtOptionC.Text.Trim();
            string optD = txtOptionD.Text.Trim();
            string correct = ddlCorrectOption.SelectedValue;

            if (string.IsNullOrEmpty(qText) || string.IsNullOrEmpty(optA) || 
                string.IsNullOrEmpty(optB) || string.IsNullOrEmpty(optC) || string.IsNullOrEmpty(optD))
            {
                lblError.Text = "Question prompt text and Options A, B, C, D are all required.";
                lblError.Visible = true;
                return;
            }

            try
            {
                string sql = @"
                    INSERT INTO Questions (ModuleId, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption)
                    VALUES (@moduleId, @qText, @optA, @optB, @optC, @optD, @correct)";

                DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@moduleId", ModuleId),
                    new SqlParameter("@qText", qText),
                    new SqlParameter("@optA", optA),
                    new SqlParameter("@optB", optB),
                    new SqlParameter("@optC", optC),
                    new SqlParameter("@optD", optD),
                    new SqlParameter("@correct", correct)
                );

                lblMessage.Text = "Quiz question added successfully!";
                lblMessage.Visible = true;

                // Clear fields
                txtQuestionText.Text = "";
                txtOptionA.Text = "";
                txtOptionB.Text = "";
                txtOptionC.Text = "";
                txtOptionD.Text = "";
                ddlCorrectOption.SelectedIndex = 0;

                LoadQuestions();
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to add question: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteQuestion")
            {
                int questionId = Convert.ToInt32(e.CommandArgument);
                int userId = CurrentUserId;

                lblError.Visible = false;
                lblMessage.Visible = false;

                try
                {
                    // Ensure the question belongs to a module owned by this trainer
                    string deleteSql = @"
                        DELETE FROM Questions 
                        WHERE Id = @questionId 
                        AND ModuleId IN (SELECT Id FROM Modules WHERE CreatedByUserId = @userId)";

                    int rows = DbHelper.ExecuteNonQuery(deleteSql,
                        new SqlParameter("@questionId", questionId),
                        new SqlParameter("@userId", userId)
                    );

                    if (rows > 0)
                    {
                        lblMessage.Text = "Question deleted successfully.";
                        lblMessage.Visible = true;
                    }
                    else
                    {
                        lblError.Text = "Failed to delete question. Verify ownership permissions.";
                        lblError.Visible = true;
                    }

                    LoadQuestions();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Failed to delete question: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }
    }
}
