using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath
{
    public partial class AssessmentList : Page
    {
        protected global::System.Web.UI.WebControls.Panel pnlCreateBtn;
        protected global::System.Web.UI.WebControls.GridView gvAssessments;
        protected global::System.Web.UI.WebControls.Literal litSuccess;
        protected global::System.Web.UI.WebControls.Panel pnlSuccess;
        protected global::System.Web.UI.WebControls.Literal litError;
        protected global::System.Web.UI.WebControls.Panel pnlError;

        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetupRoleVisibility();
                LoadAssessments();

                // Show feedback message if redirected here after an action
                if (Request.QueryString["msg"] == "created")
                    ShowSuccess("Assessment created successfully.");

                if (Request.QueryString["msg"] == "updated")
                    ShowSuccess("Assessment updated successfully.");
            }
        }

        private void SetupRoleVisibility()
        {
            bool isTrainerOrAdmin =
                User.IsInRole("Trainer") || User.IsInRole("Admin");

            pnlCreateBtn.Visible = isTrainerOrAdmin;
        }

        private void LoadAssessments()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    "SELECT AssessmentID, Title, Description, PassingScore, " +
                    "       CreatedBy, CreatedDate " +
                    "FROM   Assessments " +
                    "WHERE  IsActive = 1 " +
                    "ORDER  BY CreatedDate DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }

            gvAssessments.DataSource = dt;
            gvAssessments.DataBind();

            // Toggle action panel visibility per row based on role
            bool isTrainerOrAdmin =
                User.IsInRole("Trainer") || User.IsInRole("Admin");

            foreach (GridViewRow row in gvAssessments.Rows)
            {
                Panel pnlTrainer = row.FindControl("pnlTrainerActions") as Panel;
                Panel pnlTrainee = row.FindControl("pnlTraineeActions") as Panel;

                if (pnlTrainer != null) pnlTrainer.Visible = isTrainerOrAdmin;
                if (pnlTrainee != null) pnlTrainee.Visible = !isTrainerOrAdmin;
            }
        }

        protected void gvAssessments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAssessment")
            {
                int assessmentId = Convert.ToInt32(e.CommandArgument);
                DeleteAssessment(assessmentId);
            }
        }

        private void DeleteAssessment(int assessmentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "UPDATE Assessments " +
                        "SET    IsActive = 0 " +
                        "WHERE  AssessmentID = @ID";

                    cmd.Parameters.AddWithValue("@ID", assessmentId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ShowSuccess("Assessment deleted successfully.");
                LoadAssessments();
            }
            catch (Exception ex)
            {
                ShowError("Could not delete assessment: " + ex.Message);
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
