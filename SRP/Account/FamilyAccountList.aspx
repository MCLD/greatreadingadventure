
<%@ Page Title="Family Accounts" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="FamilyAccountList.aspx.cs" Inherits="GRA.SRP.FamilyAccounts" %>

<%@ Register src="~/Controls/ProgramBanner.ascx" tagname="ProgramBanner" tagprefix="uc2" %>

<%@ Register src="~/Controls/FamilyList.ascx" tagname="FamilyList" tagprefix="uc3" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


<!--<uc2:ProgramBanner ID="ProgramBanner1" runat="server" />-->
<uc3:FamilyList ID="FamilyList1" runat="server" />
</asp:Content>
