<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="TenantUserPasswordChange.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Tenant.TenantUserPasswordChange" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row">
            <div class="col-xs-12">
                <p class="lead">
                    <asp:Label runat="server" ID="UserName"></asp:Label>
                </p>
                <p>Password must be at least 7 characters long, and contain one alpha and one numeric character</p>
                <p id="PasswordMismatch" style="display: none;" class="lead text-danger">Password and password verification must match.</p>
                <asp:Label Visible="false" runat="server" ID="ErrorMessage" CssClass="lead text-danger">Could not change password for user.</asp:Label>
                <asp:HiddenField runat="server" ID="CameFrom" />
                <asp:HiddenField runat="server" ID="SRPUserName" />
                <asp:HiddenField runat="server" ID="SRPUserId" />
            </div>
        </div>
        <div class="row">
            <div class="col-xs-6">
                <div class="form-group">
                    <asp:TextBox runat="server" ID="NewPassword" TextMode="Password" CssClass="form-control" placeholder="New password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                        ControlToValidate="NewPassword" Display="Dynamic" ErrorMessage="New password is required"
                        SetFocusOnError="False" Font-Bold="True"> * Required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revStrength" runat="server"
                        ControlToValidate="NewPassword" Display="Dynamic" ErrorMessage="Password must be at least 7 characters long, and contain one alpha and one numeric character."
                        ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$" SetFocusOnError="False" Font-Bold="True"> * Strength
                    </asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-6">
                <div class="form-group">
                    <asp:TextBox runat="server" ID="NewPasswordVerify" TextMode="Password" CssClass="form-control" placeholder="New password verification"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                        ControlToValidate="NewPasswordVerify" Display="Dynamic" ErrorMessage="New password is required"
                        SetFocusOnError="False" Font-Bold="True"> * Required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                        ControlToValidate="NewPasswordVerify" Display="Dynamic" ErrorMessage="Password must be at least 7 characters long, and contain one alpha and one numeric character."
                        ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$" SetFocusOnError="False" Font-Bold="True"> * Strength
                    </asp:RegularExpressionValidator>

                </div>
            </div>
            <div class="col-xs-12">
                <div class="form-group">
                    <asp:Button runat="server"
                        Text="Change Password"
                        CssClass="btn btn-default"
                        ID="SubmitButton"
                        OnClientClick="return ValidateMatch();" 
                        OnClick="ChangePassword"/>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        function ValidateMatch() {
            $('#PasswordMismatch').hide();
            var newPassword = '#<%=NewPassword.ClientID%>';
            var newPasswordVerify = '#<%=NewPasswordVerify.ClientID%>';
            if ($(newPassword).val() != $(newPasswordVerify).val()) {
                $('#PasswordMismatch').show();
                return false;
            }
            return true;
        };
    </script>
</asp:Content>
