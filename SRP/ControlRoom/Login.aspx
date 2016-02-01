<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="GRA.SRP.ControlRoom.Login" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta name="ROBOTS" content="NOINDEX, NOFOLLOW">
    <title><%= GRA.SRP.ControlRoom.SRPResources.CRTitle%></title>
    <link href="~/Content/animate.min.css" rel="stylesheet" runat="server" />
    <asp:PlaceHolder runat="server">
        <script src="<%=ResolveUrl("~/Scripts/jquery-2.2.0.min.js")%>"></script>
    </asp:PlaceHolder>
    <style>
        .gra-red {
            color:  #94483D;
        }

        .logintextcenter {
            text-align: center;
        }

        .loginbox {
            margin: 10em auto 0 auto;
            width: 460px;
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
                <h1 style="margin-bottom: 0.2em;" class="gra-red">Control Room Login</h1>
                <em><strong><asp:Label runat="server" Id="SystemName"></asp:Label></strong></em>
            </div>

            <div style="margin: auto;">
                <div runat="server" id="uxMessageBox" class="MessageInstruction" visible="false" style="text-align: center;">
                    <asp:Panel Visible="true" ID="divTest" runat="server">
                        <asp:Label ForeColor="red" ID="FailureText" runat="server" EnableViewState="False" CssClass="MessageFailure"></asp:Label>
                    </asp:Panel>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
                        ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" CssClass="MessageFailure" />
                </div>
            </div>

            <div style="padding-left: 2em;">
                <asp:Login ID="uxLogin" runat="server" OnAuthenticate="OnAuthenticate"
                    TitleText="Login"
                    DisplayRememberMe="false"
                    UserNameLabelText="User name"
                    PasswordLabelText="Password"
                    TextBoxStyle-Width="200"
                    LabelStyle-VerticalAlign="Top"
                    LoginButtonText="Login"
                    FailureText="Login Failed."
                    TextLayout="TextOnTop" Width="400px">

                    <TextBoxStyle Width="200px"></TextBoxStyle>

                    <LayoutTemplate>
                        <table style="width: 100%;">
                            <tr>
                                <td>&nbsp;</td>
                                <td rowspan="5" style="vertical-align: top; text-align: right;">
                                    <img class="ControlRoomLogo animated bounceIn" runat="server" alt="" style="border-style: none;"
                                        src="~/images/gra150.png" srcset="../images/gra150.png 1x, ../images/gra300.png 2x" onclick="swingIt();" />
                                </td>
                            </tr>
                            <tr>
                                <td >
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Font-Bold="true"><%= GRA.SRP.ControlRoom.SRPResources.Username%>: </asp:Label><br />
                                    <asp:TextBox ID="UserName" runat="server" Width="200px" CssClass="gra-cr-username"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server"
                                        ControlToValidate="UserName" ErrorMessage="User Name is required"
                                        ToolTip="Username required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-top: 0.5em;">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Font-Bold="true"><%= GRA.SRP.ControlRoom.SRPResources.Password %>: </asp:Label><br />
                                    <asp:TextBox ID="Password" runat="server" TextMode="Password" Width="200px" CssClass="gra-cr-password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
                                        ControlToValidate="Password" ErrorMessage="Password is required"
                                        ToolTip="Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"> </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="logintextcenter" style="padding-top: 1em;">
                                    <asp:CheckBox ID="RememberMe" runat="server" Text="Remember username"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="logintextcenter" style="padding-top: 1em;">
                                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text='Login'
                                        ValidationGroup="uxLogin" CssClass="loginbutton btn-lg" CausesValidation="true" OnClientClick="doZoom(); return true;"></asp:Button>
                                </td>
                            </tr>
                        </table>
                    </LayoutTemplate>

                    <LabelStyle HorizontalAlign="Right"></LabelStyle>
                </asp:Login>
            </div>
            <div class="logintextcenter" style="margin-top: 2em;">
                <asp:HyperLink runat="server" NavigateUrl="~/ControlRoom/LoginRecovery.aspx"><%= GRA.SRP.ControlRoom.SRPResources.UserAccountRecovery%></asp:HyperLink>
            </div>
        </div>
        <script>
            $('.ControlRoomLogo').on('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function (eventObject) {
                $(this).removeClass('animated');
                if ($(this).hasClass('bounceIn')) {
                    $(this).removeClass('bounceIn');
                }
                if ($(this).hasClass('flip')) {
                    $(this).removeClass('flip');
                }
                if ($(this).hasClass('jello')) {
                    $(this).removeClass('jello');
                }
            });
            function swingIt() {
                $('.ControlRoomLogo').addClass('animated jello');
            }
            function doZoom() {
                if ($('.ControlRoomLogo')) {
                    $('.ControlRoomLogo').removeClass('animated bounceIn')
                    $('.ControlRoomLogo').addClass('animated flip');
                }
                return true;
            }
            $().ready(function () {
                if ($('.gra-cr-username').val().length > 0) {
                    $('.gra-cr-password').focus();
                }
            });
        </script>
    </form>
</body>
</html>
