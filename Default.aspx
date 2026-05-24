<%@ Page Title="OlyMath - Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OlyMath.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- Page specific styling if any -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="landing" class="page active">
        <div class="landing-hero">
            <div class="landing-tag">Mathematics Training Platform</div>
            <h1>Train sharper.<br>Compete <em>smarter.</em></h1>
            <p class="landing-sub">A focused platform for olympiad training — structured modules, expert trainers, real progress.</p>
            
            <div class="landing-cta">
                <asp:PlaceHolder ID="phGuestActions" runat="server">
                    <a href="Login.aspx?mode=register" class="btn-primary">Get Started</a>
                    <a href="Login.aspx" class="btn-secondary">Sign In</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phMemberActions" runat="server" Visible="false">
                    <asp:HyperLink ID="lnkDashboard" runat="server" CssClass="btn-primary" Text="Go to Dashboard" />
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="landing-features">
            <div class="feat-card">
                <div class="feat-num">01</div>
                <h3>Structured Modules</h3>
                <p>Carefully curated problem sets and study materials designed by experienced trainers.</p>
            </div>
            <div class="feat-card">
                <div class="feat-num">02</div>
                <h3>Track Progress</h3>
                <p>Detailed analytics on every assessment — know exactly where to improve next.</p>
            </div>
            <div class="feat-card">
                <div class="feat-num">03</div>
                <h3>Global Community</h3>
                <p>Connect with trainees and trainers worldwide in the Discussion Zone.</p>
            </div>
        </div>
    </div>
</asp:Content>
