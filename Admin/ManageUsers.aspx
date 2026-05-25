<%@ Page Title="OlyMath - Manage Users" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="OlyMath.Admin.ManageUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="admin-manage-users">
        <div class="page-title">Manage Users</div>
        <div class="page-subtitle">View, create, update, or remove trainees, trainers, and administrators.</div>

        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />
        <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom: 20px; display:block;" />

        <!-- Hidden field to track editing state -->
        <asp:HiddenField ID="hfEditUserId" runat="server" Value="" />

        <!-- SYSTEM USER GRID -->
        <div class="content-card">
            <h3>Registered Members</h3>
            <div class="table-wrap" style="margin-bottom: 0;">
                <table>
                    <thead>
                        <tr>
                            <th style="width: 60px;">ID</th>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Role</th>
                            <th>Location</th>
                            <th>Joined</th>
                            <th>Warnings</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptUsers" runat="server" OnItemCommand="rptUsers_ItemCommand">
                            <ItemTemplate>
                                <tr style='<%# Eval("Id").ToString() == Session["UserId"].ToString() ? "background: #F0FFF8;" : "" %>'>
                                    <td><strong><%# Eval("Id") %></strong></td>
                                    <td style="font-weight: 500;"><%# HttpUtility.HtmlEncode(Eval("FullName")) %></td>
                                    <td><%# HttpUtility.HtmlEncode(Eval("Email")) %></td>
                                    <td>
                                        <span class='badge <%# Eval("Role").ToString() == "Admin" ? "orange" : Eval("Role").ToString() == "Trainer" ? "green" : "grey" %>'>
                                            <%# Eval("Role") %>
                                        </span>
                                    </td>
                                    <td><%# HttpUtility.HtmlEncode(Eval("CityCountry")) %></td>
                                    <td><%# Eval("CreatedAt", "{0:MMM dd, yyyy}") %></td>
                                    <td style="text-align: center;">
                                        <span class='badge <%# Convert.ToInt32(Eval("WarningsCount")) > 0 ? "orange" : "grey" %>'>
                                            <%# Eval("WarningsCount") %>
                                        </span>
                                    </td>
                                    <td>
                                        <span class='badge <%# string.Equals(Eval("Status").ToString(), "Approved", StringComparison.OrdinalIgnoreCase) ? "green" : "orange" %>'>
                                            <%# Eval("Status") %>
                                        </span>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="btnApprove" runat="server" 
                                            CommandName="ApproveUser" 
                                            CommandArgument='<%# Eval("Id") %>' 
                                            CssClass="badge green" 
                                            Visible='<%# string.Equals(Eval("Status").ToString(), "Pending", StringComparison.OrdinalIgnoreCase) %>'
                                            style="border:none; cursor:pointer; background:#E6FBF7; color:#00A383; margin-right: 4px;">
                                            Approve
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="btnWarn" runat="server" 
                                            CommandName="WarnUser" 
                                            CommandArgument='<%# Eval("Id") %>' 
                                            CssClass="badge orange" 
                                            style="border:none; cursor:pointer; margin-right: 4px;">
                                            Warn
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="btnEdit" runat="server" 
                                            CommandName="EditUser" 
                                            CommandArgument='<%# Eval("Id") %>' 
                                            CssClass="badge green" 
                                            style="border:none; cursor:pointer; background:#EEF0FF; color:#5B60F0; margin-right: 4px;">
                                            Edit
                                        </asp:LinkButton>
                                        
                                        <!-- Disable deleting oneself -->
                                        <asp:LinkButton ID="btnDelete" runat="server" 
                                            CommandName="DeleteUser" 
                                            CommandArgument='<%# Eval("Id") %>' 
                                            CssClass="badge orange" 
                                            Visible='<%# Eval("Id").ToString() != Session["UserId"].ToString() %>'
                                            OnClientClick="return confirm('Are you sure you want to delete this user? All their progress records, assessments, and discussion replies will be permanently deleted.');" 
                                            style="border:none; cursor:pointer;">
                                            Delete
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- CREATE / EDIT USER FORM -->
        <div class="content-card" style="margin-top: 24px;">
            <h3><asp:Literal ID="litFormTitle" runat="server">Create New User Account</asp:Literal></h3>
            
            <div class="form-grid">
                <div class="form-group">
                    <label>Full Name</label>
                    <asp:TextBox ID="txtName" runat="server" placeholder="e.g., Alexandra Reyes" />
                </div>
                <div class="form-group">
                    <label>Email Address</label>
                    <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="you@email.com" />
                </div>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Password <span ID="spanPasswordHint" runat="server" style="color:var(--slate); font-weight:normal; font-size:0.75rem;">(Leave blank to keep current)</span></label>
                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="••••••••" />
                </div>
                <div class="form-group">
                    <label>Membership Role</label>
                    <asp:DropDownList ID="ddlRole" runat="server">
                        <asp:ListItem Value="Trainee">Trainee</asp:ListItem>
                        <asp:ListItem Value="Trainer">Trainer</asp:ListItem>
                        <asp:ListItem Value="Admin">Admin</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Location (City, Country Code)</label>
                    <asp:TextBox ID="txtLocation" runat="server" placeholder="e.g., Tokyo, JP" />
                </div>
                <div></div>
            </div>

            <div class="form-actions">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" OnClick="btnCancel_Click" Visible="false" style="padding: 10px 24px; font-size:0.88rem;" />
                <asp:Button ID="btnSave" runat="server" Text="Save User" CssClass="btn-primary" OnClick="btnSave_Click" style="padding: 10px 28px; font-size:0.88rem;" />
            </div>
        </div>
    </div>
</asp:Content>
