<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGBook.ascx.cs" Inherits="STG.SRP.Controls.MGBook" %>

<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
	<div class="span8">



<center>
    <asp:Label ID="OBID" runat="server" Text=""  Visible="false"></asp:Label>
    <asp:Label ID="CurrPage" runat="server" Text="1" Visible="false"></asp:Label>
    <asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>
    <asp:Image ID="imgSlide" runat="server" Width="500px" /> <!--Height="500px"-->
    <br /><br />
    <div style="text-align: left; width: 500px;">
    <asp:Label ID="lblEasy" runat="server" Text="" Visible="True"></asp:Label>
    <asp:Label ID="lblMedium" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblHard" runat="server" Text="" Visible="False"></asp:Label>
    </div>
    <br />
    <asp:Panel ID="pnlAudioEasy" runat="server" Visible="False">
        <audio controls>
            <source src="<%=AudioEasy%>" type="audio/mpeg">
            Your browser does not support this audio format.
        </audio>
    </asp:Panel>
    <asp:Panel ID="pnlAudioMedium" runat="server" Visible="False">
        <audio controls>
            <source src="<%=AudioMedium%>" type="audio/mpeg">
            Your browser does not support this audio format.
        </audio>
    </asp:Panel>
    <asp:Panel ID="pnlAudioHard" runat="server" Visible="False">
        <audio controls>
            <source src="<%=AudioHard%>" type="audio/mpeg">
            Your browser does not support this audio format.
        </audio>
    </asp:Panel>

    <br /><br />

    <asp:Button ID="btnPrevious" runat="server" Enabled="False" CssClass="btn c"
        Text="Minigame Instructions BtnPrev" onclick="btnPrevious_Click" />
    &nbsp;&nbsp;
    <asp:Button ID="btnNext" runat="server" Text="Minigame Instructions BtnNext" CssClass="btn c"
        onclick="btnNext_Click" />

</center>



	</div> 
	<div class="span2">
	</div>
</div> 