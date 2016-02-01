<%@ Page Title="A problem occurred!" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="GRA.SRP.Problem.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-8 col-sm-offset-2">
            <h2>A problem occurred!</h2>
            <p>We were unable to load the page you requested.</p>
            <p>
                If you need to reach someone, please
        <asp:HyperLink runat="server" NavigateUrl="~/Mail">use the mail link</asp:HyperLink>
                once you've signed up.
                <asp:Label runat="server" ID="AlternateContact"></asp:Label>
            </p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomOfPage" runat="server">
</asp:Content>
