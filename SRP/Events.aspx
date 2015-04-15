<%@ Page Title="Events" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Events.aspx.cs" Inherits="STG.SRP.Events" 
    EnableEventValidation="false" 
    %>
<%@ Import Namespace="STG.SRP.DAL" %>

<%@ Register src="~/Controls/Events.ascx" tagname="Events" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">




    <uc1:Events ID="Events1" runat="server" />




</asp:Content>
