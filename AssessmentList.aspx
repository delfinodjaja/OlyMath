<%@ Page Title="Assessments" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="AssessmentList.aspx.cs"
    Inherits="OlyMath.AssessmentList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .action-link {
        font-size: 0.78rem;
        font-weight: 500;
        text-decoration: none;
        padding: 6px 12px;
        border-radius: 999px;
        margin-right: 6px;
        display: inline-block;
        transition: opacity 0.15s;
    }
    .action-link:hover { opacity: 0.8; }
    .link-edit { background: #E0F2FE; color: #0369A1; }
    .link-questions { background: #EDE9FE; color: #6D28D9; }
    .link-delete { background: #FEE2E2; color: #991B1B; }
    .link-attempt { background: #D1FAE5; color: #065F46; }
    .empty-msg { text-align: center; padding: 48px 20px; color: var(--slate); }
</style>
</asp:Content>

<asp:Content ID="SidebarContent" ContentPlaceHolderID="SidebarContent" runat="server">
<div class="sidebar-section">
    <div class="sidebar-label">Main</div>
    <a href="TrainerDashboard.aspx" class="sidebar-item"><span class="dot"></span> Dashboard</a>
    <a href="AssessmentList.aspx" class="sidebar-item active"><span class="dot"></span> Assessments</a>
    <a href="CreateAssessment.aspx" class="sidebar-item"><span class="dot"></span> Create Assessment</a>
</div>
<div class="sidebar-section">
    <div class="sidebar-label">Learning</div>
    <a href="QuestionList.aspx" class="sidebar-item"><span class="dot"></span> Questions</a>
    <a href="Certificate.aspx" class="sidebar-item"><span class="dot"></span> Certificates</a>
</div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-title">Assessments</div>
<div class="page-subtitle">Manage assessment templates and navigate to question sets from the dashboard.</div>

<asp:Panel ID="pnlSuccess" runat="server" Visible="false">
    <div class="content-card" style="border-color:#a7f3d0;background:#f0fdf4;color:#065f46;">
        <asp:Literal ID="litSuccess" runat="server" />
    </div>
</asp:Panel>

<asp:Panel ID="pnlError" runat="server" Visible="false">
    <div class="content-card" style="border-color:#fca5a5;background:#fef2f2;color:#991b1b;">
        <asp:Literal ID="litError" runat="server" />
    </div>
</asp:Panel>

<div class="content-card">
    <div class="section-head">
        <div>
            <h3>Assessment Catalog</h3>
            <div class="page-subtitle" style="margin-bottom:0;">Visible assessments available to trainees.</div>
        </div>
        <asp:Panel ID="pnlCreateBtn" runat="server" Visible="false">
            <a href="CreateAssessment.aspx" class="btn-primary">+ New Assessment</a>
        </asp:Panel>
    </div>
</div>

<div class="table-wrap">
    <asp:GridView ID="gvAssessments" runat="server"
        AutoGenerateColumns="false"
        GridLines="None"
        OnRowCommand="gvAssessments_RowCommand"
        EmptyDataText=""
        ShowHeaderWhenEmpty="false">
        <Columns>
            <asp:BoundField DataField="Title" HeaderText="Title" />
            <asp:BoundField DataField="Description" HeaderText="Description" />
            <asp:BoundField DataField="PassingScore" HeaderText="Pass %" />
            <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
            <asp:BoundField DataField="CreatedDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="260px">
                <ItemTemplate>
                    <asp:Panel ID="pnlTrainerActions" runat="server">
                        <a class="action-link link-edit" href='<%# "EditAssessment.aspx?id=" + Eval("AssessmentID") %>'>Edit</a>
                        <a class="action-link link-questions" href='<%# "QuestionList.aspx?id=" + Eval("AssessmentID") %>'>Questions</a>
                        <asp:LinkButton ID="btnDelete" runat="server"
                            CommandName="DeleteAssessment"
                            CommandArgument='<%# Eval("AssessmentID") %>'
                            CssClass="action-link link-delete"
                            OnClientClick="return confirm('Delete this assessment and all its questions?');">Delete</asp:LinkButton>
                        <a class="action-link link-attempt" href='<%# "AttemptAssessment.aspx?id=" + Eval("AssessmentID") %>'>Preview</a>
                    </asp:Panel>
                    <asp:Panel ID="pnlTraineeActions" runat="server">
                        <a class="action-link link-attempt" href='<%# "AttemptAssessment.aspx?id=" + Eval("AssessmentID") %>'>Start</a>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="empty-msg">No assessments found.</div>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
</asp:Content>
