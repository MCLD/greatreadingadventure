<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronLogin.ascx.cs" Inherits="GRA.SRP.Classes.PatronLogin" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<div class="panel panel-default">
    <div class="panel-heading">
        <h3 class="panel-title">
            <asp:Label ID="Label1" runat="server" Text="LoginForm Title"></asp:Label></h3>
    </div>
    <div class="panel-body form-horizontal">
        <asp:Label ID="lblError" runat="server" Text="" Font-Bold="True" ForeColor="#CC0000"></asp:Label>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="List"
            ShowMessageBox="True" ShowSummary="True" ValidationGroup="uxLogin" ForeColor="#CC0000" Font-Bold="True" />

        <div class="form-group">
            <label class="col-sm-3 control-label">
                <asp:Label ID="Label6" runat="server" Text="LoginForm username"></asp:Label></label>
            <div class="col-sm-9">
                <asp:TextBox ID="PUserName" runat="server" CssClass="form-control"></asp:TextBox>
                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server"
                    ControlToValidate="PUserName" ErrorMessage="Username is required"
                    ToolTip="Username required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="form-group">
            <label class="col-sm-3 control-label">
                <asp:Label ID="Label7" runat="server" Text="LoginForm password"></asp:Label></label>
            <div class="col-sm-9">
                <asp:TextBox ID="PPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server"
                    ControlToValidate="PPassword" ErrorMessage="Password is required"
                    ToolTip="Password required" ValidationGroup="uxLogin" Display="None" EnableClientScript="false"></asp:RequiredFieldValidator>
            </div>

        </div>

    </div>
    <div class="panel-footer clearfix">
        <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-default pull-right left-spacer"
            Text="LoginForm button" CausesValidation="true"
            ValidationGroup="uxLogin" OnClick="btnLogin_Click" />

        <a href="/Register.aspx" class="btn btn-default pull-right left-spacer">
            <asp:Label ID="Label3" runat="server" Text="LoginForm register" /></a>

        <a href="/Recover.aspx" class="btn btn-default pull-right">
            <asp:Label ID="Label5" runat="server" Text="LoginForm recover" /></a>
    </div>
</div>

