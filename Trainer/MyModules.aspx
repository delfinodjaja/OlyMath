<%@ Page Title="OlyMath - My Modules" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyModules.aspx.cs" Inherits="OlyMath.Trainer.MyModules" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainer-mymodules" class="fadeInUp">
        <div class="page-title">My Modules</div>
        <div class="page-subtitle">Configure, publish, and manage your training modules.</div>
        
        <div class="section-head">
            <div class="section-title">Module List</div>
            <a href="EditModule.aspx" class="btn-primary" style="padding: 8px 20px; font-size: 0.82rem;">+ New Module</a>
        </div>
        
        <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom: 20px; display:block;" />
        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />

        <div class="table-wrap">
            <table>
                <thead>
                    <tr>
                        <th>Topic</th>
                        <th>Module Title</th>
                        <th>Enrolled Trainees</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptModules" runat="server" OnItemCommand="rptModules_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td><span class="badge green"><%# HttpUtility.HtmlEncode(Eval("Topic")) %></span></td>
                                <td style="font-weight: 500;"><%# HttpUtility.HtmlEncode(Eval("Title")) %></td>
                                <td><%# Eval("EnrolledCount") %></td>
                                <td>
                                    <span class='badge <%# string.Equals(Eval("Status").ToString(), "Draft", StringComparison.OrdinalIgnoreCase) ? "grey" : "green" %>'>
                                        <%# HttpUtility.HtmlEncode(Eval("Status")) %>
                                    </span>
                                </td>
                                <td>
                                    <a href='EditModule.aspx?id=<%# Eval("Id") %>' class="badge green" style="background:#EEF0FF; color:#5B60F0;">Edit Details</a>
                                    <a href='ManageMaterials.aspx?id=<%# Eval("Id") %>' class="badge green" style="background:#E6FBF7; color:#00A383;">Materials</a>
                                    <a href='ManageQuestions.aspx?id=<%# Eval("Id") %>' class="badge green" style="background:#FFF0EC; color:var(--orange);">Questions</a>
                                    <asp:LinkButton ID="btnDelete" runat="server" 
                                        CommandName="DeleteModule" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="badge orange" 
                                        OnClientClick="return confirm('Are you sure you want to delete this module and all its associated materials and quiz questions? This action cannot be undone.');" 
                                        style="border:none; cursor:pointer;">
                                        Delete
                                    </asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <asp:PlaceHolder ID="phNoModules" runat="server" Visible="false">
                        <tr>
                            <td colspan="5" style="text-align:center; color:var(--slate); padding:30px;">
                                You haven't created any modules yet. Click '+ New Module' above to get started!
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
