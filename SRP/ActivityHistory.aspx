<%@ Page Title="Activity History" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="ActivityHistory.aspx.cs" Inherits="GRA.SRP.ActivityHistory" %>
<%@ Import Namespace="GRA.SRP.Controls" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<%@ Register src="Controls/ActivityHistCtl.ascx" tagname="ActivityHistCtl" tagprefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<uc1:ActivityHistCtl ID="ActivityHistCtl1" runat="server" />
</asp:Content>