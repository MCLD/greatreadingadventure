<%@ Page Title="Addl Survey" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="AddlSurvey.aspx.cs" Inherits="STG.SRP.AddlSurvey" %>
<%@ Import Namespace="STG.SRP.Controls" %>
<%@ Import Namespace="STG.SRP.DAL" %>


<%@ Register src="Controls/PatronSurvey.ascx" tagname="PatronSurvey" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">


    <uc1:PatronSurvey ID="PatronSurvey1" runat="server" />


</asp:Content>

