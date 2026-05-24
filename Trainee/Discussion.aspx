<%@ Page Title="OlyMath - Discussion Zone" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Discussion.aspx.cs" Inherits="OlyMath.Trainee.Discussion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-discussion">
        <div class="page-title">Discussion Zone</div>
        <div class="page-subtitle">Connect with olympiad trainees and trainers from around the world.</div>

        <!-- TOPIC FILTERS (Feed Mode Only) -->
        <asp:PlaceHolder ID="phFilters" runat="server">
            <div class="disc-filters">
                <a href="Discussion.aspx" class="disc-filter-btn <%= string.IsNullOrEmpty(TopicFilter) ? "active" : "" %>">All Topics</a>
                <a href="Discussion.aspx?topic=nt" class="disc-filter-btn <%= TopicFilter == "nt" ? "active" : "" %>">Number Theory</a>
                <a href="Discussion.aspx?topic=co" class="disc-filter-btn <%= TopicFilter == "co" ? "active" : "" %>">Combinatorics</a>
                <a href="Discussion.aspx?topic=ge" class="disc-filter-btn <%= TopicFilter == "ge" ? "active" : "" %>">Geometry</a>
                <a href="Discussion.aspx?topic=al" class="disc-filter-btn <%= TopicFilter == "al" ? "active" : "" %>">Algebra</a>
                <a href="Discussion.aspx?topic=iq" class="disc-filter-btn <%= TopicFilter == "iq" ? "active" : "" %>">Inequalities</a>
            </div>
        </asp:PlaceHolder>

        <div class="discussion-layout">
            
            <!-- MAIN FORUM COLUMN -->
            <div>
                <asp:Label ID="lblMessage" runat="server" CssClass="form-error" Visible="false" style="margin-bottom:15px; display:block;" />

                <!-- 1. FEED VIEW -->
                <asp:PlaceHolder ID="phFeedView" runat="server">
                    <!-- Compose Box -->
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

                    <!-- Post Feed -->
                    <div id="trainee-disc-feed">
                        <asp:Repeater ID="rptDiscussions" runat="server" OnItemCommand="rptDiscussions_ItemCommand">
                            <ItemTemplate>
                                <div class="disc-post">
                                    <div class="disc-post-header" onclick="window.location='Discussion.aspx?postId=<%# Eval("Id") %>';">
                                        <div class="disc-avatar orange"><%# GetInitials(Eval("FullName").ToString()) %></div>
                                        <div class="disc-post-meta">
                                            <div class="disc-post-author"><%# Eval("FullName") %> <span style="color:var(--slate);font-weight:400">· <%# Eval("CityCountry") %></span></div>
                                            <div class="disc-post-time"><%# GetTimeAgo(Eval("CreatedAt")) %></div>
                                        </div>
                                        <span class='disc-topic-tag <%# Eval("Topic").ToString().ToLower() %>'><%# GetTopicLabel(Eval("Topic").ToString()) %></span>
                                    </div>
                                    <div class="disc-post-text" onclick="window.location='Discussion.aspx?postId=<%# Eval("Id") %>';">
                                        <strong><%# HttpUtility.HtmlEncode(Eval("Title")) %></strong><br />
                                        <%# HttpUtility.HtmlEncode(Eval("Content")) %>
                                    </div>
                                    <div class="disc-post-footer">
                                        <asp:LinkButton ID="btnLike" runat="server" CommandName="ToggleLike" CommandArgument='<%# Eval("Id") %>' CssClass='<%# Convert.ToInt32(Eval("IsLikedByUser")) == 1 ? "disc-action liked" : "disc-action" %>'>
                                            &#9825; <%# Eval("LikesCount") %> likes
                                        </asp:LinkButton>
                                        <a href='Discussion.aspx?postId=<%# Eval("Id") %>' class="disc-action">
                                            &#9633; <%# Eval("ReplyCount") %> replies
                                        </a>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="phNoPosts" runat="server" Visible="false">
                            <div class="content-card" style="text-align: center; padding: 40px; color: var(--slate);">
                                No discussion posts found under this topic. Be the first to post!
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
                            <asp:Label ID="lblThreadTopicBadge" runat="server" />
                        </div>
                        <div class="disc-post-text">
                            <strong><asp:Literal ID="litThreadTitle" runat="server" /></strong><br />
                            <asp:Literal ID="litThreadContent" runat="server" />
                        </div>
                        <div class="disc-post-footer">
                            <asp:LinkButton ID="btnThreadLike" runat="server" OnClick="btnThreadLike_Click" CssClass="disc-action">
                                &#9825; <asp:Literal ID="litThreadLikes" runat="server" /> likes
                            </asp:LinkButton>
                            <span class="disc-action">&#9633; <asp:Literal ID="litThreadRepliesCount" runat="server" /> replies</span>
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
                <!-- Online Trainees -->
                <div class="disc-sidebar-card">
                    <h4>Online Users</h4>
                    <div class="online-user">
                        <div class="online-dot"></div>
                        <div class="online-name">Alexandra M. Reyes</div>
                        <div class="online-role">Trainee</div>
                    </div>
                    <div class="online-user">
                        <div class="online-dot"></div>
                        <div class="online-name">Dr. Budi Santoso</div>
                        <div class="online-role">Admin</div>
                    </div>
                    <div class="online-user">
                        <div class="online-dot"></div>
                        <div class="online-name">Kenji Aoki</div>
                        <div class="online-role">Trainer</div>
                    </div>
                </div>

                <!-- Trending Topics -->
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
