<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeaderBoardControl.ascx.cs" Inherits="STG.SRP.Controls.LeaderBoardControl" %>


<div class="pill">
    
    <table width="100%" style="font-size: smaller;">
    <tr>
        <td colspan=4 align="center"><h4>Leaderboard</h4></td>
    </tr>
        <asp:Repeater runat="server" ID="rptr" >
            <ItemTemplate>
                <tr style="border-top: 1px dotted silver; height: 48px;">
                    <td align="right" valign="middle" style="padding-right: 5px;  "><%# Eval("Rank") %>.</td>
                    <td align="left" valign="middle"><img src='<%# string.Format("/images/avatars/sm_{0}.png", Eval("AvatarID")) %>' width="24px" height="24px"/></td>
                    <td align="left" valign="middle"><%# Eval("Username") %></td>
                    <td align="right" valign="middle" nowrap><%# String.Format("{0:#,##0}", (int)Eval("TotalPoints"))%></td>
                </tr>

            </ItemTemplate>
        </asp:Repeater>
        
                                           
    </table>
</div>