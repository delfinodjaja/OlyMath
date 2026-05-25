using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace OlyMath.Trainee
{
    public partial class Certificate : BasePage
    {
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee" };

        private string CertId
        {
            get
            {
                return Request.QueryString["certId"]?.Trim();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CertId))
            {
                string sql = "SELECT TOP 1 CertificateId FROM UserAssessments WHERE UserId = @userId AND CertificateId IS NOT NULL ORDER BY CompletedAt DESC";
                object res = DbHelper.ExecuteScalar(sql, new SqlParameter("@userId", CurrentUserId));
                if (res != null && res != DBNull.Value)
                {
                    Response.Redirect("Certificate.aspx?certId=" + res.ToString());
                    return;
                }
                else
                {
                    Response.Redirect("Achievements.aspx");
                    return;
                }
            }

            if (!IsPostBack)
            {
                LoadCertificate();
            }
        }

        private void LoadCertificate()
        {
            lblError.Visible = false;

            // Security: Verify certificate belongs to current trainee
            string sql = @"
                SELECT ua.CertificateId, ua.Score, ua.CompletedAt, m.Title AS ModuleTitle, u.FullName AS TraineeName, t.FullName AS TrainerName
                FROM UserAssessments ua
                INNER JOIN Modules m ON ua.ModuleId = m.Id
                INNER JOIN Users u ON ua.UserId = u.Id
                INNER JOIN Users t ON m.CreatedByUserId = t.Id
                WHERE ua.CertificateId = @certId AND ua.UserId = @userId";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                new SqlParameter("@certId", CertId),
                new SqlParameter("@userId", CurrentUserId)
            );

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                litTraineeName.Text = row["TraineeName"].ToString();
                litModuleTitle.Text = row["ModuleTitle"].ToString();
                litScore.Text = row["Score"].ToString();
                litTrainerName.Text = row["TrainerName"].ToString();
                litCertId.Text = row["CertificateId"].ToString();

                DateTime dtIssued = Convert.ToDateTime(row["CompletedAt"]);
                litDateIssued.Text = dtIssued.ToString("MMMM dd, yyyy");

                phCertificate.Visible = true;
            }
            else
            {
                lblError.Text = "Certificate not found, or you do not have permission to view it.";
                lblError.Visible = true;
                phCertificate.Visible = false;
            }
        }
    }
}
