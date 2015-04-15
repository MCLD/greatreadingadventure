<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyBadgesListControl.ascx.cs" Inherits="STG.SRP.Controls.MyBadgesListControl" %>


<div class="pill">
    <h4>My Badges</h4><hr />
    <!--<small>These are the badges you have earned! <a href="/MyBadges.aspx">Click here for more details! </a></small>-->
    <small>See <a href="/MyBadges.aspx">my badges </a></small><br />
    <small>Discover <a href="/BadgeGallery.aspx">more badges </a></small>

    <hr />
    <div style="font-size: smaller; max-height: 350px; overflow:auto; overflow-x: hidden; -ms-overflow-x: hidden; white-space: nowrap; border: 1px solid silver;">
    <table width="100%" style="font-size: smaller; max-height: 500px; overflow:scroll;" >

           <asp:Repeater runat="server" ID="rptr" >
                <ItemTemplate>
            
            <%# (((long)Eval("Rank")) % 2 != 0 ? "<tr>" : "") %>
            
                <td align="center" valign="top" style="padding:10px 10px 10px 10px;">
                    
                    <img src='/images/badges/sm_<%# Eval("BadgeID") %>.png' />

                </td>
            
            <%# (((long)Eval("Rank")) % 2 == 0 ? "</tr>" : "") %>                
                                
                </ItemTemplate>
                
            </asp:Repeater>
        <asp:Label ID="NoBadges" runat="server" Text="You have not earned any badges yet." Visible="false"></asp:Label>


                                                                                                                               
    </table>
    </div>
</div>