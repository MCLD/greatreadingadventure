<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PasswordReset.aspx.cs" Inherits="GRA.SRP.ControlRoom.PasswordReset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%= GRA.SRP.ControlRoom.SRPResources.CRTitle%> :: Password Reset</title>
    <script language="javascript">
        function ClientValidate(source, arguments) {
            if (document.getElementById("uxConfirmNewPasswordField").value == document.getElementById("uxNewPasswordField").value) {
                arguments.IsValid = true;
            }
            else {
                arguments.IsValid = false;
            }
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="padding: 150px; padding-top: 150px; vertical-align: middle; min-width: 550px;">
                <div style="padding: 10px; vertical-align: middle; min-width: 500px;" class="bkColor1">
                    <div style="padding: 50px; vertical-align: middle; text-align: center; min-width: 400px;" class="bkColor2">
                        <center>
                            <fieldset style="border-style: solid; padding-left: 10px; padding-right: 10px; width: 380px;" class="borderColor1">
                                <legend class="Message" style="font-size: 10pt; font-weight: bold; padding-left: 5px; padding-right: 5px;">You Must Reset Your Password</legend>
                                <div style="float: left;">
                                    <br />

                                    <table align="center">
                                        <tr>
                                            <td align="left" colspan="2">

                                                <asp:RequiredFieldValidator runat="server" ID="uxNewPasswordRequiredValidator"
                                                    CssClass="MessageFailure" Display="Dynamic" ControlToValidate="uxNewPasswordField"
                                                    ErrorMessage="Password is a required field.&lt;br&gt;"></asp:RequiredFieldValidator>

                                                <asp:RegularExpressionValidator ID="uxNewPasswordStrengthValidator" CssClass="MessageFailure"
                                                    runat="server" ControlToValidate="uxNewPasswordField" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                                                    Display="Dynamic"
                                                    ErrorMessage="New Password must be at least seven characters in length and contain one alpha and one numeric character.&lt;br&gt;"></asp:RegularExpressionValidator>

                                                <asp:RequiredFieldValidator runat="server" ID="uxConfirmNewPasswordRequiredValidator"
                                                    CssClass="MessageFailure" ControlToValidate="uxConfirmNewPasswordField" ErrorMessage="Confirm New Password is a required field.&lt;br&gt;"
                                                    Display="Dynamic"></asp:RequiredFieldValidator>

                                                <asp:CustomValidator ID="uxConfirmNewPasswordCustomValidator" CssClass="MessageFailure"
                                                    runat="server" ControlToValidate="uxConfirmNewPasswordField"
                                                    ErrorMessage="The New Password and Confirmation of New Password do not match.&lt;br&gt;"
                                                    ClientValidationFunction="ClientValidate"></asp:CustomValidator>

                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">New Password: </td>
                                            <td>
                                                <asp:TextBox ID="uxNewPasswordField" runat="server"
                                                    TextMode="Password" Width="175px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="right">Re-Enter Password: </td>
                                            <td style="margin-left: 80px">
                                                <asp:TextBox ID="uxConfirmNewPasswordField" runat="server"
                                                    TextMode="Password" Width="175px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">&nbsp;</td>
                                            <td>
                                                <asp:Button ID="Button1" runat="server" Text="Change Password" CssClass="btn-lg"
                                                    OnClick="Button1_Click" />
                                            </td>
                                        </tr>
                                    </table>



                                    <div style="text-align: center;">
                                        <br />
                                        &nbsp;
                                    </div>
                            </fieldset>
                            <br />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>

