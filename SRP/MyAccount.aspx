
<%@ Page Title="My Account" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyAccount.aspx.cs" Inherits="GRA.SRP.MyAccount" 
     MaintainScrollPositionOnPostback="true"
     %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register src="~/Controls/MyAccountCtl.ascx" tagname="MyAccountCtl" tagprefix="uc3" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc3:MyAccountCtl ID="MyAccountCtl1" runat="server" />

</asp:Content>
