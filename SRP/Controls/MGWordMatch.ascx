<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGWordMatch.ascx.cs" Inherits="STG.SRP.Controls.MGWordMatch" %>
<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
	<div class="span8">



<center>
    <asp:Label ID="WMID" runat="server" Text="" Visible="false"></asp:Label> 
    <asp:Label ID="CurrWins" runat="server" Text="0" Visible="false"></asp:Label> 
    <asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label> 
    <asp:Label ID="Correct" runat="server" Text="0" Visible="false"></asp:Label> 
    <asp:Label ID="WMIID" runat="server" Text="-1" Visible="false"></asp:Label> 
    <table width="100%">

    <tr>
         <td style="" width="100%" valign="bottom" align="center" colspan="3">
            <asp:Label ID="lblMessage" runat="server" Text="" Visible="True"></asp:Label>
         </td>
    </tr>
    
    <tr>
        <td style="padding-right: 15px;" width="10%" valign="bottom" align="center">
        &nbsp;
        </td>
        <td style="padding-right: 15px;padding-left: 15px;" width="80%" valign="bottom" align="center">
            <asp:Image ID="imgItem" runat="server" Width="100%"  /> <!-- Height="300px" -->
        </td>
        <td style="padding-left: 15px;" width="10%" valign="bottom" align="center">
            &nbsp;
        </td>
    </tr>
    <tr>
        <td style="padding-top: 25px;" width="100%" valign="bottom" align="left" colspan="3">
    <asp:Panel ID="pnlChoices" runat="server" Visible="True">

        <ol>
        <asp:Repeater ID="rptr" runat="server" onitemcommand="rptr_ItemCommand" 
                onitemdatabound="rptr_ItemDataBound">
            <ItemTemplate>
                <li>
                
                    <asp:Button ID="btnText" runat="server" Text='<%# SetButonText(Eval("EasyLabel").ToString(), Eval("MediumLabel").ToString(), Eval("HardLabel").ToString()) %>'
                     Visible="True" CommandArgument='<%# Eval("WMIID") %>'
                     CssClass="btn a" Width="100%"
                     ></asp:Button>
                     <br />
                    <div style="padding-left: 75px; ">
                        <asp:Panel ID="pnlAudioEasy" runat="server" Visible='<%#SetAudioVisibility("/images/games/mixmatch/e_" + Eval("WMIID") + ".mp3")%>'>
                            <audio controls>
                                <source src='<%# "/images/games/mixmatch/e_" + Eval("WMIID") + ".mp3"%>' type="audio/mpeg">
                                Your browser does not support this audio format.
                            </audio>
                        </asp:Panel>
                        <asp:Panel ID="pnlAudioMedium" runat="server" Visible='<%#SetAudioVisibility("/images/games/mixmatch/m_" + Eval("WMIID") + ".mp3")%>'>
                            <audio controls>
                                <source src='<%# "/images/games/mixmatch/m_" + Eval("WMIID") + ".mp3"%>' type="audio/mpeg">
                                Your browser does not support this audio format.
                            </audio>
                        </asp:Panel>
                        <asp:Panel ID="pnlAudioHard" runat="server" Visible='<%#SetAudioVisibility("/images/games/mixmatch/h_" + Eval("WMIID") + ".mp3")%>'>
                            <audio controls>
                                <source src='<%# "/images/games/mixmatch/h_" + Eval("WMIID") + ".mp3"%>' type="audio/mpeg">
                                Your browser does not support this audio format.
                            </audio>
                        </asp:Panel>

                        </div>
                        <br /><br />
                </li>
            
            
            </ItemTemplate>
        </asp:Repeater>
         </ol>       
            
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