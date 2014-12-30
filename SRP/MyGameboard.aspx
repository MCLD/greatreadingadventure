<%@ Page Title="Gameboard" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyGameboard.aspx.cs" Inherits="STG.SRP.MyGameboard" %>
<%@ Import Namespace="STG.SRP.DAL" %>


<%@ Register src="Controls/GameboardControl.ascx" tagname="GameboardControl" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <uc1:GameboardControl ID="GameboardControl1" runat="server" />


</asp:Content>
