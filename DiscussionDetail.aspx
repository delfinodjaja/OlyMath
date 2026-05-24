<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DiscussionDetail.aspx.cs" Inherits="OlyMath.DiscussionDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="discussion.css">
    <script type="text/javascript">
        function confirmDelete() {
            return confirm("Are you sure you want to delete this discussion?");
        }
        function confirmDeleteReply() {
            return confirm("Delete this reply?");
        }
        function validateEditReply(btn) {
            var txt = btn.parentElement.previousElementSibling;
            if (txt && txt.value.length > 2000) {
                alert("Reply cannot exceed 2000 characters.");
                return false;
            }
            return true;
        }
        function validateTitleAndContent() {
            var title = document.getElementById('<%= txtEditTitle.ClientID %>').value;
            var content = document.getElementById('<%= txtEditContent.ClientID %>').value;
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
        function validateNewReply() {
            var reply = document.getElementById('<%= txtNewReply.ClientID %>').value;
            if (reply.length > 2000) {
                alert("Reply cannot exceed 2000 characters.");
                return false;
            }
            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <main class="main-content">
        
        <a href="Discussions.aspx" class="back-link">← Back to Discussions</a>

        <!-- VIEW MODE PANEL -->
        <asp:Panel ID="pnlViewMode" runat="server">
            <div class="detail-main-post">
                <div style="display: flex; justify-content: space-between; align-items: flex-start;">
                    <div style="display: flex; gap: 12px; align-items: center;">
                        <div class="disc-avatar orange"><asp:Literal ID="litAvatarInitials" runat="server"></asp:Literal></div>
                        <div style="display: flex; flex-direction: column;">
                            <asp:Label ID="lblAuthor" runat="server" style="font-size: 0.85rem; font-weight: 600; color: var(--navy)"></asp:Label>
                            <asp:Label ID="lblCreatedAt" runat="server" style="font-size: 0.75rem; color: var(--slate)"></asp:Label>
                        </div>
                    </div>
                    <div style="display: flex; gap: 8px;">
                        <asp:Panel ID="pnlFlaggedBadge" runat="server" Visible="false">
                            <span style="font-size: 0.75rem; font-weight: 600; color: var(--orange); background: #FFF0EC; padding: 4px 10px; border-radius: 100px;">⚑ Flagged</span>
                        </asp:Panel>
                        <asp:Label ID="lblTopic" runat="server" style="font-size: 0.75rem; font-weight: 600; color: #8A9BB8; background: #F0F4F8; padding: 4px 10px; border-radius: 100px;"></asp:Label>
                    </div>
                </div>

                <h1 style="font-family: 'Syne', sans-serif; font-size: 1.6rem; font-weight: 800; color: var(--navy); margin-top: 20px; margin-bottom: 8px; letter-spacing: -0.5px;">
                    <asp:Label ID="lblTitle" runat="server"></asp:Label>
                </h1>

                <div class="detail-text" style="margin-top: 8px;">
                    <asp:Label ID="lblMainContent" runat="server"></asp:Label>
                </div>

                <div style="display: flex; justify-content: space-between; align-items: center; margin-top: 16px;">
                    <asp:LinkButton ID="btnLikeMain" runat="server" CssClass="disc-action-link" OnClick="btnLikeMain_Click">♡ 0 Likes</asp:LinkButton>
                    <div style="display: flex; gap: 16px;">
                        <asp:LinkButton ID="btnFlagMain" runat="server" CssClass="disc-action-link" OnClick="btnFlagMain_Click" Visible="false">⚑ Flag</asp:LinkButton>
                        <asp:LinkButton ID="btnEdit" runat="server" CssClass="disc-action-link" OnClick="btnEdit_Click" Visible="false">✎ Edit</asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="disc-action-link delete" OnClick="btnDelete_Click" OnClientClick="return confirmDelete();" Visible="false">✕ Delete</asp:LinkButton>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- EDIT MODE PANEL -->
        <asp:Panel ID="pnlEditMode" runat="server" Visible="false">
            <div class="detail-main-post">
                <div style="margin-bottom: 16px;">
                    <label style="font-weight:600;">Title (max 80 chars)</label>
                    <asp:TextBox ID="txtEditTitle" runat="server" CssClass="disc-compose-textarea" style="width:100%; margin-top:4px;"></asp:TextBox>
                </div>
                <div style="margin-bottom: 16px;">
                    <label style="font-weight:600;">Topic</label>
                    <asp:DropDownList ID="ddlEditTopic" runat="server" CssClass="disc-filter-select">
                        <asp:ListItem>Number Theory</asp:ListItem>
                        <asp:ListItem>Combinatorics</asp:ListItem>
                        <asp:ListItem>Geometry</asp:ListItem>
                        <asp:ListItem>Algebra</asp:ListItem>
                        <asp:ListItem>Inequalities</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div style="margin-bottom: 16px;">
                    <label style="font-weight:600;">Content (max 2000 chars)</label>
                    <asp:TextBox ID="txtEditContent" runat="server" TextMode="MultiLine" Rows="10" CssClass="disc-compose-textarea" style="width:100%;"></asp:TextBox>
                </div>
                <div style="display:flex; gap:12px; justify-content:flex-end;">
                    <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="disc-filter-btn" OnClick="btnCancelEdit_Click" />
                    <asp:Button ID="btnSaveEdit" runat="server" Text="Save Changes" CssClass="disc-filter-btn active" OnClick="btnSaveEdit_Click" OnClientClick="return validateTitleAndContent();" />
                </div>
            </div>
        </asp:Panel>

        <!-- COMMENT (NEW REPLY) SECTION -->
        <div class="comment-card">
            <span style="font-size: 0.85rem; font-weight: 600; color: var(--navy);">Commenting as <asp:Label ID="lblCommentingAs" runat="server" style="font-weight:normal;"></asp:Label></span>
            <asp:TextBox ID="txtNewReply" runat="server" TextMode="MultiLine" CssClass="comment-textarea" PlaceHolder="What are your thoughts? (max 2000 characters)"></asp:TextBox>
            <div style="font-size: 0.75rem; color: var(--slate); margin-top: -8px; margin-bottom: 16px; text-align: right; font-weight: 500;">
                *Content cannot be more than 2000 characters.
            </div>
            <div style="overflow: hidden;"> 
                <asp:Button ID="btnPostReply" runat="server" Text="Post Reply" CssClass="btn-dark" OnClick="btnPostReply_Click" OnClientClick="return validateNewReply();" />
            </div>
        </div>

        <!-- REPLIES HEADER -->
        <div class="replies-header"><asp:Label ID="lblReplyCount" runat="server">0 Replies</asp:Label></div>

        <!-- REPEATER FOR REPLIES (generates the same HTML as your static design) -->
        <asp:Repeater ID="rptReplies" runat="server" OnItemDataBound="rptReplies_ItemDataBound" OnItemCommand="rptReplies_ItemCommand">
            <ItemTemplate>
                <div class="reply-item">
                    <div class="disc-avatar teal"><%# Eval("UserName").ToString().Substring(0,2).ToUpper() %></div>
                    <div style="flex: 1;">
                        <!-- VIEW MODE -->
                        <asp:Panel ID="pnlReplyView" runat="server">
                            <div style="margin-bottom: 6px;">
                                <span style="font-size: 0.85rem; font-weight: 600; color: var(--navy); margin-right: 8px;"><%# Eval("UserName") %></span>
                                <span style="font-size: 0.75rem; color: var(--slate);">· <%# Convert.ToDateTime(Eval("CreatedAt")).ToString("MMM dd, yyyy HH:mm") %></span>
                                <asp:PlaceHolder runat="server" Visible='<%# Convert.ToBoolean(Eval("Edited")) %>'>
                                    <span style="font-size: 0.7rem; color: #999;"> (edited)</span>
                                </asp:PlaceHolder>
                            </div>
                            <div style="font-size: 0.9rem; color: var(--navy); line-height: 1.5; margin-bottom: 12px;">
                                <%# Eval("Content") %>
                            </div>
                            <div style="display: flex; gap: 16px;">
                                <asp:LinkButton ID="btnEditReply" runat="server" CommandName="EditReply" CommandArgument='<%# Eval("PostId") %>' CssClass="disc-action-link" style="color: #6366F1;">✎ Edit</asp:LinkButton>
                                <asp:LinkButton ID="btnDeleteReply" runat="server" CommandName="DeleteReply" CommandArgument='<%# Eval("PostId") %>' CssClass="disc-action-link delete" style="color: #6366F1;" OnClientClick="return confirmDeleteReply();">✕ Delete</asp:LinkButton>
                            </div>
                        </asp:Panel>
                        <!-- EDIT MODE (inline) -->
                        <asp:Panel ID="pnlReplyEdit" runat="server" Visible="false">
                            <asp:TextBox ID="txtReplyEdit" runat="server" TextMode="MultiLine" Rows="4" CssClass="comment-textarea" style="width:100%; margin-bottom:8px;"></asp:TextBox>
                            <div style="display:flex; gap:8px; justify-content:flex-end;">
                                <asp:LinkButton ID="btnCancelReplyEdit" runat="server" CommandName="CancelReplyEdit" CommandArgument='<%# Eval("PostId") %>' CssClass="disc-filter-btn">Cancel</asp:LinkButton>
                                <asp:LinkButton ID="btnSaveReply" runat="server" CommandName="SaveReply" CommandArgument='<%# Eval("PostId") %>' CssClass="disc-filter-btn active" OnClientClick="return validateEditReply(this);">Save</asp:LinkButton>
                            </div>
                        </asp:Panel>
                        <asp:HiddenField ID="hfPostId" runat="server" Value='<%# Eval("PostId") %>' />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </main>

    <script>
        // auto-expand for textareas (keep your existing script)
        document.addEventListener('input', function (event) {
            if (event.target.classList && event.target.classList.contains('auto-expand')) {
                event.target.style.height = 'auto';
                event.target.style.height = (event.target.scrollHeight) + 'px';
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>