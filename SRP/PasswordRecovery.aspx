<%@ Page Title="Password Recovery"
    Language="C#"
    MasterPageFile="~/Layout/SRP.Master"
    AutoEventWireup="true"
    CodeBehind="PasswordRecovery.aspx.cs"
    Inherits="GRA.SRP.PasswordRecovery" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function ClientValidate(source, arguments) {
            arguments.IsValid = ($(".pwd").val() == $(".pwd2").val());
        }
    </script>

    <div class="row">
        <div class="col-sm-12">
            <span class="h1">
                <asp:Label runat="server" Text="change-password-title"></asp:Label></span>
        </div>
    </div>
    <div class="row margin-1em-top">
        <div class="col-xs-12 col-sm-8 col-sm-offset-2 margin-1em-bottom">
            <div class="alert alert-info">
                <span class="glyphicon glyphicon-info-sign"></span>
                <asp:Label runat="server" Text="change-password-rules"></asp:Label>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 col-sm-8 col-sm-offset-2">
            <asp:ValidationSummary runat="server"
                HeaderText='<span class="glyphicon glyphicon-exclamation-sign margin-halfem-right"></span>Please correct the following errors:' />
        </div>
    </div>
    <div class="form-horizontal margin-1em-top">
        <div class="form-group form-group-lg">
            <label class="col-sm-3 control-label">
                <asp:Label runat="server" Text="change-password-new"></asp:Label>
            </label>
            <div class="col-sm-6">
                <asp:TextBox ID="NPassword"
                    runat="server"
                    CssClass="form-control pwd"
                    TextMode="Password"></asp:TextBox>
            </div>
            <div class="col-sm-3 form-control-static">
                <asp:RequiredFieldValidator runat="server" Display="Dynamic"
                    ControlToValidate="NPassword" ErrorMessage="Password is required"
                    ToolTip="Password required" SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator
                    runat="server" ControlToValidate="NPassword" ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$"
                    Display="Dynamic"
                    ErrorMessage="Please select a password of at least seven characters with at least one number and at least one letter.">* password not secure</asp:RegularExpressionValidator>
            </div>
        </div>

        <div class="form-group form-group-lg">
            <label class="col-sm-3 control-label">
                <asp:Label runat="server" Text="change-password-verify"></asp:Label>
            </label>
            <div class="col-sm-6">
                <asp:TextBox ID="NPasswordR"
                    runat="server"
                    CssClass="form-control pwd2"
                    TextMode="Password"></asp:TextBox>
            </div>
            <div class="col-sm-3 form-control-static">
                <asp:RequiredFieldValidator runat="server" Display="Dynamic"
                    ControlToValidate="NPasswordR" ErrorMessage="Password validation is required"
                    ToolTip="Password Re-entry required" SetFocusOnError="True">* required</asp:RequiredFieldValidator>
                <asp:CustomValidator
                    runat="server" ControlToValidate="NPasswordR"
                    ErrorMessage="The password and validation do not match."
                    ClientValidationFunction="ClientValidate">* password does not match</asp:CustomValidator>
            </div>
        </div>

        <div class="form-group clearfix">
            <div class="col-sm-9">
                <div class="pull-right">
                    <asp:Button
                        ID="ChangePasswordButton"
                        data-loading-text="Changing password..."
                        type="button"
                        runat="server"
                        CssClass="btn btn-success change-password-button"
                        Text="change-password-submit"
                        CausesValidation="true"
                        OnClick="btnLogin_Click"
                        OnClientClick="return loginClick();" />
                </div>
            </div>
        </div>
    </div>

    <script>
        function loginClick() {
            $('.change-password-button').button('loading');
            return true;
        }
    </script>
</asp:Content>
