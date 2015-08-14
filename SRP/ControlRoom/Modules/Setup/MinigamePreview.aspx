<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="MinigamePreview.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MinigamePreview" 
    
%>
<%@ Register src="../../Controls/MinigamePreview.ascx" tagname="MinigamePreview" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:MinigamePreview ID="MinigamePreview1" runat="server" />
<br /><br />
</asp:Content>