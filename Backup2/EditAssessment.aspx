<%@ Page Title="Edit Assessment" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="EditAssessment.aspx.cs"
    Inherits="OlyMath.EditAssessment" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .page-wrap {
        max-width: 680px;
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

    .form-card {
        background: var(--white);
        border: 1px solid var(--border);
        border-radius: var(--radius);
        padding: 40px;
        box-shadow: var(--card-shadow);
    }

    .form-card h1 {
        font-family: 'Syne', sans-serif;
        font-size: 1.8rem;
        font-weight: 800;
        letter-spacing: -0.5px;
        color: var(--navy);
        margin-bottom: 6px;
    }

    .form-card .subtitle {
        font-size: 0.88rem;
        color: var(--slate);
        margin-bottom: 32px;
        font-weight: 300;
    }

    .alert-msg {
        padding: 14px 20px;
        border-radius: 10px;
        margin-bottom: 24px;
        font-size: 0.9rem;
        font-weight: 500;
    }

    .alert-error { background: #fee2e2; color: #991b1b; border: 1px solid #fca5a5; }

    .form-actions {
        display: flex;
        gap: 12px;
        margin-top: 28px;
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-wrap">

    <a href="AssessmentList.aspx" class="back-link">&#8592; Back to Assessments</a>

    <div class="form-card">
        <h1>Edit Assessment</h1>
        <p class="subtitle">Update the assessment details below.</p>

        <asp:HiddenField ID="hfAssessmentID" runat="server" />

        <asp:Panel ID="pnlError" runat="server" Visible="false">
            <div class="alert-msg alert-error">
                <asp:Literal ID="litError" runat="server" />
            </div>
        </asp:Panel>

        <div class="form-group">
            <label for="txtTitle">Title <span style="color:var(--orange)">*</span></label>
            <asp:TextBox ID="txtTitle" runat="server" MaxLength="200" />
            <asp:RequiredFieldValidator ID="rfvTitle" runat="server"
                ControlToValidate="txtTitle"
                ErrorMessage="Title is required."
                CssClass="field-validation-error"
                Display="Dynamic" />
        </div>

        <div class="form-group">
            <label for="txtDescription">Description</label>
            <asp:TextBox ID="txtDescription" runat="server"
                TextMode="MultiLine" Rows="4" MaxLength="1000" />
        </div>

        <div class="form-group">
            <label for="txtPassingScore">Passing Score (%) <span style="color:var(--orange)">*</span></label>
            <asp:TextBox ID="txtPassingScore" runat="server" TextMode="Number" />
            <asp:RequiredFieldValidator ID="rfvPassingScore" runat="server"
                ControlToValidate="txtPassingScore"
                ErrorMessage="Passing score is required."
                CssClass="field-validation-error"
                Display="Dynamic" />
            <asp:RangeValidator ID="rvPassingScore" runat="server"
                ControlToValidate="txtPassingScore"
                MinimumValue="1" MaximumValue="100"
                Type="Integer"
                ErrorMessage="Passing score must be between 1 and 100."
                CssClass="field-validation-error"
                Display="Dynamic" />
        </div>

        <div class="form-actions">
            <asp:Button ID="btnSave" runat="server"
                Text="Save Changes"
                CssClass="btn-primary"
                OnClick="btnSave_Click" />
            <a href="AssessmentList.aspx" class="btn-secondary">Cancel</a>
        </div>
    </div>

</div>
</asp:Content>
