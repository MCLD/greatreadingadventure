<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
    CodeBehind="PatronDetails.aspx.cs" 
    Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronDetails" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register src="../../Controls/PatronCtl.ascx" tagname="PatronCtl" tagprefix="uc1" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:PatronCtl ID="PatronCtl1" runat="server" />

</asp:Content>