<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyBadgesListControl.ascx.cs" Inherits="GRA.SRP.Controls.MyBadgesListControl" %>

<div class="row">
    <div class="col-xs-12 text-center">
        <span class="lead"><asp:HyperLink runat="server" NavigateUrl="~/MyBadges.aspx">My Badges</asp:HyperLink></span>
        <hr style="margin-bottom:5px !important; margin-top:5px !important; "/>
    </div>
</div>
<%--    <small>See <a href="/MyBadges.aspx">my badges </a></small><br />
    <small>Discover <a href="/BadgeGallery.aspx">more badges </a></small>--%>
<div class="row">
    <div class="col-xs-12">
        <asp:Label ID="NoBadges" runat="server" Text="You have not earned any badges yet." Visible="false" CssClass="margin-1em-bottom"></asp:Label>
    </div>
    <asp:Repeater runat="server" ID="rptr">
        <ItemTemplate>
            <div class="<%=this.BadgeClass %> text-center">
                <img src='/images/badges/sm_<%# Eval("BadgeID") %>.png' />
            </div>
        </ItemTemplate>
    </asp:Repeater>
        <div class="col-xs-12 text-center">
        <asp:HyperLink runat="server" NavigateUrl="~/BadgeGallery.aspx"><em>Explore more available badges...</em></asp:HyperLink>
    </div>
</div>
