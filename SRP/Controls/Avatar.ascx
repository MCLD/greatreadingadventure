<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Avatar.ascx.cs" Inherits="GRA.SRP.Controls.Avatar" %>
<asp:Image ID="imgAvatar" runat="server" Width="160px" CssClass="center-block hidden-xs" />
<div class="hidden-sm hidden-md hidden-lg">
    <div class="row margin-1em-bottom">
        <div class="col-xs-12 text-center">
            <asp:Image ID="imgAvatarSm" runat="server" Width="24px" CssClass="margin-1em-right" />
            <span class="lead"><strong>Welcome,
            <asp:Label ID="patronName" runat="server"></asp:Label>!</strong></span>
        </div>
    </div>
</div>
