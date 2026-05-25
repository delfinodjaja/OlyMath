<%@ Page Title="OlyMath - Discussion Zone" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Discussion.aspx.cs" Inherits="OlyMath.Trainee.Discussion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .moderation-progress-container {
            margin-top: 12px;
            margin-bottom: 8px;
        }
        .moderation-progress-bar-wrap {
            height: 8px;
            background: var(--bg);
            border-radius: 99px;
            overflow: hidden;
        }
        .moderation-progress-bar {
            height: 100%;
            border-radius: 99px;
            background: var(--orange);
            transition: width 0.3s ease;
        }
        .badge.needs-response {
            background: #FFF0EC;
            color: var(--orange);
            font-size: 0.72rem;
            margin-left: 8px;
            border: 1px solid rgba(255, 87, 51, 0.2);
        }
        .badge.flagged-badge {
            background: #FFF0EC;
            color: var(--orange);
            font-size: 0.72rem;
            margin-left: 8px;
            font-weight: 700;
        }
        .disc-action.btn-share {
            cursor: pointer;
            transition: color 0.15s;
        }
        .disc-action.btn-share:hover {
            color: var(--teal);
        }
        .admin-action-btn {
            font-size: 0.7rem;
            padding: 3px 8px;
            margin-right: 4px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-weight: 600;
        }
        .admin-action-btn.clear {
            background: #E6FBF7;
            color: #00A383;
        }
        .admin-action-btn.warn {
            background: #FFF0EC;
            color: var(--orange);
        }
        .admin-action-btn.remove {
            background: #F2F4F7;
            color: var(--navy);
        }
        .pinned-section-header {
            font-weight: 700;
            font-size: 0.95rem;
            color: var(--orange);
            margin-bottom: 12px;
            display: flex;
            align-items: center;
            gap: 6px;
        }
    </style>
    <script type="text/javascript">
        function simulateShare(postId) {
            alert("Share link generated for post #" + postId + "! Link copied to clipboard.");
            return false;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-discussion">
        <div class="page-title">Discussion Zone</div>
        <div class="page-subtitle">Connect with olympiad trainees, trainers, and administrators from around the world.</div>

        <!-- TOPIC FILTERS (Feed Mode Only) -->
        <asp:PlaceHolder ID="phFilters" runat="server">
            <div class="disc-filters">
                <a href="Discussion.aspx" class="disc-filter-btn <%= string.IsNullOrEmpty(TopicFilter) && string.IsNullOrEmpty(SpecialFilter) ? "active" : "" %>">All</a>
                <a href="Discussion.aspx?topic=nt" class="disc-filter-btn <%= TopicFilter == "nt" ? "active" : "" %>">Number Theory</a>
                <a href="Discussion.aspx?topic=co" class="disc-filter-btn <%= TopicFilter == "co" ? "active" : "" %>">Combinatorics</a>
                <a href="Discussion.aspx?topic=ge" class="disc-filter-btn <%= TopicFilter == "ge" ? "active" : "" %>">Geometry</a>
                <a href="Discussion.aspx?topic=al" class="disc-filter-btn <%= TopicFilter == "al" ? "active" : "" %>">Algebra</a>
                <a href="Discussion.aspx?topic=iq" class="disc-filter-btn <%= TopicFilter == "iq" ? "active" : "" %>">Inequalities</a>
                
                <!-- Admin Flagged Filter -->
                <asp:HyperLink ID="lnkFlaggedFilter" runat="server" NavigateUrl="Discussion.aspx?filter=flagged" CssClass="disc-filter-btn" Visible="false">Flagged Posts</asp:HyperLink>
            </div>
        </asp:PlaceHolder>

        <div class="discussion-layout">
            
            <!-- MAIN FORUM COLUMN -->
            <div>
                <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom:15px; display:block;" />
                <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom:15px; display:block;" />

                <!-- 1. FEED VIEW -->
                <asp:PlaceHolder ID="phFeedView" runat="server">
                    
                    <!-- Admin Pin & Post Composer -->
                    <asp:PlaceHolder ID="phAdminComposer" runat="server" Visible="false">
                        <div class="disc-compose" style="border: 2px dashed var(--orange); background: #FFFDFB; margin-bottom: 24px;">
                            <div class="disc-compose-header">
                                <div class="disc-avatar orange" style="background: var(--orange);">📢</div>
                                <span style="font-size:0.85rem;font-weight:700;color:var(--orange)">Create Platform Announcement (Pinned)</span>
                            </div>
                            <asp:TextBox ID="txtAnnounceTitle" runat="server" placeholder="Announcement Title..." style="width: 100%; padding: 8px 12px; margin-bottom: 8px; border: 1.5px solid var(--border); border-radius: 8px; font-family:'DM Sans'; font-size:0.88rem;" />
                            <asp:TextBox ID="txtAnnounceContent" runat="server" TextMode="MultiLine" Rows="3" placeholder="Write the announcement description..." style="width: 100%; font-family:'DM Sans';" />
                            <div class="disc-compose-footer" style="justify-content: flex-end; margin-top: 8px;">
                                <asp:Button ID="btnAnnouncePost" runat="server" Text="Pin & Post" CssClass="btn-post" style="background: var(--orange);" OnClick="btnAnnouncePost_Click" />
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <!-- Normal Compose Box -->
                    <div class="disc-compose">
                        <div class="disc-compose-header">
                            <div class="disc-avatar orange"><%= GetInitials(CurrentUserFullName) %></div>
                            <span style="font-size:0.85rem;font-weight:500;color:var(--navy)">Share something with the community</span>
                        </div>
                        <asp:TextBox ID="txtPostTitle" runat="server" placeholder="Enter title / topic header..." style="width: 100%; padding: 8px 12px; margin-bottom: 8px; border: 1.5px solid var(--border); border-radius: 8px; font-family:'DM Sans'; font-size:0.88rem;" />
                        <asp:TextBox ID="txtPostContent" runat="server" TextMode="MultiLine" Rows="3" placeholder="Ask a question, share a trick, or discuss a problem..." />
                        
                        <div class="disc-compose-footer">
                            <asp:DropDownList ID="ddlPostTopic" runat="server" CssClass="disc-topic-select">
                                <asp:ListItem Value="nt">Number Theory</asp:ListItem>
                                <asp:ListItem Value="co">Combinatorics</asp:ListItem>
                                <asp:ListItem Value="ge">Geometry</asp:ListItem>
                                <asp:ListItem Value="al">Algebra</asp:ListItem>
                                <asp:ListItem Value="iq">Inequalities</asp:ListItem>
                            </asp:DropDownList>
                            <asp:Button ID="btnPost" runat="server" Text="Post" CssClass="btn-post" OnClick="btnPost_Click" />
                        </div>
                    </div>

                    <!-- Pinned Announcements Section -->
                    <asp:PlaceHolder ID="phPinnedAnnouncements" runat="server">
                        <div class="pinned-section-header">
                            <span>📌 Pinned Announcements</span>
                        </div>
                        <div style="margin-bottom: 24px;">
                            <asp:Repeater ID="rptPinned" runat="server" OnItemCommand="rptPinned_ItemCommand">
                                <ItemTemplate>
                                    <div class="disc-post" style="border-left: 4px solid var(--orange); background: #FFFDFB;">
                                        <div class="disc-post-header">
                                            <div class="disc-avatar orange" style="background: var(--orange);">📢</div>
                                            <div class="disc-post-meta" onclick="window.location='Discussion.aspx?postId=<%# Eval("Id") %>';" style="cursor:pointer;">
                                                <div class="disc-post-author" style="color: var(--orange); font-weight:700;"><%# Eval("FullName") %> <span style="color:var(--slate);font-weight:400">· Admin Staff</span></div>
                                                <div class="disc-post-time"><%# GetTimeAgo(Eval("CreatedAt")) %></div>
                                            </div>
                                            <span class="disc-topic-tag announcements" style="background: #FFF0EC; color: var(--orange);">Announcement</span>
                                        </div>
                                        <div class="disc-post-text" onclick="window.location='Discussion.aspx?postId=<%# Eval("Id") %>';" style="cursor:pointer;">
                                            <strong><%# HttpUtility.HtmlEncode(Eval("Title")) %></strong><br />
                                            <%# HttpUtility.HtmlEncode(Eval("Content")) %>
                                        </div>
                                        <div class="disc-post-footer" style="display:flex; justify-content: space-between; align-items:center;">
                                            <div>
                                                <asp:LinkButton ID="btnLike" runat="server" CommandName="ToggleLike" CommandArgument='<%# Eval("Id") %>' CssClass='<%# Convert.ToInt32(Eval("IsLikedByUser")) == 1 ? "disc-action liked" : "disc-action" %>'>
                                                    &#9825; <%# Eval("LikesCount") %> likes
                                                </asp:LinkButton>
                                                <a href='Discussion.aspx?postId=<%# Eval("Id") %>' class="disc-action">
                                                    &#9633; <%# Eval("ReplyCount") %> replies
                                                </a>
                                                <span class="disc-action btn-share" onclick="return simulateShare(<%# Eval("Id") %>);">&#10150; Share</span>
                                            </div>
                                            
                                            <!-- Admin Unpin Button -->
                                            <asp:LinkButton ID="btnUnpin" runat="server" CommandName="UnpinPost" CommandArgument='<%# Eval("Id") %>' CssClass="badge orange" style="border:none; cursor:pointer;" Visible='<%# Session["UserRole"] != null && string.Equals(Session["UserRole"].ToString(), "Admin", StringComparison.OrdinalIgnoreCase) %>'>
                                                Unpin
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:PlaceHolder>

                    <!-- Feed Header Label -->
                    <div style="font-weight: 700; font-size: 0.95rem; color: var(--navy); margin-bottom: 12px; margin-top: 10px;">
                        Community Feed
                    </div>

                    <!-- Post Feed -->
                    <div id="trainee-disc-feed">
                        <asp:Repeater ID="rptDiscussions" runat="server" OnItemCommand="rptDiscussions_ItemCommand">
                            <ItemTemplate>
                                <div class="disc-post" style='<%# Convert.ToInt32(Eval("IsFlagged")) == 1 ? "border-left: 4px solid var(--orange);" : "" %>'>
                                    <div class="disc-post-header">
                                        <div class="disc-avatar orange"><%# GetInitials(Eval("FullName").ToString()) %></div>
                                        <div class="disc-post-meta" onclick="window.location='Discussion.aspx?postId=<%# Eval("Id") %>';" style="cursor:pointer;">
                                            <div class="disc-post-author"><%# Eval("FullName") %> <span style="color:var(--slate);font-weight:400">· <%# Eval("CityCountry") %></span></div>
                                            <div class="disc-post-time"><%# GetTimeAgo(Eval("CreatedAt")) %></div>
                                        </div>
                                        <div>
                                            <span class='disc-topic-tag <%# Eval("Topic").ToString().ToLower() %>'><%# GetTopicLabel(Eval("Topic").ToString()) %></span>
                                            <!-- Needs Response Label -->
                                            <span class="badge needs-response" runat="server" visible='<%# Session["UserRole"] != null && string.Equals(Session["UserRole"].ToString(), "Trainer", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(Eval("ReplyCount")) == 0 %>'>Needs response</span>
                                            <!-- Flagged Badge for Admin -->
                                            <span class="badge flagged-badge" runat="server" visible='<%# Session["UserRole"] != null && string.Equals(Session["UserRole"].ToString(), "Admin", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(Eval("IsFlagged")) == 1 %>'>⚠️ Flagged</span>
                                        </div>
                                    </div>
                                    <div class="disc-post-text" onclick="window.location='Discussion.aspx?postId=<%# Eval("Id") %>';">
                                        <strong><%# HttpUtility.HtmlEncode(Eval("Title")) %></strong><br />
                                        <%# HttpUtility.HtmlEncode(Eval("Content")) %>
                                    </div>
                                    <div class="disc-post-footer" style="display:flex; flex-direction:column; gap:10px;">
                                        <div style="display:flex; justify-content:space-between; align-items:center;">
                                            <div>
                                                <asp:LinkButton ID="btnLike" runat="server" CommandName="ToggleLike" CommandArgument='<%# Eval("Id") %>' CssClass='<%# Convert.ToInt32(Eval("IsLikedByUser")) == 1 ? "disc-action liked" : "disc-action" %>'>
                                                    &#9825; <%# Eval("LikesCount") %> likes
                                                </asp:LinkButton>
                                                <a href='Discussion.aspx?postId=<%# Eval("Id") %>' class="disc-action">
                                                    &#9633; <%# Eval("ReplyCount") %> replies
                                                </a>
                                                <span class="disc-action btn-share" onclick="return simulateShare(<%# Eval("Id") %>);">&#10150; Share</span>
                                            </div>
                                            <div>
                                                <asp:LinkButton ID="btnFlag" runat="server" CommandName="FlagPost" CommandArgument='<%# Eval("Id") %>' CssClass="disc-action" Visible='<%# Convert.ToInt32(Eval("IsFlagged")) == 0 %>'>
                                                    🏳️ Flag Post
                                                </asp:LinkButton>
                                            </div>
                                        </div>

                                        <!-- Admin Actions Bar -->
                                        <div runat="server" visible='<%# Session["UserRole"] != null && string.Equals(Session["UserRole"].ToString(), "Admin", StringComparison.OrdinalIgnoreCase) %>' style="background: #F8F9FA; padding: 6px 10px; border-radius: 6px; display: flex; align-items: center;">
                                            <span style="font-size:0.72rem; color:var(--slate); font-weight:600; margin-right: 8px;">Moderation:</span>
                                            <asp:Button ID="btnAdminClear" runat="server" Text="Clear Flag" CommandName="AdminClearFlag" CommandArgument='<%# Eval("Id") %>' CssClass="admin-action-btn clear" Visible='<%# Convert.ToInt32(Eval("IsFlagged")) == 1 %>' />
                                            <asp:Button ID="btnAdminWarn" runat="server" Text="Warn User" CommandName="AdminWarnUser" CommandArgument='<%# Eval("UserId") %>' CssClass="admin-action-btn warn" />
                                            <asp:Button ID="btnAdminRemove" runat="server" Text="Remove Post" CommandName="AdminRemovePost" CommandArgument='<%# Eval("Id") %>' CssClass="admin-action-btn remove" OnClientClick="return confirm('Are you sure you want to permanently remove this post and all its replies?');" />
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="phNoPosts" runat="server" Visible="false">
                            <div class="content-card" style="text-align: center; padding: 40px; color: var(--slate);">
                                No discussion posts found. Be the first to post!
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </asp:PlaceHolder>

                <!-- 2. SINGLE THREAD VIEW -->
                <asp:PlaceHolder ID="phThreadView" runat="server" Visible="false">
                    <a href="Discussion.aspx" class="section-link" style="margin-bottom:15px; display:inline-block;">&larr; Back to Feed</a>
                    
                    <!-- Main Post Card -->
                    <div class="disc-post" style="cursor:default;">
                        <div class="disc-post-header">
                            <div class="disc-avatar orange"><asp:Literal ID="litThreadAvatar" runat="server" /></div>
                            <div class="disc-post-meta">
                                <div class="disc-post-author"><asp:Literal ID="litThreadAuthor" runat="server" /> <span style="color:var(--slate);font-weight:400">· <asp:Literal ID="litThreadLocation" runat="server" /></span></div>
                                <div class="disc-post-time"><asp:Literal ID="litThreadTime" runat="server" /></div>
                            </div>
                            <div>
                                <asp:Label ID="lblThreadTopicBadge" runat="server" />
                                <span id="lblThreadNeedsResponse" runat="server" class="badge needs-response" visible="false">Needs response</span>
                                <span id="lblThreadFlagged" runat="server" class="badge flagged-badge" visible="false">⚠️ Flagged</span>
                            </div>
                        </div>
                        <div class="disc-post-text">
                            <strong><asp:Literal ID="litThreadTitle" runat="server" /></strong><br />
                            <asp:Literal ID="litThreadContent" runat="server" />
                        </div>
                        <div class="disc-post-footer" style="display:flex; flex-direction:column; gap:10px;">
                            <div style="display:flex; justify-content:space-between; align-items:center;">
                                <div>
                                    <asp:LinkButton ID="btnThreadLike" runat="server" OnClick="btnThreadLike_Click" CssClass="disc-action">
                                        &#9825; <asp:Literal ID="litThreadLikes" runat="server" /> likes
                                    </asp:LinkButton>
                                    <span class="disc-action">&#9633; <asp:Literal ID="litThreadRepliesCount" runat="server" /> replies</span>
                                    <span class="disc-action btn-share" runat="server" id="spanThreadShare">&#10150; Share</span>
                                </div>
                                <div>
                                    <asp:LinkButton ID="lnkThreadFlag" runat="server" OnClick="btnThreadFlag_Click" CssClass="disc-action">
                                        🏳️ Flag Post
                                    </asp:LinkButton>
                                </div>
                            </div>
                            
                            <!-- Admin Actions Bar (Thread Details) -->
                            <div id="divThreadAdminActions" runat="server" visible="false" style="background: #F8F9FA; padding: 6px 10px; border-radius: 6px; display: flex; align-items: center; gap: 4px;">
                                <span style="font-size:0.72rem; color:var(--slate); font-weight:600; margin-right: 8px;">Moderation:</span>
                                <asp:Button ID="btnThreadAdminClear" runat="server" Text="Clear Flag" CssClass="admin-action-btn clear" OnClick="btnThreadAdminClear_Click" />
                                <asp:Button ID="btnThreadAdminWarn" runat="server" Text="Warn Author" CssClass="admin-action-btn warn" OnClick="btnThreadAdminWarn_Click" />
                                <asp:Button ID="btnThreadAdminRemove" runat="server" Text="Remove Post" CssClass="admin-action-btn remove" OnClick="btnThreadAdminRemove_Click" OnClientClick="return confirm('Are you sure you want to permanently remove this post?');" />
                            </div>
                        </div>
                    </div>

                    <!-- Replies List -->
                    <div class="content-card">
                        <h3>Replies</h3>
                        <div class="replies-section">
                            <asp:Repeater ID="rptReplies" runat="server">
                                <ItemTemplate>
                                    <div class="reply-item">
                                        <div class="disc-avatar navy" style="width:30px; height:30px; font-size:0.65rem;"><%# GetInitials(Eval("FullName").ToString()) %></div>
                                        <div class="reply-body">
                                            <div class="reply-author">
                                                <%# Eval("FullName") %> <span style="color:var(--slate); font-weight:400; font-size:0.72rem;">· <%# GetTimeAgo(Eval("CreatedAt")) %></span>
                                            </div>
                                            <div class="reply-text">
                                                <%# HttpUtility.HtmlEncode(Eval("Content")) %>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="phNoReplies" runat="server" Visible="false">
                                <p style="color:var(--slate); padding:10px 0;">No replies yet. Start the conversation!</p>
                            </asp:PlaceHolder>
                        </div>
                    </div>

                    <!-- Write Reply Box -->
                    <div class="disc-compose">
                        <div class="disc-compose-header">
                            <div class="disc-avatar orange"><%= GetInitials(CurrentUserFullName) %></div>
                            <span style="font-size:0.85rem;font-weight:500;color:var(--navy)">Write a reply</span>
                        </div>
                        <asp:TextBox ID="txtReplyContent" runat="server" TextMode="MultiLine" Rows="2" placeholder="Write your response to this topic..." />
                        <div class="disc-compose-footer" style="justify-content: flex-end;">
                            <asp:Button ID="btnSubmitReply" runat="server" Text="Reply" CssClass="btn-post" OnClick="btnSubmitReply_Click" />
                        </div>
                    </div>
                </asp:PlaceHolder>

            </div>

            <!-- SIDEBAR COLUMN -->
            <aside>
                
                <!-- 1. ADMIN ONLY PANEL: Moderation Queue -->
                <asp:PlaceHolder ID="phAdminModerationQueue" runat="server" Visible="false">
                    <div class="disc-sidebar-card" style="border: 1px solid rgba(255, 87, 51, 0.3); background: #FFFDFB;">
                        <h4 style="color: var(--orange); display:flex; align-items:center; justify-content:space-between;">
                            <span>Moderation Queue</span>
                            <span class="badge orange"><asp:Literal ID="litFlaggedCount" runat="server">0</asp:Literal></span>
                        </h4>
                        <p style="font-size:0.75rem; color:var(--slate); line-height:1.4; margin-bottom:8px;">
                            Flagged user threads pending audit. Keep the community safe.
                        </p>
                        <div class="moderation-progress-container">
                            <div style="font-size:0.72rem; font-weight:600; margin-bottom:4px; display:flex; justify-content:space-between;">
                                <span>Queue Backlog</span>
                                <span><asp:Literal ID="litQueuePercentage" runat="server">0</asp:Literal>% Clear</span>
                            </div>
                            <div class="moderation-progress-bar-wrap">
                                <div class="moderation-progress-bar" runat="server" id="divModerationProgressBar" style="width: 100%"></div>
                            </div>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <!-- 2. ADMIN ONLY PANEL: Quick Actions -->
                <asp:PlaceHolder ID="phAdminQuickActions" runat="server" Visible="false">
                    <div class="disc-sidebar-card">
                        <h4>Admin Quick Actions</h4>
                        <div style="display:flex; flex-direction:column; gap:8px;">
                            <asp:LinkButton ID="btnQuickBroadcast" runat="server" CssClass="badge grey" style="text-align:left; padding:8px 12px; display:block;" OnClick="btnQuickBroadcast_Click">
                                📢 Broadcast Announcement
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnQuickExport" runat="server" CssClass="badge grey" style="text-align:left; padding:8px 12px; display:block;" OnClick="btnQuickExport_Click">
                                📋 Export Post Log
                            </asp:LinkButton>
                            <a href="Discussion.aspx?filter=flagged" class="badge grey" style="text-align:left; padding:8px 12px; display:block; text-decoration:none;">
                                🛡️ Review All Flagged
                            </a>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <!-- 3. TRAINER ONLY PANEL: Unanswered Questions -->
                <asp:PlaceHolder ID="phTrainerUnanswered" runat="server" Visible="false">
                    <div class="disc-sidebar-card" style="border: 1px solid rgba(0, 201, 167, 0.2);">
                        <h4 style="color: var(--teal);">Unanswered Questions</h4>
                        <div class="unanswered-list" style="display:flex; flex-direction:column; gap:8px;">
                            <asp:Repeater ID="rptUnanswered" runat="server">
                                <ItemTemplate>
                                    <a href='Discussion.aspx?postId=<%# Eval("Id") %>' class="trending-item" style="padding:4px 0;">
                                        <div class="trending-text" style="font-size:0.78rem; font-weight:600;"><%# HttpUtility.HtmlEncode(Eval("Title")) %></div>
                                    </a>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="phNoUnanswered" runat="server" Visible="false">
                                <p style="font-size:0.75rem; color:var(--slate);">All questions answered! Great job.</p>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <!-- 4. TRAINER ONLY PANEL: Trainer Tips -->
                <asp:PlaceHolder ID="phTrainerTips" runat="server" Visible="false">
                    <div class="disc-sidebar-card">
                        <h4>Trainer Tips</h4>
                        <div style="font-size:0.78rem; color:var(--slate); line-height:1.5;">
                            <p style="margin-bottom:8px;">💡 <strong>Be Clear:</strong> Explain step-by-step without skipping algebra congruences.</p>
                            <p style="margin-bottom:8px;">💡 <strong>Link Modules:</strong> Recommend reading "Modular Arithmetic Fundamentals" or double-counting proofs.</p>
                            <p>💡 <strong>Encourage:</strong> Praise creative olympiad approaches even if incomplete!</p>
                        </div>
                    </div>
                </asp:PlaceHolder>

                <!-- 5. GENERAL: Online Now -->
                <div class="disc-sidebar-card">
                    <h4>Online Now</h4>
                    <div style="display:flex; flex-direction:column; gap:8px;">
                        <asp:Repeater ID="rptOnlineUsers" runat="server">
                            <ItemTemplate>
                                <div class="online-user">
                                    <div class="online-dot"></div>
                                    <div class="online-name"><%# HttpUtility.HtmlEncode(Eval("FullName")) %></div>
                                    <div class="online-role" style='<%# Eval("Role").ToString() == "Admin" ? "color: var(--orange);" : Eval("Role").ToString() == "Trainer" ? "color: var(--teal);" : "" %>'>
                                        <%# Eval("Role") %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <!-- 6. GENERAL: Community Stats (Becomes Moderation Stats for Admin!) -->
                <div class="disc-sidebar-card">
                    <h4><asp:Literal ID="litStatsTitle" runat="server">Community Stats</asp:Literal></h4>
                    <div style="display:grid; grid-template-columns: 1fr 1fr; gap:12px; margin-top:8px;">
                        <div style="background:var(--bg); padding:10px; border-radius:8px; text-align:center;">
                            <div style="font-size:1.1rem; font-weight:700; color:var(--navy);"><asp:Literal ID="litStatMembers" runat="server">0</asp:Literal></div>
                            <div style="font-size:0.68rem; color:var(--slate); font-weight:600; text-transform:uppercase; margin-top:2px;">Members</div>
                        </div>
                        <div style="background:var(--bg); padding:10px; border-radius:8px; text-align:center;">
                            <div style="font-size:1.1rem; font-weight:700; color:var(--teal);"><asp:Literal ID="litStatOnline" runat="server">0</asp:Literal></div>
                            <div style="font-size:0.68rem; color:var(--slate); font-weight:600; text-transform:uppercase; margin-top:2px;">Online</div>
                        </div>
                        <div style="background:var(--bg); padding:10px; border-radius:8px; text-align:center;">
                            <div style="font-size:1.1rem; font-weight:700; color:var(--navy);"><asp:Literal ID="litStatPosts" runat="server">0</asp:Literal></div>
                            <div style="font-size:0.68rem; color:var(--slate); font-weight:600; text-transform:uppercase; margin-top:2px;">Posts</div>
                        </div>
                        <div style="background:var(--bg); padding:10px; border-radius:8px; text-align:center;">
                            <div style="font-size:1.1rem; font-weight:700; color:var(--orange);"><asp:Literal ID="litStatCountries" runat="server">0</asp:Literal></div>
                            <div style="font-size:0.68rem; color:var(--slate); font-weight:600; text-transform:uppercase; margin-top:2px;">
                                <asp:Literal ID="litStatCountryLabel" runat="server">Countries</asp:Literal>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 7. GENERAL: Trending Topics -->
                <div class="disc-sidebar-card">
                    <h4>Trending Threads</h4>
                    <asp:Repeater ID="rptTrending" runat="server">
                        <ItemTemplate>
                            <a href='Discussion.aspx?postId=<%# Eval("Id") %>' class="trending-item">
                                <div class="trending-num">0<%# Container.ItemIndex + 1 %></div>
                                <div>
                                    <div class="trending-text"><%# HttpUtility.HtmlEncode(Eval("Title")) %></div>
                                    <div class="trending-count"><%# Eval("LikesCount") %> likes · <%# Eval("ReplyCount") %> replies</div>
                                </div>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </aside>

        </div>
    </div>
</asp:Content>
