using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Admin
{
    public partial class ModuleOversight : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Admin" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadModules();
            }
        }

        private void LoadModules()
        {
            lblError.Visible = false;

            try
            {
                string sql = @"
                    SELECT m.Id, m.Title, m.Topic, m.Status, u.FullName AS TrainerName,
                    (SELECT COUNT(*) FROM UserProgress up WHERE up.ModuleId = m.Id) AS EnrolledCount
                    FROM Modules m
                    INNER JOIN Users u ON m.CreatedByUserId = u.Id
                    ORDER BY m.CreatedAt DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql);

                if (dt.Rows.Count > 0)
                {
                    rptModules.DataSource = dt;
                    rptModules.DataBind();
                    phNoModules.Visible = false;
                }
                else
                {
                    rptModules.DataSource = null;
                    rptModules.DataBind();
                    phNoModules.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load modules: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void rptModules_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int moduleId = Convert.ToInt32(e.CommandArgument);
            lblError.Visible = false;
            lblMessage.Visible = false;

            if (e.CommandName == "ApproveModule")
            {
                try
                {
                    string sql = "UPDATE Modules SET Status = 'Approved' WHERE Id = @moduleId";
                    int rows = DbHelper.ExecuteNonQuery(sql, new SqlParameter("@moduleId", moduleId));
                    if (rows > 0)
                    {
                        lblMessage.Text = "Module approved successfully for public listing.";
                        lblMessage.Visible = true;
                        LoadModules();
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Failed to approve module: " + ex.Message;
                    lblError.Visible = true;
                }
            }
            else if (e.CommandName == "RejectModule")
            {
                try
                {
                    string sql = "UPDATE Modules SET Status = 'Rejected' WHERE Id = @moduleId";
                    int rows = DbHelper.ExecuteNonQuery(sql, new SqlParameter("@moduleId", moduleId));
                    if (rows > 0)
                    {
                        lblMessage.Text = "Module has been rejected and hidden from student browses.";
                        lblMessage.Visible = true;
                        LoadModules();
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Failed to reject module: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }
    }
}
