<%@ Page Title="Edit Question" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="EditQuestion.aspx.cs"
    Inherits="OlyMath.EditQuestion" %>

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
<div class="page-title">Edit Question</div>
<div class="page-subtitle">Update the question wording, options, or correct answer.</div>

<a id="backLink" runat="server" class="btn-secondary" style="display:inline-block;margin-bottom:20px;">Back to Questions</a>

<div class="content-card">
    <h3><asp:Literal ID="litAssessmentTitle" runat="server" /></h3>
    <asp:HiddenField ID="hfQuestionID" runat="server" />
    <asp:HiddenField ID="hfAssessmentID" runat="server" />

    <asp:Panel ID="pnlError" runat="server" Visible="false">
        <div class="content-card" style="border-color:#fca5a5;background:#fef2f2;color:#991b1b;padding:16px 20px;margin-bottom:24px;">
            <asp:Literal ID="litError" runat="server" />
        </div>
    </asp:Panel>

    <div class="form-group">
        <label for="txtQuestion">Question Text <span style="color:var(--orange)">*</span></label>
        <asp:TextBox ID="txtQuestion" runat="server" TextMode="MultiLine" Rows="3" MaxLength="2000" />
        <asp:RequiredFieldValidator ID="rfvQuestion" runat="server" ControlToValidate="txtQuestion" ErrorMessage="Question text is required." CssClass="field-validation-error" Display="Dynamic" />
    </div>

    <div class="form-grid">
        <div class="form-group"><label for="txtOptionA">Option A</label><asp:TextBox ID="txtOptionA" runat="server" MaxLength="500" /></div>
        <div class="form-group"><label for="txtOptionB">Option B</label><asp:TextBox ID="txtOptionB" runat="server" MaxLength="500" /></div>
        <div class="form-group"><label for="txtOptionC">Option C</label><asp:TextBox ID="txtOptionC" runat="server" MaxLength="500" /></div>
        <div class="form-group"><label for="txtOptionD">Option D</label><asp:TextBox ID="txtOptionD" runat="server" MaxLength="500" /></div>
    </div>

    <div class="form-group">
        <label>Correct Answer <span style="color:var(--orange)">*</span></label>
        <asp:RadioButtonList ID="rblCorrectAnswer" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="answer-radios">
            <asp:ListItem Text="A" Value="A" />
            <asp:ListItem Text="B" Value="B" />
            <asp:ListItem Text="C" Value="C" />
            <asp:ListItem Text="D" Value="D" />
        </asp:RadioButtonList>
    </div>

    <div class="form-group">
        <label for="txtDisplayOrder">Display Order</label>
        <asp:TextBox ID="txtDisplayOrder" runat="server" TextMode="Number" />
    </div>

    <div class="form-actions">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn-primary" OnClick="btnSave_Click" />
        <a id="cancelLink" runat="server" class="btn-secondary">Cancel</a>
    </div>
</div>
</asp:Content>
