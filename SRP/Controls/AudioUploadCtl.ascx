<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AudioUploadCtl.ascx.cs" Inherits="GRA.SRP.Controls.AudioUploadCtl" %>
<asp:Panel ID="pnlExists" runat="server" Visible="False">
<table cellspacing="0" cellpadding="0">
    <tr runat="server" id="trPreview">
        <td align="center" valign="middle">
            <audio controls>
              <source src="<%=Folder.Replace("~","") +  FileName  + "." + Extension %>" type="audio/mpeg">
              Your browser does not support this audio format.
            </audio>
        </td>
    </tr>
    <tr>
        <td colspan="3" align="center">
                <br />
            <asp:Button ID="btnReplace" runat="server" onclick="btnReplace_Click" 
                Text="Replace Sound" CssClass="btn-sm btn-black"/>
                &nbsp;<asp:Button ID="btnDelete" runat="server" onclick="btnDelete_Click" 
                    Text="Delete Sound"  CssClass="btn-sm btn-red"/>
        </td>
    </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlReplace" runat="server" Visible="False">

<table width="100%">
<tr><td colspan="2">
<b>Replace Audio File: </b>
<asp:Label ID="lblUplderr1" runat="server" Text=""></asp:Label>
</td></tr>
<tr>
    <td width="100%">
         
        <asp:FileUpload ID="flUploadReplace" runat="server" Width="100%" />
    </td>
    <td nowrap=nowrap>
        &nbsp;&nbsp;
        <asp:Button ID="btnUpload" runat="server" Text="Upload" 
            onclick="btnUpload_Click" style="margin-bottom: 0px" CssClass="btn-sm btn-green"/>
        <asp:Button ID="Button1" runat="server" onclick="btnCancel1_Click" CssClass="btn-sm btn-blue"
        Text="Cancel" />
    </td>
</tr>

</table>   

</asp:Panel>
<asp:Panel ID="pnlNew" runat="server" Visible="False">

<table width="100%">
<tr><td colspan="2">
<b>Upload Audio File: </b>
<asp:Label ID="lblUplderr" runat="server" Text=""></asp:Label>
</td></tr>
<tr>
    <td width="100%">
         
        <asp:FileUpload ID="flUpload" runat="server" Width="100%" />
    </td>
    <td nowrap=nowrap>
        &nbsp;&nbsp;
        <asp:Button ID="btnUpload0" runat="server" Text="Upload" 
            onclick="btnUpload0_Click" CssClass="btn-sm btn-green"/>
    </td>
</tr>

</table>    
</asp:Panel>


