<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Minigame.ascx.cs" Inherits="STG.SRP.Controls.Minigame" %>
<%@ Register src="MGBook.ascx" tagname="MGBook" tagprefix="uc1" %>
<%@ Register src="MGMixMatchPlay.ascx" tagname="MGMixMatchPlay" tagprefix="uc2" %>
<%@ Register src="MGCodeBreaker.ascx" tagname="MGCodeBreaker" tagprefix="uc3" %>
<%@ Register src="MGWordMatch.ascx" tagname="MGWordMatch" tagprefix="uc4" %>

<%@ Register src="MGMatchingGamePlay.ascx" tagname="MGMatchingGamePlay" tagprefix="uc7" %>
<%@ Register src="MGHiddenPicPlay.ascx" tagname="MGHiddenPicPlay" tagprefix="uc6" %>
<%@ Register src="MGChooseAdvPlay.ascx" tagname="MGChooseAdvPlay" tagprefix="uc5" %>

<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>



<asp:Label ID="MGID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="GPSID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="PID" runat="server" Text="" Visible="false"></asp:Label>
<div class="row">
	<div class="span2">
	</div>
    <div class="span8">
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
	<div class="span2">
	</div>   
</div> 
<asp:Panel ID="pnlDifficulty" runat="server" Visible="true">
<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
    <div class="span8">

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
	<div class="span2">
	</div>
</div> 

</asp:Panel>

<asp:Panel ID="pnlGame" runat="server" Visible="false">

<!--
<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
	<div class="span8">
-->
        <uc1:MGBook ID="MGBook1" runat="server" Visible="false"/>
        <uc2:MGMixMatchPlay ID="MGMixMatchPlay1" runat="server" Visible="false"/>
        <uc3:MGCodeBreaker ID="MGCodeBreaker1" runat="server" Visible="false"/>
        <uc4:MGWordMatch ID="MGWordMatch1" runat="server" Visible="false" />

        <uc5:MGChooseAdvPlay ID="MGChooseAdvPlay1" runat="server" Visible="false" />
        <uc6:MGHiddenPicPlay ID="MGHiddenPicPlay1" runat="server" Visible="false" />
        <uc7:MGMatchingGamePlay ID="MGMatchingGamePlay1" runat="server" Visible="false" />

        
                <asp:Button ID="Button1" runat="server" Text="Done" 
                CssClass="" Width="200px" onclick="Button1_Click" Visible="false"/>
        
<!--
	</div> 
	<div class="span2">
	</div>
</div> 
-->

</asp:Panel>

<asp:Panel ID="pnlWinner" runat="server" Visible="false">

<div class="row" style="min-height: 400px;">
	<div class="span2">
	</div>
	<div class="span8">

        <center>
        <h2>Congratulations, you completed this activity!</h2>
        <br /><br />
        <asp:Button ID="Button2" runat="server" Text="Continue" 
                CssClass="btn d" Width="200px" onclick="Button2_Click" Visible="true"/>
        </center>
        
	</div> 
	<div class="span2">
	</div>
</div> 

</asp:Panel>
<div class="row">
	<div class="span2">
	</div>
	<div class="span8">
        <asp:Label ID="Acknowledgements" runat="server" Text=""></asp:Label>
	</div> 
	<div class="span2">
	</div>
</div> 
</ContentTemplate>

</asp:UpdatePanel>