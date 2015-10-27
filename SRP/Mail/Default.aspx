<%@ Page Title="My Notifications" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.MyNotifications" 
    EnableEventValidation="false" 
    %>
<%@ Import Namespace="GRA.SRP.DAL" %>



<%@ Register src="~/Controls/PatronNotificationsCtl.ascx" tagname="PatronNotificationsCtl" tagprefix="uc1" %>



<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">




   




    <uc1:PatronNotificationsCtl ID="PatronNotificationsCtl1" runat="server" />




   




</asp:Content>
