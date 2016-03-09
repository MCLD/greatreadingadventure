<%@ Page Title="" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="AddFamilyMember.aspx.cs" Inherits="GRA.SRP.Account.AddFamilyMember" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-xs-10 col-xs-offset-1">
            <p class="lead">
                <asp:Label runat="server" text="family-member-add-description"></asp:Label>
            </p>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-8 col-xs-offset-2 col-sm-3 col-sm-offset-2 margin-1em-top">
            <a href="AddFamilyMemberAccount.aspx" class="btn btn-default btn-lg btn-block" style="white-space: normal;">
                <span class="glyphicon glyphicon-list-alt"
                    style="font-size: x-large; display: block; margin: 1em;"></span>
                <asp:Label runat="server" text="family-member-add-register-new"></asp:Label>
            </a>
        </div>
        <div class="col-xs-8 col-xs-offset-2 col-sm-3 col-sm-offset-2 margin-1em-top margin-1em-bottom">
            <a href="AddFamilyMemberExistingAccount.aspx" class="btn btn-default btn-lg btn-block" style="white-space: normal;">
                <span class="glyphicon glyphicon-plus-sign"
                    style="font-size: x-large; display: block; margin: 1em;"></span>
                <asp:Label runat="server" text="family-member-add-has-account"></asp:Label>
            </a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="BottomOfPage" runat="server">
</asp:Content>
