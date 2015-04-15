<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Install.aspx.cs" Inherits="STG.SRP.Install" validateRequest="false"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Database Install</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
    <tr>
        <td nowrap><b>SQL Connection String: </b><br />DBO access db user</td>
        <td>
            <asp:TextBox ID="txtDBO" runat="server" TextMode="SingleLine" Width="1200px"></asp:TextBox><br />
            Data Source=(local);Initial Catalog=SRP;User ID=dbouser;Password=dbopass
        </td>
    </tr>
    <tr>
        <td nowrap><b>Runtime SQL Connection String: </b><br />Normal db User, read/write access</td>
        <td>
            <asp:TextBox ID="txtConn" runat="server" TextMode="SingleLine" Width="1200px"></asp:TextBox><br />
            Data Source=(local);Initial Catalog=SRP;User ID=srpuser;Password=srppass
        </td>
    </tr>
    <tr>
        <td nowrap><b>Email host name: </b></td>
        <td>
            <asp:TextBox ID="txtEmailHost" runat="server" TextMode="SingleLine" Width="1200px" Text="(localhost)"></asp:TextBox>        
        </td>
    </tr>
    <tr>
        <td><b>: </b></td>
        <td>
        
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Run Scripts" />
        </td>
    </tr>    
    </table>
        
    </div>


    <center>
        <div style="padding:10px;vertical-align:middle; width: 1050px;">
            <div style="padding:10px;background-color:#4e596f;vertical-align:middle;width: 1050px;">
                <div style="padding:50px;background-color:#f0f3f9;vertical-align:middle; text-align:center; width: 950px;">
                    <center>
                    <fieldset style="border-color: #FF6600; border-style: solid; padding-left: 10px; padding-right: 10px; width:380px; ">
                        <legend class="Message" style="font-size: 10pt; font-weight: bold; padding-left:5px; padding-right:5px;">
                            Database Setup
                        </legend>
                    <div style="float: left;">
                    <br />
                    
                    
                                            
                                                <div runat="server" id="uxMessageBox" class="MessageInstruction" visible="true" style="text-align: left; ">
                                                    <asp:Panel Visible="true" id="divTest" runat="server">
                                                        <asp:Label  ForeColor="red" ID="FailureText" runat="server" EnableViewState="False"></asp:Label>
                                                    </asp:Panel>
                                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                                            ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" CssClass="MessageFailure" />
                                                </div>
                    <br />
                    
                    
                    <table border="0" cellpadding="1" cellspacing="0" 
                               style="border-collapse:collapse;">
                               <tr>
                                   <td>
                                       <table border="0" cellpadding="0">
                                           <tr>
                                               <td>



                                               </td>
                                               <td rowspan="3" valign="top">
                                                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<img 
                                                       ID="Img19" runat="server" alt="" 
                                                       src="/ControlRoom/Images/Install3.png" style="border-style: none" /></td>
                                           </tr>
                                           <tr>
                                               <td align="left">
                                                   Application Database Server/IP:<br />
                                                   <asp:TextBox ID="DBServer" runat="server" Width="200px" Text="(local)"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                                                       ControlToValidate="DBServer" ErrorMessage="Server Name/IP is required" 
                                                       ToolTip="Server Name/IP required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                       <br />
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align="left">
                                                   Application Database Name:<br />
                                                   <asp:TextBox ID="DBName" runat="server" Width="200px"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="DBNameval" runat="server" 
                                                       ControlToValidate="DBName" ErrorMessage="Application Database Name is required" 
                                                       ToolTip="Application Database Name required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                       <br />
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align="left">
                                                   SA Username/Login:<br />
                                                   <asp:TextBox ID="UserName" runat="server" Width="200px"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                                       ControlToValidate="UserName" ErrorMessage="SA Username/Login is required" 
                                                       ToolTip="SA Username/Login required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                       <br />
                                               </td>
                                               <td align="left">
                                                   SA Password: <br />
                                                   <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                                       ControlToValidate="Password" ErrorMessage="Password is required"
                                                       ToolTip="Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                               </td>

                                           </tr>
                                           <tr>
                                               <td align="left">
                                                   Runtime Username/Login:<br />
                                                   <asp:TextBox ID="RunUser" runat="server" Width="200px"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                                                       ControlToValidate="RunUser" ErrorMessage="Runtime Username/Login is required" 
                                                       ToolTip="Runtime Username/Login required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                       <br />
                                               </td>
                                               <td align="left">
                                                   Runtime Password: <br />
                                                   <asp:TextBox ID="RuntimePassword" runat="server" TextMode="Password" Width="200px"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                                                       ControlToValidate="RuntimePassword" ErrorMessage="Runtime Password is required"
                                                       ToolTip="Runtime Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align="left">
                                                   Mailserver: <br />
                                                   <asp:TextBox ID="Mailserver" runat="server"  Width="200px" Text="(localhost)"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" 
                                                       ControlToValidate="Mailserver" ErrorMessage="Mailserver is required"
                                                       ToolTip="Mailserver required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                               </td>
                                               <td align="left">
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align="center" style="color:Red;">
                                                   
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align="left">
                                                   <asp:Button ID="InstallBtn" runat="server" Text='Install' Width="100px"
                                                       ValidationGroup="uxLogin" CssClass="buttonLeft btn-lg" 
                                                       CausesValidation="true" onclick="InstallBtn_Click" ></asp:Button>
                                               </td>
                                           </tr>
                                       </table>
                                   </td>
                               </tr>
                           </table>
                    

                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    
                    <br />
                    </div>
                    <div style="text-align: center;">
                    <br />&nbsp;</div>
                    </fieldset>
                    <br />
                    <div style="width: 900px; text-align:left;">
                    <asp:Label  ForeColor="red" ID="errorLabel" runat="server" EnableViewState="False"></asp:Label>
                    </div>
                    <br />
                    <div style="text-align: center ">
                                <%= STG.SRP.ControlRoom.SRPResources.ProductName%> (v  <%= STG.SRP.ControlRoom.SRPResources.ProductVersion%>) - Copyright (c)  <%= STG.SRP.ControlRoom.SRPResources.CopyrightYear%>  <%= STG.SRP.ControlRoom.SRPResources.CopyrightEntity%> <br /> All Rights Reserved
                    </div>
                </div>
            </div>
        </div>






    </form>
</body>
</html>
