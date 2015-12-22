<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CRRibbon.ascx.cs" Inherits="GRA.SRP.ControlRoom.CRRibbon" %>
<asp:Panel ID="pnlRibbon" runat="server">
    <div id="cdribbon" class="cdSubwebBgColor" style="width: 100%">
        <table id="tblMain" cellpadding="0" cellspacing="0" width="100%" border="0" runat="server">
            <tr valign="top">
                <td width="2" class="cdribtopl">&nbsp;</td>
                <td width="100*" class="cdribtopc">
                    <div></div>
                </td>
                <td width="2" class="cdribtopr"></td>
            </tr>
            <tr valign="middle" height="79">
                <td width="1" class="cdribmidl">
                    <div></div>
                </td>
                <td width="100%" class="cdribmidc">



                    <asp:Repeater ID="rptPnl" runat="server">
                        <ItemTemplate>

                            <div class="cntRibbonborder" style="float: left; padding-right: 2px;">
                                <table cellpadding="0" cellspacing="0" width="150" border="0">
                                    <tr>
                                        <td width="2" class="cdchutopl"></td>
                                        <td class="cdchutopc">
                                            <div></div>
                                        </td>
                                        <td width="2" class="cdchutopr"></td>
                                    </tr>
                                    <tr valign="middle" height="74">
                                        <td width="1" class="cdchumidl">
                                            <div></div>
                                        </td>
                                        <td height="74px" class="cdchumidc" onmouseover="this.className='cdchumidcover'" onmouseout="this.className='cdchumidc'">
                                            <table cellpadding="0" cellspacing="0" border="0" height="100%">
                                                <tr>
                                                    <td width="35" style="padding: 0px 5px">
                                                        <img border="0" src='<%# Eval("ImagePath")%>' width="64" height="64"
                                                            <%# string.IsNullOrEmpty(Eval("ImagePath2x") as string) ? string.Empty : "srcset=\"" + Eval("ImagePath") + " 1x, " + Eval("ImagePath2x") + " 2x\"" %>
                                                            alt='<%# Eval("ImageAlt")%>' title='<%# Eval("ImageAlt")%>'>
                                                    </td>
                                                    <td width="100%" class="cntribboncolor" valign="middle" nowrap>
                                                        <%# GetLinks((System.Collections.Generic.List<GRA.SRP.Core.Utilities.RibbonLink>)Eval("Links"))%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td height="10" align="center" valign="bottom" class="cntRibboncolor" colspan="2">
                                                        <%# Eval("Name")%>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td width="1" class="cdchumidr">
                                            <div></div>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td width="2" class="cdchubotl"></td>
                                        <td class="cdchubotc">
                                            <div></div>
                                        </td>
                                        <td width="2" class="cdchubotr"></td>
                                    </tr>
                                </table>
                            </div>

                        </ItemTemplate>
                    </asp:Repeater>



                </td>
                <td width="2" class="cdribmidr">
                    <div></div>
                </td>
            </tr>
            <tr valign="top">
                <td width="2" class="cdribbotl">&nbsp;</td>
                <td width="100%" class="cdribbotc"></td>
                <td width="2" class="cdribbotr"></td>
            </tr>
        </table>
    </div>
</asp:Panel>

