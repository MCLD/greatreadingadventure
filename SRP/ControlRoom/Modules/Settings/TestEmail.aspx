<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="TestEmail.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.TestEmail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container" style="max-height: 600px; overflow-y: auto;">
        <div class="row">
            <div class="col-xs-6 col-xs-offset-3">
                <asp:Panel runat="server" ID="AlertPanel" Visible="false" role="alert">
                    <span runat="server" id="AlertGlyphicon"></span>
                    <asp:Label runat="server" ID="AlertMessage"></asp:Label>
                </asp:Panel>
            </div>
        </div>

        <div class="col-xs-12 form-inline">
            <div class="form-group">
                <label for="<%=EmailTo.ClientID %>">Email to address:</label>
                <asp:TextBox runat="server" ID="EmailTo" CssClass="form-control" Width="200"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="<%=EmailSubject.ClientID %>">Email subject:</label>
                <asp:TextBox runat="server" ID="EmailSubject" CssClass="form-control" Width="350"></asp:TextBox>
            </div>

            <asp:LinkButton runat="server" OnClick="SendTestEmail_Click" CssClass="btn btn-success" ForeColor="White"><span class="glyphicon glyphicon-send"></span>
                    Send test message
            </asp:LinkButton>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
