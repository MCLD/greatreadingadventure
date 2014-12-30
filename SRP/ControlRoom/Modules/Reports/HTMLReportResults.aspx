<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Reports.Master" 
    AutoEventWireup="true" CodeBehind="HTMLReportResults.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Reports.HTMLReportResults" 
    
%>
<%@ Import Namespace="STG.SRP.DAL" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
    <%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
    <%@ Register TagPrefix="ajax" Namespace="AjaxControlToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table style="background-color: White;">
<tr><td>
<asp:Label ID="lblFilter" runat="server" Text="" Visible="True"></asp:Label>
<asp:GridView ID="gv" runat="server">

</asp:GridView>
</td></tr>
</table>
</asp:Content>
