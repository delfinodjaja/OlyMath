<%@ Page Title="Edit Question" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="EditQuestion.aspx.cs"
    Inherits="OlyMath.EditQuestion" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
<style>
    .page-wrap {
        max-width: 700px;
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

    .options-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 16px;
    }

    .option-label {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.8rem;
        font-weight: 500;
        color: var(--navy);
        margin-bottom: 7px;
    }

    .option-tag {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        width: 22px;
        height: 22px;
        border-radius: 50%;
        background: var(--navy);
        color: var(--white);
        font-size: 0.72rem;
        font-weight: 700;
        flex-shrink: 0;
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

    .divider {
        border: none;
        border-top: 1px solid var(--border);
        margin: 28px 0;
    }
</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-wrap">

    <a id="backLink" runat="server" class="back-link">&#8592; Back to Questions</a>

    <div class="form-card">
        <h1>Edit Question</h1>
        <p class="subtitle">
            Assessment: <strong><asp:Literal ID="litAssessmentTitle" runat="server" /></strong>
        </p>

        <asp:HiddenField ID="hfQuestionID"   runat="server" />
        <asp:HiddenField ID="hfAssessmentID" runat="server" />

        <asp:Panel ID="pnlError" runat="server" Visible="false">
            <div class="alert-msg alert-error">
                <asp:Literal ID="litError" runat="server" />
            </div>
        </asp:Panel>

        <div class="form-group">
            <label for="txtQuestion">Question Text <span style="color:var(--orange)">*</span></label>
            <asp:TextBox ID="txtQuestion" runat="server"
                TextMode="MultiLine" Rows="3" MaxLength="2000" />
            <asp:RequiredFieldValidator ID="rfvQuestion" runat="server"
                ControlToValidate="txtQuestion"
                ErrorMessage="Question text is required."
                CssClass="field-validation-error"
                Display="Dynamic" />
        </div>

        <hr class="divider" />

        <div class="options-grid">

            <div class="form-group">
                <div class="option-label">
                    <span class="option-tag">A</span> Option A
                    <span style="color:var(--orange)">*</span>
                </div>
                <asp:TextBox ID="txtOptionA" runat="server" MaxLength="500" />
                <asp:RequiredFieldValidator ID="rfvA" runat="server"
                    ControlToValidate="txtOptionA"
                    ErrorMessage="Option A is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

            <div class="form-group">
                <div class="option-label">
                    <span class="option-tag">B</span> Option B
                    <span style="color:var(--orange)">*</span>
                </div>
                <asp:TextBox ID="txtOptionB" runat="server" MaxLength="500" />
                <asp:RequiredFieldValidator ID="rfvB" runat="server"
                    ControlToValidate="txtOptionB"
                    ErrorMessage="Option B is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

            <div class="form-group">
                <div class="option-label">
                    <span class="option-tag">C</span> Option C
                    <span style="color:var(--orange)">*</span>
                </div>
                <asp:TextBox ID="txtOptionC" runat="server" MaxLength="500" />
                <asp:RequiredFieldValidator ID="rfvC" runat="server"
                    ControlToValidate="txtOptionC"
                    ErrorMessage="Option C is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

            <div class="form-group">
                <div class="option-label">
                    <span class="option-tag">D</span> Option D
                    <span style="color:var(--orange)">*</span>
                </div>
                <asp:TextBox ID="txtOptionD" runat="server" MaxLength="500" />
                <asp:RequiredFieldValidator ID="rfvD" runat="server"
                    ControlToValidate="txtOptionD"
                    ErrorMessage="Option D is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

        </div>

        <hr class="divider" />

        <div class="form-group">
            <label>Correct Answer <span style="color:var(--orange)">*</span></label>
            <asp:RadioButtonList ID="rblCorrectAnswer" runat="server"
                RepeatDirection="Horizontal"
                RepeatLayout="Flow"
                CssClass="answer-radios">
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
            <asp:Button ID="btnSave" runat="server"
                Text="Save Changes"
                CssClass="btn-primary"
                OnClick="btnSave_Click" />
            <a id="cancelLink" runat="server" class="btn-secondary">Cancel</a>
        </div>
    </div>

</div>
</asp:Content>
