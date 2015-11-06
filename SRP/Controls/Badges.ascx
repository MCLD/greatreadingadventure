<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Badges.ascx.cs" Inherits="GRA.SRP.Controls.Badges" %>

<div class="row">
    <div class="col-sm-12">
        <span class="h1">
            <asp:Label runat="server" Text="badges-my-badges"></asp:Label></span>
    </div>
</div>
<div class="row margin-1em-top">
    <div class="col-xs-12">
        <asp:Label ID="NoBadges" runat="server" Text="no-badges-earned" Visible="false" CssClass="margin-1em-bottom"></asp:Label>
    </div>
    <asp:Repeater runat="server" ID="rptr" OnItemCommand="rptr_ItemCommand">
        <ItemTemplate>
            <div class="col-xs-6 col-sm-3 col-md-2">
                <asp:LinkButton runat="server" CommandName="BadgeDetails" CommandArgument='<%#Eval("BadgeID") %>' CssClass="thumbnail no-underline">
                    <div class="text-center caption"><small><%#Eval("Title") %></small></div>
                    <asp:Image runat="server" ImageUrl='<%# "~/images/badges/sm_" + Eval("BadgeID") + ".png"%>' CssClass="center-block" />
                    <div class="text-center caption"><small><em>Earned <%#((DateTime)Eval("DateEarned")).ToShortDateString() %></em></small></div>
                </asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
