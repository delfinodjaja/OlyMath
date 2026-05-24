<%@ Page Title="Home" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OlyMath.Default" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Home
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="landing-hero">
        <div class="landing-tag">Mathematics Training Platform&nbsp;&nbsp;&nbsp; 4</div>
        <h1>Train sharper.<br />Compete <em>smarter.</em></h1>
        <p class="landing-sub">A focused platform for olympiad training — structured modules, expert trainers, real progress.</p>
        <div class="landing-cta">
            <a href="/Account/Register.aspx" class="btn-primary">Get Started</a>
            <a href="/Account/Login.aspx" class="btn-secondary">Sign In</a>
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

</asp:Content>
