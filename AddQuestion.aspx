<%@ Page Title="Add Question" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="AddQuestion.aspx.cs"
    Inherits="OlyMath.AddQuestion" %>

<asp:Content ID="SidebarContent" ContentPlaceHolderID="SidebarContent" runat="server">
<div class="sidebar-section">
    <div class="sidebar-label">Main</div>
    <a href="TrainerDashboard.aspx" class="sidebar-item"><span class="dot"></span> Dashboard</a>
    <a href="AssessmentList.aspx" class="sidebar-item"><span class="dot"></span> Assessments</a>
    <a href="CreateAssessment.aspx" class="sidebar-item"><span class="dot"></span> Create Assessment</a>
</div>
<div class="sidebar-section">
    <div class="sidebar-label">Learning</div>
    <a href='<%= "QuestionList.aspx?id=" + Request.QueryString["aid"] %>' class="sidebar-item active"><span class="dot"></span> Questions</a>
    <a href="Certificate.aspx" class="sidebar-item"><span class="dot"></span> Certificates</a>
</div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-title">Add Question</div>
<div class="page-subtitle">Add a question to the selected assessment.</div>

<asp:HiddenField ID="hfAssessmentID" runat="server" />
<asp:HyperLink ID="backLink" runat="server" Text="Back to Questions" CssClass="btn-secondary" style="display:inline-block;margin-bottom:20px;" />

<div class="content-card">
    <h3><asp:Literal ID="litAssessmentTitle" runat="server" /></h3>
    <asp:Panel ID="pnlError" runat="server" Visible="false">
        <div class="content-card" style="border-color:#fca5a5;background:#fef2f2;color:#991b1b;padding:16px 20px;margin-bottom:24px;">
            <asp:Literal ID="litError" runat="server" />
        </div>
    </asp:Panel>

    <div class="form-group">
        <label>Question</label>
        <asp:TextBox ID="txtQuestion" runat="server" TextMode="MultiLine" Rows="3" />
    </div>
    <div class="form-grid">
        <div class="form-group"><label>Option A</label><asp:TextBox ID="txtOptionA" runat="server" /></div>
        <div class="form-group"><label>Option B</label><asp:TextBox ID="txtOptionB" runat="server" /></div>
        <div class="form-group"><label>Option C</label><asp:TextBox ID="txtOptionC" runat="server" /></div>
        <div class="form-group"><label>Option D</label><asp:TextBox ID="txtOptionD" runat="server" /></div>
    </div>
    <div class="form-grid single">
        <div class="form-group">
            <label>Correct Option</label>
            <asp:DropDownList ID="ddlCorrectOption" runat="server">
                <asp:ListItem Value="A">A</asp:ListItem>
                <asp:ListItem Value="B">B</asp:ListItem>
                <asp:ListItem Value="C">C</asp:ListItem>
                <asp:ListItem Value="D">D</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="form-group">
            <label>Display Order</label>
            <asp:TextBox ID="txtDisplayOrder" runat="server" Text="1" />
        </div>
    </div>
    <div class="form-actions">
        <asp:Button ID="btnSave" runat="server" Text="Save Question" CssClass="btn-primary" OnClick="btnSave_Click" />
    </div>
</div>
</asp:Content>