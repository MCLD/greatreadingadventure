﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Badges.ascx.cs" Inherits="GRA.SRP.Controls.Badges" %>

<div class="row">
    <div class="col-sm-12">
        <span class="h1">
            <asp:Label runat="server" Text="badges-my-badges"></asp:Label></span>
    </div>
</div>
<div class="row margin-1em-top">
    <div class="col-xs-12">
        <asp:Label ID="NoBadges"
            runat="server" Text="badges-none-earned"
            Visible="false"
            CssClass="margin-1em-bottom"></asp:Label>
    </div>
    <asp:Repeater runat="server" ID="rptr">
        <ItemTemplate>
            <div class="col-xs-6 col-sm-3 col-md-2">
                <a href='<%# Eval("BadgeId", "~/Badges/Details.aspx?BadgeId={0}") %>'
                    runat="server"
                    OnClick='<%# Eval("BadgeId", "return ShowBadgeInfo({0});") %>'
                    class="thumbnail no-underline badge-with-info-height"
                    >
                    <div class="thumbnail-side-padding text-center caption"><small><%#Eval("Title") %></small></div>
                    <asp:Image runat="server"
                        ImageUrl='<%# Eval("BadgeId", "~/images/badges/sm_{0}.png")%>'
                        CssClass="center-block" />
                    <div class="thumbnail-side-padding text-center caption"><small><em>earned <%#((DateTime)Eval("DateEarned")).ToShortDateString() %></em></small></div>
                </a>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
