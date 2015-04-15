<%@ Page Title="My Badges" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyBadges.aspx.cs" Inherits="STG.SRP.MyBadges" 
    EnableEventValidation="false" 
    %>
<%@ Import Namespace="STG.SRP.DAL" %>


<%@ Register src="Controls/Badges.ascx" tagname="Badges" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">




    <uc1:Badges ID="Badges1" runat="server" />




</asp:Content>
