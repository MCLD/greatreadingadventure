<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Avatar.ascx.cs" Inherits="GRA.SRP.Controls.Avatar" %>
<asp:HyperLink NavigateUrl="~/Account/" runat="server">
    <asp:Image ID="imgAvatar" runat="server" Width="160px" CssClass="center-block hidden-xs" />
</asp:HyperLink>
<div class="hidden-sm hidden-md hidden-lg">
    <div class="row margin-1em-bottom">
        <div class="col-xs-12 text-center">
            <asp:HyperLink NavigateUrl="~/Account/" runat="server">
                <asp:Image ID="imgAvatarSm" runat="server" Width="64px" height="64" CssClass="margin-1em-right" />
            </asp:HyperLink>
            <span class="lead"><strong>Welcome,
            <asp:Label ID="patronName" runat="server"></asp:Label>!</strong></span>
        </div>
    </div>
</div>
