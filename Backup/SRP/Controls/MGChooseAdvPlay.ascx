<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGChooseAdvPlay.ascx.cs" Inherits="STG.SRP.Controls.MGChooseAdvPlay" %>

<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
	<div class="span8">



<center>
    <asp:Label ID="CAID" runat="server" Text=""  Visible="false"></asp:Label>
    <asp:Label ID="CurrStep" runat="server" Text="1" Visible="false"></asp:Label>
    <asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>

    <asp:Panel ID="pnlChoices" runat="server" Visible="True">

    <table width="100%">

        <tr>
            <td width="45%" valign="bottom" align="center">

                <asp:ImageButton ID="btn1" runat="server" CommandArgument="0" Width="100%" 
                    OnCommand="btnImg_Command"/>
                            
            </td>        
            <td width="10%">&nbsp;</td>
            <td width="45%" valign="bottom" align="center">

                <asp:ImageButton ID="btn2" runat="server" CommandArgument="0" Width="100%" 
                    OnCommand="btnImg_Command"/>
            
            </td>        
        
        </tr>


        <tr>
            <td colspan="3" valign="bottom" align="center">
                <br /><br />
                <asp:Label ID="lblSlideText" runat="server" Text="" Visible="True"></asp:Label>
                <br />
                <asp:Label ID="lblSound" runat="server" Text="" Visible="True"></asp:Label>
            </td>               
        </tr>
               
    </table>
    </asp:Panel>




    <asp:Panel ID="pnlContinue" runat="server" Visible="False">
            <h3><asp:Label ID="lblEnd" runat="server" Text="Minigame Instructions ReachedtheEnd" Visible="True"></asp:Label></h3>

        <br /><br /><br /><br />
            <asp:Button ID="btnContinue" runat="server" 
            Text="Minigame Instructions BtnContinue" CssClass="btn c" onclick="btnContinue_Click"
        />
    </asp:Panel>

</center>



	</div> 
	<div class="span2">
	</div>
</div> 

