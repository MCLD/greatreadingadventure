<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronSurvey.ascx.cs" Inherits="GRA.SRP.Controls.PatronSurvey" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register src="../ControlRoom/Controls/QuestionPreview.ascx" tagname="QuestionPreview" tagprefix="uc1" %>

<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.1/jquery.min.js"></script>

<asp:Label ID="SRID" runat="server" Text="0" Visible="False"></asp:Label>
<asp:Label ID="SID" runat="server" Text="0" Visible="False"></asp:Label>
<asp:Label ID="QID" runat="server" Text="0" Visible="False"></asp:Label>
<asp:Label ID="QNumber" runat="server" Text="0" Visible="False"></asp:Label>
<asp:Label ID="SurveyTakingPage" runat="server" Text="0" Visible="False"></asp:Label>

<div class="container">
    <div class="form-wrapper form-medium">
	      <h3 class="title-divider">
              <asp:Label ID="lblSurvey" runat="server" Text="" Font-Bold="True" Font-Size="Large"></asp:Label>
          </h3>

 <hr />
    <asp:Label ID="lblPreamble" runat="server" Text="" Font-Bold="True"></asp:Label>

    <asp:Repeater ID="SurveyQLst" runat="server" onitemcommand="SurveyQLst_ItemCommand1">
    <ItemTemplate>
            <uc1:QuestionPreview ID="qp" runat="server" 
                SID = '<%# Eval("SID") %>'
                QID = '<%# Eval("QID") %>'
                QNumber = '<%# Eval("QNumber") %>'
                QType = '<%# Eval("QType") %>'
                QText = '<%# Server.HtmlDecode(Eval("QText").ToString()) %>'
                DisplayControl = '<%# Eval("DisplayControl") %>'
                DisplayDirection = '<%# Eval("DisplayDirection") %>'
                IsRequired = '<%# ((bool)Eval("IsRequired")).ToYesNo().ToLower() %>'
                IsBinding = 'True'
            /><br />
    </ItemTemplate>
    </asp:Repeater>




	    </div>

</div>
<uc1:QuestionPreview ID="QuestionPreview1" runat="server" />
