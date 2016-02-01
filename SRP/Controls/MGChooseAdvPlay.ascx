<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGChooseAdvPlay.ascx.cs" Inherits="GRA.SRP.Controls.MGChooseAdvPlay" %>

<asp:Label ID="CAID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="CurrStep" runat="server" Text="1" Visible="false"></asp:Label>
<asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>

<div class="row">
    <div class="col-xs-6">
        <asp:ImageButton ID="btn1"
            runat="server"
            CommandArgument="0"
            Width="100%"
            CssClass="img img-responsive animated fadeIn cyoImage"
            OnCommand="btnImg_Command" />
    </div>
    <div class="col-xs-6">
        <asp:ImageButton
            ID="btn2"
            runat="server"
            CommandArgument="0"
            Width="100%"
            CssClass="img img-responsive animated fadeIn cyoImage"
            OnCommand="btnImg_Command" />
    </div>
</div>

<div class="row margin-1em-top margin-1em-bottom">
    <div class="col-sm-12 animated fadeIn">
        <asp:Label ID="lblSlideText"
            runat="server"
            Visible="True"></asp:Label>
    </div>
    <div class="col-sm-12">
        <asp:Label ID="lblSound" runat="server" Visible="True"></asp:Label>
    </div>
</div>

<script>
    $('.cyoImage').on('click', function (eventObject) {
        if ($(this).hasClass('animated')) {
            $(this).removeClass('animated');
        }
        if ($(this).hasClass('fadeIn')) {
            $(this).removeClass('fadeIn');
        }
        $(this).addClass('animated pulse');
        return true;
    });
</script>
