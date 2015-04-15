<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MinigamePreview.ascx.cs" Inherits="STG.SRP.ControlRoom.Controls.MinigamePreview" %>
<%@ Register src="~/Controls/MGBook.ascx" tagname="MGBook" tagprefix="uc1" %>
<%@ Register src="~/Controls/MGMixMatchPlay.ascx" tagname="MGMixMatchPlay" tagprefix="uc2" %>
<%@ Register src="~/Controls/MGCodeBreaker.ascx" tagname="MGCodeBreaker" tagprefix="uc3" %>
<%@ Register src="~/Controls/MGWordMatch.ascx" tagname="MGWordMatch" tagprefix="uc4" %>
<%@ Register src="~/Controls/MGMatchingGamePlay.ascx" tagname="MGMatchingGamePlay" tagprefix="uc7" %>
<%@ Register src="~/Controls/MGHiddenPicPlay.ascx" tagname="MGHiddenPicPlay" tagprefix="uc6" %>
<%@ Register src="~/Controls/MGChooseAdvPlay.ascx" tagname="MGChooseAdvPlay" tagprefix="uc5" %>

<link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/themes/smoothness/jquery-ui.css" />
<script src="//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js"></script>
<script src="//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script>

<div style="max-width: 1190px; text-align:center; >

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>

<center>
<div style="width:  50%; ">
<asp:Label ID="MGID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="GPSID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="PID" runat="server" Text="" Visible="false"></asp:Label>
<div class="row">
	<div class="span1">
	</div>
    <div class="span10">
         <h2 class="title-divider">
              <asp:Label ID="MGName" runat="server" Text=""></asp:Label>
         </h2>
         <asp:Label ID="lbl_mg1" runat="server" Text="Minigame Instructions OB" Visible="False"></asp:Label>
         <asp:Label ID="lbl_mg2" runat="server" Text="Minigame Instructions MM" Visible="False"></asp:Label>
         <asp:Label ID="lbl_mg3" runat="server" Text="Minigame Instructions CB" Visible="False"></asp:Label>
         <asp:Label ID="lbl_mg4" runat="server" Text="Minigame Instructions WM" Visible="False"></asp:Label>
         <asp:Label ID="lbl_mg5" runat="server" Text="Minigame Instructions MG" Visible="False"></asp:Label>
         <asp:Label ID="lbl_mg6" runat="server" Text="Minigame Instructions HP" Visible="False"></asp:Label>
         <asp:Label ID="lbl_mg7" runat="server" Text="Minigame Instructions CA" Visible="False"></asp:Label>
    </div>
	<div class="span1">
	</div>   
</div> 
<asp:Panel ID="pnlDifficulty" runat="server" Visible="true">
<div class="row" style="min-height: 400px;">
	<div class="span1">
	</div>
    <div class="span10">

        <center>
        <asp:Label ID="lblDiff" runat="server" Text="Minigame Instructions Difficulty" Visible="true"></asp:Label>
        <br /><br />

        <asp:Button ID="BtnEasy" runat="server" Text="Minigame Instructions BtnEasy" 
                CssClass="btn b" Width="150px" onclick="BtnEasy_Click"/>
        &nbsp;
        <asp:Button ID="BtnMedium" runat="server" Text="Minigame Instructions BtnMedium" 
                CssClass="btn b" Width="150px" onclick="BtnMedium_Click"/>
        &nbsp;
        <asp:Button ID="BtnHard" runat="server" Text="Minigame Instructions BtnHard" 
                CssClass="btn b" Width="150px" onclick="BtnHard_Click"/>
        </center>
	</div> 
	<div class="span1">
	</div>
</div> 

</asp:Panel>

<asp:Panel ID="pnlGame" runat="server" Visible="false">

        <uc1:MGBook ID="MGBook1" runat="server" Visible="false"/>
        <uc2:MGMixMatchPlay ID="MGMixMatchPlay1" runat="server" Visible="false"/>
        <uc3:MGCodeBreaker ID="MGCodeBreaker1" runat="server" Visible="false"/>
        <uc4:MGWordMatch ID="MGWordMatch1" runat="server" Visible="false" />

        <uc5:MGChooseAdvPlay ID="MGChooseAdvPlay1" runat="server" Visible="false" />
        <uc6:MGHiddenPicPlay ID="MGHiddenPicPlay1" runat="server" Visible="false" />
        <uc7:MGMatchingGamePlay ID="MGMatchingGamePlay1" runat="server" Visible="false" />

</asp:Panel>

<asp:Panel ID="pnlWinner" runat="server" Visible="false">

<div class="row" style="min-height: 400px;">
	<div class="span1">
	</div>
	<div class="span10">

        <center>
        <h2>Congratulations, you completed this activity!</h2>
        <br /><br />
        <asp:Button ID="Button2" runat="server" Text="Continue" 
                CssClass="btn d" Width="200px" onclick="Button2_Click" Visible="true"/>
        </center>
        
	</div> 
	<div class="span1">
	</div>
</div> 

</asp:Panel>
<div class="row">
	<div class="span1">
	</div>
	<div class="span10">
        <asp:Label ID="Acknowledgements" runat="server" Text=""></asp:Label>

        <br />

                        <asp:Button ID="Button1" runat="server" Text="Done / Go Back To Game Setup" CssClass="" Width="300px" onclick="Button1_Click" Visible="true"/>
	</div> 


</div> 
</div>
</center>
</ContentTemplate>

</asp:UpdatePanel>

</div>