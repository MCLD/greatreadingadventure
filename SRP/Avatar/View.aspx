<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="GRA.SRP.Avatar.View" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, height=device-height, user-scalable=no, initial-scale=1, maximum-scale=1, minimum-scale=1">
    <title>Avatar Viewer</title>
    <asp:PlaceHolder runat="server" ID="Metadata"></asp:PlaceHolder>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" runat="server" />
    <link href="~/Content/gra.css" rel="stylesheet" runat="server" />
</head>
<body>
    <form id="mainForm" runat="server">
        <nav class="navbar navbar-default navbar-static-top">
            <div class="container">
                <div class="navbar-header">
                    <a class="navbar-brand" id="homeLink" href="/" runat="server">
                        <asp:Label runat="server" ID="SystemName"></asp:Label></a>
                </div>
                <div class="nav navbar-nav navbar-right hidden-xs">
                    <p class="navbar-text" id="slogan">
                        <em>
                            <asp:Label runat="server" ID="SystemSlogan"></asp:Label></em>
                    </p>
                </div>
            </div>
        </nav>
        <div class="container">
            <div class="row hidden-print">
                <div class="col-xs-12 col-sm-10 col-sm-offset-1 col-md-8 col-md-offset-2">
                    <div class="alert alert-danger" runat="server" id="AvatarAlert">
                        <span class="glyphicon glyphicon-alert"></span>
                        Couldn't find that avatar!
                    </div>
                    <div class="panel panel-default" runat="server" id="AvatarPanel">
                        <div class="panel-heading">
                            <span class="lead">Avatar Viewer</span>
                        </div>
                        <div class="panel-body text-center">
                            <asp:Image runat="server" ID="AvatarImage" />
                        </div>
                        <div class="panel-footer clearfix">
                            <div class="pull-right">
                                <asp:HyperLink runat="server" ID="TwitterShare" Visible="false" CssClass="btn btn-default" Target="_blank"><span class="glyphicon glyphicon-share"></span>
                             Twitter</asp:HyperLink>
                                <asp:HyperLink runat="server" ID="FacebookShare" Visible="false" CssClass="btn btn-default" Target="_blank"><span class="glyphicon glyphicon-share"></span>
                             Facebook</asp:HyperLink>
                                <button class="btn btn-default" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>
                                <asp:HyperLink runat="server" ID="AvatarBackLink" CssClass="btn btn-default">Back</asp:HyperLink>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row visible-print-block text-center" runat="server" id="AvatarPrintPanel">
                <div class="col-xs-12">
                    <h1>
                        <asp:Label runat="server" ID="SystemNamePrint"></asp:Label></h1>
                </div>
                <div class="col-xs-12">
                    <em style="font-size: medium;"><asp:Label runat="server" ID="SystemSloganPrint"></asp:Label></em>
                </div>
                <div class="col-xs-12" style="margin-top: 2em;">
                    <asp:Image runat="server" ID="BannerImagePrint" />
                </div>
                <div class="col-xs-12" style="margin-top: 2em;">
                    <asp:Image runat="server" ID="AvatarImagePrint" />
                </div>
                <div class="col-xs-12" style="margin-top: 1em;">
                    <h2>
                        <asp:Label runat="server" ID="MyAvatarPrint"></asp:Label></h2>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
