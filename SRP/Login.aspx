<%@ Page Title="Login" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Login.aspx.cs" Inherits="GRA.SRP.Login" %>

<%@ Register Src="Controls/PatronLogin.ascx" TagName="PatronLogin" TagPrefix="uc3" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="col-sm-10 col-sm-offset-1">
        <uc3:PatronLogin ID="PatronLogin1" runat="server" />
    </div>
</asp:Content>