<%@ Page Title="My Program" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="GRA.SRP.MyProgram" %>

<%@ Register Src="~/Controls/Welcome.ascx" TagPrefix="uc1" TagName="Welcome" %>
<%@ Register Src="~/Controls/Avatar.ascx" TagPrefix="uc1" TagName="Avatar" %>
<%@ Register Src="~/Controls/MyBadgesListControl.ascx" TagPrefix="uc1" TagName="MyBadgesListControl" %>
<%@ Register Src="~/Controls/MyPointsControl.ascx" TagPrefix="uc1" TagName="MyPointsControl" %>
<%@ Register Src="~/Controls/LeaderBoardControl.ascx" TagPrefix="uc1" TagName="LeaderBoardControl" %>






<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-sm-12 text-center">
            <uc1:Welcome runat="server" ID="Welcome" />
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                <div class="col-sm-3">
                    <div class="row">
                        <div class="col-xs-12 text-center">
                            <uc1:Avatar runat="server" ID="Avatar" />
                        </div>
                        <div class="col-xs-12">
                            <uc1:MyBadgesListControl runat="server" ID="MyBadgesListControl" />
                        </div>
                    </div>
                </div>

                <div class="col-sm-6">
                    <asp:PlaceHolder ID="CenterColumn" runat="server" />
                </div>

                <div class="col-sm-3">
                    <div class="row">
                        <div class="col-xs-12">
                            <uc1:MyPointsControl runat="server" ID="MyPointsControl" />
                        </div>
                        <div class="col-xs-12">
                            <uc1:LeaderBoardControl runat="server" ID="LeaderBoardControl" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
