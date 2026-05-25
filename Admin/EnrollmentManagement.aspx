<%@ Page Title="OlyMath - Enrollment Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EnrollmentManagement.aspx.cs" Inherits="OlyMath.Admin.EnrollmentManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .progress-bar-container {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .progress-text {
            font-size: 0.8rem;
            font-weight: 600;
            color: var(--navy);
            min-width: 32px;
        }
        .progress-bar-wrap {
            flex-grow: 1;
            height: 6px;
            background: var(--bg);
            border-radius: 99px;
            overflow: hidden;
            margin-top: 0;
        }
        .progress-bar {
            height: 100%;
            border-radius: 99px;
            background: var(--teal);
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="admin-enrollments">
        <div class="page-title">Enrollment Management</div>
        <div class="page-subtitle">Track trainee module enrollments and manually unenroll students if necessary.</div>

        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />
        <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom: 20px; display:block;" />

        <div class="table-wrap">
            <table>
                <thead>
                    <tr>
                        <th>Trainee Name</th>
                        <th>Email</th>
                        <th>Module</th>
                        <th>Last Active / Enrolled</th>
                        <th>Progress %</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptEnrollments" runat="server" OnItemCommand="rptEnrollments_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td style="font-weight: 600; color: var(--navy);"><%# HttpUtility.HtmlEncode(Eval("TraineeName")) %></td>
                                <td style="color: var(--slate); font-size: 0.85rem;"><%# HttpUtility.HtmlEncode(Eval("Email")) %></td>
                                <td style="font-weight: 500;"><%# HttpUtility.HtmlEncode(Eval("ModuleTitle")) %></td>
                                <td style="font-size: 0.85rem; color: var(--slate);"><%# Eval("LastAccessed", "{0:MMM dd, yyyy}") %></td>
                                <td>
                                    <div class="progress-bar-container">
                                        <div class="progress-bar-wrap">
                                            <div class="progress-bar" style='width: <%# Eval("ProgressPercentage") %>%'></div>
                                        </div>
                                        <span class="progress-text"><%# Eval("ProgressPercentage") %>%</span>
                                    </div>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnUnenroll" runat="server" 
                                        CommandName="UnenrollTrainee" 
                                        CommandArgument='<%# Eval("UserId") + "," + Eval("ModuleId") %>' 
                                        CssClass="badge orange" 
                                        OnClientClick="return confirm('Are you sure you want to unenroll this trainee? Their progress history and materials completion logs will be permanently deleted for this module.');" 
                                        style="border:none; cursor:pointer;">
                                        Unenroll
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <asp:PlaceHolder ID="phNoEnrollments" runat="server" Visible="false">
                        <tr>
                            <td colspan="6" style="text-align:center; color:var(--slate); padding:30px;">
                                No active course enrollments were found in the database.
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
