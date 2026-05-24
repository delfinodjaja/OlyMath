using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

namespace OlyMath
{
    public partial class Discussions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Only show flagged filter button for Trainer/Admin
                btnFlag.Visible = User.IsInRole("Trainer") || User.IsInRole("Admin");
                LoadDiscussions("All");
            }
        }

        private void LoadDiscussions(string filter, string sort = "Newest")
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string query = @"
                SELECT d.DiscussionId, d.Title, d.Content, d.IsFlagged, d.CreatedAt, d.[Like], u.UserName, u.Id AS UserId, m.Title AS Topic, 
                       (SELECT COUNT(*) FROM DiscussionPosts WHERE DiscussionId = d.DiscussionId) AS ReplyCount
                FROM Discussions d
                INNER JOIN AspNetUsers u ON d.UserId = u.Id
                INNER JOIN Modules m ON d.ModuleId = m.ModuleId
                WHERE 1=1 ";

            // Apply filter
            if (filter == "Flagged")
            {
                // Only Trainer/Admin can see flagged filter; otherwise revert to All
                if (!(User.IsInRole("Trainer") || User.IsInRole("Admin")))
                    filter = "All";
                else
                    query += " AND d.IsFlagged = 1 ";
            }
            else if (filter != "All")
            {
                // Map topic name to ModuleId
                int moduleId = GetModuleIdFromTopicName(filter);
                query += " AND d.ModuleId = @ModuleId ";
            }

            // Sorting
            switch (sort)
            {
                case "Popularity":
                    query += " ORDER BY d.[Like] DESC";
                    break;
                case "Unanswered":
                    query += " ORDER BY (SELECT COUNT(*) FROM DiscussionPosts WHERE DiscussionId = d.DiscussionId) ASC";
                    break;
                default:
                    query += " ORDER BY d.CreatedAt DESC";
                    break;
            }

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (filter != "All" && filter != "Flagged")
                {
                    int moduleId = GetModuleIdFromTopicName(filter);
                    cmd.Parameters.AddWithValue("@ModuleId", moduleId);
                }
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    rptDiscussions.DataSource = dt;
                    rptDiscussions.DataBind();
                }
            }
            UpdateButtonStyles(filter);
            ViewState["CurrentFilter"] = filter;
        }

        private int GetModuleIdFromTopicName(string topicName)
        {
            switch (topicName)
            {
                case "Number Theory": return 5;
                case "Combinatorics": return 6;
                case "Geometry": return 7;
                case "Algebra": return 8;
                case "Inequalities": return 9;
                default: return 1; // General
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string filter = btn.CommandArgument;
            string sort = ddlSort.SelectedValue;
            LoadDiscussions(filter, sort);
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            string currentFilter = ViewState["CurrentFilter"] as string ?? "All";
            LoadDiscussions(currentFilter, ddlSort.SelectedValue);
        }

        private void UpdateButtonStyles(string activeFilter)
        {
            btnAll.CssClass = "disc-filter-btn";
            btnNum.CssClass = "disc-filter-btn";
            btnCom.CssClass = "disc-filter-btn";
            btnGeo.CssClass = "disc-filter-btn";
            btnAlg.CssClass = "disc-filter-btn";
            btnFlag.CssClass = "disc-filter-btn";

            switch (activeFilter)
            {
                case "All": btnAll.CssClass += " active"; break;
                case "Number Theory": btnNum.CssClass += " active"; break;
                case "Combinatorics": btnCom.CssClass += " active"; break;
                case "Geometry": btnGeo.CssClass += " active"; break;
                case "Algebra": btnAlg.CssClass += " active"; break;
                case "Flagged": btnFlag.CssClass += " active"; break;
            }
        }

        protected void btnPost_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
                return;
            }

            string title = txtComposeTitle.Text.Trim();
            string content = txtDiscussion.Text.Trim();
            string selectedTopic = ddlComposeTopic.SelectedValue;
            int moduleId = GetModuleIdFromTopicName(selectedTopic);

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                return;
            if (title.Length > 80 || content.Length > 2000)
                return;

            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string insertSql = @"
                INSERT INTO Discussions (Title, Content, CreatedAt, UserId, [Like], IsFlagged, ModuleId)
                VALUES (@Title, @Content, GETDATE(), @UserId, 0, 0, @ModuleId)";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(insertSql, conn))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Content", content);
                cmd.Parameters.AddWithValue("@UserId", User.Identity.GetUserId());
                cmd.Parameters.AddWithValue("@ModuleId", moduleId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Clear form
            txtComposeTitle.Text = "";
            txtDiscussion.Text = "";
            // Refresh list
            string currentFilter = ViewState["CurrentFilter"] as string ?? "All";
            LoadDiscussions(currentFilter, ddlSort.SelectedValue);
        }

        protected void rptDiscussions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int discussionId = Convert.ToInt32(e.CommandArgument);
            string currentUserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            switch (e.CommandName)
            {
                case "Like":
                    if (!User.Identity.IsAuthenticated)
                    {
                        Response.Redirect("~/Account/Login.aspx");
                        return;
                    }
                    using (SqlConnection conn = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand("UPDATE Discussions SET [Like] = [Like] + 1 WHERE DiscussionId = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", discussionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    RefreshCurrentView();
                    break;

                case "Flag":
                    if (!User.IsInRole("Trainer")) return;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand("UPDATE Discussions SET IsFlagged = 1 WHERE DiscussionId = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", discussionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    RefreshCurrentView();
                    break;

                case "Edit":
                    Response.Redirect($"DiscussionDetail.aspx?id={discussionId}&edit=true");
                    break;

                case "Delete":
                    // Allow only Admin or discussion owner
                    if (!(User.IsInRole("Admin") || IsDiscussionOwner(discussionId, currentUserId, connStr)))
                        return;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Discussions WHERE DiscussionId = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", discussionId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    RefreshCurrentView();
                    break;

                case "Reply":
                    Response.Redirect($"DiscussionDetail.aspx?id={discussionId}#reply");
                    break;
            }
        }

        private void RefreshCurrentView()
        {
            string currentFilter = ViewState["CurrentFilter"] as string ?? "All";
            LoadDiscussions(currentFilter, ddlSort.SelectedValue);
        }

        private bool IsDiscussionOwner(int discussionId, string userId, string connStr)
        {
            if (string.IsNullOrEmpty(userId)) return false;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT UserId FROM Discussions WHERE DiscussionId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", discussionId);
                conn.Open();
                string ownerId = cmd.ExecuteScalar()?.ToString();
                return ownerId == userId;
            }
        }

        protected void rptDiscussions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DataRowView row = (DataRowView)e.Item.DataItem;
            string postOwnerId = row["UserId"].ToString();
            string currentUserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            bool isOwner = currentUserId != null && currentUserId == postOwnerId;
            bool isAdmin = User.IsInRole("Admin");
            bool isTrainer = User.IsInRole("Trainer");
            bool isFlagged = Convert.ToBoolean(row["IsFlagged"]);

            // Edit button (owner only)
            LinkButton btnEdit = (LinkButton)e.Item.FindControl("btnEditPost");
            if (btnEdit != null) btnEdit.Visible = isOwner;

            // Delete button (admin or owner)
            LinkButton btnDelete = (LinkButton)e.Item.FindControl("btnDeletePost");
            if (btnDelete != null) btnDelete.Visible = isAdmin || isOwner;

            // Flag button (trainer only, and only if not already flagged)
            LinkButton btnFlag = (LinkButton)e.Item.FindControl("btnFlagPost");
            if (btnFlag != null) btnFlag.Visible = isTrainer && !isFlagged;

            // Like button visible only to logged-in users
            LinkButton btnLike = (LinkButton)e.Item.FindControl("btnLikePost");
            if (btnLike != null) btnLike.Visible = User.Identity.IsAuthenticated;
        }
    }
}