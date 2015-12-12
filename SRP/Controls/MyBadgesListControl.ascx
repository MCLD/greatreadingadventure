<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyBadgesListControl.ascx.cs" Inherits="GRA.SRP.Controls.MyBadgesListControl" %>

<div class="row">
    <div class="col-xs-12 text-center">
        <span class="lead">
            <asp:HyperLink runat="server" NavigateUrl="~/Badges/MyBadges.aspx"><asp:Label runat="server" Text="badges-my-badges"></asp:Label></asp:HyperLink></span>
        <hr style="margin-bottom: 5px !important; margin-top: 5px !important;" />
    </div>
</div>
<div class="row">
    <div class="col-xs-12">
        <asp:Label ID="NoBadges" runat="server" Text="badges-none-earned" Visible="false" CssClass="margin-1em-bottom"></asp:Label>
    </div>
</div>
<div class="row">
    <asp:Repeater runat="server" ID="rptr">
        <ItemTemplate>
            <span runat="server" visible="<%# Container.ItemIndex < 5 %>">
                <div class="<%=this.BadgeClass %> text-center"
                    data-toggle="tooltip"
                    data-placement="left"
                    title="<%# "You earned the " + Eval("Title") + " badge on " + ((DateTime)Eval("DateEarned")).ToShortDateString() %>">
                    <a href='<%# Eval("BadgeId", "~/Badges/Details.aspx?BadgeId={0}") %>'
                        runat="server"
                        onclick='<%# Eval("BadgeId", "return HideTooltipShowBadgeInfo(this.parentElement, {0});") %>'
                        style="display: inline-block; width: 74px; height: 74px;"
                        class="thumbnail">
                        <asp:Image runat="server" ImageUrl='<%# Eval("BadgeId", "~/images/badges/sm_{0}.png")%>' Width="64" Height="64" />
                    </a>
                </div>
            </span>
            <span runat="server" visible="<%# Container.ItemIndex == 5 %>">
                <div class="<%=this.BadgeClass %> text-center"
                    data-toggle="tooltip"
                    data-placement="left"
                    title="Click here to see more badges you've earned!">
                    <a href="~/Badges/MyBadges.aspx" runat="server" class="thumbnail"
                        style="display: inline-block; width: 74px; height: 74px; padding-top: 2.2em;">
                        <span class="glyphicon glyphicon-option-horizontal" style="font-size: 2.8em; margin-left: -0.1em;"></span>
                    </a>
                </div>
            </span>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div class="row">
    <div class="col-xs-12 text-center">
        <asp:HyperLink runat="server" NavigateUrl="~/Badges/"><em>Explore more badges...</em></asp:HyperLink>
    </div>
</div>
