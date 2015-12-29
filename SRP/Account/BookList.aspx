<%@ Page Title="Book List" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="BookList.aspx.cs" Inherits="GRA.SRP.ViewBookList" %>

<%@ Register Src="~/Controls/BooksRead.ascx" TagPrefix="uc1" TagName="BooksRead" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc1:BooksRead runat="server" id="BooksRead" />
</asp:Content>