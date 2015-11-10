
<%@ Page Title="Login" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Recover.aspx.cs" Inherits="GRA.SRP.Recover" %>

<%@ Register src="~/Controls/ProgramTabs.ascx" tagname="ProgramTabs" tagprefix="uc1" %>
<%@ Register src="~/Controls/ProgramBanner.ascx" tagname="ProgramBanner" tagprefix="uc2" %>

<%@ Register src="~/Controls/RecoverPassword.ascx" tagname="RecoverPassword" tagprefix="uc3" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<!--<uc1:ProgramTabs ID="ProgramTabs1" runat="server" />-->
<uc3:RecoverPassword ID="RecoverPassword1" runat="server" />
<!--<uc2:ProgramBanner ID="ProgramBanner1" runat="server" />-->

</asp:Content>
