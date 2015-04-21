<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimpleLoggingControl.ascx.cs" Inherits="STG.SRP.Controls.SimpleLoggingControl" %>





<br />
<div class="pill">

<h3><!--Log Activity-->Reading Log</h3>
<style>
.padded {padding: 10px 10px 10px 10px;}
.attn { border: 1px solid green; }
</style>
   
<table width="100%"> 
<tr>
    <td colspan="3" class="padded ">
        <asp:Label ID="lblMessage" runat="server" Text="" Font-Bold="true" ></asp:Label>
    </td>
</tr> 
</table>
<table width="100%" runat="server" ID="EntryTable"> 
<tr>
    <td valign="top" nowrap> <span style="padding-top:5px;">I read </span>
        <asp:TextBox ID="txtCountSubmitted" runat="server" Width="50px"></asp:TextBox>
    </td>
    <td  nowrap>
        <asp:RadioButtonList ID="rbActivityType" runat="server">
        </asp:RadioButtonList>
    </td>
    <td width="100%"></td>
</tr>
<tr>
<td colspan="3">
    <asp:Panel ID="pnlTitleAndAuthor" runat="server" Visible="true">
        <table width="98%">
            <tr>
                <td width="50%"><b>Title</b></td>
                <td width="50%"><b>Author</b></td>
            </tr>
            <tr>
                <td><asp:TextBox ID="txtTitle" runat="server"  width="100%"></asp:TextBox></td>
                <td><asp:TextBox ID="txtAuthor" runat="server" width="100%"></asp:TextBox></td>            
            </tr>        
        </table>

    </asp:Panel>
</td>
</tr>
<tr>
<td colspan="3">
    <asp:Panel ID="pnlReview" runat="server" Visible="false">
        <b>Write a Review:</b><br />
        <asp:TextBox ID="Review" runat="server"  width="98%" Rows="4" TextMode="MultiLine"></asp:TextBox>
    </asp:Panel>
</td>
</tr>
<tr>
    <td colspan="3" style="">
        <strong>OR</strong>
    </td>
</tr> 
<tr>
    <td valign="top" nowrap style="padding-top:7px;"><asp:Label ID="lblRedeem" runat="server" Text="Secret Code: "></asp:Label> &nbsp;</td>
    <td nowrap>
        <asp:TextBox ID="txtProgramCode" runat="server" Width="200px"></asp:TextBox>
    </td>
    <td width="100%"></td>
</tr>
</table>
<table width="100%">
<tr>
    <td colspan="3">
        <asp:Label ID="lblFB" runat="server" Text="" Visible="false"></asp:Label>
        <asp:Button ID="btnSubmit" runat="server" Text="Submit"  CssClass="btn a" onclick="btnSubmit_Click" /> 
        <asp:Button ID="btnReSubmit" runat="server" Text="Submit More"  CssClass="btn a" onclick="btnReSubmit_Click" Visible="false"/> 
        &nbsp; 
        <asp:Button ID="btnHistory"   CssClass="btn a" runat="server" Text="History" onclick="btnHistory_Click" /> 
    </td>
</tr> 

</table>  
<asp:Label ID="lblPID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="lblPGID" runat="server" Text="" Visible="false"></asp:Label>


</div>


