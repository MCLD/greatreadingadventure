<%@ Page Title="Offers" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.MyOffers"
    EnableEventValidation="false" %>

<%@ Import Namespace="GRA.SRP.DAL" %>

<%@ Register Src="~/Controls/Offers.ascx" TagName="Offers" TagPrefix="uc1" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <uc1:Offers ID="Offers1" runat="server" />
        </div>
    </div>
</asp:Content>
