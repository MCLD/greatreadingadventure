<%@ Page Title="Page not found!" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="NotFound.aspx.cs" Inherits="GRA.SRP.NotFound" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-8 col-sm-offset-2">
            <h2>Page not found!</h2>
            <p>We couldn't find the page you requested.</p>
            <p>If you need to reach someone, please <asp:HyperLink runat="server" NavigateUrl="~/Mail">use the mail link</asp:HyperLink> once you've signed up. <asp:Label runat="server" ID="AlternateContact"></asp:Label></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomOfPage" runat="server">
</asp:Content>
