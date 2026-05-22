<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="OlyMath.ErrorPage" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Error
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 60px 40px;">
        <h1 style="color: #FF5733;">Error.</h1>
        <h2 style="color: #FF5733;">An error occurred while processing your request.</h2>

        <asp:Panel ID="RequestIdPanel" runat="server" Visible="false">
            <p>
                <strong>Request ID:</strong>
                <code><asp:Literal ID="RequestIdLiteral" runat="server" /></code>
            </p>
        </asp:Panel>

        <h3>Development Mode</h3>
        <p>
            Swapping to <strong>Development</strong> environment will display more detailed information
            about the error that occurred.
        </p>
        <p>
            <strong>The Development environment shouldn't be enabled for deployed applications.</strong>
            For local debugging, set <code>compilation debug="true"</code> in <code>Web.config</code>.
        </p>
    </div>
</asp:Content>
