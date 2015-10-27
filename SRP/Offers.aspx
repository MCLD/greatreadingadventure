<%@ Page Title="My Offers" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Offers.aspx.cs" Inherits="GRA.SRP.MyOffers" 
    EnableEventValidation="false" 
    %>
<%@ Import Namespace="GRA.SRP.DAL" %>


<%@ Register src="Controls/Offers.ascx" tagname="Offers" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">




    <uc1:Offers ID="Offers1" runat="server" />




</asp:Content>
