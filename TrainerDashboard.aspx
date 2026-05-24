<%@ Page Title="Trainer Dashboard" Language="C#" MasterPageFile="~/Dashboard.Master"
    AutoEventWireup="true" CodeBehind="TrainerDashboard.aspx.cs"
    Inherits="OlyMath.TrainerDashboard" %>

<asp:Content ID="TitleContent" ContentPlaceHolderID="TitleContent" runat="server">
    Trainer Dashboard
</asp:Content>

<asp:Content ID="SidebarContent" ContentPlaceHolderID="SidebarContent" runat="server">
<div class="sidebar-section">
    <div class="sidebar-label">Main</div>
    <a href="TrainerDashboard.aspx" class="sidebar-item active"><span class="dot"></span> Dashboard</a>
    <a href="AssessmentList.aspx" class="sidebar-item"><span class="dot"></span> Assessment List</a>
    <a href="CreateAssessment.aspx" class="sidebar-item"><span class="dot"></span> Create Assessment</a>
</div>
<div class="sidebar-section">
    <div class="sidebar-label">Learning</div>
    <a href="QuestionList.aspx" class="sidebar-item"><span class="dot"></span> Question List</a>
    <a href="Certificate.aspx" class="sidebar-item"><span class="dot"></span> Certificates</a>
</div>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="page-title">Trainer Dashboard</div>
<div class="page-subtitle">Manage assessments, questions, and certificates from here.</div>

<div class="content-card">
    <h3>Quick Actions</h3>
    <div style="display:flex;flex-wrap:wrap;gap:12px;">
        <a href="AssessmentList.aspx" class="btn-primary">View Assessments</a>
        <a href="CreateAssessment.aspx" class="btn-secondary">Create Assessment</a>
        <a href="QuestionList.aspx" class="btn-secondary">Question List</a>
        <a href="Certificate.aspx" class="btn-secondary">Certificates</a>
    </div>
</div>
</asp:Content>