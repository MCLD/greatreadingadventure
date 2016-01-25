<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGBook.ascx.cs" Inherits="GRA.SRP.Controls.MGBook" %>

<asp:Label ID="OBID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="CurrPage" runat="server" Text="1" Visible="false"></asp:Label>
<asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>

<script>
    $('.animated').on('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function (eventObject) {
        $(this).removeClass('animated');
        if ($(this).hasClass('fadeOutLeftBig')) {
            $(this).hide();
            $(this).removeClass('fadeOutLeftBig');
        }
        if ($(this).hasClass('fadeOutRightBig')) {
            $(this).hide();
            $(this).removeClass('fadeOutRightBig');
        }
        if ($(this).hasClass('fadeIn')) {
            $(this).removeClass('fadeIn');
        }
    });
</script>

<div class="bookPageContent">
    <div class="row">
        <div class="col-sm-12">
            <asp:Image ID="imgSlide" 
                runat="server" 
                Width="500px" 
                CssClass="img img-responsive center-block animated fadeIn" />
        </div>
    </div>
    <div class="row margin-1em-top">
        <div class="col-sm-12 text-center">
            <asp:Label ID="lblText" 
                runat="server" 
                Visible="True"
                CssClass="animated fadeIn"></asp:Label>
        </div>
    </div>
</div>

<asp:Panel ID="pnlAudio" runat="server" Visible="False" CssClass="row margin-1em-top">
    <div class="col-sm-12">
        <asp:Label ID="lblSound" runat="server"></asp:Label>
    </div>
</asp:Panel>

<div class="row margin-1em-top">
    <div class="col-sm-3 col-sm-offset-2 col-xs-6 text-center">
        <asp:Button
            ID="btnPrevious"
            runat="server"
            Enabled="False"
            CssClass="btn btn-default"
            Text="adventures-instructions-button-previous"
            OnClick="btnPrevious_Click"
            OnClientClick="bookPrev(); return true;" />

    </div>
    <div class="col-sm-3 col-sm-offset-2 col-xs-6 text-center">
        <asp:Button ID="btnNext"
            runat="server"
            Text="adventures-instructions-button-next"
            CssClass="btn btn-default"
            OnClick="btnNext_Click"
            OnClientClick="bookNext(); return true;" />
    </div>
</div>
<script>
    function bookNext() {
        $('.bookPageContent').addClass('animated fadeOutLeftBig');
    }

    function bookPrev() {
        $('.bookPageContent').addClass('animated fadeOutRightBig');
    }
</script>