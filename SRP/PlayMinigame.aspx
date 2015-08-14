<%@ Page Title="Play Minigame" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="PlayMinigame.aspx.cs" Inherits="GRA.SRP.PlayMinigame" 
    MaintainScrollPositionOnPostback="true"
%>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register src="~/Controls/Minigame.ascx" tagname="Minigame" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc1:Minigame ID="MinigamePlay" runat="server" />

</asp:Content>
