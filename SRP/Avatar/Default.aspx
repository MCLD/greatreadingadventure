<%@ Page Title="Avatar" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="GRA.SRP.MyAvatar" 
    EnableEventValidation="false" 
    %>
<%@ Import Namespace="GRA.SRP.DAL" %>

<%@ Register src="~/Controls/MyAvatarControl.ascx" tagname="MyAvatarControl" tagprefix="uc1" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
    <script src="<%=ResolveUrl("~/Scripts/avatar.js")%>"></script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <uc1:MyAvatarControl ID="AvatarControl" runat="server" />
</asp:Content>