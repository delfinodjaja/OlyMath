<%@ Page Title="OlyMath - Trainee Progress" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TraineeProgress.aspx.cs" Inherits="OlyMath.Trainer.TraineeProgress" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .progress-bar-container {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        .progress-text {
            font-size: 0.8rem;
            font-weight: 600;
            color: var(--navy);
            min-width: 32px;
        }
        .progress-bar-wrap {
            flex-grow: 1;
            height: 6px;
            background: var(--bg);
            border-radius: 99px;
            overflow: hidden;
            margin-top: 0;
        }
        .progress-bar {
            height: 100%;
            border-radius: 99px;
            background: var(--teal);
            transition: width 0.3s ease;
        }
        .progress-bar.behind {
            background: var(--orange);
        }
        .progress-bar.complete {
            background: #5B60F0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainer-progress">
        <div class="page-title">Trainee Progress</div>
        <div class="page-subtitle">Monitor the performance and completion status of your enrolled students.</div>

        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />

        <div class="table-wrap">
            <table>
                <thead>
                    <tr>
                        <th>Trainee Name</th>
                        <th>Module</th>
                        <th>Progress</th>
                        <th>Last Active</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptTraineeProgress" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td style="font-weight: 600; color: var(--navy);">
                                    <%# HttpUtility.HtmlEncode(Eval("TraineeName")) %>
                                </td>
                                <td style="font-weight: 500; color: var(--slate);">
                                    <%# HttpUtility.HtmlEncode(Eval("ModuleTitle")) %>
                                </td>
                                <td>
                                    <div class="progress-bar-container">
                                        <div class="progress-bar-wrap">
                                            <div class="progress-bar <%# Convert.ToInt32(Eval("ProgressPercentage")) == 100 ? "complete" : (Convert.ToInt32(Eval("ProgressPercentage")) < 50 ? "behind" : "") %>" 
                                                 style='width: <%# Eval("ProgressPercentage") %>%'></div>
                                        </div>
                                        <span class="progress-text"><%# Eval("ProgressPercentage") %>%</span>
                                    </div>
                                </td>
                                <td style="font-size: 0.85rem; color: var(--slate);">
                                    <%# Eval("LastAccessed", "{0:MMM dd, yyyy HH:mm}") %>
                                </td>
                                <td>
                                    <%# GetStatusBadge(Convert.ToInt32(Eval("ProgressPercentage"))) %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    
                    <asp:PlaceHolder ID="phNoProgress" runat="server" Visible="false">
                        <tr>
                            <td colspan="5" style="text-align:center; color:var(--slate); padding:30px;">
                                No trainee activity or enrollment found for your modules.
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </tbody>
            </table>
        </div>
    </div>
</asp:Content>
