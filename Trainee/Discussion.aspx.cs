using System;
using System.Data;
using System.Data.SQLite;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OlyMath.Trainee
{
    public partial class Discussion : BasePage
    {
        // Accessible by all authenticated roles (Trainees, Trainers, Admins)
        public override string[] AllowedRoles { get; set; } = new string[] { "Trainee", "Trainer", "Admin" };

        public int PostId
        {
            get
            {
                int id = 0;
                int.TryParse(Request.QueryString["postId"], out id);
                return id;
            }
        }

        public string TopicFilter
        {
            get
            {
                return Request.QueryString["topic"]?.Trim().ToLower();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfigureView();
                LoadTrending();
            }
        }

        private void ConfigureView()
        {
            if (PostId > 0)
            {
                // Single Thread Mode
                phFeedView.Visible = false;
                phThreadView.Visible = true;
                phFilters.Visible = false;
                
                LoadThreadDetails();
                LoadReplies();
            }
            else
            {
                // Feed Mode
                phFeedView.Visible = true;
                phThreadView.Visible = false;
                phFilters.Visible = true;

                LoadFeed();
            }
        }

        private void LoadFeed()
        {
            string sql = @"
                SELECT d.Id, d.Title, d.Content, d.Topic, d.LikesCount, d.CreatedAt, u.FullName, u.CityCountry,
                (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount,
                (SELECT COUNT(*) FROM DiscussionLikes dl WHERE dl.DiscussionId = d.Id AND dl.UserId = @userId) AS IsLikedByUser
                FROM Discussions d
                INNER JOIN Users u ON d.UserId = u.Id";

            DataTable dt;
            if (!string.IsNullOrEmpty(TopicFilter))
            {
                sql += " WHERE d.Topic = @topic ORDER BY d.CreatedAt DESC";
                dt = DbHelper.ExecuteQuery(sql, 
                    new SQLiteParameter("@userId", CurrentUserId),
                    new SQLiteParameter("@topic", TopicFilter)
                );
            }
            else
            {
                sql += " ORDER BY d.CreatedAt DESC";
                dt = DbHelper.ExecuteQuery(sql, new SQLiteParameter("@userId", CurrentUserId));
            }

            if (dt.Rows.Count > 0)
            {
                rptDiscussions.DataSource = dt;
                rptDiscussions.DataBind();
                phNoPosts.Visible = false;
            }
            else
            {
                rptDiscussions.DataSource = null;
                rptDiscussions.DataBind();
                phNoPosts.Visible = true;
            }
        }

        private void LoadThreadDetails()
        {
            string sql = @"
                SELECT d.Id, d.Title, d.Content, d.Topic, d.LikesCount, d.CreatedAt, u.FullName, u.CityCountry,
                (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount,
                (SELECT COUNT(*) FROM DiscussionLikes dl WHERE dl.DiscussionId = d.Id AND dl.UserId = @userId) AS IsLikedByUser
                FROM Discussions d
                INNER JOIN Users u ON d.UserId = u.Id
                WHERE d.Id = @postId";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                new SQLiteParameter("@postId", PostId),
                new SQLiteParameter("@userId", CurrentUserId)
            );

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("Discussion.aspx");
                return;
            }

            DataRow row = dt.Rows[0];
            string authorName = row["FullName"].ToString();
            litThreadAuthor.Text = authorName;
            litThreadAvatar.Text = GetInitials(authorName);
            litThreadLocation.Text = row["CityCountry"]?.ToString() ?? "Global";
            litThreadTime.Text = GetTimeAgo(row["CreatedAt"]);
            litThreadTitle.Text = row["Title"].ToString();
            litThreadContent.Text = row["Content"].ToString().Replace("\n", "<br />");
            litThreadLikes.Text = row["LikesCount"].ToString();
            litThreadRepliesCount.Text = row["ReplyCount"].ToString();

            // Set topic tag class and text
            string tCode = row["Topic"].ToString();
            lblThreadTopicBadge.Text = GetTopicLabel(tCode);
            lblThreadTopicBadge.CssClass = $"disc-topic-tag {tCode}";

            // Style like button
            bool isLiked = Convert.ToInt32(row["IsLikedByUser"]) == 1;
            btnThreadLike.CssClass = isLiked ? "disc-action liked" : "disc-action";
        }

        private void LoadReplies()
        {
            string sql = @"
                SELECT dr.*, u.FullName
                FROM DiscussionReplies dr
                INNER JOIN Users u ON dr.UserId = u.Id
                WHERE dr.DiscussionId = @postId
                ORDER BY dr.CreatedAt ASC";

            DataTable dt = DbHelper.ExecuteQuery(sql, new SQLiteParameter("@postId", PostId));
            if (dt.Rows.Count > 0)
            {
                rptReplies.DataSource = dt;
                rptReplies.DataBind();
                phNoReplies.Visible = false;
            }
            else
            {
                rptReplies.DataSource = null;
                rptReplies.DataBind();
                phNoReplies.Visible = true;
            }
        }

        private void LoadTrending()
        {
            string sql = @"
                SELECT d.Id, d.Title, d.LikesCount,
                (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount
                FROM Discussions d
                ORDER BY d.LikesCount DESC LIMIT 3";

            DataTable dt = DbHelper.ExecuteQuery(sql);
            rptTrending.DataSource = dt;
            rptTrending.DataBind();
        }

        protected void btnPost_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            string title = txtPostTitle.Text.Trim();
            string content = txtPostContent.Text.Trim();
            string topic = ddlPostTopic.SelectedValue;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                lblMessage.Text = "Please enter both a title and post content.";
                lblMessage.Visible = true;
                return;
            }

            try
            {
                string sql = "INSERT INTO Discussions (UserId, Title, Content, Topic, LikesCount) VALUES (@userId, @title, @content, @topic, 0)";
                DbHelper.ExecuteNonQuery(sql,
                    new SQLiteParameter("@userId", CurrentUserId),
                    new SQLiteParameter("@title", title),
                    new SQLiteParameter("@content", content),
                    new SQLiteParameter("@topic", topic)
                );

                // Reset inputs and reload
                txtPostTitle.Text = "";
                txtPostContent.Text = "";
                LoadFeed();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to post discussion: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void btnSubmitReply_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            string content = txtReplyContent.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            try
            {
                string sql = "INSERT INTO DiscussionReplies (DiscussionId, UserId, Content) VALUES (@postId, @userId, @content)";
                DbHelper.ExecuteNonQuery(sql,
                    new SQLiteParameter("@postId", PostId),
                    new SQLiteParameter("@userId", CurrentUserId),
                    new SQLiteParameter("@content", content)
                );

                // Reset and refresh thread
                txtReplyContent.Text = "";
                LoadThreadDetails();
                LoadReplies();
                LoadTrending(); // Refresh replies counts
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Failed to post reply: " + ex.Message;
                lblMessage.Visible = true;
            }
        }

        protected void rptDiscussions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ToggleLike")
            {
                int pId = Convert.ToInt32(e.CommandArgument);
                ToggleLikePost(pId);
                LoadFeed();
                LoadTrending();
            }
        }

        protected void btnThreadLike_Click(object sender, EventArgs e)
        {
            ToggleLikePost(PostId);
            LoadThreadDetails();
            LoadTrending();
        }

        private void ToggleLikePost(int pId)
        {
            int userId = CurrentUserId;

            // 1. Check if user already liked the post
            string checkSql = "SELECT COUNT(*) FROM DiscussionLikes WHERE UserId = @userId AND DiscussionId = @postId";
            long count = (long)DbHelper.ExecuteScalar(checkSql, 
                new SQLiteParameter("@userId", userId),
                new SQLiteParameter("@postId", pId)
            );

            if (count > 0)
            {
                // Unlike: Delete like record
                string deleteSql = "DELETE FROM DiscussionLikes WHERE UserId = @userId AND DiscussionId = @postId";
                DbHelper.ExecuteNonQuery(deleteSql,
                    new SQLiteParameter("@userId", userId),
                    new SQLiteParameter("@postId", pId)
                );

                // Decrement count
                string decSql = "UPDATE Discussions SET LikesCount = MAX(0, LikesCount - 1) WHERE Id = @postId";
                DbHelper.ExecuteNonQuery(decSql, new SQLiteParameter("@postId", pId));
            }
            else
            {
                // Like: Insert like record
                string insertSql = "INSERT INTO DiscussionLikes (UserId, DiscussionId) VALUES (@userId, @postId)";
                DbHelper.ExecuteNonQuery(insertSql,
                    new SQLiteParameter("@userId", userId),
                    new SQLiteParameter("@postId", pId)
                );

                // Increment count
                string incSql = "UPDATE Discussions SET LikesCount = LikesCount + 1 WHERE Id = @postId";
                DbHelper.ExecuteNonQuery(incSql, new SQLiteParameter("@postId", pId));
            }
        }

        // formatting helpers
        public string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "OM";
            string[] parts = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        public string GetTimeAgo(object dateObj)
        {
            if (dateObj == null || dateObj == DBNull.Value) return "just now";
            DateTime dt = Convert.ToDateTime(dateObj);
            TimeSpan span = DateTime.Now - dt;

            if (span.TotalSeconds < 0) return "just now";
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return $"{Math.Floor(span.TotalMinutes)} min ago";
            if (span.TotalHours < 24) return $"{Math.Floor(span.TotalHours)} hours ago";
            if (span.TotalDays < 2) return "yesterday";
            return dt.ToString("MMM dd, yyyy");
        }

        public string GetTopicLabel(string topicCode)
        {
            switch (topicCode?.ToLower())
            {
                case "nt": return "Number Theory";
                case "co": return "Combinatorics";
                case "ge": return "Geometry";
                case "al": return "Algebra";
                case "iq": return "Inequalities";
                case "lg": return "Logic";
                default: return "General";
            }
        }
    }
}
