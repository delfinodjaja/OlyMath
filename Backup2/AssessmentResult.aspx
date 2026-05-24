<%@ Page Title="Assessment Result" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="AssessmentResult.aspx.cs"
    Inherits="OlyMath.AssessmentResult" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .page-wrap {
        max-width: 680px;
        margin: 0 auto;
        padding: 48px 24px;
    }

    .result-card {
        background: var(--white);
        border: 1px solid var(--border);
        border-radius: var(--radius);
        padding: 52px 48px;
        box-shadow: var(--card-shadow);
        text-align: center;
        animation: slideUp 0.35s ease;
    }

    @keyframes slideUp {
        from { opacity: 0; transform: translateY(20px); }
        to   { opacity: 1; transform: translateY(0); }
    }

    .result-icon {
        font-size: 3.5rem;
        margin-bottom: 20px;
        line-height: 1;
    }

    .result-verdict {
        font-family: 'Syne', sans-serif;
        font-size: 2rem;
        font-weight: 800;
        letter-spacing: -0.5px;
        margin-bottom: 10px;
    }

    .result-verdict.passed { color: #059669; }
    .result-verdict.failed { color: var(--orange); }

    .result-score {
        font-family: 'Syne', sans-serif;
        font-size: 4rem;
        font-weight: 800;
        letter-spacing: -2px;
        margin: 28px 0;
        color: var(--navy);
    }

    .score-label {
        font-size: 0.85rem;
        color: var(--slate);
        font-weight: 300;
    }

    .result-details {
        background: var(--bg);
        border: 1px solid var(--border);
        border-radius: 12px;
        padding: 20px 24px;
        margin: 28px 0;
        text-align: left;
    }

    .detail-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 8px 0;
        font-size: 0.88rem;
        border-bottom: 1px solid var(--border);
    }

    .detail-row:last-child { border-bottom: none; }
    .detail-row .label { color: var(--slate); }
    .detail-row .value { font-weight: 600; color: var(--navy); }

    .actions {
        display: flex;
        gap: 12px;
        justify-content: center;
        margin-top: 32px;
        flex-wrap: wrap;
    }

    .pass-banner {
        background: #d1fae5;
        border: 1px solid #a7f3d0;
        border-radius: 10px;
        padding: 14px 20px;
        color: #065f46;
        font-size: 0.9rem;
        font-weight: 500;
        margin-bottom: 24px;
    }

    .fail-banner {
        background: #fee2e2;
        border: 1px solid #fca5a5;
        border-radius: 10px;
        padding: 14px 20px;
        color: #991b1b;
        font-size: 0.9rem;
        font-weight: 500;
        margin-bottom: 24px;
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-wrap">
    <div class="result-card">

        <div class="result-icon">
            <asp:Literal ID="litIcon" runat="server" />
        </div>

        <div class="result-verdict">
            <asp:Literal ID="litVerdict" runat="server" />
        </div>

        <asp:Panel ID="pnlPassBanner" runat="server" Visible="false">
            <div class="pass-banner">
                Congratulations! You passed this assessment.
                Your certificate has been generated.
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlFailBanner" runat="server" Visible="false">
            <div class="fail-banner">
                You did not meet the passing score. You may try again.
            </div>
        </asp:Panel>

        <div class="result-score">
            <asp:Literal ID="litScore" runat="server" /><span style="font-size:1.5rem;color:var(--slate)">%</span>
        </div>
        <div class="score-label">Your Score</div>

        <div class="result-details">

            <div class="detail-row">
                <span class="label">Assessment</span>
                <span class="value"><asp:Literal ID="litAssessmentTitle" runat="server" /></span>
            </div>

            <div class="detail-row">
                <span class="label">Passing Score</span>
                <span class="value"><asp:Literal ID="litPassingScore" runat="server" />%</span>
            </div>

            <div class="detail-row">
                <span class="label">Date</span>
                <span class="value"><asp:Literal ID="litAttemptDate" runat="server" /></span>
            </div>

        </div>

        <div class="actions">

            <asp:Panel ID="pnlCertBtn" runat="server" Visible="false">
                <a id="certLink" runat="server" class="btn-primary">
                    View Certificate
                </a>
            </asp:Panel>

            <a href="AssessmentList.aspx" class="btn-secondary">
                Back to Assessments
            </a>

        </div>
    </div>
</div>
</asp:Content>
