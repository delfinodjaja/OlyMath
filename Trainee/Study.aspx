<%@ Page Title="OlyMath - Study Materials" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Study.aspx.cs" Inherits="OlyMath.Trainee.Study" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-study">
        <div class="page-title">Study Materials</div>
        <div class="page-subtitle"><asp:Literal ID="litModuleTitle" runat="server" /></div>
        
        <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom:20px;" />
        
        <!-- MATERIALS LIST -->
        <div>
            <asp:Repeater ID="rptStudyMaterials" runat="server" OnItemCommand="rptStudyMaterials_ItemCommand">
                <ItemTemplate>
                    <div class="list-item" style='<%# Eval("IsLocked").ToString() == "1" ? "opacity:0.5; pointer-events:none;" : "" %>'>
                        <div class='list-item-icon <%# GetIconClass(Eval("Type").ToString()) %>'>
                            <%# Eval("Type") %>
                        </div>
                        <div class="list-item-body">
                            <div class="list-item-title"><%# Eval("Title") %></div>
                            <div class="list-item-sub">
                                <%# Eval("Type").ToString() == "QZ" ? "Quiz" : Eval("Type").ToString() == "VID" ? "Video" : "Reading" %> 
                                · <%# Eval("FileSizeText") %>
                                · Uploaded <%# Eval("UploadDate", "{0:MMM dd, yyyy}") %>
                                · <%# Eval("IsDone").ToString() == "1" ? "Completed" : "Not read yet" %>
                            </div>
                        </div>

                        <!-- Action Button -->
                        <asp:LinkButton ID="btnAction" runat="server" 
                            CommandName="StudyMaterial" 
                            CommandArgument='<%# Eval("Id") %>' 
                            CssClass='<%# GetButtonClass(Eval("IsDone"), Eval("IsLocked")) %>'>
                            <%# GetButtonText(Eval("Type").ToString(), Eval("IsDone"), Eval("IsLocked")) %>
                        </asp:LinkButton>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div style="margin-top:24px;">
            <asp:Button ID="btnProceed" runat="server" Text="Proceed to Assessment" CssClass="btn-primary" OnClick="btnProceed_Click" />
            <a href='ModuleDetail.aspx?id=<%= ModuleId %>' class="btn-secondary" style="margin-left:10px;">Back to Module</a>
        </div>
    </div>
</asp:Content>
