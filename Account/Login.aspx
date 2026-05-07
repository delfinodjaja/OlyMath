
<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="OlyMath.Account.Login" %>


<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Login
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="auth-wrap">
        <div class="auth-card">
            <div class="auth-header">
                <h2>Welcome back.</h2>
                <p>Sign in to continue your journey.</p>
            </div>

            <div class="auth-tabs">
                <a href="/Account/Login.aspx" class="auth-tab active">Login</a>
                <a href="/Account/Register.aspx" class="auth-tab">Register</a>
            </div>

            <!-- Error message panel -->
            <asp:Panel ID="ErrorPanel" runat="server" Visible="false">
                <div class="validation-summary-errors">
                    <ul>
                        <li><asp:Literal ID="ErrorMessage" runat="server" /></li>
                    </ul>
                </div>
            </asp:Panel>

            <div class="form-group">
                <label for="EmailInput">Email</label>
                <asp:TextBox ID="EmailInput" runat="server" TextMode="Email" placeholder="you@email.com" CssClass="form-control" />
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
                <asp:TextBox ID="PasswordInput" runat="server" TextMode="Password" placeholder="••••••••" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
                    ControlToValidate="PasswordInput"
                    ErrorMessage="Password is required."
                    CssClass="field-validation-error"
                    Display="Dynamic" />
            </div>

            <asp:Button ID="LoginButton" runat="server" Text="Sign In"
                CssClass="btn-primary btn-full"
                Style="margin-top:8px"
                OnClick="LoginButton_Click" />

            <p class="form-hint">
                Don't have an account? <a href="/Account/Register.aspx">Register here</a>
            </p>
            <p class="form-hint">
                <a href="/Default.aspx">← Back to home</a>
            </p>
        </div>
    </div>

</asp:Content>
