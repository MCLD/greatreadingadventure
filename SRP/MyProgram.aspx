<%@ Page Title="My Program" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyProgram.aspx.cs" Inherits="GRA.SRP.MyProgram" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Register Src="~/Controls/ProgramTabs.ascx" TagName="ProgramTabs" TagPrefix="uc1" %>
<%@ Register Src="~/Controls/ProgramBanner.ascx" TagName="ProgramBanner" TagPrefix="uc2" %>

<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    <div class="col-sm-12 text-center clearfix">
        <h1>Welcome, <% = ((Patron)Session["Patron"]).FirstName %>!</h1>
    </div>
    <hr />

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="col-sm-3">
                <asp:PlaceHolder ID="LeftColumn" runat="server" />
            </div>

            <div class="col-sm-6">
                <asp:PlaceHolder ID="CenterColumn" runat="server" />
            </div>

            <div class="col-sm-3">
                <asp:PlaceHolder ID="RightColumn" runat="server" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
