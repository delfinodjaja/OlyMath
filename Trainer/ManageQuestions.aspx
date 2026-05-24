<%@ Page Title="OlyMath - Manage Questions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageQuestions.aspx.cs" Inherits="OlyMath.Trainer.ManageQuestions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainer-manage-questions">
        <div class="page-title">Manage Quiz Questions</div>
        <div class="page-subtitle">Module: <span style="font-weight: 600; color: var(--navy);"><asp:Literal ID="litModuleTitle" runat="server" /></span></div>
        
        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom: 20px; display:block;" />
        <asp:Label ID="lblMessage" runat="server" CssClass="form-success" Visible="false" style="margin-bottom: 20px; display:block;" />

        <!-- EXISTING QUESTIONS TABLE -->
        <div class="content-card">
            <h3>Current Assessment Questions</h3>
            <div class="table-wrap" style="margin-bottom: 0; overflow-x: auto;">
                <table>
                    <thead>
                        <tr>
                            <th style="width: 60px;">#</th>
                            <th>Question Text</th>
                            <th>Options (A, B, C, D)</th>
                            <th style="width: 100px;">Correct</th>
                            <th style="width: 100px;">Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:Repeater ID="rptQuestions" runat="server" OnItemCommand="rptQuestions_ItemCommand">
                            <ItemTemplate>
                                <tr>
                                    <td><strong><%# Container.ItemIndex + 1 %></strong></td>
                                    <td style="font-weight: 500; max-width: 320px; white-space: normal;"><%# HttpUtility.HtmlEncode(Eval("QuestionText")) %></td>
                                    <td style="font-size:0.78rem; line-height: 1.4; color:var(--slate); max-width: 300px; white-space: normal;">
                                        <strong>A:</strong> <%# HttpUtility.HtmlEncode(Eval("OptionA")) %><br />
                                        <strong>B:</strong> <%# HttpUtility.HtmlEncode(Eval("OptionB")) %><br />
                                        <strong>C:</strong> <%# HttpUtility.HtmlEncode(Eval("OptionC")) %><br />
                                        <strong>D:</strong> <%# HttpUtility.HtmlEncode(Eval("OptionD")) %>
                                    </td>
                                    <td><span class="badge green"><%# Eval("CorrectOption") %></span></td>
                                    <td>
                                        <asp:LinkButton ID="btnDelete" runat="server" 
                                            CommandName="DeleteQuestion" 
                                            CommandArgument='<%# Eval("Id") %>' 
                                            CssClass="badge orange" 
                                            OnClientClick="return confirm('Are you sure you want to delete this question?');" 
                                            style="border:none; cursor:pointer;">
                                            Delete
                                        </asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        
                        <asp:PlaceHolder ID="phNoQuestions" runat="server" Visible="false">
                            <tr>
                                <td colspan="5" style="text-align:center; color:var(--slate); padding:20px;">
                                    No assessment questions have been added to this module yet. Fill the form below to add one!
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                    </tbody>
                </table>
            </div>
        </div>

        <!-- ADD QUESTION FORM -->
        <div class="content-card" style="margin-top: 24px;">
            <h3>Add New Quiz Question</h3>
            
            <div class="form-group">
                <label>Question Text</label>
                <asp:TextBox ID="txtQuestionText" runat="server" TextMode="MultiLine" Rows="3" placeholder="e.g., What is the value of 2^10 mod 7?" />
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Option A</label>
                    <asp:TextBox ID="txtOptionA" runat="server" placeholder="Residue value A" />
                </div>
                <div class="form-group">
                    <label>Option B</label>
                    <asp:TextBox ID="txtOptionB" runat="server" placeholder="Residue value B" />
                </div>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Option C</label>
                    <asp:TextBox ID="txtOptionC" runat="server" placeholder="Residue value C" />
                </div>
                <div class="form-group">
                    <label>Option D</label>
                    <asp:TextBox ID="txtOptionD" runat="server" placeholder="Residue value D" />
                </div>
            </div>

            <div class="form-grid">
                <div class="form-group">
                    <label>Correct Option</label>
                    <asp:DropDownList ID="ddlCorrectOption" runat="server">
                        <asp:ListItem Value="A">Option A</asp:ListItem>
                        <asp:ListItem Value="B">Option B</asp:ListItem>
                        <asp:ListItem Value="C">Option C</asp:ListItem>
                        <asp:ListItem Value="D">Option D</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div></div>
            </div>

            <div class="form-actions">
                <a href="Dashboard.aspx" class="btn-secondary" style="padding: 10px 24px; font-size:0.88rem;">Back to Dashboard</a>
                <asp:Button ID="btnAdd" runat="server" Text="Add Question" CssClass="btn-primary" OnClick="btnAdd_Click" style="padding: 10px 28px; font-size:0.88rem;" />
            </div>
        </div>
    </div>
</asp:Content>
