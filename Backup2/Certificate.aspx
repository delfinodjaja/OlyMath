<%@ Page Title="Certificate" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Certificate.aspx.cs"
    Inherits="OlyMath.Certificate" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .page-wrap {
        max-width: 860px;
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

    .print-btn {
        float: right;
    }

    .cert-frame {
        background: var(--white);
        border: 2px solid var(--navy);
        border-radius: 20px;
        padding: 0;
        overflow: hidden;
        box-shadow: 0 8px 48px rgba(13,27,42,0.14);
        position: relative;
        animation: slideUp 0.35s ease;
    }

    @keyframes slideUp {
        from { opacity: 0; transform: translateY(20px); }
        to   { opacity: 1; transform: translateY(0); }
    }

    .cert-banner {
        background: var(--navy);
        color: var(--white);
        padding: 28px 48px;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    .cert-banner .logo-text {
        font-family: 'Syne', sans-serif;
        font-size: 1.6rem;
        font-weight: 800;
        letter-spacing: -0.5px;
    }

    .cert-banner .logo-text span { color: var(--orange); }

    .cert-banner .cert-label {
        font-size: 0.78rem;
        font-weight: 500;
        letter-spacing: 0.12em;
        text-transform: uppercase;
        color: rgba(255,255,255,0.6);
    }

    .cert-body {
        padding: 52px 64px;
        text-align: center;
    }

    .cert-presents {
        font-size: 0.82rem;
        font-weight: 500;
        letter-spacing: 0.12em;
        text-transform: uppercase;
        color: var(--slate);
        margin-bottom: 16px;
    }

    .cert-name {
        font-family: 'Syne', sans-serif;
        font-size: 2.8rem;
        font-weight: 800;
        letter-spacing: -1px;
        color: var(--navy);
        margin-bottom: 24px;
        padding-bottom: 24px;
        border-bottom: 2px solid var(--border);
    }

    .cert-completed {
        font-size: 0.92rem;
        color: var(--slate);
        margin-bottom: 14px;
        font-weight: 300;
    }

    .cert-assessment {
        font-family: 'Syne', sans-serif;
        font-size: 1.6rem;
        font-weight: 700;
        color: var(--navy);
        margin-bottom: 32px;
    }

    .cert-meta {
        display: flex;
        justify-content: center;
        gap: 60px;
        margin-top: 36px;
        padding-top: 36px;
        border-top: 1px solid var(--border);
        flex-wrap: wrap;
    }

    .meta-block {
        text-align: center;
    }

    .meta-block .meta-val {
        font-family: 'Syne', sans-serif;
        font-size: 1.6rem;
        font-weight: 800;
        color: var(--orange);
    }

    .meta-block .meta-label {
        font-size: 0.78rem;
        color: var(--slate);
        text-transform: uppercase;
        letter-spacing: 0.1em;
        font-weight: 500;
        margin-top: 4px;
    }

    .cert-footer {
        background: var(--bg);
        border-top: 1px solid var(--border);
        padding: 20px 48px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        flex-wrap: wrap;
        gap: 12px;
    }

    .cert-id {
        font-size: 0.75rem;
        color: var(--slate);
        font-family: monospace;
    }

    .cert-seal {
        font-size: 2rem;
    }

    /* Print styles */
    @media print {
        .back-link,
        .print-btn,
        .topnav { display: none !important; }

        .page-body { padding-top: 0 !important; }
        .cert-frame { box-shadow: none; border-color: #ccc; }
        body::before { display: none; }
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-wrap">

    <div style="display:flex; align-items:center; justify-content:space-between; margin-bottom:28px;">
        <a href="AssessmentList.aspx" class="back-link">&#8592; Back to Assessments</a>
        <button onclick="window.print()" class="btn-secondary" style="padding:10px 22px;">
            🖨 Print Certificate
        </button>
    </div>

    <div class="cert-frame">

        <%-- Header banner --%>
        <div class="cert-banner">
            <div class="logo-text">Oly<span>Math</span></div>
            <div class="cert-label">Certificate of Completion</div>
        </div>

        <%-- Main body --%>
        <div class="cert-body">
            <p class="cert-presents">This certifies that</p>

            <div class="cert-name">
                <asp:Literal ID="litTraineeName" runat="server" />
            </div>

            <p class="cert-completed">has successfully completed the assessment</p>

            <div class="cert-assessment">
                <asp:Literal ID="litAssessmentTitle" runat="server" />
            </div>

            <div class="cert-meta">

                <div class="meta-block">
                    <div class="meta-val">
                        <asp:Literal ID="litScore" runat="server" />%
                    </div>
                    <div class="meta-label">Final Score</div>
                </div>

                <div class="meta-block">
                    <div class="meta-val">
                        <asp:Literal ID="litIssueDate" runat="server" />
                    </div>
                    <div class="meta-label">Issue Date</div>
                </div>

                <div class="meta-block">
                    <div class="meta-val">✓</div>
                    <div class="meta-label">Verified Pass</div>
                </div>

            </div>
        </div>

        <%-- Footer --%>
        <div class="cert-footer">
            <span class="cert-id">
                Certificate ID: <asp:Literal ID="litCertID" runat="server" />
            </span>
            <span class="cert-seal">🎓</span>
        </div>

    </div>

</div>
</asp:Content>
