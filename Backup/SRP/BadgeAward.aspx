<%@ Page Title="Badge Award" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="BadgeAward.aspx.cs" Inherits="STG.SRP.BadgeAward" %>


<%@ Register src="~/Controls/BadgeAwardCtl.ascx" tagname="BadgeAwardCtl" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc1:BadgeAwardCtl ID="BadgeAwardObj" runat="server" />
    
</asp:Content>

