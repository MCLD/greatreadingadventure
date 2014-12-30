<%@ Page Title="Register" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="STG.SRP.Register"  MaintainScrollPositionOnPostback="true"
    %>

<%@ Register src="Controls/PatronRegistration.ascx" tagname="PatronRegistration" tagprefix="uc3" %>

<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">

</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <uc3:PatronRegistration ID="PatronRegistrationCtl" runat="server" />

</asp:Content>
