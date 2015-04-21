<%@ Page Title="Reading Lists" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="ReadingLists.aspx.cs" Inherits="STG.SRP.ReadingLists" %>




<%@ Register src="~/Controls/ReadingListCtl.ascx" tagname="ReadingListCtl" tagprefix="uc1" %>




<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

    
    <uc1:ReadingListCtl ID="ReadingListCtl1" runat="server" />

    
</asp:Content>


