<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploadCtl.ascx.cs" Inherits="GRA.SRP.Classes.FileDownloadCtl" %>
<asp:Panel ID="pnlExists" runat="server" Visible="False">
    <table cellspacing="0" cellpadding="5">
        <tr>
            <td colspan="3"><b>Preview</b></td>
        </tr>
        <tr>
            <td align="center" valign="middle">
                <asp:HyperLink ID="PreviewImage1" runat="server"
                    ImageUrl="~/images/incorrect.png" CssClass="fancybox controlroom-small-preview">HyperLink</asp:HyperLink>
            </td>
            <td align="center" valign="middle">
                <asp:HyperLink ID="PreviewImage2" runat="server"
                    ImageUrl="~/images/incorrect.png" CssClass="fancybox controlroom-medium-preview">HyperLink</asp:HyperLink>
            </td>
            <td align="center" valign="middle">
                <asp:HyperLink ID="PreviewImage3" runat="server"
                    ImageUrl="~/images/incorrect.png" CssClass="fancybox controlroom-large-preview">HyperLink</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:Label ID="lblSm" runat="server" Text="<b>Thumnbail</b><br />"></asp:Label></td>
            <td align="center">
                <asp:Label ID="lblMd" runat="server" Text="<b>Medium Size</b><br />"></asp:Label></td>
            <td align="center">
                <asp:Label ID="lblLg" runat="server" Text="<b>Image</b><br />"></asp:Label></td>
        </tr>
        <tr>
            <td colspan="3" align="center">
                <br />
                <asp:Button ID="btnReplace" runat="server" OnClick="btnReplace_Click"
                    Text="Replace Image" CssClass="btn-sm btn-black" />
                &nbsp;<asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click"
                    Text="Delete Image" CssClass="btn-sm btn-red" />
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlReplace" runat="server" Visible="False">

    <table width="100%">
        <tr>
            <td colspan="2">
                <b>Replace Image: </b>
                <asp:Label ID="lblUplderr1" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td width="100%">
                <asp:FileUpload ID="flUploadReplace" runat="server" Width="100%" />
            </td>
            <td nowrap="nowrap">&nbsp;&nbsp;
        <asp:Button ID="btnUpload" runat="server" Text="Upload"
            OnClick="btnUpload_Click" Style="margin-bottom: 0px" CssClass="btn-sm btn-green" />
                <asp:Button ID="Button1" runat="server" OnClick="btnCancel1_Click" CssClass="btn-sm btn-blue"
                    Text="Cancel" />
                <script>
                    $().ready(function () {
                        $('#<%=flUploadReplace.ClientID%>').on('change', function () {
                            $('#<%=btnUpload.ClientID%>').click();
                        });
                    });
                </script>

            </td>
        </tr>

    </table>

</asp:Panel>
<asp:Panel ID="pnlNew" runat="server" Visible="False">

    <table width="100%">
        <tr>
            <td colspan="2">
                <b>Upload Image: </b>
                <asp:Label ID="lblUplderr" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td width="100%">

                <asp:FileUpload ID="flUpload" runat="server" Width="100%" />
            </td>
            <td nowrap="nowrap">&nbsp;&nbsp;
        <asp:Button ID="btnUpload0" runat="server" Text="Upload"
            OnClick="btnUpload0_Click" CssClass="btn-sm btn-green" />
                <script>
                    $().ready(function () {
                        $('#<%=flUpload.ClientID%>').on('change', function () {
                            $('#<%=btnUpload0.ClientID%>').click();
                        });
                    });
                </script>
            </td>
        </tr>

    </table>
</asp:Panel>

