<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/Layout/SRP.Master"
    AutoEventWireup="true"
    CodeBehind="AddFamilyMemberExistingAccount.aspx.cs"
    Inherits="GRA.SRP.Account.AddFamilyMemberExistingAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-12 margin-1em-bottom">
            <span class="h1">
                <asp:Label runat="server" Text="family-member-add-title"></asp:Label></span>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12">
            <asp:Label runat="server" Text="family-member-add-existing-instructions"></asp:Label>
        </div>
    </div>

    <div class="row margin-1em-top">
        <div class="col-xs-12 col-sm-8 col-sm-offset-2 margin-1em-bottom">
            <asp:Panel CssClass="alert alert-info" runat="server" id="alertContainer" visible="false">
                <span class="glyphicon glyphicon-info-sign" runat="server" id="alertGlyphicon"></span>
                <asp:Label runat="server" ID="alertText"></asp:Label>
            </asp:Panel>
        </div>
    </div>

    <div class="form-horizontal">
        <div class="form-group">
            <label class="col-sm-3 control-label">
                Username:
            </label>
            <div class="col-sm-6">
                <asp:TextBox ID="Username" runat="server" CssClass="form-control gra-register-username" data-asterisk="UsernameReq" Enabled="true" MaxLength="25"></asp:TextBox>
            </div>
        </div>
        <div class="form-group">
            <label class="col-sm-3 control-label">Password:</label>
            <div class="col-sm-6">
                <asp:TextBox ID="Password" runat="server" CssClass="pwd form-control" data-asterisk="PasswordReq" TextMode="Password"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="col-xs-9 clearfix">
                <div class="pull-right">
                    <asp:HyperLink runat="server" CssClass="btn btn-default" NavigateUrl="~/Account/"><asp:Label runat="server" Text="family-member-add-cancel"></asp:Label></asp:HyperLink>
                    <asp:LinkButton runat="server"
                        CausesValidation="true"
                        OnClick="Save_Click"
                        CssClass="btn btn-success account-save-button">
                        <span class="glyphicon glyphicon-save margin-halfem-right"></span>
                        <%=this.SaveButtonText %>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomOfPage" runat="server">
</asp:Content>
