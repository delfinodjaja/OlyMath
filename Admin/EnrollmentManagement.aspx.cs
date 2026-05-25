using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Admin
{
    public partial class EnrollmentManagement : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Admin" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEnrollments();
            }
        }

        private void LoadEnrollments()
        {
            lblError.Visible = false;

            try
            {
                string sql = @"
                    SELECT up.UserId, up.ModuleId, up.ProgressPercentage, up.LastAccessed,
                    u.FullName AS TraineeName, u.Email, m.Title AS ModuleTitle
                    FROM UserProgress up
                    INNER JOIN Users u ON up.UserId = u.Id
                    INNER JOIN Modules m ON up.ModuleId = m.Id
                    ORDER BY up.LastAccessed DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql);

                if (dt.Rows.Count > 0)
                {
                    rptEnrollments.DataSource = dt;
                    rptEnrollments.DataBind();
                    phNoEnrollments.Visible = false;
                }
                else
                {
                    rptEnrollments.DataSource = null;
                    rptEnrollments.DataBind();
                    phNoEnrollments.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load active enrollments: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void rptEnrollments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblError.Visible = false;
            lblMessage.Visible = false;

            if (e.CommandName == "UnenrollTrainee")
            {
                try
                {
                    string[] parts = e.CommandArgument.ToString().Split(',');
                    int userId = Convert.ToInt32(parts[0]);
                    int moduleId = Convert.ToInt32(parts[1]);

                    // Clean up material completion status
                    string cleanMatStatusSql = @"
                        DELETE FROM UserMaterialsStatus 
                        WHERE UserId = @userId 
                        AND MaterialId IN (SELECT Id FROM StudyMaterials WHERE ModuleId = @moduleId)";
                    DbHelper.ExecuteNonQuery(cleanMatStatusSql, 
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@moduleId", moduleId)
                    );

                    // Delete progress row
                    string deleteProgressSql = "DELETE FROM UserProgress WHERE UserId = @userId AND ModuleId = @moduleId";
                    int rows = DbHelper.ExecuteNonQuery(deleteProgressSql,
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@moduleId", moduleId)
                    );

                    if (rows > 0)
                    {
                        lblMessage.Text = "Trainee unenrolled and progress history wiped successfully.";
                        lblMessage.Visible = true;
                        LoadEnrollments();
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Failed to unenroll trainee: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }
    }
}
