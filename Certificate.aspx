<%@ Page Title="Certificate" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="Certificate.aspx.cs"
    Inherits="OlyMath.Certificate" %>

<asp:Content ID="SidebarContent" ContentPlaceHolderID="SidebarContent" runat="server">
<div class="sidebar-section">
    <div class="sidebar-label">Main</div>
    <a href="TrainerDashboard.aspx" class="sidebar-item"><span class="dot"></span> Dashboard</a>
    <a href="AssessmentList.aspx" class="sidebar-item active"><span class="dot"></span> Assessments</a>
    <a href="CreateAssessment.aspx" class="sidebar-item"><span class="dot"></span> Create Assessment</a>
</div>
<div class="sidebar-section">
    <div class="sidebar-label">Learning</div>
    <a href='<%= "QuestionList.aspx?id=" + Request.QueryString["attemptId"] %>' class="sidebar-item"><span class="dot"></span> Questions</a>
    <a href="Certificate.aspx" class="sidebar-item"><span class="dot"></span> Certificates</a>
</div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-title">Certificate</div>
<div class="page-subtitle">Print or download the passed assessment certificate.</div>

<div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:20px; gap:12px; flex-wrap:wrap;">
    <a href="AssessmentList.aspx" class="btn-secondary" style="display:inline-block;">Back to Assessments</a>
    <button onclick="window.print()" class="btn-secondary" style="padding:10px 22px;">🖨 Print Certificate</button>
</div>

<div class="cert-wrap">
    <div class="cert-card">
        <div class="cert-tag">Certificate of Completion</div>
        <div class="cert-title">Oly<span style="color:var(--orange)">Math</span></div>
        <div class="cert-divider"></div>
        <div class="cert-name"><asp:Literal ID="litTraineeName" runat="server" /></div>
        <div class="cert-module">has successfully completed the assessment</div>
        <div class="cert-module" style="font-family:'Syne',sans-serif;font-size:1.4rem;font-weight:700;color:var(--navy);">
            <asp:Literal ID="litAssessmentTitle" runat="server" />
        </div>
        <div class="stat-grid" style="margin-top:34px;margin-bottom:0;">
            <div class="stat-card"><div class="stat-val stat-accent"><asp:Literal ID="litScore" runat="server" />%</div><div class="stat-label">Final Score</div></div>
            <div class="stat-card"><div class="stat-val stat-teal"><asp:Literal ID="litIssueDate" runat="server" /></div><div class="stat-label">Issue Date</div></div>
            <div class="stat-card"><div class="stat-val">✓</div><div class="stat-label">Verified Pass</div></div>
        </div>
        <div class="cert-footer">
            <span class="cert-id">Certificate ID: <asp:Literal ID="litCertID" runat="server" /></span>
            <span class="cert-seal">🎓</span>
        </div>
    </div>
</div>
</asp:Content>
