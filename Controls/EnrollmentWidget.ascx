<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnrollmentWidget.ascx.cs" Inherits="OlyMath.Controls.EnrollmentWidget" %>

<div class="dashboard-widget">
    <h3>My Enrolled Modules</h3>
    <asp:Repeater ID="rptEnrollments" runat="server">
        <HeaderTemplate>
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Module Title</th>
                        <th>Enrolled Date</th>
                        <th>Progress</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("Module.Title") %></td>
                <td><%# Eval("EnrolledAt", "{0:dd MMM yyyy}") %></td>
                <td>
                    <div class="progress">
                        <div class="progress-bar" role="progressbar" style="width: <%# Eval("ProgressStatus") %>" aria-valuenow="<%# Eval("ProgressStatus").ToString().Replace("%","") %>" aria-valuemin="0" aria-valuemax="100">
                            <%# Eval("ProgressStatus") %>
                        </div>
                    </div>
                </td>
                <td>
                    <asp:HyperLink runat="server" NavigateUrl='<%# "~/Trainee/ViewModule.aspx?id=" + Eval("ModuleId") %>' CssClass="btn btn-sm btn-primary">Continue</asp:HyperLink>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    
    <asp:Label ID="lblNoModules" runat="server" Visible="false" Text="You are not enrolled in any modules yet." CssClass="text-muted"></asp:Label>
</div>