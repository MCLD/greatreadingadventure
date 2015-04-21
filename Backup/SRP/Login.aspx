<%@ Page Title="Login" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="STG.SRP.Login" %>

<%@ Register src="Controls/ProgramTabs.ascx" tagname="ProgramTabs" tagprefix="uc1" %>
<%@ Register src="Controls/ProgramBanner.ascx" tagname="ProgramBanner" tagprefix="uc2" %>

<%@ Register src="Controls/PatronLogin.ascx" tagname="PatronLogin" tagprefix="uc3" %>

<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<!--<uc1:ProgramTabs ID="ProgramTabs1" runat="server" />-->
<uc3:PatronLogin ID="PatronLogin1" runat="server" />
<!--<uc2:ProgramBanner ID="ProgramBanner1" runat="server" />-->

</asp:Content>
