<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
    CodeBehind="PatronLogAddEdit.aspx.cs" 
    Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronLogAddEdit" 
    
%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register src="~/ControlRoom/Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>
<%@ Register src="~/ControlRoom/Controls/PatronLogEntryCtl.ascx" tagname="PatronLogEntryCtl" tagprefix="uc2" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:PatronContext ID="pcCtl" runat="server" />

    <uc2:PatronLogEntryCtl ID="PatronLogEntryCtl1" runat="server" />
</asp:Content>