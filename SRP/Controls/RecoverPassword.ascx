<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecoverPassword.ascx.cs" Inherits="GRA.SRP.Classes.RecoverPassword" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<div class="row">
    <div class="col-sm-12 margin-1em-bottom">
        <span class="h1">
            <asp:Label runat="server" Text="password-recovery-title"></asp:Label></span>
    </div>
</div>

<div class="row">
    <div class="col-sm-12 margin-1em-bottom">
        <p>You can recover a forgotten password if your email address is configured for your account. Enter your username here and you'll receive an email with password reset instructions.</p>
        </p>
    </div>
</div>


<div class="row">
    <div class="col-sm-12">
        <div class="form-inline">
            <div class="form-group">
                <label>
                    <asp:Label runat="server" Text="password-recovery-username"></asp:Label></label>
                <asp:TextBox ID="PUsername" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server"
                    ControlToValidate="PUsername" ErrorMessage="Email address is required"
                    ToolTip="Email address is required" Display="Dynamic" EnableClientScript="false"><span class="text-danger glyphicon glyphicon-asterisk glyphicon-sm"></span> required</asp:RequiredFieldValidator>
            </div>
            <asp:Button runat="server" CssClass="btn btn-primary margin-halfem-left"
                Text="password-recovery-submit" CausesValidation="true"
                OnClick="btnEmail_Click" />
        </div>
    </div>
</div>
