<%@ Page Title="Adventures" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.MyLogEntry"
    EnableEventValidation="false" %>

<%@ Register Src="~/Controls/Avatar.ascx" TagPrefix="avatar" TagName="Avatar" %>
<%@ Register Src="~/Controls/MyPointsControl.ascx" TagPrefix="points" TagName="MyPointsControl" %>
<%@ Register Src="~/Controls/GameLoggingControl.ascx" TagName="GameLoggingControl" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/MyGamemapNavControl.ascx" TagName="MyGamemapNavControl" TagPrefix="uc3" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-sm-3">
            <div class="row">
                <div class="col-xs-12 text-center">
                    <avatar:Avatar runat="server" ID="Avatar" />
                </div>
            </div>
            <div class="row margin-1em-bottom">
                <div class="col-xs-12 text-center margin-halfem-top margin-halfem-bottom">
                    <points:MyPointsControl runat="server" ID="MyPointsControl" />
                </div>
            </div>
        </div>
        <div class="col-sm-9">
            <uc1:GameLoggingControl ID="GameLoggingControl1" runat="server" />
        </div>
    </div>
</asp:Content>
