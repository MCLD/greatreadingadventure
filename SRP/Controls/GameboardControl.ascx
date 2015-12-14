<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GameboardControl.ascx.cs" Inherits="GRA.SRP.Controls.GameboardControl" %>

<%@ Register src="MyPointsControl.ascx" tagname="MyPointsControl" tagprefix="uc1" %>
<%@ Register src="LeaderBoardControl.ascx" tagname="LeaderBoardControl" tagprefix="uc2" %>
<%@ Register src="MyGameLoggingNavControl.ascx" tagname="MyGameLoggingNavControl" tagprefix="uc3" %>

<div class="row">
	<div class="span9">
        <div style="border: 5px solid silver;" id="Border">
        <table width="100%" cellspacing="0" cellpadding="0" border="0" id="GameBoard" style="position: relative; z-index: 0; ">
            <tr>
                <td>
                    <asp:Image runat="server" ImageUrl="~/Adventures/Map.aspx" ID="BoardImg" Width="100%" AlternateText="Game Board" />
                </td>
            </tr>
        </table>
        </div>
    </div>
        <div class="span3">
        
         
        <uc1:MyPointsControl ID="MyPointsControl1" runat="server" />
        <br />
        
        <uc2:LeaderBoardControl ID="LeaderBoardControl1" runat="server" />

        <uc3:MyGameLoggingNavControl ID="MyGameLoggingNavControl1" runat="server" />


        
        </div>
	</div>



<hr />


