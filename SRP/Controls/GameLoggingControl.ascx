<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GameLoggingControl.ascx.cs" Inherits="GRA.SRP.Controls.GameLoggingControl" %>
<div class="row">
    <asp:Panel runat="server" Visible="false" ID="NoAdventures">
        <p class="lea" d>No adventures are available right now, check back as you earn points!</p>
    </asp:Panel>
    <asp:Repeater runat="server" ID="rptrx1" OnItemCommand="rptrx1_ItemCommand"
        OnItemDataBound="rptrx1_ItemDataBound">
        <ItemTemplate>
            <div class="col-xs-6 col-sm-3 col-md-2">
                <asp:LinkButton runat='server'
                    CommandName="Play"
                    CommandArgument='<%# Eval("MGID") %>'
                    class="text-center adventure-card">
                        <asp:Image runat='server'
                            ImageUrl='<%# (System.IO.File.Exists(Server.MapPath(string.Format("/images/Games/{0}.png",Eval("MGID"))))? string.Format("~/images/Games/{0}.png",Eval("MGID")) : "~/images/MiniGames.png") %>'
                            CssClass='img-responsive'
                            Width='100%' />
                    <div class="adventure-caption-container">
                        <small class="adventure-caption"><%# Eval("GameName") %></small>
                    </div>
                </asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater runat="server" ID="rptrx2" OnItemCommand="rptrx1_ItemCommand"
        OnItemDataBound="rptrx2_ItemDataBound">
        <ItemTemplate>
            <div class="col-xs-6 col-sm-3 col-md-2">
                <asp:LinkButton runat='server'
                    CommandName="Play"
                    CommandArgument='<%# Eval("MGID") %>'
                    class="text-center adventure-card">
                        <asp:Image runat='server'
                            ImageUrl='<%# (System.IO.File.Exists(Server.MapPath(string.Format("/images/Games/{0}.png",Eval("MGID"))))? string.Format("~/images/Games/{0}.png",Eval("MGID")) : "~/images/MiniGames.png") %>'
                            CssClass='img-responsive'
                            Width='100%' />
                    <div class="adventure-caption-container">
                        <small class="adventure-caption"><%# Eval("GameName") %></small>
                    </div>
                </asp:LinkButton>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>