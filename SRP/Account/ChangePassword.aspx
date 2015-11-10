
<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="ChangePassword.aspx.cs" Inherits="GRA.SRP.ChangePassword" %>

<%@ Register src="~/Controls/ChangePassword.ascx" tagname="ChangePassword" tagprefix="uc3" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<uc3:ChangePassword ID="ChangePassword1" runat="server" />
</asp:Content>
<asp:Content ID="BottomOfPage" runat="server" ContentPlaceHolderID="BottomOfPage">
    <script>
        $().ready(focusFirstField());
    </script>
</asp:Content>
