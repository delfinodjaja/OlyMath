<%@ Page Title="Question List" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="QuestionList.aspx.cs"
    Inherits="OlyMath.QuestionList" %>

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
    .link-delete { background: #FEE2E2; color: #991B1B; }
    .empty-msg { text-align: center; padding: 48px 20px; color: var(--slate); }
    .option-list { list-style: none; padding: 0; margin: 0; }
    .option-list li { font-size: 0.84rem; color: var(--slate); margin-bottom: 4px; }
    .option-list li span { font-weight: 600; color: var(--navy); }
    .answer-badge { display: inline-block; padding: 3px 10px; border-radius: 999px; font-size: 0.75rem; font-weight: 600; background: #E6FBF7; color: #00A383; }
</style>
</asp:Content>

<asp:Content ID="SidebarContent" ContentPlaceHolderID="SidebarContent" runat="server">
<div class="sidebar-section">
    <div class="sidebar-label">Assessment</div>
    <a href="AssessmentList.aspx" class="sidebar-item"><span class="dot"></span> Assessments</a>
    <a href="CreateAssessment.aspx" class="sidebar-item"><span class="dot"></span> New Assessment</a>
</div>
<div class="sidebar-section">
    <div class="sidebar-label">Questions</div>
    <a href='<%= "QuestionList.aspx?id=" + Request.QueryString["id"] %>' class="sidebar-item active"><span class="dot"></span> Question List</a>
</div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-title">Questions</div>
<div class="page-subtitle">Assessment: <strong><asp:Literal ID="litAssessmentTitle" runat="server" /></strong></div>

<a href="AssessmentList.aspx" class="btn-secondary" style="display:inline-block;margin-bottom:20px;">Back to Assessments</a>
<asp:HyperLink ID="hlAddQuestion" runat="server" CssClass="btn-primary" Text="+ Add Question" style="margin-left:12px;" />

<asp:HiddenField ID="hfAssessmentID" runat="server" />

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

<div class="table-wrap">
    <asp:GridView ID="gvQuestions" runat="server"
        AutoGenerateColumns="false"
        GridLines="None"
        OnRowCommand="gvQuestions_RowCommand">
        <Columns>
            <asp:BoundField DataField="DisplayOrder" HeaderText="#" ItemStyle-Width="40px" />
            <asp:BoundField DataField="QuestionText" HeaderText="Question" />
            <asp:TemplateField HeaderText="Options">
                <ItemTemplate>
                    <ul class="option-list">
                        <li><span>A.</span> <%# Eval("OptionA") %></li>
                        <li><span>B.</span> <%# Eval("OptionB") %></li>
                        <li><span>C.</span> <%# Eval("OptionC") %></li>
                        <li><span>D.</span> <%# Eval("OptionD") %></li>
                    </ul>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Answer" ItemStyle-Width="80px">
                <ItemTemplate><span class="answer-badge"><%# Eval("CorrectAnswer") %></span></ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="160px">
                <ItemTemplate>
                    <a class="action-link link-edit" href='<%# "EditQuestion.aspx?qid=" + Eval("QuestionID") + "&aid=" + Eval("AssessmentID") %>'>Edit</a>
                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteQuestion" CommandArgument='<%# Eval("QuestionID") %>' CssClass="action-link link-delete" OnClientClick="return confirm('Delete this question?');">Delete</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="empty-msg">No questions yet. Click "+ Add Question" to get started.</div>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
</asp:Content>
