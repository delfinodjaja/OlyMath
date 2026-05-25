<%@ Page Title="OlyMath - Certificate" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Certificate.aspx.cs" Inherits="OlyMath.Trainee.Certificate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* Print Styles to isolate and print only the Certificate Card */
        @media print {
            body::before, 
            .topnav, 
            .sidebar, 
            .btn-primary, 
            .btn-secondary, 
            .page-title, 
            .page-subtitle,
            .cert-actions {
                display: none !important;
            }
            .main-content {
                margin-left: 0 !important;
                padding: 0 !important;
            }
            .cert-wrap {
                padding: 0 !important;
                margin: 0 !important;
            }
            .cert-card {
                border: 2px solid #0d1b2a !important;
                box-shadow: none !important;
                page-break-inside: avoid;
                margin: 0 auto !important;
                width: 100% !important;
                max-width: 640px !important;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-cert">
        <div class="page-title cert-actions">Certificate of Completion</div>
        <div class="page-subtitle cert-actions">Your achievement is ready to share.</div>
        
        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px;" />

        <asp:PlaceHolder ID="phCertificate" runat="server">
            <div class="cert-wrap">
                <div class="cert-card">
                    <div class="cert-tag">OlyMath &mdash; Verified Certificate</div>
                    <div class="cert-divider"></div>
                    <div style="font-size:0.82rem;color:var(--slate);margin-bottom:6px">This certifies that</div>
                    <div class="cert-name"><asp:Literal ID="litTraineeName" runat="server" /></div>
                    <div style="font-size:0.82rem;color:var(--slate)">has successfully completed</div>
                    <div class="cert-title"><asp:Literal ID="litModuleTitle" runat="server" /></div>
                    <div class="cert-module">
                        Score: <asp:Literal ID="litScore" runat="server" /> / 100 &nbsp;&middot;&nbsp; 
                        Trainer: <asp:Literal ID="litTrainerName" runat="server" />
                    </div>
                    <div class="cert-footer">
                        <span>Certificate ID: <asp:Literal ID="litCertId" runat="server" /></span>
                        <span>Issued: <asp:Literal ID="litDateIssued" runat="server" /></span>
                    </div>
                </div>
            </div>
            
            <div style="display:flex;justify-content:center;gap:12px;margin-top:4px" class="cert-actions">
                <button type="button" class="btn-primary" onclick="window.print(); return false;">Download PDF</button>
                <a href="Achievements.aspx" class="btn-secondary">View All Certificates</a>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
