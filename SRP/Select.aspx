<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Layout/TSelection.Master" AutoEventWireup="true"
    CodeBehind="Select.aspx.cs" Inherits="GRA.SRP.Select" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <span class="lead">
                <asp:Label ID="txtMasterDesc" runat="server"></asp:Label></span>
        </div>
    </div>

    <div class="row" style="margin-top: 2em; margin-bottom: 2em;">
        <div class="col-xs-12 col-sm-8 col-sm-offset-1">
            <asp:Label Text="tenant-selection" runat="server"></asp:Label>
            <asp:DropDownList ID="ddTenants" runat="server" AutoPostBack="true" AppendDataBoundItems="true"
                OnSelectedIndexChanged="ddTenants_SelectedIndexChanged" CssClass="ovr">
                <asp:ListItem Value="" Text="[Select a Program]"></asp:ListItem>
            </asp:DropDownList>
            <asp:Button ID="btnSelProgram" runat="server" Text="Select" CssClass="btn btn-default btn-xs"
                OnClick="btnSelProgram_Click" />
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12">
            <asp:Label ID="txtTenDesc" runat="server"></asp:Label>
        </div>
    </div>
</asp:Content>
