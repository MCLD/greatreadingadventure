<%@ Page Title="Activity History" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="ActivityHistory.aspx.cs" Inherits="STG.SRP.ActivityHistory" %>
<%@ Import Namespace="STG.SRP.Controls" %>
<%@ Import Namespace="STG.SRP.DAL" %>

<%@ Register src="Controls/ActivityHistCtl.ascx" tagname="ActivityHistCtl" tagprefix="uc1" %>

<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


<uc1:ActivityHistCtl ID="ActivityHistCtl1" runat="server" />


</asp:Content>
