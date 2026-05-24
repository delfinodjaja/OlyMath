<%@ Page Title="OlyMath - Module Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModuleDetail.aspx.cs" Inherits="OlyMath.Trainee.ModuleDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-module-detail">
        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom:20px;" />
        
        <!-- HEADER BLOCK -->
        <div class="detail-header" id="divHeader" runat="server">
            <div class="detail-header-text">
                <h2><asp:Literal ID="litTitle" runat="server" /></h2>
                <p><asp:Literal ID="litDescription" runat="server" /></p>
                <div style="display:flex;gap:16px;margin-top:16px;align-items:center;">
                    <asp:Label ID="lblEnrollBadge" runat="server" CssClass="badge green" Text="Enrolled" />
                    <span style="font-size:0.8rem;color:var(--slate)">
                        <asp:Literal ID="litMetaStats" runat="server" />
                    </span>
                </div>
            </div>
            <div class="detail-actions">
                <asp:Button ID="btnEnroll" runat="server" Text="Enroll in Module" CssClass="btn-primary" OnClick="btnEnroll_Click" Visible="false" />
                <asp:Button ID="btnStudyTop" runat="server" Text="Study Now" CssClass="btn-primary" OnClick="btnStudy_Click" Visible="false" />
            </div>
        </div>

        <!-- PROGRESS CARD (Shown only if enrolled) -->
        <div class="content-card" id="divProgressCard" runat="server" visible="false">
            <h3>Module Progress</h3>
            <div style="display:flex;align-items:center;gap:16px">
                <div style="flex:1">
                    <div style="display:flex;justify-content:space-between;margin-bottom:8px">
                        <span style="font-size:0.82rem;color:var(--slate)">Overall completion</span>
                        <span style="font-size:0.82rem;font-weight:600;color:var(--navy)">
                            <asp:Literal ID="litProgressText" runat="server" />%
                        </span>
                    </div>
                    <div class="progress-bar-wrap" style="height:7px">
                        <div class="progress-bar" id="divProgressBar" runat="server"></div>
                    </div>
                </div>
            </div>
        </div>

        <!-- MATERIALS LIST -->
        <div class="content-card">
            <h3>Materials</h3>
            <asp:Repeater ID="rptMaterials" runat="server">
                <ItemTemplate>
                    <div class="material-item" style='<%# Eval("IsLocked").ToString() == "1" ? "opacity:0.6;" : "" %>'>
                        <div class='mat-icon <%# GetMaterialIconClass(Eval("Type").ToString()) %>'>
                            <%# Eval("Type") %>
                        </div>
                        <div class="mat-body">
                            <div class="mat-title"><%# Eval("Title") %></div>
                            <div class="mat-size"><%# GetMaterialMeta(Eval("Type").ToString(), Eval("FileSizeText").ToString(), Eval("IsLocked").ToString()) %></div>
                        </div>
                        <span class='badge <%# GetStatusBadgeClass(Eval("IsDone"), Eval("IsLocked")) %>'>
                            <%# GetStatusText(Eval("IsDone"), Eval("IsLocked")) %>
                        </span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="phNoMaterials" runat="server" Visible="false">
                <p style="color:var(--slate); text-align:center; padding:15px;">No study materials have been added to this module yet.</p>
            </asp:PlaceHolder>
        </div>

        <!-- FOOTER BUTTONS -->
        <div style="display:flex;gap:12px;" id="divFooterActions" runat="server" visible="false">
            <asp:Button ID="btnStudyFooter" runat="server" Text="Study Materials" CssClass="btn-primary" OnClick="btnStudy_Click" />
            <asp:Button ID="btnAssessment" runat="server" Text="Take Assessment" CssClass="btn-secondary" OnClick="btnAssessment_Click" />
        </div>
    </div>
</asp:Content>
