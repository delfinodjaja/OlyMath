<%@ Page Title="Question List" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="QuestionList.aspx.cs"
    Inherits="OlyMath.QuestionList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .page-wrap {
        max-width: 1100px;
        margin: 0 auto;
        padding: 48px 24px;
    }

    .back-link {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        font-size: 0.85rem;
        color: var(--slate);
        text-decoration: none;
        margin-bottom: 28px;
        transition: color 0.15s;
    }

    .back-link:hover { color: var(--navy); }

    .page-header {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        margin-bottom: 28px;
        flex-wrap: wrap;
        gap: 16px;
    }

    .page-header h1 {
        font-family: 'Syne', sans-serif;
        font-size: 1.8rem;
        font-weight: 800;
        letter-spacing: -0.5px;
        color: var(--navy);
    }

    .page-header .sub {
        font-size: 0.88rem;
        color: var(--slate);
        margin-top: 4px;
    }

    .alert-msg {
        padding: 14px 20px;
        border-radius: 10px;
        margin-bottom: 24px;
        font-size: 0.9rem;
        font-weight: 500;
    }

    .alert-success { background: #d1fae5; color: #065f46; border: 1px solid #a7f3d0; }
    .alert-error   { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

    .data-table {
        width: 100%;
        background: var(--white);
        border: 1px solid var(--border);
        border-radius: var(--radius);
        overflow: hidden;
        box-shadow: var(--card-shadow);
        border-collapse: collapse;
    }

    .data-table th {
        background: var(--navy);
        color: var(--white);
        padding: 14px 18px;
        font-family: 'Syne', sans-serif;
        font-size: 0.82rem;
        font-weight: 600;
        letter-spacing: 0.06em;
        text-transform: uppercase;
        text-align: left;
    }

    .data-table td {
        padding: 14px 18px;
        font-size: 0.9rem;
        color: var(--navy);
        border-bottom: 1px solid var(--border);
        vertical-align: top;
    }

    .data-table tr:last-child td { border-bottom: none; }
    .data-table tr:hover td { background: var(--bg); }

    .answer-badge {
        display: inline-block;
        padding: 3px 10px;
        border-radius: 100px;
        font-size: 0.78rem;
        font-weight: 700;
        background: var(--teal);
        color: var(--navy);
    }

    .option-list {
        list-style: none;
        padding: 0;
        margin: 0;
    }

    .option-list li {
        font-size: 0.85rem;
        color: var(--slate);
        margin-bottom: 4px;
    }

    .option-list li span {
        font-weight: 600;
        color: var(--navy);
    }

    .action-link {
        font-size: 0.82rem;
        font-weight: 500;
        text-decoration: none;
        padding: 5px 12px;
        border-radius: 6px;
        margin-right: 4px;
        display: inline-block;
        transition: opacity 0.15s;
    }

    .action-link:hover { opacity: 0.75; }
    .link-edit   { background: #e0f2fe; color: #0369a1; }
    .link-delete { background: #fee2e2; color: #991b1b; }

    .empty-msg {
        text-align: center;
        padding: 60px 24px;
        color: var(--slate);
        font-size: 0.95rem;
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-wrap">

    <a href="AssessmentList.aspx" class="back-link">&#8592; Back to Assessments</a>

    <div class="page-header">
        <div>
            <h1>Questions</h1>
            <p class="sub">
                Assessment:
                <strong><asp:Literal ID="litAssessmentTitle" runat="server" /></strong>
            </p>
        </div>
        <asp:HyperLink ID="hlAddQuestion" runat="server"
            CssClass="btn-primary"
            Text="+ Add Question" />
    </div>

    <asp:HiddenField ID="hfAssessmentID" runat="server" />

    <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
        <div class="alert-msg alert-success">
            <asp:Literal ID="litSuccess" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlError" runat="server" Visible="false">
        <div class="alert-msg alert-error">
            <asp:Literal ID="litError" runat="server" />
        </div>
    </asp:Panel>

    <asp:GridView ID="gvQuestions" runat="server"
        AutoGenerateColumns="false"
        CssClass="data-table"
        GridLines="None"
        OnRowCommand="gvQuestions_RowCommand">
        <Columns>

            <asp:BoundField DataField="DisplayOrder" HeaderText="#"
                ItemStyle-Width="40px" />

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
                <ItemTemplate>
                    <span class="answer-badge"><%# Eval("CorrectAnswer") %></span>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="160px">
                <ItemTemplate>
                    <a class="action-link link-edit"
                       href='<%# "EditQuestion.aspx?qid=" + Eval("QuestionID") + "&aid=" + Eval("AssessmentID") %>'>
                       Edit
                    </a>
                    <asp:LinkButton ID="btnDelete" runat="server"
                        CommandName="DeleteQuestion"
                        CommandArgument='<%# Eval("QuestionID") %>'
                        CssClass="action-link link-delete"
                        OnClientClick="return confirm('Delete this question?');">
                        Delete
                    </asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
        <EmptyDataTemplate>
            <div class="empty-msg">No questions yet. Click "+ Add Question" to get started.</div>
        </EmptyDataTemplate>
    </asp:GridView>

</div>
</asp:Content>
