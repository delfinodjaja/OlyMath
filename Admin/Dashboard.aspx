<%@ Page Title="OlyMath - Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="OlyMath.Admin.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="admin-dash">
        <div class="page-title">Admin Dashboard</div>
        <div class="page-subtitle">Monitor OlyMath platform activities, metrics, and manage user memberships.</div>

        <!-- STATS GRID -->
        <div class="stat-grid">
            <div class="stat-card">
                <div class="stat-val stat-teal"><asp:Literal ID="litTotalUsers" runat="server">0</asp:Literal></div>
                <div class="stat-label">Total Users</div>
            </div>
            <div class="stat-card">
                <div class="stat-val" style="color: #5B60F0;"><asp:Literal ID="litTotalTrainers" runat="server">0</asp:Literal></div>
                <div class="stat-label">Trainers</div>
            </div>
            <div class="stat-card">
                <div class="stat-val stat-accent"><asp:Literal ID="litActiveModules" runat="server">0</asp:Literal></div>
                <div class="stat-label">Active Modules</div>
            </div>
            <div class="stat-card">
                <div class="stat-val" style="color: var(--teal);"><asp:Literal ID="litTotalEnrollments" runat="server">0</asp:Literal></div>
                <div class="stat-label">Enrollments</div>
            </div>
        </div>

        <!-- RECENT REGISTRATIONS -->
        <div class="section-head" style="margin-top: 30px;">
            <div class="section-title">Recent Registrations</div>
        </div>

        <div class="table-wrap" style="margin-bottom: 30px;">
            <table>
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Role</th>
                        <th>Join Date</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptRecentUsers" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td style="font-weight: 500;"><%# HttpUtility.HtmlEncode(Eval("FullName")) %></td>
                                <td><%# HttpUtility.HtmlEncode(Eval("Email")) %></td>
                                <td><span class="badge grey"><%# Eval("Role") %></span></td>
                                <td><%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></td>
                                <td>
                                    <span class='badge <%# string.Equals(Eval("Status").ToString(), "Approved", StringComparison.OrdinalIgnoreCase) ? "green" : "orange" %>'>
                                        <%# Eval("Status") %>
                                    </span>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="phNoRecent" runat="server" Visible="false">
                        <tr>
                            <td colspan="5" style="text-align:center; color:var(--slate); padding:20px;">
                                No recent registrations found.
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>

        <!-- QUICK LINKS -->
        <div class="section-head">
            <div class="section-title">System Administration</div>
        </div>

        <div class="module-grid" style="grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));">
            <a href="ManageUsers.aspx" class="module-card">
                <div class="module-tag">Membership</div>
                <h4>User Management</h4>
                <div class="module-meta">
                    <span style="font-size:0.8rem; color:var(--slate); line-height: 1.5;">Approve pending accounts, warn users, or edit details.</span>
                </div>
            </a>
            
            <a href="ModuleOversight.aspx" class="module-card">
                <div class="module-tag">Oversight</div>
                <h4>Module Oversight</h4>
                <div class="module-meta">
                    <span style="font-size:0.8rem; color:var(--slate); line-height: 1.5;">Review and approve trainer course modules before public listing.</span>
                </div>
            </a>

            <a href="EnrollmentManagement.aspx" class="module-card">
                <div class="module-tag">Enrollments</div>
                <h4>Enrollment Management</h4>
                <div class="module-meta">
                    <span style="font-size:0.8rem; color:var(--slate); line-height: 1.5;">Track and manage active course enrollments and trainee progress.</span>
                </div>
            </a>

            <a href="../Trainee/Discussion.aspx" class="module-card teal">
                <div class="module-tag teal">Moderation</div>
                <h4>Discussion Zone</h4>
                <div class="module-meta">
                    <span style="font-size:0.8rem; color:var(--slate); line-height: 1.5;">Manage posts, clear flags, handle moderation logs, or post global pins.</span>
                </div>
            </a>
        </div>
    </div>
</asp:Content>

