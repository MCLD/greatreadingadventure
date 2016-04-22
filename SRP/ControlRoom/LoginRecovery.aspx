<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginRecovery.aspx.cs" Inherits="GRA.SRP.ControlRoom.LoginRecovery" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta name="ROBOTS" content="NOINDEX, NOFOLLOW">
    <title><%= GRA.SRP.ControlRoom.SRPResources.CRTitle%> - Password Reset Request</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" runat="server" />
    <asp:PlaceHolder runat="server">
        <script src="<%=ResolveUrl("~/Scripts/jquery-2.2.3.min.js")%>"></script>
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
            width: 500px;
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
                <h1 style="margin-bottom: 0.2em;" class="gra-red">Password Reset Request</h1>
                <em style="font-size: larger;"><strong>Enter your email address to begin the 
                    password recovery process.</strong></em>
            </div>

            <div style="margin-top: 0.5em; margin-left: 1em; margin-right: 1em;">
                <asp:RequiredFieldValidator runat="server" ID="uxv1"
                    CssClass="MessageFailure" Display="Dynamic" ControlToValidate="uxEmailaddress"
                    ErrorMessage="Email address is a required field."></asp:RequiredFieldValidator>
                <asp:Label ID="lblMessage" runat="server" Font-Bold="True" Font-Size="Small"
                    ForeColor="#006600"></asp:Label>
            </div>

            <div>
                <table style="width: 100%;">
                    <tr>
                        <td style="padding-top: 0.5em; padding-left: 6em;">
                            <asp:TextBox ID="uxEmailaddress" runat="server" Width="300px" CssClass="gra-cr-email form-control"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="logintextcenter" style="padding-top: 3em; padding-bottom: 2em;">
                            <asp:Button ID="ChangePassword" runat="server" OnClick="Button1_Click" Text='Request Password Reset'
                                CssClass="loginbutton btn-lg" CausesValidation="true"></asp:Button>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <script>
            $().ready(function () {
                $('.gra-cr-email').focus();
            });
        </script>
    </form>
</body>
</html>
