<%@ Page Title="Log Reading for a Family Member" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="EnterFamMemberLog.aspx.cs" Inherits="GRA.SRP.EnterFamMemberLog" %>

<%@ Register Src="~/Controls/FamilyReadingLogControl.ascx" TagPrefix="uc1" TagName="FamilyReadingLogControl" %>
<%@ Register Src="~/Controls/FamilyCodeControl.ascx" TagPrefix="uc1" TagName="FamilyCodeControl" %>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-xs-12 text-center text-danger margin-1em-top">
            <strong style="font-size: larger;">
                <asp:Label runat="server" ID="NotYet" Visible="false"></asp:Label>
            </strong>
        </div>
    </div>
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
    <div class="row margin-1em-top">
        <div class="col-sm-6 col-sm-offset-3 text-center margin-1em-top">
            <asp:HyperLink ID="FamilyAccountList"
                CausesValidation="false"
                CssClass="btn btn-default"
                runat="server"
                NavigateUrl="~/Account/FamilyAccountList.aspx">
                        <span class="glyphicon glyphicon-th-list margin-halfem-right"></span>
                        <asp:Label runat="server" Text="myaccount-family"></asp:Label>
            </asp:HyperLink>
        </div>
    </div>
</asp:Content>
