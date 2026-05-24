<%@ Page Title="OlyMath - Browse Modules" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BrowseModules.aspx.cs" Inherits="OlyMath.Trainee.BrowseModules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-modules">
        <div class="page-title">Browse Modules</div>
        <div class="page-subtitle">Explore all available training modules.</div>
        
        <div class="module-grid">
            <asp:Repeater ID="rptAllModules" runat="server">
                <ItemTemplate>
                    <a href='ModuleDetail.aspx?id=<%# Eval("Id") %>' class='module-card <%# GetTopicClass(Eval("Topic").ToString()) %>'>
                        <div class='module-tag <%# GetTopicClass(Eval("Topic").ToString()) %>'><%# Eval("Topic") %></div>
                        <h4><%# Eval("Title") %></h4>
                        <div class="module-meta">
                            <span><%# Eval("MaterialCount") %> materials</span>
                            <span style="font-weight: 500; color: var(--navy);"><%# GetEnrollmentStatus(Eval("ProgressPercentage")) %></span>
                        </div>
                        <div class="progress-bar-wrap">
                            <div class="progress-bar" style='width:<%# Eval("ProgressPercentage") == DBNull.Value ? "0" : Eval("ProgressPercentage") %>%'></div>
                        </div>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
