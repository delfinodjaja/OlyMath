<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="OlyMath.Account.Register" %>


<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="auth-wrap">
        <div class="auth-card">
            <div class="auth-header">
                <h2>Create account.</h2>
                <p>Join OlyMath and start training.</p>
            </div>

            <div class="auth-tabs">
                <a href="/Account/Login.aspx" class="auth-tab">Login</a>
                <a href="/Account/Register.aspx" class="auth-tab active">Register</a>
            </div>

            <!-- Error messages -->
            <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
                <div class="validation-summary-errors">
                    <ul>
                        <asp:Literal ID="ErrorMessages" runat="server" />
                    </ul>
                </div>
            </asp:Panel>

            <div class="form-group">
                <label for="NameInput">Full Name</label>
                <asp:TextBox ID="NameInput" runat="server" placeholder="Your full name" />
                <asp:RequiredFieldValidator ID="NameRequired" runat="server"
                    ControlToValidate="NameInput"
                    ErrorMessage="Name is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

            <div class="form-group">
                <label for="EmailInput">Email</label>
                <asp:TextBox ID="EmailInput" runat="server" TextMode="Email" placeholder="you@email.com" />
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server"
                    ControlToValidate="EmailInput"
                    ErrorMessage="Email is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
                <asp:RegularExpressionValidator ID="EmailFormat" runat="server"
                    ControlToValidate="EmailInput"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                    ErrorMessage="Invalid email address."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

            <div class="form-group">
                <label for="PasswordInput">Password</label>
                <asp:TextBox ID="PasswordInput" runat="server" TextMode="Password" placeholder="Min. 6 characters" />
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
                    ControlToValidate="PasswordInput"
                    ErrorMessage="Password is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
                <asp:RegularExpressionValidator ID="PasswordLength" runat="server"
                    ControlToValidate="PasswordInput"
                    ValidationExpression=".{6,}"
                    ErrorMessage="Password must be at least 6 characters."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

            <div class="form-group">
                <label for="RoleDropDown">Register as</label>
                <asp:DropDownList ID="RoleDropDown" runat="server">
                    <asp:ListItem Value="Trainee">Trainee</asp:ListItem>
                    <asp:ListItem Value="Trainer">Trainer</asp:ListItem>
                </asp:DropDownList>
            </div>

            <asp:Button ID="RegisterButton" runat="server" Text="Create Account"
                CssClass="btn-primary btn-full"
                Style="margin-top:8px"
                OnClick="RegisterButton_Click" />

            <p class="form-hint">
                Already have an account? <a href="/Account/Login.aspx">Sign in</a>
            </p>
            <p class="form-hint">
                <a href="/Default.aspx">← Back to home</a>
            </p>
        </div>
    </div>

</asp:Content>
