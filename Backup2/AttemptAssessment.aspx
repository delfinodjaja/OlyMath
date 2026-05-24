<%@ Page Title="Take Assessment" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="AttemptAssessment.aspx.cs"
    Inherits="OlyMath.AttemptAssessment" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .page-wrap {
        max-width: 800px;
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

    .assessment-header {
        background: var(--white);
        border: 1px solid var(--border);
        border-radius: var(--radius);
        padding: 32px 36px;
        margin-bottom: 28px;
        box-shadow: var(--card-shadow);
    }

    .assessment-header h1 {
        font-family: 'Syne', sans-serif;
        font-size: 1.8rem;
        font-weight: 800;
        letter-spacing: -0.5px;
        color: var(--navy);
        margin-bottom: 8px;
    }

    .assessment-header p {
        font-size: 0.9rem;
        color: var(--slate);
        font-weight: 300;
        line-height: 1.6;
    }

    .assessment-meta {
        display: flex;
        gap: 24px;
        margin-top: 16px;
        flex-wrap: wrap;
    }

    .meta-item {
        font-size: 0.82rem;
        color: var(--slate);
    }

    .meta-item strong { color: var(--navy); }

    .question-card {
        background: var(--white);
        border: 1px solid var(--border);
        border-radius: var(--radius);
        padding: 28px 32px;
        margin-bottom: 18px;
        box-shadow: var(--card-shadow);
    }

    .question-num {
        font-family: 'Syne', sans-serif;
        font-size: 0.75rem;
        font-weight: 700;
        letter-spacing: 0.1em;
        text-transform: uppercase;
        color: var(--orange);
        margin-bottom: 10px;
    }

    .question-text {
        font-size: 1rem;
        font-weight: 500;
        color: var(--navy);
        line-height: 1.6;
        margin-bottom: 20px;
    }

    .options-list {
        list-style: none;
        padding: 0;
        margin: 0;
        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    .options-list li {
        display: flex;
        align-items: center;
    }

    .option-radio {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px 16px;
        border: 1.5px solid var(--border);
        border-radius: 10px;
        cursor: pointer;
        transition: all 0.15s;
        width: 100%;
        font-size: 0.92rem;
        color: var(--navy);
    }

    .option-radio:has(input:checked) {
        border-color: var(--navy);
        background: #f0f4ff;
    }

    .option-radio:hover {
        border-color: var(--slate);
        background: var(--bg);
    }

    .option-letter {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 26px;
        height: 26px;
        border-radius: 50%;
        background: var(--bg);
        border: 1.5px solid var(--border);
        font-size: 0.78rem;
        font-weight: 700;
        color: var(--navy);
        flex-shrink: 0;
        transition: all 0.15s;
    }

    .submit-section {
        margin-top: 32px;
        padding: 28px 32px;
        background: var(--white);
        border: 1px solid var(--border);
        border-radius: var(--radius);
        box-shadow: var(--card-shadow);
        display: flex;
        align-items: center;
        justify-content: space-between;
        flex-wrap: wrap;
        gap: 16px;
    }

    .submit-section p {
        font-size: 0.88rem;
        color: var(--slate);
    }

    .alert-msg {
        padding: 14px 20px;
        border-radius: 10px;
        margin-bottom: 24px;
        font-size: 0.9rem;
        font-weight: 500;
    }

    .alert-error { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

    .no-questions {
        text-align: center;
        padding: 80px 24px;
        color: var(--slate);
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-wrap">

    <a href="AssessmentList.aspx" class="back-link">&#8592; Back to Assessments</a>

    <asp:HiddenField ID="hfAssessmentID" runat="server" />

    <div class="assessment-header">
        <h1><asp:Literal ID="litTitle" runat="server" /></h1>
        <p><asp:Literal ID="litDescription" runat="server" /></p>
        <div class="assessment-meta">
            <span class="meta-item">
                Questions: <strong><asp:Literal ID="litQuestionCount" runat="server" /></strong>
            </span>
            <span class="meta-item">
                Passing score: <strong><asp:Literal ID="litPassingScore" runat="server" />%</strong>
            </span>
        </div>
    </div>

    <asp:Panel ID="pnlError" runat="server" Visible="false">
        <div class="alert-msg alert-error">
            <asp:Literal ID="litError" runat="server" />
        </div>
    </asp:Panel>

    <%-- Questions are rendered dynamically from code-behind --%>
    <asp:PlaceHolder ID="phQuestions" runat="server" />

    <asp:Panel ID="pnlSubmit" runat="server" Visible="false">
        <div class="submit-section">
            <p>Answer all questions before submitting.</p>
            <asp:Button ID="btnSubmit" runat="server"
                Text="Submit Assessment"
                CssClass="btn-primary"
                OnClick="btnSubmit_Click"
                OnClientClick="return confirm('Submit now? You cannot change your answers after this.');" />
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlNoQuestions" runat="server" Visible="false">
        <div class="no-questions">
            <p>This assessment has no questions yet.</p>
        </div>
    </asp:Panel>

</div>
</asp:Content>
