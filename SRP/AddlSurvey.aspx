<%@ Page Title="Initial Program Questions" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="AddlSurvey.aspx.cs" Inherits="GRA.SRP.AddlSurvey" %>

<%@ Import Namespace="GRA.SRP.Controls" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<%@ Register Src="Controls/PatronSurvey.ascx" TagName="PatronSurvey" TagPrefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <uc1:PatronSurvey ID="PatronSurvey1" runat="server" />
</asp:Content>