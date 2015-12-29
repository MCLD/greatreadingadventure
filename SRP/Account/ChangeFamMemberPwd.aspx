<%@ Page Title="Change Password for a Family Member" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="ChangeFamMemberPwd.aspx.cs" Inherits="GRA.SRP.ChangeFamMemberPwd" %>

<%@ Register Src="~/Controls/ProgramTabs.ascx" TagName="ProgramTabs" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ProgramBanner.ascx" TagName="ProgramBanner" TagPrefix="uc2" %>

<%@ Register Src="~/Controls/ChangeFamilyPassword.ascx" TagName="ChangeFamilyPassword" TagPrefix="uc3" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <uc3:ChangeFamilyPassword ID="ChangeFamilyPassword1" runat="server" />
</asp:Content>
