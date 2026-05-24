<%@ Page Title="Assessment Result" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="AssessmentResult.aspx.cs"
    Inherits="OlyMath.AssessmentResult" %>

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
<div class="page-title">Assessment Result</div>
<div class="page-subtitle">Review your score and open the certificate if you passed.</div>

<div class="result-card content-card" style="text-align:center;">
    <div class="result-icon"><asp:Literal ID="litIcon" runat="server" /></div>
    <div class="result-verdict"><asp:Literal ID="litVerdict" runat="server" /></div>

    <asp:Panel ID="pnlPassBanner" runat="server" Visible="false">
        <div class="content-card" style="border-color:#a7f3d0;background:#f0fdf4;color:#065f46;">Congratulations! You passed this assessment. Your certificate has been generated.</div>
    </asp:Panel>

    <asp:Panel ID="pnlFailBanner" runat="server" Visible="false">
        <div class="content-card" style="border-color:#fca5a5;background:#fef2f2;color:#991b1b;">You did not meet the passing score. You may try again.</div>
    </asp:Panel>

    <div class="result-score"><asp:Literal ID="litScore" runat="server" /><span style="font-size:1.5rem;color:var(--slate)">%</span></div>
    <div class="score-label">Your Score</div>

    <div class="content-card" style="text-align:left;">
        <div class="detail-row"><span class="label">Assessment</span><span class="value"><asp:Literal ID="litAssessmentTitle" runat="server" /></span></div>
        <div class="detail-row"><span class="label">Passing Score</span><span class="value"><asp:Literal ID="litPassingScore" runat="server" />%</span></div>
        <div class="detail-row"><span class="label">Date</span><span class="value"><asp:Literal ID="litAttemptDate" runat="server" /></span></div>
    </div>

    <div class="actions" style="display:flex;gap:12px;justify-content:center;flex-wrap:wrap;">
        <asp:Panel ID="pnlCertBtn" runat="server" Visible="false">
            <a id="certLink" runat="server" class="btn-primary">View Certificate</a>
        </asp:Panel>
        <a href="AssessmentList.aspx" class="btn-secondary">Back to Assessments</a>
    </div>
</div>
</asp:Content>
