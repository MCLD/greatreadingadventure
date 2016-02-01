<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="SRP._Default" %>

<%@ Register Src="~/Controls/ProgramBanner.ascx" TagName="ProgramBanner" TagPrefix="uc2" %>
<%@ Register Src="~/Controls/PublicProgramView.ascx" TagName="PublicProgramView" TagPrefix="uc3" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeaderContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12">
            <uc2:ProgramBanner ID="ProgramBanner1" runat="server" />
        </div>
        <div class="col-xs-12">
            <uc3:PublicProgramView ID="PublicProgramView1" runat="server" />
        </div>
    </div>
</asp:Content>
