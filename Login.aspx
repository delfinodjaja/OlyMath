<%@ Page Title="OlyMath - Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="OlyMath.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function switchTab(tab) {
            document.getElementById('<%= hfActiveTab.ClientID %>').value = tab;
            if (tab === 'login') {
                document.getElementById('tab-login').classList.add('active');
                document.getElementById('tab-register').classList.remove('active');
                document.getElementById('login-form').classList.remove('hidden');
                document.getElementById('register-form').classList.add('hidden');
                document.getElementById('auth-title').innerText = 'Welcome back.';
                document.getElementById('auth-desc').innerText = 'Sign in to continue your journey.';
            } else {
                document.getElementById('tab-login').classList.remove('active');
                document.getElementById('tab-register').classList.add('active');
                document.getElementById('login-form').classList.add('hidden');
                document.getElementById('register-form').classList.remove('hidden');
                document.getElementById('auth-title').innerText = 'Join OlyMath.';
                document.getElementById('auth-desc').innerText = 'Create an account to start training.';
            }
        }

        // Apply state on load
        window.addEventListener('DOMContentLoaded', (event) => {
            var active = document.getElementById('<%= hfActiveTab.ClientID %>').value;
            switchTab(active);
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="auth-wrapper">
        <div class="auth-card">
            
            <asp:HiddenField ID="hfActiveTab" runat="server" Value="login" />

            <div class="auth-header">
                <h2 id="auth-title">Welcome back.</h2>
                <p id="auth-desc">Sign in to continue your journey.</p>
                
                <!-- Server error/success displays -->
                <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" />
                <asp:Label ID="lblSuccess" runat="server" CssClass="form-success" Visible="false" />
            </div>

            <!-- TABS -->
            <div class="auth-tabs">
                <button type="button" class="auth-tab active" id="tab-login" onclick="switchTab('login')">Login</button>
                <button type="button" class="auth-tab" id="tab-register" onclick="switchTab('register')">Register</button>
            </div>

            <!-- LOGIN FORM -->
            <div id="login-form">
                <div class="form-group">
                    <label>Email Address</label>
                    <asp:TextBox ID="txtLoginEmail" runat="server" TextMode="Email" placeholder="you@email.com" />
                </div>
                <div class="form-group">
                    <label>Password</label>
                    <asp:TextBox ID="txtLoginPassword" runat="server" TextMode="Password" placeholder="••••••••" />
                </div>
                <div class="form-group">
                    <label>Login as</label>
                    <asp:DropDownList ID="ddlLoginRole" runat="server">
                        <asp:ListItem Value="Trainee">Trainee</asp:ListItem>
                        <asp:ListItem Value="Trainer">Trainer</asp:ListItem>
                        <asp:ListItem Value="Admin">Admin</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="btn-primary btn-full" OnClick="btnSignIn_Click" style="margin-top:8px" />
            </div>

            <!-- REGISTER FORM -->
            <div id="register-form" class="hidden">
                <div class="form-group">
                    <label>Full Name</label>
                    <asp:TextBox ID="txtRegName" runat="server" placeholder="Your name" />
                </div>
                <div class="form-group">
                    <label>Email Address</label>
                    <asp:TextBox ID="txtRegEmail" runat="server" TextMode="Email" placeholder="you@email.com" />
                </div>
                <div class="form-group">
                    <label>Password</label>
                    <asp:TextBox ID="txtRegPassword" runat="server" TextMode="Password" placeholder="Create password" />
                </div>
                <div class="form-group">
                    <label>Register as</label>
                    <asp:DropDownList ID="ddlRegRole" runat="server">
                        <asp:ListItem Value="Trainee">Trainee</asp:ListItem>
                        <asp:ListItem Value="Trainer">Trainer</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:Button ID="btnRegister" runat="server" Text="Create Account" CssClass="btn-primary btn-full" OnClick="btnRegister_Click" style="margin-top:8px" />
            </div>

            <p class="form-hint" id="auth-hint">Back to <a href="Default.aspx">home</a></p>
        </div>
    </div>
</asp:Content>
