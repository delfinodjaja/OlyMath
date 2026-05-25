<%@ Page Title="OlyMath - Trainer Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="OlyMath.Trainer.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainer-dashboard" class="fadeInUp">
        <div class="page-title">Good morning, <asp:Literal ID="litTrainerName" runat="server" />.</div>
        <div class="page-subtitle">Track trainee achievements, active enrollments, and coordinate olympiad study modules.</div>
        
        <!-- STATISTICS GRID -->
        <div class="stat-grid" style="margin-bottom: 30px;">
            <div class="stat-card">
                <div class="stat-val"><asp:Literal ID="litActiveModulesCount" runat="server">0</asp:Literal></div>
                <div class="stat-label">Active Modules</div>
            </div>
            <div class="stat-card">
                <div class="stat-val stat-teal"><asp:Literal ID="litEnrolledTraineesCount" runat="server">0</asp:Literal></div>
                <div class="stat-label">Enrolled Trainees</div>
            </div>
            <div class="stat-card">
                <div class="stat-val stat-accent"><asp:Literal ID="litAssessmentsCount" runat="server">0</asp:Literal></div>
                <div class="stat-label">Assessments Submitted</div>
            </div>
            <div class="stat-card">
                <div class="stat-val"><asp:Literal ID="litPendingReviewsCount" runat="server">0</asp:Literal></div>
                <div class="stat-label">Pending Reviews</div>
            </div>
        </div>

        <div style="display: grid; grid-template-columns: 1.5fr 1fr; gap: 30px; align-items: start;">
            
            <!-- RECENT ACTIVITY -->
            <div>
                <div class="section-head">
                    <div class="section-title">Recent Activity</div>
                </div>
                
                <asp:Repeater ID="rptRecentActivity" runat="server">
                    <ItemTemplate>
                        <div class="list-item" style="margin-bottom: 12px; background: var(--bg); border: 1.5px solid var(--border); border-radius: 12px;">
                            <div class='list-item-icon <%# Eval("ActivityType").ToString() == "Submission" ? "orange" : "teal" %>'>
                                <%# Eval("ActivityType").ToString() == "Submission" ? "SUB" : "ENR" %>
                            </div>
                            <div class="list-item-body">
                                <div class="list-item-title" style="font-size: 0.88rem; color: var(--navy);">
                                    <strong><%# HttpUtility.HtmlEncode(Eval("FullName")) %></strong> 
                                    <%# Eval("ActivityType").ToString() == "Submission" ? "submitted assessment for" : "enrolled in" %> 
                                    <strong><%# HttpUtility.HtmlEncode(Eval("ModuleTitle")) %></strong>
                                </div>
                                <div class="list-item-sub" style="font-size: 0.78rem; margin-top: 4px;">
                                    <%# Eval("ActivityType").ToString() == "Submission" ? ("Score achieved: " + Eval("Score") + "%") : "Trainee registered" %> 
                                    · <%# GetTimeAgo(Eval("ActivityDate")) %>
                                </div>
                            </div>
                            <span class='list-item-badge <%# Eval("ActivityType").ToString() == "Submission" ? "pending" : "active" %>'>
                                <%# Eval("ActivityType").ToString() == "Submission" ? "Review" : "Active" %>
                            </span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="phNoActivity" runat="server" Visible="false">
                    <div class="content-card" style="text-align: center; padding: 40px; border-radius: 12px;">
                        <p style="color:var(--slate);">No recent trainee activity found for your modules.</p>
                    </div>
                </asp:PlaceHolder>
            </div>

            <!-- LATEST DISCUSSIONS PREVIEW -->
            <div>
                <div class="section-head">
                    <div class="section-title">Latest Discussions</div>
                    <a class="section-link" href="../Trainee/Discussion.aspx">Respond to trainees</a>
                </div>

                <asp:Repeater ID="rptLatestDiscussions" runat="server">
                    <ItemTemplate>
                        <a href='../Trainee/Discussion.aspx?postId=<%# Eval("Id") %>' class="disc-post" style="margin-bottom:12px; display: block; border-radius: 12px; padding: 16px;">
                            <div class="disc-post-header" style="margin-bottom: 8px;">
                                <div class="disc-avatar orange"><%# GetInitials(Eval("FullName").ToString()) %></div>
                                <div class="disc-post-meta">
                                    <div class="disc-post-author" style="font-size:0.82rem;"><%# Eval("FullName") %></div>
                                    <div class="disc-post-time" style="font-size:0.75rem;"><%# GetTimeAgo(Eval("CreatedAt")) %></div>
                                </div>
                                <span class='disc-topic-tag <%# Eval("Topic").ToString().ToLower() %>' style="font-size:0.7rem;"><%# GetTopicLabel(Eval("Topic").ToString()) %></span>
                            </div>
                            <div class="disc-post-text" style="font-size: 0.82rem; line-height: 1.5;">
                                <strong><%# HttpUtility.HtmlEncode(Eval("Title")) %></strong><br />
                                <%# HttpUtility.HtmlEncode(Eval("Content").ToString().Length > 120 ? Eval("Content").ToString().Substring(0, 120) + "..." : Eval("Content")) %>
                            </div>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="phNoDiscussions" runat="server" Visible="false">
                    <div class="content-card" style="text-align: center; padding: 40px; border-radius: 12px;">
                        <p style="color:var(--slate);">No community posts found.</p>
                    </div>
                </asp:PlaceHolder>
            </div>

        </div>
    </div>
</asp:Content>
