<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecoverPassword.ascx.cs" Inherits="GRA.SRP.Classes.RecoverPassword" %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<div class="row">
    <div class="col-sm-12 margin-1em-bottom">
        <span class="h1">
            <asp:Label runat="server" Text="password-recovery-title"></asp:Label></span>
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
                    ToolTip="Email address is required" ValidationGroup="uxRecover" Display="Dynamic" EnableClientScript="false">* required</asp:RequiredFieldValidator>
            </div>
            <asp:Button runat="server" CssClass="btn btn-primary"
                Text="password-recovery-submit" CausesValidation="true"
                OnClick="btnEmail_Click" />
        </div>
    </div>
</div>
