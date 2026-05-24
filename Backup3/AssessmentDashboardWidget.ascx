<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssessmentDashboardWidget.ascx.cs" Inherits="OlyMath.AssessmentDashboardWidget" %>

<div class="panel panel-default">
    <div class="panel-heading">
        <h3 class="panel-title">Assessment Summary</h3>
    </div>

    <div class="panel-body">

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="text-muted">
        </asp:Label>

        <div class="row">

            <div class="col-md-4">
                <div class="well text-center">
                    <h3>
                        <asp:Label ID="lblTotalAssessments" runat="server" Text="0"></asp:Label>
                    </h3>
                    <p>Total Assessments</p>
                </div>
            </div>

            <div class="col-md-4">
                <div class="well text-center">
                    <h3>
                        <asp:Label ID="lblTotalAttempts" runat="server" Text="0"></asp:Label>
                    </h3>
                    <p>Total Attempts</p>
                </div>
            </div>

            <div class="col-md-4">
                <div class="well text-center">
                    <h3>
                        <asp:Label ID="lblPassedAttempts" runat="server" Text="0"></asp:Label>
                    </h3>
                    <p>Passed Attempts</p>
                </div>
            </div>

        </div>

        <h4>Recent Scores</h4>

        <asp:GridView
            ID="gvRecentAttempts"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="table table-bordered table-striped">

            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Assessment" />
                <asp:BoundField DataField="UserName" HeaderText="Trainee" />
                <asp:BoundField DataField="Score" HeaderText="Score" />
                <asp:BoundField DataField="IsPassed" HeaderText="Passed" />
                <asp:BoundField DataField="AttemptDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
            </Columns>

        </asp:GridView>

    </div>
</div>