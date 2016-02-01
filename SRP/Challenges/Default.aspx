<%@ Page Title="Challenges" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.Challenges.Default" %>

<%@ Register Src="~/Controls/ChallengesCtl.ascx" TagPrefix="uc1" TagName="ChallengesCtl" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:ChallengesCtl runat="server" ID="ChallengesCtl" />
</asp:Content>
