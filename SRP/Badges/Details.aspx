<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="GRA.SRP.Badges.Details" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel runat="server" ID="badgeDetails" CssClass="row margin-1em-top">
        <div class="row">
            <div class="col-sm-6 col-sm-offset-3">
                <span class="h1">
                    <asp:Label runat="server" ID="badgeTitle"></asp:Label></span>
            </div>
        </div>
        <div class="col-sm-6 col-sm-offset-3">
            <asp:Image ID="badgeImage" runat="server" CssClass="center-block" />
        </div>
        <asp:Panel ID="badgeEarnPanel" runat="server" CssClass="col-sm-6 col-sm-offset-3 margin-1em-top">
            Ways to earn this badge:
                    <div class="margin-halfem-top">
                        <asp:Label ID="badgeEarnLabel" runat="server" Text=""></asp:Label>
                    </div>
        </asp:Panel>
    </asp:Panel>
    <div class="row margin-1em-top">
        <div class="col-sm-6 col-sm-offset-3">
            <div class="pull-right clearfix">
                <asp:HyperLink runat="server" ID="badgeBackLink" CssClass="btn btn-default">Back</asp:HyperLink>
            </div>
        </div>
    </div>
</asp:Content>
