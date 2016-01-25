<%@ Page Title="Go on an adventure" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Adventure.aspx.cs" Inherits="GRA.SRP.PlayMinigame"
    MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register Src="~/Controls/Minigame.ascx" TagName="Minigame" TagPrefix="uc1" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <uc1:Minigame ID="MinigamePlay" runat="server" />
</asp:Content>
