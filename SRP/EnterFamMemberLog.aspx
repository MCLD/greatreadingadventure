<%@ Page Title="Enter Family Member Log Entry" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="EnterFamMemberLog.aspx.cs" Inherits="GRA.SRP.EnterFamMemberLog" %>


<%@ Register src="~/Controls/SimpleLoggingFamilyControl.ascx" tagname="SimpleLoggingFamilyControl" tagprefix="uc1" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc1:SimpleLoggingFamilyControl ID="LogCtl" runat="server" />
    
</asp:Content>
