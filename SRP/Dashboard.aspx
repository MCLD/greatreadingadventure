<%@ Page Title="My Program" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="GRA.SRP.MyProgram" %>

<%@ Register Src="~/Controls/Welcome.ascx" TagPrefix="uc1" TagName="Welcome" %>
<%@ Register Src="~/Controls/Avatar.ascx" TagPrefix="uc1" TagName="Avatar" %>
<%@ Register Src="~/Controls/MyBadgesListControl.ascx" TagPrefix="uc1" TagName="MyBadgesListControl" %>
<%@ Register Src="~/Controls/MyPointsControl.ascx" TagPrefix="uc1" TagName="MyPointsControl" %>
<%@ Register Src="~/Controls/LeaderBoardControl.ascx" TagPrefix="uc1" TagName="LeaderBoardControl" %>
<%@ Register Src="~/Controls/SimpleLoggingControl.ascx" TagName="SimpleLoggingControl" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ReadingLogControl.ascx" TagPrefix="rlc" TagName="ReadingLogControl" %>
<%@ Register Src="~/Controls/CodeControl.ascx" TagPrefix="cc" TagName="CodeControl" %>



<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-sm-3">
            <div class="row">
                <div class="col-xs-12 text-center">
                    <uc1:Avatar runat="server" ID="Avatar" />
                </div>
                <div class="col-xs-12 text-center">
                    <uc1:MyPointsControl runat="server" ID="MyPointsControl" />
                </div>
                <div class="col-xs-12">
                    <uc1:LeaderBoardControl runat="server" ID="LeaderBoardControl" />
                </div>
            </div>
        </div>

        <div class="col-sm-6">
            <div class="text-center">
                <uc1:Welcome runat="server" ID="Welcome" />
            </div>
            <asp:Label ID="ProgramNotOpenText" runat="server" Text="" Visible="false"></asp:Label>

            <rlc:ReadingLogControl runat="server" ID="ReadingLogControl" />

            <div class="margin-1em-top">
                <cc:CodeControl runat="server" ID="CodeControl" />
            </div>
            <br />
            <asp:Label ID="SponsorText" runat="server" Text="Label"></asp:Label>
            <asp:Label ID="FooterText" runat="server" Text="Label"></asp:Label>
        </div>

        <div class="col-sm-3">
            <div class="row">
                <div class="col-xs-12 margin-1em-top">
                    <uc1:MyBadgesListControl runat="server" ID="MyBadgesListControl" />
                </div>
            </div>
        </div>

    </div>
</asp:Content>
