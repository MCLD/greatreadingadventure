<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GameboardControl.ascx.cs" Inherits="STG.SRP.Controls.GameboardControl" %>

<%@ Register src="MyPointsControl.ascx" tagname="MyPointsControl" tagprefix="uc1" %>
<%@ Register src="LeaderBoardControl.ascx" tagname="LeaderBoardControl" tagprefix="uc2" %>

<div class="row">
	<div class="span9">
        <div style="border: 5px solid silver;" id="Border">
        <table width="100%" cellspacing="0" cellpadding="0" border="0" id="GameBoard" style="position: relative; z-index: 0; ">
            <tr>
                <td>
                    <img src="/GameMap.aspx" id="BoardImg" style="width:100%;" alt="Game Board"/>
                </td>
            </tr>
        </table>
        </div>
    </div>
        <div class="span3">
        
         
        <uc1:MyPointsControl ID="MyPointsControl1" runat="server" />
        <br />
        
        <uc2:LeaderBoardControl ID="LeaderBoardControl1" runat="server" />

    <div class="secondary-nav" style="height: 230px;">
        <ul class="nav">
            <li><a href="/MyLogEntry.aspx" 
                    > 
                     <img src="/images/Games/Board/add.png" id="Enter" style="width:100px; float:left; padding-right: 5px;padding-top: 5px;padding-bottom: 5px;" alt="Reading Log"/>
                     <!--Log your activity -->Play Now:
                     <br /><small><!--Enter your reading or event attendance to earn points, badges and prizes or play bonus games.-->
                                    Log my reading, enter a secret code, and play mini-games</small></a></li>
        </ul>
    </div>

        
        </div>
	</div>



<hr />


