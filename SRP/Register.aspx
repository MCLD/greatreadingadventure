<%@ Page Title="Register" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="GRA.SRP.Register" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="Controls/PatronRegistration.ascx" TagName="PatronRegistration" TagPrefix="uc3" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
    <script src="<%=ResolveUrl("~/Scripts/jquery.plugin.min.js")%>"></script>
    <script src="<%=ResolveUrl("~/Scripts/jquery.datepick.min.js")%>"></script>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <uc3:PatronRegistration ID="PatronRegistrationCtl" runat="server" />
        </div>
    </div>
</asp:Content>