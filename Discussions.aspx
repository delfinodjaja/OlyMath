<%@ Page Title="Discussion Zone" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Discussions.aspx.cs" Inherits="OlyMath.Discussions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Discussion Zone
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="discussion.css">
    <script type="text/javascript">
        function confirmDelete() {
            return confirm("Are you sure you want to delete this discussion? This cannot be undone.");
        }
        function validateNewPost() {
            var title = document.getElementById('<%= txtComposeTitle.ClientID %>').value;
            var content = document.getElementById('<%= txtDiscussion.ClientID %>').value;
            if (title.length > 80) {
                alert("Title cannot exceed 80 characters.");
                return false;
            }
            if (content.length > 2000) {
                alert("Content cannot exceed 2000 characters.");
                return false;
            }
            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <main class="main-content">
        <div class="page-title">Discussion Zone</div>
        <div class="page-subtitle">Engage with trainees worldwide — answer questions, share insights, spark discussion.</div>

        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px;">
            <div class="disc-filters" style="margin-bottom: 0;">
                <asp:Button ID="btnAll" runat="server" Text="All Topics" CssClass="disc-filter-btn active" OnClick="btnFilter_Click" CommandArgument="All" />
                <asp:Button ID="btnNum" runat="server" Text="Number Theory" CssClass="disc-filter-btn" OnClick="btnFilter_Click" CommandArgument="Number Theory" />
                <asp:Button ID="btnCom" runat="server" Text="Combinatorics" CssClass="disc-filter-btn" OnClick="btnFilter_Click" CommandArgument="Combinatorics" />
                <asp:Button ID="btnGeo" runat="server" Text="Geometry" CssClass="disc-filter-btn" OnClick="btnFilter_Click" CommandArgument="Geometry" />
                <asp:Button ID="btnAlg" runat="server" Text="Algebra" CssClass="disc-filter-btn" OnClick="btnFilter_Click" CommandArgument="Algebra" />
                <asp:Button ID="btnFlag" runat="server" Text="⚑ Flagged" CssClass="disc-filter-btn" OnClick="btnFilter_Click" CommandArgument="Flagged" />
            </div>
            <div style="display: flex; align-items: center; gap: 10px;">
                <span style="font-size: 0.8rem; color: var(--slate); font-weight: 500;">Sort by:</span>
                <asp:DropDownList ID="ddlSort" runat="server" CssClass="disc-filter-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">
                    <asp:ListItem Text="Newest First" Value="Newest" />
                    <asp:ListItem Text="Most Popular" Value="Popularity" />
                    <asp:ListItem Text="Unanswered" Value="Unanswered" />
                </asp:DropDownList>
            </div>
        </div>

        <!-- Compose new discussion -->
        <div class="disc-compose">
            <div class="disc-compose-header">
                <!-- <div class="disc-avatar teal">HR</div> -->
                <span style="font-size:0.85rem;font-weight:500;color:var(--navy)">Share expertise with the community</span>
            </div>
            <asp:TextBox ID="txtComposeTitle" runat="server" CssClass="disc-compose-textarea" 
                PlaceHolder="Enter discussion title (max 80 chars)" 
                style="font-size: 1rem; font-weight: 700; color: var(--navy); margin-bottom: 12px; min-height: 48px;"></asp:TextBox>
            <asp:TextBox ID="txtDiscussion" runat="server" TextMode="MultiLine" CssClass="disc-compose-textarea auto-expand" 
                PlaceHolder="Post a tip, answer a question, or share a resource... (max 2000 chars)"></asp:TextBox>
            <div style="font-size: 0.75rem; color: var(--slate); margin-top: 8px; text-align: right; font-weight: 500;">
                *Title ≤ 80 chars, Content ≤ 2000 chars.
            </div>
            <div class="disc-compose-footer">
                <asp:DropDownList ID="ddlComposeTopic" runat="server" CssClass="disc-filter-select">
                    <asp:ListItem Value="Number Theory">Number Theory</asp:ListItem>
                    <asp:ListItem Value="Combinatorics">Combinatorics</asp:ListItem>
                    <asp:ListItem Value="Geometry">Geometry</asp:ListItem>
                    <asp:ListItem Value="Algebra">Algebra</asp:ListItem>
                    <asp:ListItem Value="Inequalities">Inequalities</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnPost" runat="server" Text="Post" CssClass="disc-filter-btn active" OnClick="btnPost_Click" OnClientClick="return validateNewPost();" />
            </div>
        </div>

        <!-- Repeater for discussions -->
        <asp:Repeater ID="rptDiscussions" runat="server" OnItemCommand="rptDiscussions_ItemCommand" OnItemDataBound="rptDiscussions_ItemDataBound">
            <ItemTemplate>
                <div class="disc-post" style="cursor: pointer;" onclick="window.location.href='DiscussionDetail.aspx?id=<%# Eval("DiscussionId") %>';">
                    <div class="disc-post-header" style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px;">
                        <div style="display: flex; align-items: center; gap: 12px;">
                            <div class="disc-avatar teal"><%# Eval("UserName").ToString().Substring(0,2).ToUpper() %></div>
                            <div style="display: flex; flex-direction: column;">
                                <span style="font-size: 0.85rem; font-weight: 600; color: var(--navy)"><%# Eval("UserName") %></span>
                                <span style="font-size: 0.65rem; color: var(--slate)"><%# Convert.ToDateTime(Eval("CreatedAt")).ToString("MMM dd, yyyy") %></span>
                            </div>
                        </div>
                        <div style="display: flex; gap: 8px;">
                            <span class="disc-topic-tag" style="background: #F0F4F8; color: var(--slate); padding: 4px 8px; border-radius: 6px; font-size: 0.7rem;"><%# Eval("Topic") %></span>
                            <asp:PlaceHolder runat="server" Visible='<%# Convert.ToBoolean(Eval("IsFlagged")) %>'>
                                <span class="disc-topic-tag" style="background: #FFF0EC; color: var(--orange); padding: 4px 8px; border-radius: 6px; font-size: 0.7rem; font-weight: 600;">⚑ Flagged</span>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                    <h3 style="font-family: 'Syne', sans-serif; font-size: 1.15rem; font-weight: 700; color: var(--navy); margin-bottom: 8px;">
                        <%# Eval("Title") %>
                    </h3>
                    <div class="disc-post-text" style="font-size: 0.88rem; color: var(--navy); margin-bottom: 16px; line-height: 1.5;">
                        <%# Eval("Content").ToString().Length > 200 ? Eval("Content").ToString().Substring(0,200) + "..." : Eval("Content") %>
                    </div>
                    <div class="disc-post-footer" style="display: flex; justify-content: space-between; align-items: center; border-top: 1px solid var(--border); padding-top: 12px;">
                        <div style="display: flex; gap: 16px; align-items: center;">
                            <asp:LinkButton ID="btnLikePost" runat="server" CommandName="Like" CommandArgument='<%# Eval("DiscussionId") %>' CssClass="disc-action-link">♡ <%# Eval("Like") %> Likes</asp:LinkButton>
                            <span style="font-size: 0.75rem; font-weight: 600; color: var(--slate);">💬 <%# Eval("ReplyCount") %> Replies</span>
                            <asp:LinkButton ID="btnReplyPost" runat="server" CommandName="Reply" CommandArgument='<%# Eval("DiscussionId") %>' CssClass="disc-action-link">Reply</asp:LinkButton>
                            <asp:LinkButton ID="btnFlagPost" runat="server" CommandName="Flag" CommandArgument='<%# Eval("DiscussionId") %>' CssClass="disc-action-link">⚑ Flag</asp:LinkButton>
                        </div>
                        <div onclick="event.stopPropagation();" style="display: flex; gap: 14px; align-items: center;">
                            <asp:LinkButton ID="btnEditPost" runat="server" CommandName="Edit" CommandArgument='<%# Eval("DiscussionId") %>' CssClass="disc-action-link">✎ Edit</asp:LinkButton>
                            <asp:LinkButton ID="btnDeletePost" runat="server" CommandName="Delete" CommandArgument='<%# Eval("DiscussionId") %>' CssClass="disc-action-link delete" OnClientClick="return confirmDelete();">✕ Delete</asp:LinkButton>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Pagination placeholders (optional) -->
        <!-- <div class="disc-pagination">
            <button class="page-btn disabled">&lt;</button>
            <button class="page-btn active">1</button>
            <button class="page-btn">2</button>
            <button class="page-btn">3</button>
            <span class="page-dots">...</span>
            <button class="page-btn">12</button>
            <button class="page-btn">&gt;</button>
        </div> -->
    </main>

    <script>
        document.addEventListener('input', function (event) {
            if (event.target.classList && event.target.classList.contains('auto-expand')) {
                event.target.style.height = 'auto';
                event.target.style.height = (event.target.scrollHeight) + 'px';
            }
        });
    </script>
</asp:Content>