<%@ Page Title="OlyMath - Achievements" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Achievements.aspx.cs" Inherits="OlyMath.Trainee.Achievements" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-achievements">
        <div class="page-title">Achievement History</div>
        <div class="page-subtitle">A record of your completed modules and scores.</div>
        
        <!-- HISTORY TABLE -->
        <div class="table-wrap">
            <table>
                <thead>
                    <tr>
                        <th>Module</th>
                        <th>Completed</th>
                        <th>Score</th>
                        <th>Certificate</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptAssessmentHistory" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("ModuleTitle") %></td>
                                <td><%# Eval("CompletedAt", "{0:MMM dd, yyyy}") %></td>
                                <td><%# Eval("Score") %> / <%# Eval("MaxScore") %></td>
                                <td>
                                    <span class='badge <%# Eval("CertificateId") != DBNull.Value ? "green" : "grey" %>'>
                                        <%# Eval("CertificateId") != DBNull.Value ? "Earned" : "Failed" %>
                                    </span>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <!-- Display in-progress modules as pending -->
                    <asp:Repeater ID="rptInProgressModules" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("Title") %></td>
                                <td>In progress</td>
                                <td>—</td>
                                <td><span class="badge grey">Pending</span></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <asp:PlaceHolder ID="phNoHistory" runat="server" Visible="false">
                        <tr>
                            <td colspan="4" style="text-align:center; color:var(--slate); padding:20px;">No assessment records found.</td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>

        <!-- CERTIFICATES LIST -->
        <div class="section-head">
            <div class="section-title">Certificates</div>
        </div>

        <asp:Repeater ID="rptCertificates" runat="server">
            <ItemTemplate>
                <div class="list-item" onclick="window.location='Certificate.aspx?certId=<%# Eval("CertificateId") %>';">
                    <div class="list-item-icon teal">CERT</div>
                    <div class="list-item-body">
                        <div class="list-item-title"><%# Eval("ModuleTitle") %></div>
                        <div class="list-item-sub">Issued <%# Eval("CompletedAt", "{0:MMM dd, yyyy}") %></div>
                    </div>
                    <span class="list-item-badge active">View</span>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
        <asp:PlaceHolder ID="phNoCerts" runat="server" Visible="false">
            <div class="content-card" style="text-align: center; padding: 30px;">
                <p style="color:var(--slate);">You haven't earned any certificates yet. Complete a module and pass its assessment to get certified!</p>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
