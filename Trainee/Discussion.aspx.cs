using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
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

        public string SpecialFilter
        {
            get
            {
                return Request.QueryString["filter"]?.Trim().ToLower();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblError.Visible = false;

            if (!IsPostBack)
            {
                ConfigureRoleSpecificUI();
                ConfigureView();
                LoadOnlineUsers();
                LoadCommunityStats();
                LoadTrending();
            }
        }

        private void ConfigureRoleSpecificUI()
        {
            string role = CurrentUserRole;

            if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                lnkFlaggedFilter.Visible = true;
                phAdminComposer.Visible = true;
                phAdminModerationQueue.Visible = true;
                phAdminQuickActions.Visible = true;

                // Load Admin Mod Queue
                LoadModerationQueue();
            }
            else if (string.Equals(role, "Trainer", StringComparison.OrdinalIgnoreCase))
            {
                phTrainerUnanswered.Visible = true;
                phTrainerTips.Visible = true;

                // Load Unanswered Questions
                LoadUnansweredQuestions();
            }
        }

        private void LoadModerationQueue()
        {
            try
            {
                string sql = "SELECT COUNT(*) FROM Discussions WHERE IsFlagged = 1";
                int flaggedCount = Convert.ToInt32(DbHelper.ExecuteScalar(sql));

                litFlaggedCount.Text = flaggedCount.ToString();

                // Compute clear percentage: e.g. 100% if 0 flags, -10% per flag
                int percentage = Math.Max(0, 100 - (flaggedCount * 15));
                litQueuePercentage.Text = percentage.ToString();
                divModerationProgressBar.Style["width"] = percentage + "%";
            }
            catch (Exception)
            {
                litFlaggedCount.Text = "0";
                litQueuePercentage.Text = "100";
                divModerationProgressBar.Style["width"] = "100%";
            }
        }

        private void LoadUnansweredQuestions()
        {
            try
            {
                string sql = @"
                    SELECT TOP 3 d.Id, d.Title 
                    FROM Discussions d 
                    WHERE (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) = 0 
                    ORDER BY d.CreatedAt DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql);

                if (dt.Rows.Count > 0)
                {
                    rptUnanswered.DataSource = dt;
                    rptUnanswered.DataBind();
                    phNoUnanswered.Visible = false;
                }
                else
                {
                    rptUnanswered.DataSource = null;
                    rptUnanswered.DataBind();
                    phNoUnanswered.Visible = true;
                }
            }
            catch (Exception)
            {
                phNoUnanswered.Visible = true;
            }
        }

        private void LoadOnlineUsers()
        {
            try
            {
                // Pull a subset of members as simulated online users
                string sql = "SELECT TOP 5 FullName, Role FROM Users ORDER BY CreatedAt DESC";
                DataTable dt = DbHelper.ExecuteQuery(sql);
                rptOnlineUsers.DataSource = dt;
                rptOnlineUsers.DataBind();
            }
            catch (Exception)
            {
                // Silent fallback
            }
        }

        private void LoadCommunityStats()
        {
            try
            {
                // 1. Members
                string sqlMembers = "SELECT COUNT(*) FROM Users";
                litStatMembers.Text = DbHelper.ExecuteScalar(sqlMembers).ToString();

                // 2. Online: simulated count
                int totalUsers = Convert.ToInt32(DbHelper.ExecuteScalar(sqlMembers));
                litStatOnline.Text = Math.Max(1, totalUsers / 2 + 1).ToString();

                // 3. Posts
                string sqlPosts = "SELECT COUNT(*) FROM Discussions";
                litStatPosts.Text = DbHelper.ExecuteScalar(sqlPosts).ToString();

                // 4. Role specific statistic
                if (string.Equals(CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    litStatsTitle.Text = "Moderation Stats";
                    litStatCountryLabel.Text = "Flagged Posts";

                    string sqlFlagged = "SELECT COUNT(*) FROM Discussions WHERE IsFlagged = 1";
                    litStatCountries.Text = DbHelper.ExecuteScalar(sqlFlagged).ToString();
                }
                else
                {
                    litStatsTitle.Text = "Community Stats";
                    litStatCountryLabel.Text = "Countries";

                    string sqlCountries = "SELECT COUNT(DISTINCT CityCountry) FROM Users";
                    litStatCountries.Text = DbHelper.ExecuteScalar(sqlCountries).ToString();
                }
            }
            catch (Exception)
            {
                litStatMembers.Text = "0";
                litStatOnline.Text = "1";
                litStatPosts.Text = "0";
                litStatCountries.Text = "0";
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

                LoadPinnedAnnouncements();
                LoadFeed();
            }
        }

        private void LoadPinnedAnnouncements()
        {
            try
            {
                // Announcements are where IsAnnouncement = 1 and IsPinned = 1
                string sql = @"
                    SELECT d.Id, d.Title, d.Content, d.Topic, d.LikesCount, d.CreatedAt, u.FullName,
                    (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount,
                    (SELECT COUNT(*) FROM DiscussionLikes dl WHERE dl.DiscussionId = d.Id AND dl.UserId = @userId) AS IsLikedByUser
                    FROM Discussions d
                    INNER JOIN Users u ON d.UserId = u.Id
                    WHERE d.IsPinned = 1 AND d.IsAnnouncement = 1
                    ORDER BY d.CreatedAt DESC";

                DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@userId", CurrentUserId));

                if (dt.Rows.Count > 0)
                {
                    rptPinned.DataSource = dt;
                    rptPinned.DataBind();
                    phPinnedAnnouncements.Visible = true;
                }
                else
                {
                    phPinnedAnnouncements.Visible = false;
                }
            }
            catch (Exception)
            {
                phPinnedAnnouncements.Visible = false;
            }
        }

        private void LoadFeed()
        {
            string sql = @"
                SELECT d.Id, d.Title, d.Content, d.Topic, d.LikesCount, d.CreatedAt, d.UserId, d.IsFlagged, d.IsPinned, u.FullName, u.CityCountry,
                (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount,
                (SELECT COUNT(*) FROM DiscussionLikes dl WHERE dl.DiscussionId = d.Id AND dl.UserId = @userId) AS IsLikedByUser
                FROM Discussions d
                INNER JOIN Users u ON d.UserId = u.Id
                WHERE d.IsAnnouncement = 0 OR d.IsPinned = 0"; // Exclude active pinned announcements from main flow

            DataTable dt;
            if (string.Equals(SpecialFilter, "flagged", StringComparison.OrdinalIgnoreCase))
            {
                sql += " AND d.IsFlagged = 1 ORDER BY d.CreatedAt DESC";
                dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@userId", CurrentUserId));
            }
            else if (!string.IsNullOrEmpty(TopicFilter))
            {
                sql += " AND d.Topic = @topic ORDER BY d.CreatedAt DESC";
                dt = DbHelper.ExecuteQuery(sql, 
                    new SqlParameter("@userId", CurrentUserId),
                    new SqlParameter("@topic", TopicFilter)
                );
            }
            else
            {
                sql += " ORDER BY d.CreatedAt DESC";
                dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@userId", CurrentUserId));
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
                SELECT d.Id, d.Title, d.Content, d.Topic, d.LikesCount, d.CreatedAt, d.UserId, d.IsFlagged,
                (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount,
                (SELECT COUNT(*) FROM DiscussionLikes dl WHERE dl.DiscussionId = d.Id AND dl.UserId = @userId) AS IsLikedByUser
                FROM Discussions d
                INNER JOIN Users u ON d.UserId = u.Id
                WHERE d.Id = @postId";

            DataTable dt = DbHelper.ExecuteQuery(sql, 
                new SqlParameter("@postId", PostId),
                new SqlParameter("@userId", CurrentUserId)
            );

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("Discussion.aspx");
                return;
            }

            DataRow row = dt.Rows[0];
            
            // Query author name & details manually to avoid ambiguity in joins
            int authorId = Convert.ToInt32(row["UserId"]);
            string authorName = "Global Member";
            string authorLoc = "Global";
            DataTable dtAuthor = DbHelper.ExecuteQuery("SELECT FullName, CityCountry FROM Users WHERE Id = @id", new SqlParameter("@id", authorId));
            if (dtAuthor.Rows.Count > 0)
            {
                authorName = dtAuthor.Rows[0]["FullName"].ToString();
                authorLoc = dtAuthor.Rows[0]["CityCountry"]?.ToString() ?? "Global";
            }

            litThreadAuthor.Text = authorName;
            litThreadAvatar.Text = GetInitials(authorName);
            litThreadLocation.Text = authorLoc;
            litThreadTime.Text = GetTimeAgo(row["CreatedAt"]);
            litThreadTitle.Text = row["Title"].ToString();
            litThreadContent.Text = row["Content"].ToString().Replace("\n", "<br />");
            litThreadLikes.Text = row["LikesCount"].ToString();
            litThreadRepliesCount.Text = row["ReplyCount"].ToString();

            // Set topic tag class and text
            string tCode = row["Topic"].ToString();
            lblThreadTopicBadge.Text = GetTopicLabel(tCode);
            lblThreadTopicBadge.CssClass = "disc-topic-tag " + (tCode != null ? tCode.ToLower() : "");

            // Style like button
            bool isLiked = Convert.ToInt32(row["IsLikedByUser"]) == 1;
            btnThreadLike.CssClass = isLiked ? "disc-action liked" : "disc-action";

            // Share dynamic script simulation
            spanThreadShare.Attributes["onclick"] = "return simulateShare(" + PostId + ");";
            spanThreadShare.Attributes["style"] = "cursor:pointer;";

            // Flag badge and visibility
            bool isFlagged = Convert.ToInt32(row["IsFlagged"]) == 1;
            lblThreadFlagged.Visible = isFlagged;
            lnkThreadFlag.Visible = !isFlagged;

            // Trainer unanswered response badge
            bool isTrainer = string.Equals(CurrentUserRole, "Trainer", StringComparison.OrdinalIgnoreCase);
            int replyCount = Convert.ToInt32(row["ReplyCount"]);
            lblThreadNeedsResponse.Visible = isTrainer && (replyCount == 0);

            // Admin panel details
            if (string.Equals(CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                divThreadAdminActions.Visible = true;
                btnThreadAdminClear.Visible = isFlagged;
                
                // Set command argument to the author's userId for Warn button
                btnThreadAdminWarn.CommandArgument = authorId.ToString();
            }
            else
            {
                divThreadAdminActions.Visible = false;
            }
        }

        private void LoadReplies()
        {
            string sql = @"
                SELECT dr.*, u.FullName
                FROM DiscussionReplies dr
                INNER JOIN Users u ON dr.UserId = u.Id
                WHERE dr.DiscussionId = @postId
                ORDER BY dr.CreatedAt ASC";

            DataTable dt = DbHelper.ExecuteQuery(sql, new SqlParameter("@postId", PostId));
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
                SELECT TOP 3 d.Id, d.Title, d.LikesCount,
                (SELECT COUNT(*) FROM DiscussionReplies dr WHERE dr.DiscussionId = d.Id) AS ReplyCount
                FROM Discussions d
                ORDER BY d.LikesCount DESC";

            DataTable dt = DbHelper.ExecuteQuery(sql);
            rptTrending.DataSource = dt;
            rptTrending.DataBind();
        }

        protected void btnPost_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblError.Visible = false;

            string title = txtPostTitle.Text.Trim();
            string content = txtPostContent.Text.Trim();
            string topic = ddlPostTopic.SelectedValue;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                lblError.Text = "Please enter both a title and post content.";
                lblError.Visible = true;
                return;
            }

            try
            {
                string sql = "INSERT INTO Discussions (UserId, Title, Content, Topic, LikesCount, IsFlagged, IsPinned, IsAnnouncement) VALUES (@userId, @title, @content, @topic, 0, 0, 0, 0)";
                DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@userId", CurrentUserId),
                    new SqlParameter("@title", title),
                    new SqlParameter("@content", content),
                    new SqlParameter("@topic", topic)
                );

                // Reset inputs and reload
                txtPostTitle.Text = "";
                txtPostContent.Text = "";
                lblMessage.Text = "Discussion post shared successfully!";
                lblMessage.Visible = true;

                ConfigureView();
                LoadCommunityStats();
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to post discussion: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnAnnouncePost_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblError.Visible = false;

            string title = txtAnnounceTitle.Text.Trim();
            string content = txtAnnounceContent.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                lblError.Text = "Please enter both an announcement title and description.";
                lblError.Visible = true;
                return;
            }

            try
            {
                // Platform announcements are pinned, and categorized under 'lg' (Logic/Platform)
                string sql = "INSERT INTO Discussions (UserId, Title, Content, Topic, LikesCount, IsFlagged, IsPinned, IsAnnouncement) VALUES (@userId, @title, @content, 'lg', 0, 0, 1, 1)";
                DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@userId", CurrentUserId),
                    new SqlParameter("@title", title),
                    new SqlParameter("@content", content)
                );

                txtAnnounceTitle.Text = "";
                txtAnnounceContent.Text = "";
                lblMessage.Text = "Platform announcement pinned and broadcasted successfully!";
                lblMessage.Visible = true;

                ConfigureView();
                LoadCommunityStats();
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to post announcement: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnSubmitReply_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            lblError.Visible = false;

            string content = txtReplyContent.Text.Trim();
            if (string.IsNullOrEmpty(content)) return;

            try
            {
                string sql = "INSERT INTO DiscussionReplies (DiscussionId, UserId, Content) VALUES (@postId, @userId, @content)";
                DbHelper.ExecuteNonQuery(sql,
                    new SqlParameter("@postId", PostId),
                    new SqlParameter("@userId", CurrentUserId),
                    new SqlParameter("@content", content)
                );

                txtReplyContent.Text = "";
                
                LoadThreadDetails();
                LoadReplies();
                LoadTrending(); 
                LoadCommunityStats();
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to post reply: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void rptDiscussions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMessage.Visible = false;
            lblError.Visible = false;

            if (e.CommandName == "ToggleLike")
            {
                int pId = Convert.ToInt32(e.CommandArgument);
                ToggleLikePost(pId);
                LoadFeed();
                LoadTrending();
            }
            else if (e.CommandName == "FlagPost")
            {
                int pId = Convert.ToInt32(e.CommandArgument);
                FlagPostAction(pId);
            }
            else if (e.CommandName == "AdminClearFlag")
            {
                int pId = Convert.ToInt32(e.CommandArgument);
                ClearFlagAction(pId);
            }
            else if (e.CommandName == "AdminWarnUser")
            {
                int uId = Convert.ToInt32(e.CommandArgument);
                WarnUserAction(uId);
            }
            else if (e.CommandName == "AdminRemovePost")
            {
                int pId = Convert.ToInt32(e.CommandArgument);
                RemovePostAction(pId);
            }
        }

        protected void rptPinned_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMessage.Visible = false;
            lblError.Visible = false;

            if (e.CommandName == "ToggleLike")
            {
                int pId = Convert.ToInt32(e.CommandArgument);
                ToggleLikePost(pId);
                LoadPinnedAnnouncements();
            }
            else if (e.CommandName == "UnpinPost")
            {
                int pId = Convert.ToInt32(e.CommandArgument);
                try
                {
                    string sql = "UPDATE Discussions SET IsPinned = 0 WHERE Id = @postId";
                    DbHelper.ExecuteNonQuery(sql, new SqlParameter("@postId", pId));
                    lblMessage.Text = "Announcement unpinned successfully.";
                    lblMessage.Visible = true;
                    ConfigureView();
                }
                catch (Exception ex)
                {
                    lblError.Text = "Unpin failed: " + ex.Message;
                    lblError.Visible = true;
                }
            }
        }

        protected void btnThreadLike_Click(object sender, EventArgs e)
        {
            ToggleLikePost(PostId);
            LoadThreadDetails();
            LoadTrending();
        }

        protected void btnThreadFlag_Click(object sender, EventArgs e)
        {
            FlagPostAction(PostId);
            LoadThreadDetails();
        }

        protected void btnThreadAdminClear_Click(object sender, EventArgs e)
        {
            ClearFlagAction(PostId);
            LoadThreadDetails();
            if (string.Equals(CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                LoadModerationQueue();
            }
        }

        protected void btnThreadAdminWarn_Click(object sender, EventArgs e)
        {
            try
            {
                // Fetch post author to warn
                string sql = "SELECT UserId FROM Discussions WHERE Id = @postId";
                int authorId = Convert.ToInt32(DbHelper.ExecuteScalar(sql, new SqlParameter("@postId", PostId)));
                WarnUserAction(authorId);
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to warn author: " + ex.Message;
                lblError.Visible = true;
            }
        }

        protected void btnThreadAdminRemove_Click(object sender, EventArgs e)
        {
            RemovePostAction(PostId);
        }

        private void ToggleLikePost(int pId)
        {
            int userId = CurrentUserId;

            try
            {
                string checkSql = "SELECT COUNT(*) FROM DiscussionLikes WHERE UserId = @userId AND DiscussionId = @postId";
                long count = Convert.ToInt64(DbHelper.ExecuteScalar(checkSql, 
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@postId", pId)
                ));

                if (count > 0)
                {
                    string deleteSql = "DELETE FROM DiscussionLikes WHERE UserId = @userId AND DiscussionId = @postId";
                    DbHelper.ExecuteNonQuery(deleteSql,
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@postId", pId)
                    );

                    string decSql = "UPDATE Discussions SET LikesCount = CASE WHEN LikesCount > 0 THEN LikesCount - 1 ELSE 0 END WHERE Id = @postId";
                    DbHelper.ExecuteNonQuery(decSql, new SqlParameter("@postId", pId));
                }
                else
                {
                    string insertSql = "INSERT INTO DiscussionLikes (UserId, DiscussionId) VALUES (@userId, @postId)";
                    DbHelper.ExecuteNonQuery(insertSql,
                        new SqlParameter("@userId", userId),
                        new SqlParameter("@postId", pId)
                    );

                    string incSql = "UPDATE Discussions SET LikesCount = LikesCount + 1 WHERE Id = @postId";
                    DbHelper.ExecuteNonQuery(incSql, new SqlParameter("@postId", pId));
                }
            }
            catch (Exception)
            {
                // Silent fail
            }
        }

        private void FlagPostAction(int pId)
        {
            try
            {
                string sql = "UPDATE Discussions SET IsFlagged = 1 WHERE Id = @postId";
                int rows = DbHelper.ExecuteNonQuery(sql, new SqlParameter("@postId", pId));
                if (rows > 0)
                {
                    lblMessage.Text = "Post has been flagged for administrator review.";
                    lblMessage.Visible = true;
                    ConfigureView();
                    LoadCommunityStats();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to flag post: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void ClearFlagAction(int pId)
        {
            try
            {
                string sql = "UPDATE Discussions SET IsFlagged = 0 WHERE Id = @postId";
                int rows = DbHelper.ExecuteNonQuery(sql, new SqlParameter("@postId", pId));
                if (rows > 0)
                {
                    lblMessage.Text = "Flags cleared successfully.";
                    lblMessage.Visible = true;
                    ConfigureView();
                    LoadCommunityStats();
                    if (string.Equals(CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        LoadModerationQueue();
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to clear flag: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void WarnUserAction(int uId)
        {
            try
            {
                string sql = "UPDATE Users SET WarningsCount = WarningsCount + 1 WHERE Id = @userId";
                int rows = DbHelper.ExecuteNonQuery(sql, new SqlParameter("@userId", uId));
                if (rows > 0)
                {
                    lblMessage.Text = "User issued a warning successfully.";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to warn user: " + ex.Message;
                lblError.Visible = true;
            }
        }

        private void RemovePostAction(int pId)
        {
            try
            {
                // Clean up likes and replies manually to satisfy No Action rules if needed, though they cascade on DB level
                DbHelper.ExecuteNonQuery("DELETE FROM DiscussionLikes WHERE DiscussionId = @postId", new SqlParameter("@postId", pId));
                DbHelper.ExecuteNonQuery("DELETE FROM DiscussionReplies WHERE DiscussionId = @postId", new SqlParameter("@postId", pId));
                
                int rows = DbHelper.ExecuteNonQuery("DELETE FROM Discussions WHERE Id = @postId", new SqlParameter("@postId", pId));
                if (rows > 0)
                {
                    lblMessage.Text = "Discussion post removed successfully.";
                    lblMessage.Visible = true;
                    
                    if (PostId > 0)
                    {
                        // Redirect to main board if in single post view
                        Response.Redirect("Discussion.aspx");
                    }
                    else
                    {
                        ConfigureView();
                        LoadCommunityStats();
                        if (string.Equals(CurrentUserRole, "Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            LoadModerationQueue();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to remove post: " + ex.Message;
                lblError.Visible = true;
            }
        }

        // Quick Actions clicks
        protected void btnQuickBroadcast_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "Simulating global system notification broadcast! All online users received notice.";
            lblMessage.Visible = true;
        }

        protected void btnQuickExport_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "Discussion post moderation logs exported to CSV successfully! (Simulated download)";
            lblMessage.Visible = true;
        }

        // formatting helpers
        public string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "OM";
            string[] parts = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "OM";
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
            if (span.TotalMinutes < 60) return HttpUtility.HtmlEncode(Math.Floor(span.TotalMinutes) + " min ago");
            if (span.TotalHours < 24) return HttpUtility.HtmlEncode(Math.Floor(span.TotalHours) + " hours ago");
            if (span.TotalDays < 2) return "yesterday";
            return dt.ToString("MMM dd, yyyy");
        }

        public string GetTopicLabel(string topicCode)
        {
            if (topicCode == null) return "General";
            switch (topicCode.ToLower())
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
