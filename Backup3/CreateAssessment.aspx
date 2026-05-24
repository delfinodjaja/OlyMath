<%@ Page Title="Create Assessment" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="CreateAssessment.aspx.cs"
    Inherits="OlyMath.CreateAssessment" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .form-note { color: var(--slate); font-size: 0.88rem; margin-top: 4px; }
</style>
</asp:Content>

<asp:Content ID="SidebarContent" ContentPlaceHolderID="SidebarContent" runat="server">
<div class="sidebar-section">
    <div class="sidebar-label">Assessment</div>
    <a href="AssessmentList.aspx" class="sidebar-item"><span class="dot"></span> Assessments</a>
    <a href="CreateAssessment.aspx" class="sidebar-item active"><span class="dot"></span> New Assessment</a>
</div>
<div class="sidebar-section">
    <div class="sidebar-label">Questions</div>
    <a href="AssessmentList.aspx" class="sidebar-item"><span class="dot"></span> Select an Assessment</a>
</div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-title">Create Assessment</div>
<div class="page-subtitle">Add a new assessment shell before attaching questions.</div>

<a href="AssessmentList.aspx" class="btn-secondary" style="display:inline-block;margin-bottom:20px;">Back to Assessments</a>

<div class="content-card">
    <asp:Panel ID="pnlError" runat="server" Visible="false">
        <div class="content-card" style="border-color:#fca5a5;background:#fef2f2;color:#991b1b;padding:16px 20px;margin-bottom:24px;">
            <asp:Literal ID="litError" runat="server" />
        </div>
    </asp:Panel>

    <h3>Assessment Details</h3>
    <div class="form-note">Provide a title, description, and passing score.</div>

    <div class="form-group">
        <label for="txtTitle">Title <span style="color:var(--orange)">*</span></label>
        <asp:TextBox ID="txtTitle" runat="server" MaxLength="200" placeholder="e.g. Algebra Fundamentals Quiz" />
        <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle" ErrorMessage="Title is required." CssClass="field-validation-error" Display="Dynamic" />
    </div>

    <div class="form-group">
        <label for="txtDescription">Description</label>
        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" MaxLength="1000" placeholder="Brief description of what this assessment covers..." />
    </div>

    <div class="form-group">
        <label for="txtPassingScore">Passing Score (%) <span style="color:var(--orange)">*</span></label>
        <asp:TextBox ID="txtPassingScore" runat="server" TextMode="Number" placeholder="60" />
        <asp:RequiredFieldValidator ID="rfvPassingScore" runat="server" ControlToValidate="txtPassingScore" ErrorMessage="Passing score is required." CssClass="field-validation-error" Display="Dynamic" />
        <asp:RangeValidator ID="rvPassingScore" runat="server" ControlToValidate="txtPassingScore" MinimumValue="1" MaximumValue="100" Type="Integer" ErrorMessage="Passing score must be between 1 and 100." CssClass="field-validation-error" Display="Dynamic" />
    </div>

    <div class="form-actions">
        <asp:Button ID="btnSave" runat="server" Text="Create Assessment" CssClass="btn-primary" OnClick="btnSave_Click" />
        <a href="AssessmentList.aspx" class="btn-secondary">Cancel</a>
    </div>
</div>
</asp:Content>
