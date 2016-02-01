<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setup.aspx.cs" Inherits="GRA.SRP.ControlRoom.DBCreate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Great Reading Adventure - Initial Setup</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 10px; vertical-align: middle; margin-left: auto; margin-right: auto;">
            <div style="padding: 50px; background-color: #f0f3f9; vertical-align: middle; text-align: center; border: 10px solid #4e596f;">
                <fieldset style="border-color: #FF6600; border-style: solid; padding-left: 10px; padding-right: 10px; width: 420px; margin-left: auto; margin-right: auto;">
                    <legend class="Message" style="font-size: 10pt; font-weight: bold; padding-left: 5px; padding-right: 5px; margin-left: auto; margin-right: auto;">Great Reading Adventure - Initial Setup
                    </legend>
                    <div>
                        <br />
                        <div runat="server" id="uxMessageBox" class="MessageInstruction" visible="true" style="text-align: left;">
                            <asp:Panel Visible="true" ID="divTest" runat="server">
                                <asp:Label ForeColor="red" ID="FailureText" runat="server" EnableViewState="False"></asp:Label>
                            </asp:Panel>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" CssClass="MessageFailure" />
                        </div>
                        <br />


                        <table border="0" cellpadding="1" cellspacing="0"
                            style="border-collapse: collapse;">
                            <tr>
                                <td>
                                    <table border="0" cellpadding="0">
                                        <tr>
                                            <td></td>
                                            <td rowspan="3" valign="top" align="center">
                                                <img
                                                    id="Img19" runat="server" alt=""
                                                    src="/ControlRoom/RibbonImages/SystemSettings.png" style="border-style: none" /></td>
                                        </tr>
                                        <tr>
                                            <td align="left">Database server name or IP address:<br />
                                                <asp:TextBox ID="DBServer" runat="server" Width="200px" Text="(LocalDB)"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                    ControlToValidate="DBServer" ErrorMessage="Server Name/IP is required"
                                                    ToolTip="Server Name/IP required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                <br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">Database name:<br />
                                                <asp:TextBox ID="DBName" runat="server" Width="200px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="DBNameval" runat="server"
                                                    ControlToValidate="DBName" ErrorMessage="Application Database Name is required"
                                                    ToolTip="Application Database Name required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">SA Username/Login:<br />
                                                <asp:TextBox ID="UserName" runat="server" Width="200px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                    ControlToValidate="UserName" ErrorMessage="SA Username/Login is required"
                                                    ToolTip="SA Username/Login required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                <br />
                                            </td>
                                            <td align="left">SA Password:
                                                            <br />
                                                <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
                                                    ControlToValidate="Password" ErrorMessage="Password is required"
                                                    ToolTip="Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                            </td>

                                        </tr>
                                        <tr>
                                            <td align="left">Runtime Username/Login:<br />
                                                <asp:TextBox ID="RunUser" runat="server" Width="200px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                                    ControlToValidate="RunUser" ErrorMessage="Runtime Username/Login is required"
                                                    ToolTip="Runtime Username/Login required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                <br />
                                            </td>
                                            <td align="left">Runtime Password:
                                                            <br />
                                                <asp:TextBox ID="RuntimePassword" runat="server" TextMode="Password" Width="200px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                                    ControlToValidate="RuntimePassword" ErrorMessage="Runtime Password is required"
                                                    ToolTip="Runtime Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td align="left">Mail server:
                                                            <br />
                                                <asp:TextBox ID="Mailserver" runat="server" Width="200px" Text="(localhost)"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                                    ControlToValidate="Mailserver" ErrorMessage="Mailserver is required"
                                                    ToolTip="Mailserver required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                            </td>
                                            <td align="left">Your email address:
                                                            <br />
                                                <asp:TextBox ID="Mailaddress" runat="server" Width="200px" Text=""></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                                    ControlToValidate="Mailaddress" ErrorMessage="Your email address is required"
                                                    ToolTip="Your email address is required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"></asp:RequiredFieldValidator>
                                            </td>
                                            <td align="left"></td>
                                        </tr>
                                        <tr>
                                            <td align="center" style="color: Red;">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <asp:Button ID="InstallBtn" runat="server" Text='Install' Width="100px"
                                                    ValidationGroup="uxLogin" CssClass="buttonLeft btn-lg"
                                                    CausesValidation="true" OnClick="InstallBtn_Click"></asp:Button>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <br />
                    </div>
                    <div style="text-align: center;">
                        <br />
                        &nbsp;
                    </div>
                </fieldset>
                <br />
                <div style="width: 900px; text-align: left;">
                    <asp:Label ForeColor="red" ID="errorLabel" runat="server" EnableViewState="False"></asp:Label>
                </div>
                <br />
            </div>
        </div>
    </form>
</body>
</html>
