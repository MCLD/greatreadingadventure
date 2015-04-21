<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyGamemapNavControl.ascx.cs" Inherits="STG.SRP.Controls.MyGamemapNavControl" %>

<style>
.NotCnt
{
	 position:relative; left:-65px; top:-15px;
}
.ra
{
	 text-align: right;
}
</style>

    <div class="secondary-nav" style="margin-top: -20px!important; margin-bottom:40px;">
        <ul class="nav">
              <li><a href="/MyGameboard.aspx" 
                    >
                    <table><tr><td nowrap>
                        
                        <img border="1" src="/images/game_map_icon.png" width="128" />
                        <span Class="NotCnt"><asp:Label ID="Count1" runat="server" Text="" Width="20px" CssClass="ra"></asp:Label></span>
                        </td><td style="padding-left:10px; ">
                        <asp:Label ID="lbl" runat="server" Text="Play Now"></asp:Label>
                        
                        </td></tr></table>
                     
                    </a></li>
        </ul>
      
    </div>


