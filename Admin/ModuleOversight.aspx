<%@ Page Title="OlyMath - Module Oversight" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModuleOversight.aspx.cs" Inherits="OlyMath.Admin.ModuleOversight" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="admin-module-oversight">
        <div class="page-title">Module Oversight</div>
        <div class="page-subtitle">Review, approve, or reject training modules uploaded by trainers.</div>

        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />
        <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom: 20px; display:block;" />

        <div class="table-wrap">
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Module Title</th>
                        <th>Trainer</th>
                        <th>Topic</th>
                        <th>Enrolled Trainees</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptModules" runat="server" OnItemCommand="rptModules_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><strong><%# Eval("Id") %></strong></td>
                                <td style="font-weight: 500; color: var(--navy);"><%# HttpUtility.HtmlEncode(Eval("Title")) %></td>
                                <td><%# HttpUtility.HtmlEncode(Eval("TrainerName")) %></td>
                                <td><span class="badge grey"><%# Eval("Topic") %></span></td>
                                <td style="font-weight: 600;"><%# Eval("EnrolledCount") %></td>
                                <td>
                                    <span class='badge <%# string.Equals(Eval("Status").ToString(), "Approved", StringComparison.OrdinalIgnoreCase) ? "green" : string.Equals(Eval("Status").ToString(), "Pending", StringComparison.OrdinalIgnoreCase) ? "orange" : "grey" %>'>
                                        <%# Eval("Status") %>
                                    </span>
                                </td>
                                <td>
                                    <asp:LinkButton ID="btnApprove" runat="server" 
                                        CommandName="ApproveModule" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="badge green" 
                                        Visible='<%# !string.Equals(Eval("Status").ToString(), "Approved", StringComparison.OrdinalIgnoreCase) %>'
                                        style="border:none; cursor:pointer; background:#E6FBF7; color:#00A383; margin-right: 6px;">
                                        Approve
                                    </asp:LinkButton>

                                    <asp:LinkButton ID="btnReject" runat="server" 
                                        CommandName="RejectModule" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="badge orange" 
                                        Visible='<%# !string.Equals(Eval("Status").ToString(), "Rejected", StringComparison.OrdinalIgnoreCase) %>'
                                        style="border:none; cursor:pointer;">
                                        Reject
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <asp:PlaceHolder ID="phNoModules" runat="server" Visible="false">
                        <tr>
                            <td colspan="7" style="text-align:center; color:var(--slate); padding:30px;">
                                No modules are registered in the system.
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
