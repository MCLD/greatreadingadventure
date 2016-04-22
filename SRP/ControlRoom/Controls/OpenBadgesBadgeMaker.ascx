<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OpenBadgesBadgeMaker.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.OpenBadgesBadgeMaker" %>
<script>
    $(document).ready(function () {
        window.onmessage = function (e) {
            if (e.origin == 'https://www.openbadges.me') {
                $.fancybox.close();
                if (e.data != 'cancelled') {
                    __doPostBack('<%=fblaunch.ClientID%>', e.data);
                }
            }
        };
    });

    $('#<%=fblaunch.ClientID%>').fancybox({
        width: '95%',
        height: '70%',
        minHeight: 680,
        autoSize: false,
        closeClick: false,
        openEffect: 'fade',
        closeEffect: 'none'
    });
</script>
<asp:HiddenField ID="BadgeFileName" runat="server" />
<asp:HiddenField ID="SmallThumbnailSize" runat="server" />
<asp:HiddenField ID="MediumThumbnailSize" runat="server" />
<a id="fblaunch" data-fancybox-type="iframe" runat="server" class="btn-lg btn-blue">Badge designer
</a>