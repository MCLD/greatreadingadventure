<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="ProgramByBranch.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.ProgramByBranch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="row">
            <div class="col-xs-6 col-xs-offset-3">
                <asp:Panel runat="server" ID="AlertPanel" Visible="false" role="alert" CssClass="alert alert-danger">
                    <span class="glyphicon glyphicon-alert"></span>
                    <asp:Label runat="server" ID="AlertMessage"></asp:Label>
                </asp:Panel>
            </div>
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
