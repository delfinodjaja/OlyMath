<%@ Page Title="Take Assessment" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="AttemptAssessment.aspx.cs"
    Inherits="OlyMath.AttemptAssessment" %>

<asp:Content ID="SidebarContent" ContentPlaceHolderID="SidebarContent" runat="server">
<div class="sidebar-section">
    <div class="sidebar-label">Main</div>
    <a href="TrainerDashboard.aspx" class="sidebar-item"><span class="dot"></span> Dashboard</a>
    <a href="AssessmentList.aspx" class="sidebar-item active"><span class="dot"></span> Assessments</a>
    <a href="CreateAssessment.aspx" class="sidebar-item"><span class="dot"></span> Create Assessment</a>
</div>
<div class="sidebar-section">
    <div class="sidebar-label">Learning</div>
    <a href='<%= "QuestionList.aspx?id=" + Request.QueryString["id"] %>' class="sidebar-item"><span class="dot"></span> Questions</a>
    <a href="Certificate.aspx" class="sidebar-item"><span class="dot"></span> Certificates</a>
</div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-title"><asp:Literal ID="litTitle" runat="server" /></div>
<div class="page-subtitle"><asp:Literal ID="litDescription" runat="server" /></div>

<a href="AssessmentList.aspx" class="btn-secondary" style="display:inline-block;margin-bottom:20px;">Back to Assessments</a>
<asp:HiddenField ID="hfAssessmentID" runat="server" />

<div class="content-card">
    <div class="section-head" style="margin-bottom:0;">
        <div>
            <h3>Assessment Overview</h3>
            <div class="page-subtitle" style="margin-bottom:0;">Questions: <asp:Literal ID="litQuestionCount" runat="server" /> | Passing score: <asp:Literal ID="litPassingScore" runat="server" />%</div>
        </div>
    </div>
</div>

<asp:Panel ID="pnlError" runat="server" Visible="false">
    <div class="content-card" style="border-color:#fca5a5;background:#fef2f2;color:#991b1b;">
        <asp:Literal ID="litError" runat="server" />
    </div>
</asp:Panel>

<asp:PlaceHolder ID="phQuestions" runat="server" />

<asp:Panel ID="pnlSubmit" runat="server" Visible="false">
    <div class="content-card" style="display:flex;align-items:center;justify-content:space-between;gap:16px;flex-wrap:wrap;">
        <p style="color:var(--slate);margin:0;">Answer all questions before submitting.</p>
        <asp:Button ID="btnSubmit" runat="server" Text="Submit Assessment" CssClass="btn-primary" OnClick="btnSubmit_Click" OnClientClick="return confirm('Submit now? You cannot change your answers after this.');" />
    </div>
</asp:Panel>

<asp:Panel ID="pnlNoQuestions" runat="server" Visible="false">
    <div class="content-card" style="text-align:center;padding:48px 24px;">This assessment has no questions yet.</div>
</asp:Panel>
</asp:Content>
