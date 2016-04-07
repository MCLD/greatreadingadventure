<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs"
    Inherits="GRA.SRP.ControlRoom._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid" style="width: 1024px;">
        <asp:Panel runat="server" ID="StatusPanel">
            <div class="row h1 text-center visible-print-block">
                <div class="col-xs-12">
                    <br />
                    <asp:Label runat="server" ID="OrganizationName" CssClass="ataglance-orgname"></asp:Label>
                    <br />
                    <br />
                </div>
            </div>

            <div class="row visible-print-block">
                <div class="col-xs-12">
                    <asp:Image runat="server" ID="ProgramImage" CssClass="img img-responsive" />
                    <br />
                    <br />
                </div>
            </div>

            <div class="row" style="margin-top: 2em;">
                <div class="col-xs-3">
                    <div class="ataglance ataglance-participants">
                        <span class="glyphicon glyphicon-user"></span>
                        <br />
                        Patrons:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="RegisteredPatrons" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
                <div class="col-xs-3">
                    <div class="ataglance ataglance-points">
                        <span class="glyphicon glyphicon-dashboard"></span>
                        <br />
                        Points:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="PointsEarned" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
                <div class="col-xs-3">
                    <div class="ataglance ataglance-points-books">
                        <span class="glyphicon glyphicon-book"></span>
                        <br />
                        Reading points:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="PointsEarnedReading" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
                <div class="col-xs-3">
                    <div class="ataglance ataglance-challenges">
                        <span class="glyphicon glyphicon-star"></span>
                        <br />
                        Challenges:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="ChallengesCompleted" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="row visible-print-block">
                <div class="col-xs-12">
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                </div>
            </div>
            <div class="row">
                <div class="col-xs-3">
                    <div class="ataglance ataglance-adventures">
                        <span class="glyphicon glyphicon-picture"></span>
                        <br />
                        Adventures:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="AdventuresCompleted" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
                <div class="col-xs-3">
                    <div class="ataglance ataglance-badges">
                        <span class="glyphicon glyphicon-certificate"></span>
                        <br />
                        Badges:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="BadgesAwarded" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
                <div class="col-xs-3">
                    <div class="ataglance ataglance-codes">
                        <span class="glyphicon glyphicon-barcode"></span>
                        <br />
                        Secret Codes:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="SecretCodesRedeemed" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
                <div class="col-xs-3" runat="server" id="ProgramCodesDiv">
                    <div class="ataglance ataglance-program-rewards">
                        <span class="glyphicon glyphicon-qrcode"></span>
                        <br />
                        <asp:Literal runat="server" ID="ProgramRewardCodeLabel"></asp:Literal>:<div class="visible-print-block">
                            <br />
                        </div>
                        <asp:Label runat="server" ID="ProgramCodesRedeemed" CssClass="ataglance-stat"></asp:Label>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
