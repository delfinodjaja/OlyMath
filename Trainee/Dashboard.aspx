<%@ Page Title="OlyMath - Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="OlyMath.Trainee.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-home">
        <div class="page-title">Good morning, <asp:Literal ID="litTraineeName" runat="server" />.</div>
        <div class="page-subtitle">Here is what is happening with your training today.</div>
        
        <!-- STATISTICS GRID -->
        <div class="stat-grid">
            <div class="stat-card">
                <div class="stat-val"><asp:Literal ID="litActiveModulesCount" runat="server">0</asp:Literal></div>
                <div class="stat-label">Active Modules</div>
            </div>
            <div class="stat-card">
                <div class="stat-val stat-teal"><asp:Literal ID="litAvgProgress" runat="server">0</asp:Literal>%</div>
                <div class="stat-label">Avg. Progress</div>
            </div>
            <div class="stat-card">
                <div class="stat-val stat-accent"><asp:Literal ID="litCertsCount" runat="server">0</asp:Literal></div>
                <div class="stat-label">Certificates Earned</div>
            </div>
            <div class="stat-card">
                <div class="stat-val"><asp:Literal ID="litAssessmentsCount" runat="server">0</asp:Literal></div>
                <div class="stat-label">Assessments Done</div>
            </div>
        </div>

        <!-- CONTINUE LEARNING -->
        <div class="section-head">
            <div class="section-title">Continue Learning</div>
            <a class="section-link" href="BrowseModules.aspx">View all</a>
        </div>
        
        <div class="module-grid">
            <asp:Repeater ID="rptEnrolledModules" runat="server">
                <ItemTemplate>
                    <a href='ModuleDetail.aspx?id=<%# Eval("Id") %>' class='module-card <%# GetTopicClass(Eval("Topic").ToString()) %>'>
                        <div class='module-tag <%# GetTopicClass(Eval("Topic").ToString()) %>'><%# Eval("Topic") %></div>
                        <h4><%# Eval("Title") %></h4>
                        <div class="module-meta">
                            <span><%# Eval("MaterialCount") %> materials</span>
                            <span><%# Eval("EstimatedTime") %></span>
                        </div>
                        <div class="progress-bar-wrap">
                            <div class="progress-bar" style='width:<%# Eval("ProgressPercentage") %>%'></div>
                        </div>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="phNoModules" runat="server" Visible="false">
                <div class="content-card" style="grid-column: 1 / -1; text-align: center; padding: 40px;">
                    <p style="color:var(--slate); margin-bottom: 20px;">You are not currently enrolled in any modules.</p>
                    <a href="BrowseModules.aspx" class="btn-primary">Browse & Enroll Now</a>
                </div>
            </asp:PlaceHolder>
        </div>

        <!-- LATEST DISCUSSIONS -->
        <div class="section-head">
            <div class="section-title">Latest Discussions</div>
            <a class="section-link" href="Discussion.aspx">Join the zone</a>
        </div>

        <asp:Repeater ID="rptLatestDiscussions" runat="server">
            <ItemTemplate>
                <a href='Discussion.aspx?postId=<%# Eval("Id") %>' class="disc-post" style="margin-bottom:10px;">
                    <div class="disc-post-header">
                        <div class="disc-avatar orange"><%# GetInitials(Eval("FullName").ToString()) %></div>
                        <div class="disc-post-meta">
                            <div class="disc-post-author"><%# Eval("FullName") %> <span style="color:var(--slate);font-weight:400">· <%# Eval("CityCountry") %></span></div>
                            <div class="disc-post-time"><%# GetTimeAgo(Eval("CreatedAt")) %></div>
                        </div>
                        <span class='disc-topic-tag <%# Eval("Topic").ToString().ToLower() %>'><%# GetTopicLabel(Eval("Topic").ToString()) %></span>
                    </div>
                    <div class="disc-post-text"><strong><%# Eval("Title") %></strong><br /><%# Eval("Content") %></div>
                    <div class="disc-post-footer">
                        <span class="disc-action">&#9825; <%# Eval("LikesCount") %> likes</span>
                        <span class="disc-action">&#9633; <%# Eval("ReplyCount") %> replies</span>
                    </div>
                </a>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
