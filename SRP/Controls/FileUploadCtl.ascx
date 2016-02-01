<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploadCtl.ascx.cs" Inherits="GRA.SRP.Classes.FileDownloadCtl" %>
   <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
   <script type="text/javascript" src="/Scripts/jquery.bpopup.min.js"></script>
<asp:Panel ID="pnlExists" runat="server" Visible="False">
<table cellspacing="0" cellpadding="5">
    <tr>
        <td colspan="3"><b>Preview</b></td>
    </tr>
    <tr>
        <td align="center" valign="middle">
            <asp:HyperLink ID="PreviewImage1" runat="server" rel='lightbox[Image 90% 90%]'
                ImageUrl="~/images/incorrect.png" CssClass="controlroom-small-preview">HyperLink</asp:HyperLink>
        </td>
        <td align="center" valign="middle">
            <asp:HyperLink ID="PreviewImage2" runat="server" rel='lightbox[Image 90% 90%]'
                ImageUrl="~/images/incorrect.png"  CssClass="controlroom-medium-preview">HyperLink</asp:HyperLink>
        </td>
        <td align="center" valign="middle">
            <asp:HyperLink ID="PreviewImage3" runat="server" rel='lightbox[Image 90% 90%]'
                ImageUrl="~/images/incorrect.png"  CssClass="controlroom-large-preview">HyperLink</asp:HyperLink>
            
                <div id='element_to_pop_up1<%# PreviewImage1.ClientID %>'>
                    <asp:Image ID="Image1" runat="server" />
                </div>
                <div id='element_to_pop_up2<%# PreviewImage1.ClientID %>'>
                    <asp:Image ID="Image2" runat="server" />
                </div>
                <div id='element_to_pop_up3<%# PreviewImage1.ClientID %>'>
                    <asp:Image ID="Image3" runat="server" />
                </div>
                <style>
                    #element_to_pop_up1<%# PreviewImage1.ClientID %> { display:none; }
                    #element_to_pop_up2<%# PreviewImage1.ClientID %> { display:none; }
                    #element_to_pop_up3<%# PreviewImage1.ClientID %> { display:none; }
                    #element_to_pop_up3<%# PreviewImage1.ClientID %>, 
                    #element_to_pop_up2<%# PreviewImage1.ClientID %>, 
                    #element_to_pop_up1<%# PreviewImage1.ClientID %> {background-color:#fff;border-radius:10px 10px 10px 10px;box-shadow:0 0 25px 5px #999;color:#111;display:none;min-width:50px;padding:25px}
                     
                </style>
                <script >

                  (function ($) {

                        // DOM Ready
                        $(function () {

                            $('<%# "#" + PreviewImage1.ClientID %>').bind('click', function (e) {
                                e.preventDefault();
                                $('#element_to_pop_up1<%# PreviewImage1.ClientID %>').bPopup();
                            });

                            $('<%# "#" + PreviewImage2.ClientID %>').bind('click', function (e) {
                                e.preventDefault();
                                $('#element_to_pop_up2<%# PreviewImage1.ClientID %>').bPopup();
                            });

                            $('<%# "#" + PreviewImage3.ClientID %>').bind('click', function (e) {
                                e.preventDefault();
                                $('#element_to_pop_up3<%# PreviewImage1.ClientID %>').bPopup();
                            });

                        });

                    })(jQuery);
                </script>


        </td>
    </tr>
    <tr>
        <td align="center"><asp:Label ID="lblSm" runat="server" Text="<b>Thumnbail</b><br />"></asp:Label></td>
        <td align="center"><asp:Label ID="lblMd" runat="server" Text="<b>Medium Size</b><br />"></asp:Label></td>
        <td align="center"><asp:Label ID="lblLg" runat="server" Text="<b>Image</b><br />"></asp:Label></td>
    </tr>
    <tr>
        <td colspan="3" align="center">
                <br />
            <asp:Button ID="btnReplace" runat="server" onclick="btnReplace_Click" 
                Text="Replace Image" CssClass="btn-sm btn-black"/>
                &nbsp;<asp:Button ID="btnDelete" runat="server" onclick="btnDelete_Click" 
                    Text="Delete Image"  CssClass="btn-sm btn-red"/>
        </td>
    </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlReplace" runat="server" Visible="False">

<table width="100%">
<tr><td colspan="2">
<b>Replace Image: </b>
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
<b>Upload Image: </b>
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

