<%@ Page Title="Assessments" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="AssessmentList.aspx.cs"
    Inherits="OlyMath.AssessmentList" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .page-wrap {
        max-width: 1100px;
        margin: 0 auto;
        padding: 48px 24px;
    }

    .page-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 32px;
        flex-wrap: wrap;
        gap: 16px;
    }

    .page-header h1 {
        font-family: 'Syne', sans-serif;
        font-size: 2rem;
        font-weight: 800;
        letter-spacing: -1px;
        color: var(--navy);
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
        vertical-align: middle;
    }

    .data-table tr:last-child td { border-bottom: none; }
    .data-table tr:hover td { background: var(--bg); }

    .badge-pass {
        display: inline-block;
        padding: 3px 10px;
        border-radius: 100px;
        font-size: 0.75rem;
        font-weight: 600;
        background: #d1fae5;
        color: #065f46;
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
    .link-edit     { background: #e0f2fe; color: #0369a1; }
    .link-questions{ background: #ede9fe; color: #6d28d9; }
    .link-delete   { background: #fee2e2; color: #991b1b; }
    .link-attempt  { background: #d1fae5; color: #065f46; }

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

    <div class="page-header">
        <h1>Assessments</h1>

        <%-- Only Trainer and Admin can create assessments --%>
        <asp:Panel ID="pnlCreateBtn" runat="server" Visible="false">
            <a href="CreateAssessment.aspx" class="btn-primary">+ New Assessment</a>
        </asp:Panel>
    </div>

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

    <asp:GridView ID="gvAssessments" runat="server"
        AutoGenerateColumns="false"
        CssClass="data-table"
        GridLines="None"
        OnRowCommand="gvAssessments_RowCommand"
        EmptyDataText=""
        ShowHeaderWhenEmpty="false">
        <Columns>

            <asp:BoundField DataField="Title"       HeaderText="Title"       />
            <asp:BoundField DataField="Description" HeaderText="Description" />
            <asp:BoundField DataField="PassingScore" HeaderText="Pass %" />
            <asp:BoundField DataField="CreatedBy"   HeaderText="Created By"  />
            <asp:BoundField DataField="CreatedDate" HeaderText="Date"
                DataFormatString="{0:dd MMM yyyy}" />

            <%-- Trainer/Admin actions column --%>
            <asp:TemplateField HeaderText="Actions" ItemStyle-Width="260px">
                <ItemTemplate>
                    <asp:Panel ID="pnlTrainerActions" runat="server">

                        <a class="action-link link-edit"
                           href='<%# "EditAssessment.aspx?id=" + Eval("AssessmentID") %>'>
                           Edit
                        </a>

                        <a class="action-link link-questions"
                           href='<%# "QuestionList.aspx?id=" + Eval("AssessmentID") %>'>
                           Questions
                        </a>

                        <asp:LinkButton ID="btnDelete" runat="server"
                            CommandName="DeleteAssessment"
                            CommandArgument='<%# Eval("AssessmentID") %>'
                            CssClass="action-link link-delete"
                            OnClientClick="return confirm('Delete this assessment and all its questions?');">
                            Delete
                        </asp:LinkButton>

                        <a class="action-link link-attempt"
                           href='<%# "AttemptAssessment.aspx?id=" + Eval("AssessmentID") %>'>
                           Preview
                        </a>
                    </asp:Panel>

                    <asp:Panel ID="pnlTraineeActions" runat="server">
                        <a class="action-link link-attempt"
                           href='<%# "AttemptAssessment.aspx?id=" + Eval("AssessmentID") %>'>
                           Start
                        </a>
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
