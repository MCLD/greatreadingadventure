<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGMixMatchPlay.ascx.cs" Inherits="STG.SRP.Controls.MGMixMatchPlay" %>
<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
	<div class="span8">





<center>
    <asp:Label ID="MMID" runat="server" Text="" Visible="false"></asp:Label>
    <asp:Label ID="CurrWins" runat="server" Text="0" Visible="false"></asp:Label>
    <asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>
    <asp:Label ID="Correct" runat="server" Text="0" Visible="false"></asp:Label>
    <asp:Label ID="MMIID" runat="server" Text="2" Visible="false"></asp:Label>
    <table width="100%">


    
    <tr>
        <td style="padding-right: 15px; padding-left: 15px;" width="33%" valign="bottom" align="center">
            <asp:ImageButton ID="btn1" runat="server" CommandArgument="1" Width="100%" 
                OnCommand="btnImg_Command"/> <!-- Height="300px" -->
        </td>
        <td style="padding-right: 15px;padding-left: 15px;" width="33%" valign="bottom" align="center">
            <asp:ImageButton ID="btn2" runat="server" CommandArgument="2" Width="100%"  
                OnCommand="btnImg_Command"/> <!-- Height="300px" -->
        </td>
        <td style="padding-right: 15px;padding-left: 15px;" width="33%" valign="bottom" align="center">
            <asp:ImageButton ID="btn3" runat="server" CommandArgument="3" Width="100%"  
                OnCommand="btnImg_Command"/> <!-- Height="300px" -->
        </td>
        <td style="padding-left: 0px; padding-left: 0px;" width="1%" valign="bottom" align="center">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td style="padding-top: 25px;" width="100%" valign="bottom" align="center" colspan="4">
            <asp:Label ID="lblEasy" runat="server" Text="" Visible="True" CssClass="mmFont"></asp:Label>
            <asp:Label ID="lblMedium" runat="server" Text="" Visible="False" CssClass="mmFont"></asp:Label>
            <asp:Label ID="lblHard" runat="server" Text="" Visible="False" CssClass="mmFont"></asp:Label>
            <asp:Label ID="lblMessage" runat="server" Text="" Visible="True"></asp:Label>

                <asp:Panel ID="pnlAudioEasy" runat="server" Visible="False">
                    <audio controls>
                        <source src="<%=AudioEasy%>" type="audio/mpeg">
                        Your browser does not support this audio format.
                    </audio>
                </asp:Panel>
                <asp:Panel ID="pnlAudioMedium" runat="server" Visible="False">
                    <audio controls>
                        <source src="<%=AudioMedium%>" type="audio/mpeg">
                        Your browser does not support this audio format.
                    </audio>
                </asp:Panel>
                <asp:Panel ID="pnlAudioHard" runat="server" Visible="False">
                    <audio controls>
                        <source src="<%=AudioHard%>" type="audio/mpeg">
                        Your browser does not support this audio format.
                    </audio>
                </asp:Panel>

        </td>
    </tr>
    
    </table>


    <asp:Panel ID="pnlContinue" runat="server" Visible="False">
        <br />
            <asp:Button ID="btnContinue" runat="server" 
            Text="Minigame Instructions BtnContinue" CssClass="btn c" onclick="btnContinue_Click"
        />
    </asp:Panel>


</center>




	</div> 
	<div class="span2">
	</div>
</div> 