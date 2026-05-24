using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;

namespace OlyMath
{
    public partial class DiscussionDetail : System.Web.UI.Page
    {
        private int DiscussionId
        {
            get { return ViewState["DiscussionId"] != null ? (int)ViewState["DiscussionId"] : 0; }
            set { ViewState["DiscussionId"] = value; }
        }

        private int? EditingReplyId
        {
            get { return ViewState["EditingReplyId"] as int?; }
            set { ViewState["EditingReplyId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["id"], out int id) || id <= 0)
            {
                Response.Redirect("Discussions.aspx");
                return;
            }
            DiscussionId = id;

            if (!IsPostBack)
            {
                LoadDiscussion();
                LoadReplies();
                if (Request.QueryString["edit"] == "true")
                    EnterMainEditMode();
            }
        }

        private void LoadDiscussion()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            const string sql = @"
                SELECT d.DiscussionId, d.Title, d.Content, d.IsFlagged, d.CreatedAt, d.UserId, d.[Like], 
                       u.UserName, m.Title AS Topic
                FROM Discussions d
                INNER JOIN AspNetUsers u ON d.UserId = u.Id
                INNER JOIN Modules m ON d.ModuleId = m.ModuleId
                WHERE d.DiscussionId = @Id";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", DiscussionId);
                conn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        Response.Redirect("Discussions.aspx");
                        return;
                    }

                    string postOwnerId = dr["UserId"].ToString();
                    string currentUserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
                    bool isOwner = currentUserId != null && currentUserId == postOwnerId;
                    bool isAdmin = User.IsInRole("Admin");
                    bool isTrainer = User.IsInRole("Trainer");
                    bool isFlagged = Convert.ToBoolean(dr["IsFlagged"]);

                    // Set view data
                    litAvatarInitials.Text = dr["UserName"].ToString().Length >= 2
                        ? dr["UserName"].ToString().Substring(0, 2).ToUpper()
                        : dr["UserName"].ToString().ToUpper();
                    lblAuthor.Text = dr["UserName"].ToString();
                    lblCreatedAt.Text = Convert.ToDateTime(dr["CreatedAt"]).ToString("MMM dd, yyyy");
                    lblTitle.Text = dr["Title"].ToString();
                    lblMainContent.Text = dr["Content"].ToString();
                    lblTopic.Text = dr["Topic"].ToString();
                    btnLikeMain.Text = $"♡ {dr["Like"]} Likes";

                    pnlFlaggedBadge.Visible = isFlagged;
                    btnFlagMain.Visible = isTrainer && !isFlagged;
                    btnEdit.Visible = isOwner;
                    btnDelete.Visible = isAdmin || isOwner;

                    // Pre-fill edit fields
                    txtEditTitle.Text = dr["Title"].ToString();
                    txtEditContent.Text = dr["Content"].ToString();
                    ddlEditTopic.SelectedValue = dr["Topic"].ToString();
                }
            }

            lblCommentingAs.Text = User.Identity.IsAuthenticated ? User.Identity.Name : "Guest";
        }

        private void LoadReplies()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            const string sql = @"
                SELECT p.PostId, p.Content, p.Edited, p.CreatedAt, p.UserId, u.UserName
                FROM DiscussionPosts p
                INNER JOIN AspNetUsers u ON p.UserId = u.Id
                WHERE p.DiscussionId = @Id
                ORDER BY p.CreatedAt ASC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", DiscussionId);
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    lblReplyCount.Text = $"{dt.Rows.Count} {(dt.Rows.Count == 1 ? "Reply" : "Replies")}";
                    rptReplies.DataSource = dt;
                    rptReplies.DataBind();
                }
            }
        }

        // MAIN POST ACTIONS
        protected void btnLikeMain_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("~/Account/Login.aspx");
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("UPDATE Discussions SET [Like] = [Like] + 1 WHERE DiscussionId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", DiscussionId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadDiscussion();
            LoadReplies();
        }

        protected void btnFlagMain_Click(object sender, EventArgs e)
        {
            if (!User.IsInRole("Trainer")) return;
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("UPDATE Discussions SET IsFlagged = 1 WHERE DiscussionId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", DiscussionId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadDiscussion();
            LoadReplies();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (!VerifyDiscussionOwner()) return;
            EnterMainEditMode();
        }

        private void EnterMainEditMode()
        {
            pnlViewMode.Visible = false;
            pnlEditMode.Visible = true;
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlViewMode.Visible = true;
            pnlEditMode.Visible = false;
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (!VerifyDiscussionOwner()) return;
            string newTitle = txtEditTitle.Text.Trim();
            string newContent = txtEditContent.Text.Trim();
            string newTopic = ddlEditTopic.SelectedValue;
            if (string.IsNullOrEmpty(newTitle) || string.IsNullOrEmpty(newContent)) return;
            if (newTitle.Length > 80 || newContent.Length > 2000) return;

            int newModuleId = GetModuleIdFromTopicName(newTopic);
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE Discussions SET Title = @Title, Content = @Content, ModuleId = @ModuleId WHERE DiscussionId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Title", newTitle);
                cmd.Parameters.AddWithValue("@Content", newContent);
                cmd.Parameters.AddWithValue("@ModuleId", newModuleId);
                cmd.Parameters.AddWithValue("@Id", DiscussionId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            pnlViewMode.Visible = true;
            pnlEditMode.Visible = false;
            LoadDiscussion();
            LoadReplies();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (!User.IsInRole("Admin") && !VerifyDiscussionOwner()) return;
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Discussions WHERE DiscussionId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", DiscussionId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            Response.Redirect("Discussions.aspx");
        }

        // REPLIES ACTIONS
        protected void btnPostReply_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("~/Account/Login.aspx");
            string content = txtNewReply.Text.Trim();
            if (string.IsNullOrEmpty(content) || content.Length > 2000) return;

            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO DiscussionPosts (DiscussionId, Content, Edited, CreatedAt, UserId)
                VALUES (@DiscussionId, @Content, 0, GETDATE(), @UserId)", conn))
            {
                cmd.Parameters.AddWithValue("@DiscussionId", DiscussionId);
                cmd.Parameters.AddWithValue("@Content", content);
                cmd.Parameters.AddWithValue("@UserId", User.Identity.GetUserId());
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            txtNewReply.Text = "";
            LoadReplies();
        }

        protected void rptReplies_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DataRowView row = (DataRowView)e.Item.DataItem;
            int postId = Convert.ToInt32(row["PostId"]);
            string replyOwnerId = row["UserId"].ToString();
            string currentUserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null;
            bool isOwner = currentUserId != null && currentUserId == replyOwnerId;
            bool isAdmin = User.IsInRole("Admin");

            LinkButton btnEditReply = (LinkButton)e.Item.FindControl("btnEditReply");
            LinkButton btnDeleteReply = (LinkButton)e.Item.FindControl("btnDeleteReply");
            Panel pnlReplyView = (Panel)e.Item.FindControl("pnlReplyView");
            Panel pnlReplyEdit = (Panel)e.Item.FindControl("pnlReplyEdit");
            HiddenField hfPostId = (HiddenField)e.Item.FindControl("hfPostId");

            if (hfPostId != null) hfPostId.Value = postId.ToString();
            if (btnEditReply != null) btnEditReply.Visible = isOwner;
            if (btnDeleteReply != null) btnDeleteReply.Visible = isAdmin || isOwner;

            bool isEditing = EditingReplyId.HasValue && EditingReplyId.Value == postId && isOwner;
            if (pnlReplyView != null) pnlReplyView.Visible = !isEditing;
            if (pnlReplyEdit != null)
            {
                pnlReplyEdit.Visible = isEditing;
                if (isEditing)
                {
                    TextBox txtReplyEdit = (TextBox)e.Item.FindControl("txtReplyEdit");
                    if (txtReplyEdit != null) txtReplyEdit.Text = row["Content"].ToString();
                }
            }
        }

        protected void rptReplies_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("~/Account/Login.aspx");

            int postId = Convert.ToInt32(e.CommandArgument);
            string currentUserId = User.Identity.GetUserId();
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            switch (e.CommandName)
            {
                case "EditReply":
                    if (!VerifyReplyOwner(postId, currentUserId, connStr)) return;
                    EditingReplyId = postId;
                    LoadReplies();
                    break;

                case "SaveReply":
                    if (!VerifyReplyOwner(postId, currentUserId, connStr)) return;
                    string newContent = ((TextBox)e.Item.FindControl("txtReplyEdit")).Text.Trim();
                    if (string.IsNullOrEmpty(newContent) || newContent.Length > 2000) return;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand("UPDATE DiscussionPosts SET Content = @Content, Edited = 1 WHERE PostId = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Content", newContent);
                        cmd.Parameters.AddWithValue("@Id", postId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    EditingReplyId = null;
                    LoadReplies();
                    break;

                case "CancelReplyEdit":
                    EditingReplyId = null;
                    LoadReplies();
                    break;

                case "DeleteReply":
                    if (!User.IsInRole("Admin") && !VerifyReplyOwner(postId, currentUserId, connStr)) return;
                    using (SqlConnection conn = new SqlConnection(connStr))
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM DiscussionPosts WHERE PostId = @Id", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", postId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    LoadReplies();
                    break;
            }
        }

        // SECURITY HELPERS
        private bool VerifyDiscussionOwner()
        {
            if (!User.Identity.IsAuthenticated) return false;
            string currentUserId = User.Identity.GetUserId();
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT UserId FROM Discussions WHERE DiscussionId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", DiscussionId);
                conn.Open();
                string ownerId = cmd.ExecuteScalar()?.ToString();
                return ownerId == currentUserId;
            }
        }

        private bool VerifyReplyOwner(int postId, string userId, string connStr)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT UserId FROM DiscussionPosts WHERE PostId = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", postId);
                conn.Open();
                string ownerId = cmd.ExecuteScalar()?.ToString();
                return ownerId == userId;
            }
        }

        private int GetModuleIdFromTopicName(string topicName)
        {
            switch (topicName)
            {
                case "Number Theory": return 5;   // adjust to your ModuleId values
                case "Combinatorics": return 6;
                case "Geometry": return 7;
                case "Algebra": return 8;
                case "Inequalities": return 9;
                default: return 5;
            }
        }
    }
}