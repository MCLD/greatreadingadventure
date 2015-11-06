<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyBadgesListControl.ascx.cs" Inherits="GRA.SRP.Controls.MyBadgesListControl" %>

<div class="row">
    <div class="col-xs-12 text-center">
        <span class="lead"><asp:HyperLink runat="server" NavigateUrl="~/MyBadges.aspx"><asp:Label runat="server" Text="badges-my-badges"></asp:Label></asp:HyperLink></span>
        <hr style="margin-bottom:5px !important; margin-top:5px !important; "/>
    </div>
</div>
<div class="row">
    <div class="col-xs-12">
        <asp:Label ID="NoBadges" runat="server" Text="badges-none-earned" Visible="false" CssClass="margin-1em-bottom"></asp:Label>
    </div>
    <asp:Repeater runat="server" ID="rptr" OnItemCommand="Repeater_ItemCommand">
        <ItemTemplate>
            <div class="<%=this.BadgeClass %> text-center" data-toggle="tooltip" data-placement="right" title="<%# "You earned the " + Eval("Title") + " badge on " + ((DateTime)Eval("DateEarned")).ToShortDateString() %>">
                <asp:LinkButton runat="server" CommandName="BadgeDetails" CommandArgument='<%#Eval("BadgeID") %>' CssClass="thumbnail">
                    <asp:Image runat="server" ImageUrl='<%# "~/images/badges/sm_" + Eval("BadgeID") + ".png"%>'/>
                </asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:Repeater>
        <div class="col-xs-12 text-center">
        <asp:HyperLink runat="server" NavigateUrl="~/BadgeGallery.aspx"><em>Explore more badges...</em></asp:HyperLink>
    </div>
</div>
