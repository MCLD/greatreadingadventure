<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CRRibbon.ascx.cs" Inherits="STG.SRP.ControlRoom.CRRibbon" %>
<asp:Panel ID="pnlRibbon" runat="server">
<div id="cdribbon">
<div id="cdribboninner" >

		
		
		
            <asp:Repeater ID="rptPnl" runat="server">
            <ItemTemplate>
            
            <div class="cntRibbonborder" style="float:left;">
            <div class="cntRibbonborderinner">
				<table cellpadding="0" cellspacing="0" width="150" border="0">
					<tr valign="middle" height="74">
						<td height="74px" class="cdchumidc" onmouseover="this.className='cdchumidcover'" onmouseout="this.className='cdchumidc'">
							<table cellpadding="0" cellspacing="0" border="0" height=100%>
								<tr>
									<td width="35" style="padding: 0px 5px"><img border="0" src='<%# Eval("ImagePath")%>' width="50" height="50"
										alt='<%# Eval("ImageAlt")%>' title='<%# Eval("ImageAlt")%>'>
									</td>
									<td width="100%" class="cntribboncolor" valign="middle" nowrap>
									
									<%# GetLinks((System.Collections.Generic.List<STG.SRP.Core.Utilities.RibbonLink>)Eval("Links"))%>
									
									</td>
								</tr>
								<tr>
									<td height="10" align="center" valign="bottom" class="cntRibboncolor" colspan="2">
										<%# Eval("Name")%>
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
            </div>
			</div>
            
            </ItemTemplate>
            </asp:Repeater>
		
</div>		
</div>				
</asp:Panel>
				
