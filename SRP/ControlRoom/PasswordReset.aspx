<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PasswordReset.aspx.cs" Inherits="GRA.SRP.ControlRoom.PasswordReset" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta name="ROBOTS" content="NOINDEX, NOFOLLOW">
    <title><%= GRA.SRP.ControlRoom.SRPResources.CRTitle%> - Change Password</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" runat="server" />
    <asp:PlaceHolder runat="server">
        <script src="<%=ResolveUrl("~/Scripts/jquery-2.2.0.min.js")%>"></script>
    </asp:PlaceHolder>
    <style>
        .gra-red {
            color: #94483D;
        }

        .logintextcenter {
            text-align: center;
        }

        .loginbox {
            margin: 10em auto 0 auto;
            width: 400px;
            border: 10px solid #555;
            background: white;
            display: block;
            padding: 1em;
        }

        .loginbutton {
            border-style: none;
            color: white;
            font-size: 1.5em !important;
            padding: 0.4em 0.9em !important;
            background-color: #678FC2;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="loginbox">
            <div class="logintextcenter" style="margin-bottom: 1em;">
                <h1 style="margin-bottom: 0.2em;" class="gra-red">Change Password</h1>
                <em style="font-size: larger;"><strong>You must select a new password.</strong></em>
            </div>

            <div style="margin-top: 0.5em; margin-left: 1em; margin-right: 1em;">
                <asp:RequiredFieldValidator runat="server" ID="uxNewPasswordRequiredValidator"
                    CssClass="MessageFailure" Display="Dynamic" ControlToValidate="Password"
                    ErrorMessage="&lt;p&gt;You must enter a new password.&lt;/p&gt;"></asp:RequiredFieldValidator>

                <asp:RegularExpressionValidator ID="uxNewPasswordStrengthValidator" CssClass="MessageFailure"
                    runat="server" ControlToValidate="Password" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                    Display="Dynamic"
                    ErrorMessage="&lt;p&gt;Your new password must be at least seven characters in length and contain one alpha and one numeric character.&lt;/p&gt;"></asp:RegularExpressionValidator>

                <asp:RequiredFieldValidator runat="server" ID="uxConfirmNewPasswordRequiredValidator"
                    CssClass="MessageFailure" ControlToValidate="PasswordVerify" ErrorMessage="&lt;p&gt;You must enter a new password verification.&lt;/p&gt;"
                    Display="Dynamic"></asp:RequiredFieldValidator>

                <asp:CustomValidator ID="uxConfirmNewPasswordCustomValidator" CssClass="MessageFailure"
                    runat="server" ControlToValidate="PasswordVerify"
                    ErrorMessage="&lt;p&gt;Your new password and new password verification must match.&lt;/p&gt;"
                    ClientValidationFunction="ClientValidate"></asp:CustomValidator>
            </div>

            <div>
                <table style="width: 100%;">
                    <tr>
                        <td style="padding-top: 0.5em; padding-left: 6em;">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Font-Bold="true"><%= GRA.SRP.ControlRoom.SRPResources.Password %>: </asp:Label><br />
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px" CssClass="gra-cr-password form-control"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-top: 0.5em; padding-left: 6em;">
                            <asp:Label ID="PasswordVerifyLabel" runat="server" AssociatedControlID="PasswordVerify" Font-Bold="true"><%= GRA.SRP.ControlRoom.SRPResources.Password %> verification: </asp:Label><br />
                            <asp:TextBox ID="PasswordVerify" runat="server" TextMode="Password" Width="200px" CssClass="form-control"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="logintextcenter" style="padding-top: 3em; padding-bottom: 2em;">
                            <asp:Button ID="ChangePassword" runat="server" OnClick="Button1_Click" Text='Change Password'
                                CssClass="loginbutton btn-lg" CausesValidation="true"></asp:Button>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <script>
            $().ready(function () {
                $('.gra-cr-password').focus();
            });

            function ClientValidate(source, arguments) {
                arguments.IsValid = $('#<%=Password.ClientID%>').val() == $('#<%=PasswordVerify.ClientID%>').val();
            }
        </script>
    </form>
</body>
</html>
