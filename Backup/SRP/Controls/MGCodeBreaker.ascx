<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGCodeBreaker.ascx.cs" Inherits="STG.SRP.Controls.MGCodeBreaker" %>
<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
	<div class="span8">
    
    
    
<center>
    <asp:Label ID="CBID" runat="server" Text="" Visible="false"></asp:Label>
    <asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>
    <asp:Label ID="Correct" runat="server" Text="0" Visible="false" CssStyle="Correct"></asp:Label>
    <table width="100%">

    <tr>
         <td style="" width="100%" valign="bottom" align="center" colspan="2">
         </td>
    </tr>
    
    <tr>
        <td style="padding-right: 15px;" width="100%" valign="top" align="center">            
            <br/><br />
                        <asp:Label ID="lblMessage" runat="server" Text="" Visible="True"></asp:Label>

            <br/><br />
            <asp:Label ID="lblEncoded" runat="server" ></asp:Label>

            <br/><br />
            <asp:Label ID="lblAnswer" runat="server" Text="Minigame label Enter Decoded" Visible="True"></asp:Label>
            <br/><br />
            <asp:Label ID="lblMessage2" runat="server" Text="" Visible="True"></asp:Label>
            <asp:TextBox ID="txtAnswer" runat="server" Text="" Visible="True" Width="100%"></asp:TextBox>
            <br /><br />
            <asp:Button ID="btnScore" runat="server" Text="Minigame btnScore" 
                onclick="btnScore_Click" CssClass="btn c"/>

            <asp:Button ID="btnContinue" runat="server" 
            Text="Minigame BtnContinue" CssClass="btn c" onclick="btnContinue_Click" Visible="false" />

        </td>
        <td style="padding-left: 15px;"  valign="top" align="center" >
            <H3><asp:Label ID="lblMsgKey" runat="server" Text-="Minigame Key"></asp:Label></H3>
            <hr />
            <asp:Label ID="lblKey" runat="server" ></asp:Label>
        </td>
    </tr>

    
    </table>

</center>





	</div> 
	<div class="span2">
	</div>
</div> 