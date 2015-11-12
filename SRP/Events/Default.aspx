<%@ Page Title="Events" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.EventsPage"
    EnableEventValidation="false" %>

<%@ Import Namespace="GRA.SRP.DAL" %>

<%@ Register Src="~/Controls/Events.ascx" TagName="Events" TagPrefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
    <script src="<%=ResolveUrl("~/Scripts/jquery.plugin.min.js")%>"></script>
    <script src="<%=ResolveUrl("~/Scripts/jquery.datepick.min.js")%>"></script>
</asp:Content>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <uc1:Events ID="EventsControl" runat="server" />
        </div>
    </div>
</asp:Content>
