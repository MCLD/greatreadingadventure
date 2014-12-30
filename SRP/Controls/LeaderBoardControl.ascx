<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeaderBoardControl.ascx.cs" Inherits="STG.SRP.Controls.LeaderBoardControl" %>


<div class="pill">
    <h4>Leaderboard</h4>
    <table width="100%" style="font-size: smaller;">

        <asp:Repeater runat="server" ID="rptr" >
            <ItemTemplate>
                <tr>
                    <td align="right" valign="top" style="padding-right: 5px;"><%# Eval("Rank") %>.</td>
                    <td align="left" valign="top"><%# Eval("Username") %></td>
                    <td align="right" valign="top" nowrap><%# String.Format("{0:#,##0}", (int)Eval("TotalPoints"))%></td>
                </tr>

            </ItemTemplate>
        </asp:Repeater>
        
                                           
    </table>
</div>