<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Security.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        function flipHelp(id) {
            var tr = document.getElementById("help_" + id)
            if (tr != null) {
                if (tr.style.visibility == "hidden") tr.style.visibility= string.Empty
                else tr.style.visibility = "hidden"
                if (tr.style.display == "none") tr.style.display= string.Empty
                else tr.style.display = "none"
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />
    <asp:Label ID="Error" runat="server" Color="red"></asp:Label>

    <table style="" width="600px" border="0">
        <tr>
            <td style="width: 150px;" valign="top">
                <b>Changing password for user:</b>
            </td>
            <td valign="top" style="width: 350px;">
                <asp:Label ID="Username" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 150px;" valign="top">
                <b>New Password:</b>
            </td>
            <td valign="top">
                <asp:TextBox ID="uxPassword" runat="server" TextMode="Password" Width="350px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                    ControlToValidate="uxPassword" Display="Dynamic" ErrorMessage="New password is required"
                    SetFocusOnError="False" Font-Bold="True"> * Required</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revStrength" runat="server"
                    ControlToValidate="uxPassword" Display="Dynamic" ErrorMessage="Password must be at least 7 characters long, and contain one alpha and one numeric character."
                    ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$" SetFocusOnError="False" Font-Bold="True"> * Strength
                </asp:RegularExpressionValidator>

                <div id="help_1" style="display: none; visibility: hidden;">
                    <b>The password is a required field.
                    Password must be at least 7 characters long, and contain one alpha and one numeric character.
                    </b>
                </div>
            </td>
            <td valign="top" align="left">&nbsp;
                <img alt="Help Info" src="/ControlRoom/Images/info.png" width="20px" onclick="flipHelp(1)" />
                &nbsp;
            </td>
        </tr>
        <tr>
            <td valign="top">
                <b>Re-Enter Password:</b>
            </td>
            <td valign="top">

                <asp:TextBox ID="uxReEnter" runat="server" TextMode="Password" Width="350px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvReenter" runat="server"
                    ControlToValidate="uxReEnter" Display="Dynamic" ErrorMessage="Password re-enter is required"
                    SetFocusOnError="False" Font-Bold="True"> * Required</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvMatch" runat="server"
                    ControlToCompare="uxReEnter" ControlToValidate="uxPassword" Display="Dynamic"
                    ErrorMessage="The password re-entry does not match." SetFocusOnError="False" Font-Bold="True"> * No match</asp:CompareValidator>

                <div id="help_2" style="display: none; visibility: hidden;">
                    <b>For verification purposes, re-enter your new password.  It should match the password entered above.
                    </b>

                </div>

            </td>
            <td valign="top" align="left">&nbsp;
                <img alt="Help Info" src="/ControlRoom/Images/info.png" width="20px" onclick="flipHelp(2)" />
                &nbsp;
            </td>
        </tr>

        <tr>
            <td valign="top">&nbsp;</td>
            <td valign="top" align="center">

                <asp:Button ID="ResetPassword" runat="server" onclick="ResetPassword_Click" 
                    Text="Reset Password" CssClass="btn-lg btn-green" />

                &nbsp;
            
                <asp:Button ID="Cancel" runat="server" CausesValidation="False"
                     onclick="Cancel_Click"  Text="Cancel" CssClass="btn-lg btn-blue" />

            </td>
            <td valign="top">&nbsp;</td>
        </tr>
    </table>
</asp:Content>
