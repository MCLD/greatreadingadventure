<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGChooseAdvPlay.ascx.cs" Inherits="GRA.SRP.Controls.MGChooseAdvPlay" %>

<asp:Label ID="CAID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="CurrStep" runat="server" Text="1" Visible="false"></asp:Label>
<asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>

<div class="row">
    <div class="col-sm-12 animated fadeIn">
        <asp:Literal ID="lblSlideText"
            runat="server"></asp:Literal>
    </div>
    <div class="col-sm-12">
        <asp:Label ID="lblSound" runat="server" Visible="True"></asp:Label>
    </div>
</div>

<div class="cyo-choose-text">
    <asp:Literal runat="server" Text="adventures-instructions-cyoa-choose"></asp:Literal></div>

<div class="row margin-1em-top margin-1em-bottom">
    <div class="col-xs-12 col-sm-6 margin-1em-bottom" runat="server" id="cyoContainer1">
        <asp:ImageButton ID="btn1"
            runat="server"
            CommandArgument="0"
            Width="100%"
            CssClass="img img-responsive animated fadeIn cyo-image"
            OnCommand="btnImg_Command" />
    </div>
    <div class="col-xs-12 col-sm-6 margin-1em-bottom" runat="server" id="cyoContainer2">
        <asp:ImageButton
            ID="btn2"
            runat="server"
            CommandArgument="0"
            Width="100%"
            CssClass="img img-responsive animated fadeIn cyo-image"
            OnCommand="btnImg_Command" />
    </div>
</div>

<script>
    $('.cyo-image').on('click', function (eventObject) {
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
