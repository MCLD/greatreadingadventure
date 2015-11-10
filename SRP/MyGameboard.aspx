<%@ Page Title="Gameboard" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyGameboard.aspx.cs" Inherits="GRA.SRP.MyGameboard" %>
<%@ Import Namespace="GRA.SRP.DAL" %>


<%@ Register src="Controls/GameboardControl.ascx" tagname="GameboardControl" tagprefix="uc1" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <uc1:GameboardControl ID="GameboardControl1" runat="server" />


</asp:Content>
