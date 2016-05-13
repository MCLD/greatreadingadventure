<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="SurveyPreview.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyPreview" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register Src="../../Controls/QuestionPreview.ascx" TagName="QuestionPreview" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="SRID" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="SID" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="QID" runat="server" Text="0" Visible="False"></asp:Label>
    <asp:Label ID="QNumber" runat="server" Text="0" Visible="False"></asp:Label>

    <asp:Label ID="lblSurvey" runat="server" Text="" Font-Bold="True" Font-Size="Large"></asp:Label>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="lb1" CausesValidation="false"
        runat="server" OnClick="lb1_Click">[Back to Test]</asp:LinkButton>&nbsp;&nbsp;<asp:LinkButton CausesValidation="false"
            ID="lb2" runat="server" OnClick="lb2_Click">[Back to Questions]</asp:LinkButton>
    <hr />
    <strong><asp:Literal ID="lblPreamble" runat="server" Text=""></asp:Literal></strong>

    <asp:Repeater ID="SurveyQLst" runat="server" OnItemCommand="SurveyQLst_ItemCommand1">
        <ItemTemplate>
            <uc1:QuestionPreview ID="qp" runat="server"
                SID='<%# Eval("SID") %>'
                QID='<%# Eval("QID") %>'
                QNumber='<%# Eval("QNumber") %>'
                QType='<%# Eval("QType") %>'
                QText='<%# Server.HtmlDecode(Eval("QText").ToString()) %>'
                DisplayControl='<%# Eval("DisplayControl") %>'
                DisplayDirection='<%# Eval("DisplayDirection") %>'
                IsRequired='<%# ((bool)Eval("IsRequired")).ToYesNo().ToLower() %>'
                IsBinding='True' />
            <br />
        </ItemTemplate>
    </asp:Repeater>



</asp:Content>
