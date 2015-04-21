<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="STG.SRP.ControlRoom.Login" 
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">

    <title><%= STG.SRP.ControlRoom.SRPResources.CRTitle%></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding:150px; padding-top:150px; vertical-align:middle; min-width: 550px;" >
            <div style="padding:10px;vertical-align:middle; min-width: 500px;" class="bkColor1">
                <div style="padding:50px;vertical-align:middle; text-align:center; min-width: 400px;" class="bkColor2">
                    <center>
                    <fieldset style=" border-style: solid; padding-left: 10px; padding-right: 10px; width:380px; " class="borderColor1">
                        <legend class="Message" style="font-size: 10pt; font-weight: bold; padding-left:5px; padding-right:5px;"><%= STG.SRP.ControlRoom.SRPResources.LoginGroup%></legend>
                    <div style="float: left;">
                    <br />
                            <div runat="server" id="uxMessageBox" class="MessageInstruction" visible="false" style="text-align: left; ">
                                <asp:Panel Visible="true" id="divTest" runat="server">
                                    <asp:Label  ForeColor="red" ID="FailureText" runat="server" EnableViewState="False"></asp:Label>
                                </asp:Panel>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                                        ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" CssClass="MessageFailure" />
                            </div>
                    <br />
                    
                    
                    <asp:Login ID="uxLogin" runat="server"  OnAuthenticate="OnAuthenticate"      
                        TitleText="Login"                 
                        DisplayRememberMe="false"                   
                        UserNameLabelText="User name"    
                        PasswordLabelText="Password"  
                        TextBoxStyle-Width="200"  
                        LabelStyle-VerticalAlign="Top"  
                        LoginButtonText="Login"  
                        FailureText="Login Failed."
			            TextLayout="TextOnTop" Width="400px" 
                    >             
               
                    <TextBoxStyle Width="200px"></TextBoxStyle>

                       <LayoutTemplate>
                           <table border="0" cellpadding="1" cellspacing="0" 
                               style="border-collapse:collapse;">
                               <tr>
                                   <td>
                                       <table border="0" cellpadding="0">
                                           <tr>
                                               <td>



                                               </td>
                                               <td rowspan="5" valign="middle">
                                                   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;<img ID="Img19" runat="server" alt="" 
                                                       src="~/ControlRoom/Images/Login_Manager.png" style="border-style: none" /></td>
                                           </tr>
                                           <tr>
                                               <td align=left>
                                                   <asp:Label  ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Font-Bold=true><%= STG.SRP.ControlRoom.SRPResources.Username%>: </asp:Label><br />
                                                   <asp:TextBox ID="UserName" runat="server" Width="200px"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                                       ControlToValidate="UserName" ErrorMessage="User Name is required" 
                                                       ToolTip="Username required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                                       <br />
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align=left>
                                                   <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Font-Bold=true><%= STG.SRP.ControlRoom.SRPResources.Password %>: </asp:Label><br />
                                                   <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px"></asp:TextBox>
                                                   <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                                       ControlToValidate="Password" ErrorMessage="Password is required"
                                                       ToolTip="Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align="center" style="color:Red;">
                                                   
                                               </td>
                                           </tr>
                                           <tr>
                                               <td align="center">
                                                   <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text='Login' 
                                                       ValidationGroup="uxLogin" CssClass="btn-lg" CausesValidation="true" ></asp:Button>
                                               </td>
                                           </tr>
                                       </table>
                                   </td>
                               </tr>
                           </table>
                       </LayoutTemplate>

                    <LabelStyle HorizontalAlign="Right"></LabelStyle>
                    </asp:Login>
 
                    
                    <br />
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/ControlRoom/LoginRecovery.aspx"><%= STG.SRP.ControlRoom.SRPResources.UserAccountRecovery%></asp:HyperLink>
                    </div>
                    <div style="text-align: center;">
                    <br />
                            &nbsp;</div>
                    </fieldset>
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
