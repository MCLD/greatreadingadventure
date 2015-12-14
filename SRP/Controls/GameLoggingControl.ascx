<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GameLoggingControl.ascx.cs" Inherits="GRA.SRP.Controls.GameLoggingControl" %>
<div class="row">
    <div class="col-xs-12">
        <div style="width: 90%; padding: 5px 5px 5px 5px; border: 1px solid gray; overflow: auto; overflow-y: hidden; -ms-overflow-y: hidden; padding-right: 20px;">
            <table cellpadding="0" cellspacing="0" border="0" style="table-layout: fixed; margin-left: 5px; margin-right: 15px;">
                <tr>
                    <asp:Repeater runat="server" ID="rptrx1" OnItemCommand="rptrx1_ItemCommand"
                        OnItemDataBound="rptrx1_ItemDataBound">
                        <ItemTemplate>

                            <td
                                style="height: 170px; border: 1px solid gray; padding: 10px 10px 10px 10px;"
                                valign="middle"
                                align="center">
                                <div style="width: 85px; border: 0px solid red; word-wrap: break-word; height: 100%;">
                                    <table height="100%" border="0" style="height: 100%;">
                                        <tr>
                                            <td height="100%" align="center" valign="top">
                                                <asp:ImageButton ID="ImageButton1" runat="server" CommandArgument='<%# Eval("MGID") %>' CommandName="Play"
                                                    ImageUrl='<%# (System.IO.File.Exists(Server.MapPath(string.Format("/images/Games/{0}.png",Eval("MGID"))))? string.Format("~/images/Games/{0}.png",Eval("MGID")) : "~/images/MiniGames.png") %>'
                                                    Width="80px"
                                                    BorderStyle="Ridge" BorderWidth="3px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" valign="bottom">

                                                <b><small><%# Eval("GameName") %></small></b>
                                                <br />
                                                <asp:Button ID="Button6" runat="server" Text="Play" CommandArgument='<%# Eval("MGID") %>' CommandName="Play" Width="75px" />
                                                <br />
                                            </td>
                                        </tr>
                                    </table>

                                </div>
                            </td>
                        </ItemTemplate>
                        <FooterTemplate>
                            <div style="text-align: left; font-size: smaller;">
                                <asp:Label ID="lblEmptyMsg" runat="server" Text="<img src='/images/shield_blue.png' align='left' border=0 style='padding-right:15px;' width='75px'>Keep reading, and keep logging your progress.  As you get more points for reading and entering secret codes, new games and new levels will be unlocked." Visible="false"></asp:Label>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </tr>
            </table>
        </div>
    </div>
</div>
<div class="row">
    <div class="col-xs-12">

        <div style="width: 90%; padding: 5px 5px 5px 5px; border: 1px solid gray; overflow: auto; overflow-y: hidden; -ms-overflow-y: hidden; padding-right: 20px;">
            <table cellpadding="0" cellspacing="0" border="0" style="table-layout: fixed; margin-left: 5px; margin-right: 15px;">
                <tr>

                    <asp:Repeater runat="server" ID="rptrx2" OnItemCommand="rptrx1_ItemCommand"
                        OnItemDataBound="rptrx2_ItemDataBound">
                        <ItemTemplate>


                            <td
                                style="height: 170px; border: 1px solid gray; padding: 10px 10px 10px 10px;"
                                valign="middle"
                                align="center">
                                <div style="width: 85px; border: 0px solid red; word-wrap: break-word; height: 100%;">
                                    <table height="100%" border="0" style="height: 100%;">
                                        <tr>
                                            <td height="100%" align="center" valign="top">
                                                <asp:ImageButton ID="ImageButton1" runat="server" CommandArgument='<%# Eval("MGID") %>' CommandName="Play"
                                                    ImageUrl='<%# (System.IO.File.Exists(Server.MapPath(string.Format("/images/Games/{0}.png",Eval("MGID"))))? string.Format("~/images/Games/{0}.png",Eval("MGID")) : "~/images/MiniGames.png") %>'
                                                    Width="80px"
                                                    BorderStyle="Ridge" BorderWidth="3px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center" valign="bottom">

                                                <b><small><%# Eval("GameName") %></small></b>
                                                <br />
                                                <asp:Button ID="Button6" runat="server" Text="Play" CommandArgument='<%# Eval("MGID") %>' CommandName="Play" Width="75px" />
                                                <br />
                                            </td>
                                        </tr>
                                    </table>

                                </div>
                            </td>
                        </ItemTemplate>
                        <FooterTemplate>
                            <div style="text-align: left; font-size: smaller;">
                                <asp:Label ID="lblEmptyMsg" runat="server" Text="<img src='/images/shield_green.png' align='left' border=0 style='padding-right:15px;' width='75px'>Keep reading, and keep logging your progress.  As you get more points for reading and entering secret codes, new games and new levels will be unlocked." Visible="false"></asp:Label>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>

                </tr>
            </table>
        </div>
    </div>
</div>