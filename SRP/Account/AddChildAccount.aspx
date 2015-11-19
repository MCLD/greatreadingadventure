<%@ Page Title="Add Child Account" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="AddChildAccount.aspx.cs" Inherits="GRA.SRP.AddChildAccount" 
     MaintainScrollPositionOnPostback="true"
    %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register src="~/Controls/AddChildCtl.ascx" tagname="AddChildCtl" tagprefix="uc3" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc3:AddChildCtl ID="AddChildCtl1" runat="server" />

</asp:Content>
