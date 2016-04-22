<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid" style="width: 1024px;">
        <div class="row hidden-print">
            <div class="col-xs-4">
                <asp:DropDownList ID="ProgramList"
                    runat="server"
                    DataSourceID="ProgramListData"
                    DataTextField="AdminName"
                    DataValueField="PID"
                    AppendDataBoundItems="True"
                    CssClass="form-control"
                    AutoPostBack="true">
                    <asp:ListItem Value="0" Text="All programs"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-xs-4">
                <asp:DropDownList ID="LibraryDistrictList"
                    runat="server"
                    DataSourceID="LibraryDistrictData"
                    DataTextField="Code"
                    DataValueField="CID"
                    AppendDataBoundItems="True"
                    CssClass="form-control"
                    AutoPostBack="true"
                    OnDataBound="DropDownDataBound"
                    OnSelectedIndexChanged="SelectedDistrict">
                    <asp:ListItem Value="0" Text="All library districts"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-xs-4">
                <asp:DropDownList ID="LibraryBranchList"
                    runat="server"
                    DataSourceID="LibraryBranchData"
                    DataTextField="Code"
                    DataValueField="CID"
                    AppendDataBoundItems="True"
                    CssClass="form-control"
                    AutoPostBack="true"
                    OnDataBound="DropDownDataBound">
                    <asp:ListItem Value="0" Text="All library branches"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

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
            </div>
        </div>

        <div class="row h3 visible-print-block" style="margin-top: 2em; margin-bottom: 2em;">
            <div class="col-xs-4 text-left">
                <asp:Label runat="server" ID="ProgramName"></asp:Label>
            </div>
            <div class="col-xs-4 text-center">
                <asp:Label runat="server" ID="DistrictName"></asp:Label>
            </div>
            <div class="col-xs-4 text-right">
                <asp:Label runat="server" ID="BranchName"></asp:Label>
            </div>
            <br />
            <br />
        </div>

        <div class="row" style="margin-top: 2em;">
            <div class="col-xs-3">
                <div class="ataglance ataglance-participants">
                    <span class="glyphicon glyphicon-user"></span>
                    <br />
                    Patrons:<div class="visible-print-block"><br /></div>
                    <asp:Label runat="server" ID="RegisteredPatrons" CssClass="ataglance-stat"></asp:Label>
                </div>
            </div>
            <div class="col-xs-3">
                <div class="ataglance ataglance-points">
                    <span class="glyphicon glyphicon-dashboard"></span>
                    <br />
                    Points:<div class="visible-print-block"><br /></div>
                    <asp:Label runat="server" ID="PointsEarned" CssClass="ataglance-stat"></asp:Label>
                </div>
            </div>
            <div class="col-xs-3">
                <div class="ataglance ataglance-points-books">
                    <span class="glyphicon glyphicon-book"></span>
                    <br />
                    Reading points:<div class="visible-print-block"><br /></div>
                    <asp:Label runat="server" ID="PointsEarnedReading" CssClass="ataglance-stat"></asp:Label>
                </div>
            </div>
            <div class="col-xs-3">
                <div class="ataglance ataglance-challenges">
                    <span class="glyphicon glyphicon-star"></span>
                    <br />
                    Challenges:<div class="visible-print-block"><br /></div>
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
                    Adventures:<div class="visible-print-block"><br /></div>
                    <asp:Label runat="server" ID="AdventuresCompleted" CssClass="ataglance-stat"></asp:Label>
                </div>
            </div>
            <div class="col-xs-3">
                <div class="ataglance ataglance-badges">
                    <span class="glyphicon glyphicon-certificate"></span>
                    <br />
                    Badges:<div class="visible-print-block"><br /></div>
                    <asp:Label runat="server" ID="BadgesAwarded" CssClass="ataglance-stat"></asp:Label>
                </div>
            </div>
            <div class="col-xs-3">
                <div class="ataglance ataglance-codes">
                    <span class="glyphicon glyphicon-barcode"></span>
                    <br />
                    Secret Codes:<div class="visible-print-block"><br /></div>
                    <asp:Label runat="server" ID="SecretCodesRedeemed" CssClass="ataglance-stat"></asp:Label>
                </div>
            </div>
            <div class="col-xs-3" runat="server" id="ProgramCodesDiv">
                <div class="ataglance ataglance-program-rewards">
                    <span class="glyphicon glyphicon-qrcode"></span>
                    <br />
                    <asp:Literal runat="server" ID="ProgramRewardCodeLabel"></asp:Literal>:<div class="visible-print-block"><br /></div>
                        <asp:Label runat="server" ID="ProgramCodesRedeemed" CssClass="ataglance-stat"></asp:Label>
                </div>
            </div>
        </div>
    </div>
    <asp:ObjectDataSource ID="ProgramListData" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Programs"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="LibraryDistrictData" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="LibraryBranchData" runat="server"
        SelectMethod="GetFilteredBranchDDValues"
        TypeName="GRA.SRP.DAL.LibraryCrosswalk">
        <SelectParameters>
            <asp:ControlParameter
                ControlID="LibraryDistrictList"
                DefaultValue="0"
                Name="districtId"
                PropertyName="Text"
                Type="Int32" />
            <asp:Parameter Name="city" DefaultValue="" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
