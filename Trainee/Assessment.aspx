<%@ Page Title="OlyMath - Assessment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Assessment.aspx.cs" Inherits="OlyMath.Trainee.Assessment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .option-container {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 12px 16px;
            border: 1.5px solid var(--border);
            border-radius: 10px;
            cursor: pointer;
            font-size: 0.88rem;
            transition: all 0.18s;
            margin-bottom: 8px;
            user-select: none;
        }
        .option-container:hover {
            border-color: var(--navy);
            background: var(--bg);
        }
        .option-container input[type="radio"] {
            accent-color: var(--navy);
            width: 16px;
            height: 16px;
            cursor: pointer;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="trainee-assessment">
        <div class="page-title">Assessment</div>
        <div class="page-subtitle"><asp:Literal ID="litModuleTitle" runat="server" /> — Final Quiz</div>
        
        <asp:Label ID="lblError" runat="server" CssClass="form-error" Visible="false" style="margin-bottom:20px; font-size:0.95rem; padding: 12px; border-radius: 10px; background: #FFF0EC; border: 1px solid var(--orange);" />
        
        <asp:PlaceHolder ID="phQuiz" runat="server">
            <asp:Repeater ID="rptQuestions" runat="server">
                <ItemTemplate>
                    <div class="content-card">
                        <h3>Question <%# Container.ItemIndex + 1 %></h3>
                        
                        <p style="margin:12px 0 20px; font-size:0.9rem; line-height:1.7; color:var(--navy)">
                            <%# Eval("QuestionText") %>
                        </p>
                        
                        <div style="display:flex; flex-direction:column; gap:6px">
                            <label class="option-container">
                                <input type="radio" name='<%# "Q_" + Eval("Id") %>' value="A" />
                                <span>A. <%# HttpUtility.HtmlEncode(Eval("OptionA")) %></span>
                            </label>
                            
                            <label class="option-container">
                                <input type="radio" name='<%# "Q_" + Eval("Id") %>' value="B" />
                                <span>B. <%# HttpUtility.HtmlEncode(Eval("OptionB")) %></span>
                            </label>
                            
                            <label class="option-container">
                                <input type="radio" name='<%# "Q_" + Eval("Id") %>' value="C" />
                                <span>C. <%# HttpUtility.HtmlEncode(Eval("OptionC")) %></span>
                            </label>
                            
                            <label class="option-container">
                                <input type="radio" name='<%# "Q_" + Eval("Id") %>' value="D" />
                                <span>D. <%# HttpUtility.HtmlEncode(Eval("OptionD")) %></span>
                            </label>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <div style="display:flex; gap:12px; align-items:center; margin-top:20px;">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit Assessment" CssClass="btn-primary" OnClick="btnSubmit_Click" />
                <span style="font-size:0.8rem; color:var(--slate)">Passing Score: 60% &nbsp;|&nbsp; Estimated time: 30 min</span>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phNoQuestions" runat="server" Visible="false">
            <div class="content-card" style="text-align: center; padding: 50px;">
                <h3 style="color: var(--navy); margin-bottom: 12px;">No Questions Available</h3>
                <p style="color: var(--slate); margin-bottom: 20px;">There are no active quiz questions set up for this module yet.</p>
                <a href='ModuleDetail.aspx?id=<%= ModuleId %>' class="btn-primary">Return to Module Details</a>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
