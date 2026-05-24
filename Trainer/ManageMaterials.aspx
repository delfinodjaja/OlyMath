<%@ Page Title="OlyMath - Manage Materials" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageMaterials.aspx.cs" Inherits="OlyMath.Trainer.ManageMaterials" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainer-manage-materials">
        <div class="page-title">Manage Study Materials</div>
        <div class="page-subtitle">Module: <span style="font-weight: 600; color: var(--navy);"><asp:Literal ID="litModuleTitle" runat="server" /></span></div>
        
        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />
        <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom: 20px; display:block;" />

        <!-- EXISTING MATERIALS TABLE -->
        <div class="content-card">
            <h3>Current Course Materials</h3>
            <div class="table-wrap" style="margin-bottom: 0;">
                <table>
                    <thead>
                        <tr>
                            <th style="width: 80px;">Order</th>
                            <th>Material Title</th>
                            <th>Type</th>
                            <th>Metadata / Size</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptMaterials" runat="server" OnItemCommand="rptMaterials_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td><strong><%# Eval("OrderIndex") %></strong></td>
                                    <td style="font-weight: 500;"><%# HttpUtility.HtmlEncode(Eval("Title")) %></td>
                                    <td><span class="badge grey"><%# Eval("Type") %></span></td>
                                    <td><%# HttpUtility.HtmlEncode(Eval("FileSizeText")) %></td>
                                    <td>
                                        <span class='badge <%# Eval("IsLocked").ToString() == "1" ? "orange" : "green" %>'>
                                            <%# Eval("IsLocked").ToString() == "1" ? "Locked" : "Unlocked" %>
                                        </span>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="btnDelete" runat="server" 
                                            CommandName="DeleteMaterial" 
                                            CommandArgument='<%# Eval("Id") %>' 
                                            CssClass="badge orange" 
                                            OnClientClick="return confirm('Are you sure you want to delete this study material?');" 
                                            style="border:none; cursor:pointer;">
                                            Delete
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        
                        <asp:PlaceHolder ID="phNoMaterials" runat="server" Visible="false">
                            <tr>
                                <td colspan="6" style="text-align:center; color:var(--slate); padding:20px;">
                                    No study materials have been added to this module yet. Fill the form below to add one!
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- ADD MATERIAL FORM -->
        <div class="content-card" style="margin-top: 24px;">
            <h3>Add New Study Material</h3>
            
            <div class="form-grid">
                <div class="form-group">
                    <label>Material Title</label>
                    <asp:TextBox ID="txtTitle" runat="server" placeholder="e.g., Chinese Remainder Theorem" />
                </div>
                <div class="form-group">
                    <label>Material Type</label>
                    <asp:DropDownList ID="ddlType" runat="server">
                        <asp:ListItem Value="PDF">PDF Reading</asp:ListItem>
                        <asp:ListItem Value="VID">Video Lecture</asp:ListItem>
                        <asp:ListItem Value="QZ">Self-Assessment Link</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Metadata / Size (e.g., 2.4 MB or 15 min)</label>
                    <asp:TextBox ID="txtSize" runat="server" placeholder="e.g., 1.2 MB or 24 min" />
                </div>
                <div class="form-group">
                    <label>Content File / URL Link</label>
                    <asp:TextBox ID="txtUrl" runat="server" placeholder="e.g., https://youtube.com/watch?v=... or crt.pdf" />
                </div>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Ordering Index (e.g., 1, 2, 3)</label>
                    <asp:TextBox ID="txtOrder" runat="server" TextMode="Number" placeholder="1" />
                </div>
                <div class="form-group" style="display:flex; align-items:center; height:100%; padding-top:20px;">
                    <label style="display:flex; align-items:center; gap:8px; cursor:pointer; font-size:0.88rem;">
                        <asp:CheckBox ID="chkLocked" runat="server" />
                        <span>Lock material initially (requires higher progress)</span>
                    </label>
                </div>
            </div>

            <div class="form-actions">
                <a href="Dashboard.aspx" class="btn-secondary" style="padding: 10px 24px; font-size:0.88rem;">Back to Dashboard</a>
                <asp:Button ID="btnAdd" runat="server" Text="Add Material" CssClass="btn-primary" OnClick="btnAdd_Click" style="padding: 10px 28px; font-size:0.88rem;" />
            </div>
        </div>
    </div>
</asp:Content>
