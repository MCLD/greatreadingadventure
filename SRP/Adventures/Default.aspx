<%@ Page Title="Game Log" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.MyLogEntry"
    EnableEventValidation="false" %>

<%@ Register Src="~/Controls/GameLoggingControl.ascx" TagName="GameLoggingControl" TagPrefix="uc1" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <uc1:GameLoggingControl ID="GameLoggingControl1" runat="server" />
        </div>
    </div>
</asp:Content>
