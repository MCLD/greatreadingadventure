<%@ Page Title="Gameboard" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Gameboard.aspx.cs" Inherits="GRA.SRP.MyGameboard" %>

<%@ Import Namespace="GRA.SRP.DAL" %>


<%@ Register Src="~/Controls/GameboardControl.ascx" TagName="GameboardControl" TagPrefix="uc1" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <uc1:GameboardControl ID="GameboardControl1" runat="server" />


</asp:Content>
