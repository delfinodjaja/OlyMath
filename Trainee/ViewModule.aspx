<%@ Page Title="Module Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewModule.aspx.cs" Inherits="OlyMath.Trainee.ViewModule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Module Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .module-header {
            margin-bottom: 40px;
            text-align: left;
        }
        .module-header h1 {
            font-family: 'Syne', sans-serif;
            font-size: 2.2rem;
            color: var(--navy);
            margin-bottom: 8px;
        }
        .module-header p {
            color: var(--slate);
            font-size: 1.05rem;
        }

        .section-container {
            margin-bottom: 48px;
        }
        .section-title {
            font-family: 'Syne', sans-serif;
            font-size: 1.4rem;
            color: var(--navy);
            border-bottom: 2px solid var(--border);
            padding-bottom: 12px;
            margin-bottom: 24px;
        }

        .card-list {
            display: flex;
            flex-direction: column;
            gap: 16px;
        }

        .item-card {
            background: var(--white);
            border: 1px solid var(--border);
            border-radius: var(--radius);
            padding: 20px 28px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            box-shadow: var(--card-shadow);
            transition: transform 0.2s, box-shadow 0.2s;
        }

        .item-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 24px rgba(13,27,42,0.12); 
        }

        .item-info h4 {
            font-family: 'Syne', sans-serif;
            font-size: 1.15rem;
            color: var(--navy);
            margin-bottom: 6px;
        }

        .item-info .badge {
            font-size: 0.75rem;
            font-weight: 600;
            padding: 4px 10px;
            border-radius: 100px;
            background: var(--bg);
            color: var(--slate);
            text-transform: uppercase;
            letter-spacing: 0.05em;
            display: inline-block;
        }

        .item-info .badge.marks {
            background: rgba(0, 201, 167, 0.15); 
            color: var(--teal);
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width: 900px; margin: 0 auto; padding: 40px 20px;">
        
        <div class="module-header">
            <h1><asp:Literal ID="litModuleTitle" runat="server"></asp:Literal></h1>
            <p><asp:Literal ID="litModuleDesc" runat="server"></asp:Literal></p>
        </div>

        <div class="section-container">
            <h3 class="section-title">📚 Study Materials</h3>
            <div class="card-list">
                <asp:Repeater ID="rptMaterials" runat="server">
                    <ItemTemplate>
                        <div class="item-card">
                            <div class="item-info">
                                <h4><%# Eval("Title") %></h4>
                                <span class="badge"><%# Eval("Type") %></span>
                            </div>
                            <asp:HyperLink ID="lnkView" runat="server" CssClass="btn-secondary" NavigateUrl='<%# Eval("ContentUrl") %>' Target="_blank">
                                View Material
                            </asp:HyperLink>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="section-container">
            <h3 class="section-title">📝 Assessments</h3>
            <div class="card-list">
                <asp:Repeater ID="rptAssessments" runat="server">
                    <ItemTemplate>
                        <div class="item-card">
                            <div class="item-info">
                                <h4><%# Eval("Title") %></h4>
                                <span class="badge marks">Total Marks: <%# Eval("TotalMarks") %></span>
                            </div>
                            <asp:LinkButton ID="btnTakeAssessment" runat="server" CssClass="btn-primary" CommandArgument='<%# Eval("AssessmentId") %>'>
                                Take Assessment
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptsContent" runat="server">
</asp:Content>