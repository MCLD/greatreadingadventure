<%@ Page Title="Add Child Account" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="AddChildAccount.aspx.cs" Inherits="STG.SRP.AddChildAccount" 
     MaintainScrollPositionOnPostback="true"
    %>
<%@ Import Namespace="STG.SRP.DAL" %>
<%@ Register src="~/Controls/AddChildCtl.ascx" tagname="AddChildCtl" tagprefix="uc3" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc3:AddChildCtl ID="AddChildCtl1" runat="server" />

</asp:Content>
