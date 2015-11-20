<%@ Page Title="Add Family Member Account" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="AddFamilyMemberAccount.aspx.cs" Inherits="GRA.SRP.AddFamilyMemberAccount"
    MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register Src="~/Controls/AddFamilyMemberControl.ascx" TagPrefix="afmc" TagName="AddFamilyMemberControl" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <afmc:AddFamilyMemberControl runat="server" ID="AddFamilyMemberControl" />
</asp:Content>
