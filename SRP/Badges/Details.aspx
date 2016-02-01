<%@ Page Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="GRA.SRP.Badges.Details" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderContent" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-8 col-sm-offset-2 col-md-6 col-md-offset-3">
            <asp:Panel runat="server" ID="badgeDetails" CssClass="panel panel-default">
                <div class="panel-heading">
                    <span class="lead">
                        <asp:Label runat="server" ID="badgeTitle"></asp:Label></span>
                </div>
                <div class="panel-body">
                    <asp:Image ID="badgeImage" runat="server" CssClass="center-block" />
                    <asp:Panel ID="badgeEarnPanel" runat="server" CssClass="margin-1em-top">
                        Ways to earn this badge:
                    <div class="margin-halfem-top">
                        <asp:Label ID="badgeEarnLabel" runat="server" Text=""></asp:Label>
                    </div>
                    </asp:Panel>
                </div>
                <div class="panel-footer clearfix hidden-print">
                    <div class="pull-right">
                        <button class="btn btn-default" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>
                        <asp:HyperLink runat="server" ID="badgeBackLink" CssClass="btn btn-default">Back</asp:HyperLink>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <script>
        if(<%=this.PrintPage%> == true) {
            window.print();
        }
    </script>
</asp:Content>
