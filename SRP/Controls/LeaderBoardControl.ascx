<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeaderBoardControl.ascx.cs" Inherits="GRA.SRP.Controls.LeaderBoardControl" %>

<div class="row">
    <div class="col-xs-12">
        <table class="table table-striped table-hover table-condensed">
            <thead>
                <tr>
                    <th colspan="4">
                        <h4>Leaderboard</h4>
                    </th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="rptr" OnItemDataBound="rptr_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("Rank") %>.</td>
                            <td><asp:Image runat="server" Width="24" Height="24" ImageUrl='<%#Eval("AvatarId", "~/Images/Avatars/sm_{0}.png") %>' id="SmallAvatar"/></td>
                            <td><%# Eval("Username") %></td>
                            <td><%# String.Format("{0:#,##0}", (int)Eval("TotalPoints"))%></td>
                        </tr>

                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</div>
