<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Offers.ascx.cs" Inherits="GRA.SRP.Controls.Offers" %>

<asp:Panel ID="pnlList" runat="server" Visible="true">
    <div class="row">
        <div class="col-xs-12">
            <h1>
                <asp:Label ID="Label1" runat="server" Text="offers-title"></asp:Label></h1>
        </div>
    </div>
    <div class="row">
        <asp:Repeater runat="server" ID="rptr" OnItemCommand="rptr_ItemCommand" OnItemDataBound="rptr_ItemBound">
            <ItemTemplate>
                <div class="col-xs-6 col-sm-3 col-md-2">
                    <div class="clearfix">
                        <asp:LinkButton runat="server" ID="eventDetailsLink"
                            CssClass="thumbnail no-underline offer-height">
                        <div class="text-center caption" style="padding-left: 2px; padding-right: 2px;"><small><%# Eval("Title") %></small></div>
                            <asp:Image runat="server" ImageUrl='<%#Eval("OID", "~/images/offers/md_{0}.png") %>'
                                CssClass="center-block margin-1em-bottom" />
                        </asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>
<asp:Panel ID="pnlDetail" runat="server" Visible="false">
    <div class="row">
        <div class="col-xs-12">
            <div class="text-center">
                <asp:Label ID="lblTitle" runat="server" CssClass="lead"></asp:Label></div>
        </div>
        <div class="col-xs-12 margin-1em-top">
            <asp:Image ID="Coupon" runat="server" CssClass="center-block"/>
        </div>
        <div class="col-xs-12 margin-halfem-top">
            <div class="text-center">
                <asp:Label runat="server" Text="offers-serial"></asp:Label><asp:Label ID="lblSerial" runat="server"></asp:Label></div>
        </div>
        <div class="col-xs-12 margin-1em-top hidden-print">
            <div class="text-center"><asp:Button runat="server" Text="offers-back" OnClick="btnList_Click" CssClass="btn btn-default btn-lg"></asp:Button></div>
        </div>
    </div>
</asp:Panel>
