<%@ Page Title="My Account" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.MyAccount"
    MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register Src="~/Controls/MyAccountCtl.ascx" TagName="MyAccountCtl" TagPrefix="uc3" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="col-sm-12">
        <uc3:MyAccountCtl ID="MyAccountCtl1" runat="server" />
    </div>
    Tenant: <%=Session["TenantID"].ToString() %>
</asp:Content>
