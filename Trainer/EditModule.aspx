<%@ Page Title="OlyMath - Edit Module" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EditModule.aspx.cs" Inherits="OlyMath.Trainer.EditModule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainer-edit-module">
        <div class="page-title"><asp:Literal ID="litPageTitle" runat="server">Create New Module</asp:Literal></div>
        <div class="page-subtitle">Configure topic, title, description, and time requirements for this module.</div>
        
        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />

        <div class="content-card">
            <h3>Module Details</h3>
            
            <div class="form-grid">
                <div class="form-group">
                    <label>Module Title</label>
                    <asp:TextBox ID="txtTitle" runat="server" placeholder="e.g., Modular Arithmetic Fundamentals" />
                </div>
                <div class="form-group">
                    <label>Training Topic</label>
                    <asp:DropDownList ID="ddlTopic" runat="server">
                        <asp:ListItem Value="Number Theory">Number Theory</asp:ListItem>
                        <asp:ListItem Value="Combinatorics">Combinatorics</asp:ListItem>
                        <asp:ListItem Value="Geometry">Geometry</asp:ListItem>
                        <asp:ListItem Value="Algebra">Algebra</asp:ListItem>
                        <asp:ListItem Value="Inequalities">Inequalities</asp:ListItem>
                        <asp:ListItem Value="Logic">Logic</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Estimated Duration (e.g., 4h 20m)</label>
                    <asp:TextBox ID="txtTime" runat="server" placeholder="e.g., 4h 20m" />
                </div>
                <div class="form-group">
                    <label>Difficulty Level</label>
                    <asp:DropDownList ID="ddlDifficulty" runat="server">
                        <asp:ListItem Value="Easy">Easy</asp:ListItem>
                        <asp:ListItem Value="Medium">Medium</asp:ListItem>
                        <asp:ListItem Value="Hard">Hard</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-group" style="margin-top: 10px;">
                <label>Description</label>
                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" placeholder="Brief outline covering residues, congruences, Fermat's little theorem, and CRT..." />
            </div>

            <div class="form-actions" style="display: flex; gap: 12px;">
                <a href="MyModules.aspx" class="btn-secondary" style="padding: 10px 24px; font-size:0.88rem;">Cancel</a>
                <asp:Button ID="btnSaveDraft" runat="server" Text="Save Draft" CssClass="btn-secondary" OnClick="btnSaveDraft_Click" style="padding: 10px 24px; font-size:0.88rem;" />
                <asp:Button ID="btnSaveAddMaterials" runat="server" Text="Save + Add Materials" CssClass="btn-primary" OnClick="btnSaveAddMaterials_Click" style="padding: 10px 28px; font-size:0.88rem;" />
            </div>
        </div>
    </div>
</asp:Content>
