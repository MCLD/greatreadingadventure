<%@ Page Title="Enter Family Member Log Entry" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="EnterFamMemberLog.aspx.cs" Inherits="GRA.SRP.EnterFamMemberLog" %>

<%@ Register Src="~/Controls/FamilyReadingLogControl.ascx" TagPrefix="uc1" TagName="FamilyReadingLogControl" %>
<%@ Register Src="~/Controls/FamilyCodeControl.ascx" TagPrefix="uc1" TagName="FamilyCodeControl" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-sm-6 col-sm-offset-3">
            <uc1:FamilyReadingLogControl runat="server" ID="FamilyReadingLogControl" />

        </div>
    </div>
    <div class="row margin-1em-top">
        <div class="col-sm-6 col-sm-offset-3">
            <uc1:FamilyCodeControl runat="server" ID="FamilyCodeControl" />
        </div>
    </div>
</asp:Content>
