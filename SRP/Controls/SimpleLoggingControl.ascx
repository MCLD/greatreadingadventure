<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimpleLoggingControl.ascx.cs" Inherits="GRA.SRP.Controls.SimpleLoggingControl" %>
<asp:Panel runat="server" ID="simpleLoggingControlPanel">
    <div class="row">
        <div class="col-xs-12 text-center">
            <span class="lead">
                <asp:Label runat="server" Text="readinglog-reading-log"></asp:Label></span>
            <hr style="margin-bottom: 5px !important; margin-top: 5px !important;" />
        </div>
    </div>
    <div class="row">
        <div class="form-inline text-center">
            <div class="col-xs-12">
                <div class="form-group">
                    <label for="txtCountSubmitted">
                        <asp:Label runat="server" Text="readinglog-reading-prompt"></asp:Label></label>
                    <asp:TextBox ID="txtCountSubmitted"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="5"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="radio-inline">
                        <asp:RadioButtonList ID="rbActivityType" runat="server">
                        </asp:RadioButtonList></label>
                </div>
            </div>
        </div>
    </div>

    <div class="row margin-1em-top">
        <div class="col-xs-12 col-md-6">
            <div class="form-group">
                <label for="txtTitle">Title:</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>

        <div class="col-xs-12 col-md-6">
            <div class="form-group">
                <label for="txtAuthor">Author:</label>
                <asp:TextBox ID="txtAuthor" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlReview" runat="server" Visible="false">
        <div class="row">
            <div class="col-xs-12">
                Write a Review:
             <asp:TextBox ID="Review" runat="server" Width="98%" Rows="4" TextMode="MultiLine"></asp:TextBox>
            </div>
        </div>
    </asp:Panel>

    <div class="row">
        <div class="col-xs-12 text-center">
            <strong>OR</strong>
        </div>
    </div>


    <div class="row margin-1em-top">
        <div class="col-xs-12">
            <div class="form-inline text-center">
                <label for="txtProgramCode">
                    <asp:Label ID="lblRedeem" runat="server" Text="secret-code-prompt"></asp:Label></label>
                <asp:TextBox ID="txtProgramCode" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-xs-12">
            <div class="pull-right">
                <asp:Button ID="btnSubmit" runat="server" Text="readinglog-submit-minutes" CssClass="btn btn-default" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
    <asp:Label ID="lblPID" runat="server" Visible="false"></asp:Label>
    <asp:Label ID="lblPGID" runat="server" Visible="false"></asp:Label>
</asp:Panel>
