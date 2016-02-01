<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronLogin.ascx.cs" Inherits="GRA.SRP.Classes.PatronLogin" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<asp:Panel runat="server" DefaultButton="btnLogin" CssClass="panel panel-default">
    <div class="panel-heading">
        <span class="modal-title lead">
            <asp:Label ID="loginTitle" runat="server" Text="loginform-title"></asp:Label></span>
    </div>
    <div class="panel-body form-horizontal">
        <p class="text-danger margin-1em-bottom" style="font-weight: bold; display: none;" runat="server" clientidmode="Static" id="loginErrorMessage"></p>
        <div class="form-group form-group-lg has-feedback" id="loginLoginGroup">
            <label class="col-sm-3 control-label">
                <asp:Label runat="server" Text="loginform-username"></asp:Label></label>
            <div class="col-sm-9">
                <asp:TextBox ClientIDMode="Static" ID="loginUsername" runat="server" CssClass="form-control"></asp:TextBox>
                <span id="loginUsernameErrorGlyph" class="glyphicon glyphicon-remove form-control-feedback" aria-hidden="true" style="display: none;"></span>
            </div>
        </div>
        <div class="form-group form-group-lg  has-feeback" id="loginPasswordGroup">
            <label class="col-sm-3 control-label">
                <asp:Label runat="server" Text="loginform-password"></asp:Label></label>
            <div class="col-sm-9">
                <asp:TextBox ClientIDMode="Static" ID="loginPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                <span id="loginPasswordErrorGlyph" class="glyphicon glyphicon-remove form-control-feedback" aria-hidden="true" style="display: none; right: 15px;"></span>
            </div>
        </div>
    </div>
    <div class="panel-footer clearfix">
        <div class="row">
            <div class="col-xs-12 col-sm-5">
                <div class="pull-left login-remember-page margin-halfem-top">
                    <asp:CheckBox runat="server"
                        ID="loginRememberMe" Text="Remember my username" />
                </div>
            </div>
            <div class="col-xs-12 col-sm-7">
                <div class="pull-right margin-halfem-top">
                    <a href="~/Recover.aspx" runat="server" class="btn btn-default margin-halfem-bottom" clientidmode="Static"
                        id="recoverButton">
                        <asp:Label runat="server" Text="loginform-recover" /></a>
                    <a href="~/RegisterILS.aspx" runat="server" class="btn btn-info margin-halfem-bottom" clientidmode="Static"
                        id="registerButton">
                        <asp:Label runat="server" Text="loginform-register" /></a>
                    <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-success margin-halfem-bottom" ClientIDMode="Static"
                        Text="loginform-submit-button" OnClientClick="return validateLogin();"
                        OnClick="loginClick" data-loading-text="Verifying..." />
                </div>

            </div>
        </div>
    </div>
</asp:Panel>
<script>
    function validateLogin() {
        var valid = true;

        var username = $("#loginUsername").val();
        if (!username || username.trim().length == 0) {
            $("#loginUsernameErrorGlyph").show();
            if (!$("#loginLoginGroup").hasClass("has-error")) {
                $("#loginLoginGroup").toggleClass("has-error");
            }
            valid = false;
        } else {
            $("#loginUsernameErrorGlyph").hide();
            if ($("#loginLoginGroup").hasClass("has-error")) {
                $("#loginLoginGroup").toggleClass("has-error");
            }
        }

        var password = $("#loginPassword").val();
        if (!password || password.trim().length == 0) {
            $("#loginPasswordErrorGlyph").show();
            if (!$("#loginPasswordGroup").hasClass("has-error")) {
                $("#loginPasswordGroup").toggleClass("has-error");
            }
            valid = false;
        } else {
            $("#loginPasswordErrorGlyph").hide();
            if ($("#loginPasswordGroup").hasClass("has-error")) {
                $("#loginPasswordGroup").toggleClass("has-error");
            }
        }
        if (valid) {
            $("#btnLogin").button("loading");
            $("#loginErrorMessage").hide();
            $("#loginUsername").prop("readonly", true);
            $("#loginPassword").prop("readonly", true);
            $("#recoverButton").toggleClass("disabled");
            $("#registerButton").toggleClass("disabled");

        } else {
            $('#loginErrorMessage').text("Please enter a username and password.");
            $("#loginErrorMessage").show();
        }
        return valid;
    }
</script>
