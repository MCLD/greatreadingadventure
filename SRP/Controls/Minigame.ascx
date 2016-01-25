<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Minigame.ascx.cs" Inherits="GRA.SRP.Controls.Minigame" %>
<%@ Register Src="MGBook.ascx" TagName="MGBook" TagPrefix="uc1" %>
<%@ Register Src="MGMixMatchPlay.ascx" TagName="MGMixMatchPlay" TagPrefix="uc2" %>
<%@ Register Src="MGCodeBreaker.ascx" TagName="MGCodeBreaker" TagPrefix="uc3" %>
<%@ Register Src="MGWordMatch.ascx" TagName="MGWordMatch" TagPrefix="uc4" %>

<%@ Register Src="MGMatchingGamePlay.ascx" TagName="MGMatchingGamePlay" TagPrefix="uc7" %>
<%@ Register Src="MGHiddenPicPlay.ascx" TagName="MGHiddenPicPlay" TagPrefix="uc6" %>
<%@ Register Src="MGChooseAdvPlay.ascx" TagName="MGChooseAdvPlay" TagPrefix="uc5" %>

<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>

        <asp:Label ID="MGID" runat="server" Text="" Visible="false"></asp:Label>
        <asp:Label ID="GPSID" runat="server" Text="" Visible="false"></asp:Label>
        <asp:Label ID="PID" runat="server" Text="" Visible="false"></asp:Label>
        <div class="row">
            <div class="col-sm-12">
                <h2>
                    <asp:Label ID="MGName" runat="server" Text=""></asp:Label>
                </h2>
                <p>
                    <asp:Label ID="lbl_mg1" runat="server" Text="adventures-instructions-onlinebook" Visible="False"></asp:Label>
                    <asp:Label ID="lbl_mg2" runat="server" Text="adventures-instructions-mixandmatch" Visible="False"></asp:Label>
                    <asp:Label ID="lbl_mg3" runat="server" Text="adventures-instructions-codebreaker" Visible="False"></asp:Label>
                    <asp:Label ID="lbl_mg4" runat="server" Text="adventures-instructions-wordmatch" Visible="False"></asp:Label>
                    <asp:Label ID="lbl_mg5" runat="server" Text="adventures-instructions-matchinggame" Visible="False"></asp:Label>
                    <asp:Label ID="lbl_mg6" runat="server" Text="adventures-instructions-hiddenpicture" Visible="False"></asp:Label>
                    <asp:Label ID="lbl_mg7" runat="server" Text="adventures-instructions-cyoa" Visible="False"></asp:Label>
                </p>
            </div>
        </div>
        </div>
        <asp:Panel ID="pnlDifficulty" runat="server" Visible="true">
            <div class="row">
                <div class="col-sm-12">
                    <div class="margin-halfem-bottom">
                        <asp:Label ID="lblDiff" runat="server" Text="adventures-instructions-difficulty" Visible="true"></asp:Label>
                    </div>
                    <asp:Button ID="BtnEasy" runat="server" Text="adventures-instructions-button-easy"
                        CssClass="btn btn-default" Width="150px" OnClick="BtnEasy_Click" />
                    <asp:Button ID="BtnMedium" runat="server" Text="adventures-instructions-button-medium"
                        CssClass="btn btn-default" Width="150px" OnClick="BtnMedium_Click" />
                    <asp:Button ID="BtnHard" runat="server" Text="adventures-instructions-button-hard"
                        CssClass="btn btn-default" Width="150px" OnClick="BtnHard_Click" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel ID="pnlGame" runat="server" Visible="false">
            <uc1:MGBook ID="MGBook1" runat="server" Visible="false" />
            <uc2:MGMixMatchPlay ID="MGMixMatchPlay1" runat="server" Visible="false" />
            <uc3:MGCodeBreaker ID="MGCodeBreaker1" runat="server" Visible="false" />
            <uc4:MGWordMatch ID="MGWordMatch1" runat="server" Visible="false" />
            <uc5:MGChooseAdvPlay ID="MGChooseAdvPlay1" runat="server" Visible="false" />
            <uc6:MGHiddenPicPlay ID="MGHiddenPicPlay1" runat="server" Visible="false" />
            <uc7:MGMatchingGamePlay ID="MGMatchingGamePlay1" runat="server" Visible="false" />
            <div class="row">
                <div class="col-sm-12">
                    <asp:Button ID="CompleteButton"
                                runat="server"
                                Text="Done"
                                CssClass="btn btn-success btn-lg"
                                OnClick="CompleteButton_Click"
                                Visible="false" />
                </div>
            </div>
        </asp:Panel>

        <div class="row">
            <div class="col-sm-10 col-sm-offset-1">
                <asp:Label ID="Acknowledgements" runat="server" Text=""></asp:Label>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
