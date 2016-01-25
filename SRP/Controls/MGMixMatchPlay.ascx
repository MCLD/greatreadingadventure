<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGMixMatchPlay.ascx.cs" Inherits="GRA.SRP.Controls.MGMixMatchPlay" %>

<asp:Label ID="MMID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="CurrWins" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>
<asp:Label ID="Correct" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="MMIID" runat="server" Text="2" Visible="false"></asp:Label>

<div class="row margin-1em-top">
    <div class="col-xs-4">
        <asp:ImageButton ID="btn1"
            runat="server"
            CommandArgument="1"
            Width="100%"
            CssClass="img img-responsive animated fadeIn mixMatchImg"
            OnCommand="btnImg_Command" />
    </div>
    <div class="col-xs-4">
        <asp:ImageButton ID="btn2"
            runat="server"
            CommandArgument="2"
            Width="100%"
            CssClass="img img-responsive animated fadeIn mixMatchImg"
            OnCommand="btnImg_Command" />
    </div>
    <div class="col-xs-4">
        <asp:ImageButton ID="btn3"
            runat="server"
            CommandArgument="3"
            Width="100%"
            CssClass="img img-responsive animated fadeIn mixMatchImg"
            OnCommand="btnImg_Command" />
    </div>
</div>
<div class="row margin-1em-top">
    <div class="col-sm-12 text-center animated fadeIn">
        <asp:Label ID="lblMixMatch" runat="server" Visible="True" CssClass="h1"></asp:Label>
    </div>
</div>

<asp:Panel ID="pnlAudio" runat="server" Visible="False" CssClass="row">
    <div class="col-sm-12">
        <asp:Label ID="lblSound" runat="server" Visible="True"></asp:Label>
    </div>
</asp:Panel>

<asp:Panel ID="pnlContinue" runat="server" Visible="False" CssClass="row margin-1em-top">
    <div class="col-sm-4 col-sm-offset-4">
        <asp:Button ID="btnContinue" 
            runat="server"
            CssClass="btn btn-success btn-block"
            OnClick="btnContinue_Click" />
    </div>
</asp:Panel>


<script>
    $('.mixMatchImg').on('click', function (eventObject) {
        if ($(this).hasClass('animated')) {
            $(this).removeClass('animated');
        }
        if ($(this).hasClass('fadeIn')) {
            $(this).removeClass('fadeIn');
        }
        $(this).addClass('animated jello');
        return true;
    });
</script>
