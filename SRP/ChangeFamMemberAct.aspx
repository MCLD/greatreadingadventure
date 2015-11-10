
<%@ Page Title="Change Family Member Account" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="ChangeFamMemberAct.aspx.cs" Inherits="GRA.SRP.ChangeFamMemberAct" 
     MaintainScrollPositionOnPostback="true"
%>
<%@ Import Namespace="GRA.SRP.Controls" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register src="~/Controls/FamilyAccountCtl.ascx" tagname="FamilyAccountCtl" tagprefix="uc1" %>
<%@ Register src="~/Controls/ProgramBanner.ascx" tagname="ProgramBanner" tagprefix="uc2" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc1:FamilyAccountCtl ID="FamilyAccountCtl1" runat="server" />
<!--<uc2:ProgramBanner ID="ProgramBanner1" runat="server" />-->
<hr />
</asp:Content>
