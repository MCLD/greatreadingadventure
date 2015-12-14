<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyGamemapNavControl.ascx.cs" Inherits="GRA.SRP.Controls.MyGamemapNavControl" %>

<style>
    .NotCnt {
        position: relative;
        left: -65px;
        top: -15px;
    }

    .ra {
        text-align: right;
    }
</style>

<div runat="server" id="gamemapNav" class="secondary-nav" style="margin-top: -20px!important; margin-bottom: 40px;">
    <ul class="nav">
        <li>
            <asp:HyperLink runat="server" NavigateUrl="~/Adventures/Gameboard.aspx">
                <table>
                    <tr>
                        <td nowrap>
                            <asp:Image runat="server" BorderWidth="1" ImageUrl="~/images/game_map_icon.png" Width="128" />
                            <span class="NotCnt">
                                <asp:Label ID="Count1" runat="server" Text="" Width="20px" CssClass="ra"></asp:Label></span>
                        </td>
                        <td style="padding-left: 10px;">
                            <asp:Label ID="lbl" runat="server" Text="Play Now"></asp:Label>

                        </td>
                    </tr>
                </table>

            </asp:HyperLink></li>
    </ul>

</div>


