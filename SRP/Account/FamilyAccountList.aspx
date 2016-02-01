<%@ Page Title="My Family Members" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="FamilyAccountList.aspx.cs" Inherits="GRA.SRP.FamilyAccounts" %>

<%@ Register Src="~/Controls/ProgramBanner.ascx" TagName="ProgramBanner" TagPrefix="uc2" %>

<%@ Register Src="~/Controls/FamilyList.ascx" TagName="FamilyList" TagPrefix="uc3" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <uc3:FamilyList ID="FamilyList1" runat="server" />
</asp:Content>