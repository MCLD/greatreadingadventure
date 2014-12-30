<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginRecovery.aspx.cs" Inherits="STG.SRP.ControlRoom.LoginRecovery" 
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title><%= STG.SRP.ControlRoom.SRPResources.CRTitle%></title>
    <style type="text/css">
        .style1
        {
            height: 26px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div>
        <div style="padding:150px; padding-top:150px; vertical-align:middle; min-width: 550px;" >
            <div style="padding:10px;vertical-align:middle; min-width: 500px;" class="bkColor1">
                <div style="padding:50px;vertical-align:middle; text-align:center; min-width: 400px;" class="bkColor2">
                    <center>
                    <fieldset style=" border-style: solid; padding-left: 10px; padding-right: 10px; width:380px; " class="borderColor1">
                    <legend class="Message" style="font-size: 10pt; font-weight: bold; padding-left:5px; padding-right:5px;">
                        Password Recovery</legend>
                    <div style="float: left; width: 100%; ">
                    <br />
                    
                    <table align="center">
                    <tr>
                    <td align="left" colspan="2">
                    
    <asp:RequiredFieldValidator runat="server" ID="uxv1" 
                            CssClass="MessageFailure" Display="Dynamic" ControlToValidate="uxUsername" 
            ErrorMessage="User Name is a required field.&lt;br&gt;"></asp:RequiredFieldValidator>
                        <asp:Label ID="lblMessage" runat="server" Font-Bold="True" Font-Size="Small" 
                            ForeColor="#006600"></asp:Label>
                            <br /><br />
                    </td>
                    </tr>
                    <tr>
                    <td align="right" class="style1">User Name:&nbsp; </td>
                    <td align="left" class="style1">
                        <asp:TextBox ID="uxUsername" runat="server" width="200px"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                    <td ></td><td align="center" >
                    <br />
                        <asp:Button ID="Button1" runat="server" Text="Email My Password" CssClass="btn-lg"
                            onclick="Button1_Click" />
                        </td>
                    </tr>
                    </table>
                    
                    
                    
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
    </div>
    </form>
</body>
</html>
