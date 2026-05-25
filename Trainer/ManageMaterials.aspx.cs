using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Trainer
{
    public partial class ManageMaterials : BasePage
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
                string sql = "SELECT TOP 1 Id FROM Modules WHERE CreatedByUserId = @userId ORDER BY CreatedAt DESC";
                object res = DbHelper.ExecuteScalar(sql, new SqlParameter("@userId", CurrentUserId));
                if (res != null && res != DBNull.Value)
                {
                    Response.Redirect("ManageMaterials.aspx?id=" + res.ToString());
                    return;
                }
                else
                {
                    Response.Redirect("MyModules.aspx");
                    return;
                }
            }

            if (!IsPostBack)
            {
                VerifyModuleOwnership();
                LoadMaterials();
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
                    // Not owned by current trainer
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

        private void LoadMaterials()
        {
            lblError.Visible = false;

            try
            {
                string sql = "SELECT * FROM StudyMaterials WHERE ModuleId = @moduleId ORDER BY OrderIndex ASC";
                DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@moduleId", ModuleId));

                if (dt.Rows.Count > 0)
                {
                    rptMaterials.DataSource = dt;
                    rptMaterials.DataBind();
                    phNoMaterials.Visible = false;
                }
                else
                {
                    rptMaterials.DataSource = null;
                    rptMaterials.DataBind();
                    phNoMaterials.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to load materials list: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;
            lblMessage.Visible = false;

            string title = txtTitle.Text.Trim();
            string type = ddlType.SelectedValue;
            string size = txtSize.Text.Trim();
            string url = txtUrl.Text.Trim();
            string orderStr = txtOrder.Text.Trim();
            bool isLocked = chkLocked.Checked;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(orderStr))
            {
                lblError.Text = "Title, metadata size, and order index are required fields.";
                lblError.Visible = true;
                return;
            }

            int order = 1;
            int.TryParse(orderStr, out order);

            try
            {
                string sql = @"
                    INSERT INTO StudyMaterials (ModuleId, Title, Type, FileSizeText, ContentUrl, OrderIndex, IsLocked)
                    VALUES (@moduleId, @title, @type, @size, @url, @order, @isLocked)";

                DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@moduleId", ModuleId),
                    new SqlParameter("@title", title),
                    new SqlParameter("@type", type),
                    new SqlParameter("@size", size),
                    new SqlParameter("@url", url),
                    new SqlParameter("@order", order),
                    new SqlParameter("@isLocked", isLocked ? 1 : 0)
                );

                lblMessage.Text = $"Material '{title}' added successfully!";
                lblMessage.Visible = true;

                // Clear inputs
                txtTitle.Text = "";
                txtSize.Text = "";
                txtUrl.Text = "";
                txtOrder.Text = "";
                chkLocked.Checked = false;

                LoadMaterials();
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to add study material: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void rptMaterials_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteMaterial")
            {
                int materialId = Convert.ToInt32(e.CommandArgument);
                int userId = CurrentUserId;

                lblError.Visible = false;
                lblMessage.Visible = false;

                try
                {
                    // Ensure the material belongs to a module owned by this trainer
                    string deleteSql = @"
                        DELETE FROM StudyMaterials 
                        WHERE Id = @materialId 
                        AND ModuleId IN (SELECT Id FROM Modules WHERE CreatedByUserId = @userId)";

                    int rows = DbHelper.ExecuteNonQuery(deleteSql,
                        new SqlParameter("@materialId", materialId),
                        new SqlParameter("@userId", userId)
                    );

                    if (rows > 0)
                    {
                        lblMessage.Text = "Study material deleted successfully.";
                        lblMessage.Visible = true;
                    }
                    else
                    {
                        lblError.Text = "Failed to delete material. Verify ownership permissions.";
                        lblError.Visible = true;
                    }

                    LoadMaterials();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Failed to delete material: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }
    }
}
