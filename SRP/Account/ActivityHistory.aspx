<%@ Page Title="Activity History" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="ActivityHistory.aspx.cs" Inherits="GRA.SRP.ActivityHistory" %>

<%@ Import Namespace="GRA.SRP.Controls" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<%@ Register Src="~/Controls/ActivityHistCtl.ascx" TagName="ActivityHistCtl" TagPrefix="uc1" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <uc1:ActivityHistCtl ID="ActivityHistCtl1" runat="server" />
</asp:Content>
