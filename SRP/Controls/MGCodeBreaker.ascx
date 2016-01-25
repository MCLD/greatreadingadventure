<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGCodeBreaker.ascx.cs" Inherits="GRA.SRP.Controls.MGCodeBreaker" %>

<asp:Label ID="CBID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>
<asp:Label ID="Correct" runat="server" Text="0" Visible="false" CssStyle="Correct"></asp:Label>

<div class="row">
    <div class="col-sm-4 col-sm-push-8">
        <h3>
            <asp:Label
                runat="server"
                Text="adventures-codebreaker-key"></asp:Label></h3>
        <div class="row">
            <asp:Label ID="lblKey" runat="server"></asp:Label>
        </div>
    </div>
    <asp:Panel CssClass="col-sm-8 col-sm-pull-4 form-group form-group-lg"
        runat="server"
        ID="formGroup">
        <h3>
            <asp:Label
                runat="server"
                Text="adventures-codebreaker-message"></asp:Label></h3>
        <asp:Label ID="lblEncoded" runat="server" CssClass="animated fadeIn"></asp:Label>
        <asp:TextBox ID="txtAnswer"
            runat="server"
            Visible="True"
            CssClass="form-control margin-1em-bottom margin-1em-top"
            Width="100%"></asp:TextBox>
        <asp:Button ID="btnScore"
            runat="server"
            Text="adventures-codebreaker-verify"
            OnClick="btnScore_Click"
            CssClass="btn btn-success form-control" />
    </asp:Panel>
</div>
