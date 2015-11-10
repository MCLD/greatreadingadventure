<%@ Page Title="My Badges" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyBadges.aspx.cs" Inherits="GRA.SRP.MyBadges"
    EnableEventValidation="false" %>

<%@ Import Namespace="GRA.SRP.DAL" %>

<%@ Register Src="~/Controls/Badges.ascx" TagName="Badges" TagPrefix="uc1" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <uc1:Badges ID="Badges" runat="server" />
        </div>
    </div>
</asp:Content>
