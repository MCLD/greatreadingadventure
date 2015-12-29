<%@ Page Title="Password Recovery" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Recover.aspx.cs" Inherits="GRA.SRP.Recover" %>

<%@ Register Src="~/Controls/RecoverPassword.ascx" TagName="RecoverPassword" TagPrefix="uc3" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <uc3:RecoverPassword ID="RecoverPassword1" runat="server" />
        </div>
    </div>
</asp:Content>
