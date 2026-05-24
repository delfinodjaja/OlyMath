<%@ Page Title="OlyMath - Admin Metrics" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="OlyMath.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="admin-dash">
        <div class="page-title">Admin System Metrics</div>
        <div class="page-subtitle">Monitor OlyMath platform activities, metrics, and manage user memberships.</div>

        <!-- STATS GRID -->
        <div class="stat-grid">
            <div class="stat-card">
                <div class="stat-val stat-teal"><asp:Literal ID="litTotalUsers" runat="server">0</asp:Literal></div>
                <div class="stat-label">Total Registered Users</div>
            </div>
            <div class="stat-card">
                <div class="stat-val"><asp:Literal ID="litTotalModules" runat="server">0</asp:Literal></div>
                <div class="stat-label">Course Modules</div>
            </div>
            <div class="stat-card">
                <div class="stat-val stat-accent"><asp:Literal ID="litTotalCerts" runat="server">0</asp:Literal></div>
                <div class="stat-label">Certificates Issued</div>
            </div>
            <div class="stat-card">
                <div class="stat-val"><asp:Literal ID="litTotalPosts" runat="server">0</asp:Literal></div>
                <div class="stat-label">Discussion Threads</div>
            </div>
        </div>

        <!-- ADMIN CONTROLS -->
        <div class="section-head">
            <div class="section-title">Administrative Actions</div>
        </div>

        <div class="module-grid" style="grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));">
            <a href="ManageUsers.aspx" class="module-card">
                <div class="module-tag">Membership</div>
                <h4>Manage Platform Users</h4>
                <div class="module-meta">
                    <span style="font-size:0.8rem; color:var(--slate); line-height: 1.5;">Create, edit, reset passwords, or delete Trainees, Trainers, and Admin users.</span>
                </div>
            </a>
            
            <a href="../Trainee/Discussion.aspx" class="module-card teal">
                <div class="module-tag teal">Moderation</div>
                <h4>Discussion Board Feed</h4>
                <div class="module-meta">
                    <span style="font-size:0.8rem; color:var(--slate); line-height: 1.5;">Audit user-submitted topics, check discussion analytics, and reply directly as an administrator.</span>
                </div>
            </a>
        </div>
    </div>
</asp:Content>
