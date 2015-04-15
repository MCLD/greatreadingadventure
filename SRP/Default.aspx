<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="SRP._Default" 
  
%>

<%@ Register src="~/Controls/ProgramTabs.ascx" tagname="ProgramTabs" tagprefix="uc1" %>

<%@ Register src="~/Controls/ProgramBanner.ascx" tagname="ProgramBanner" tagprefix="uc2" %>

<%@ Register src="~/Controls/PublicProgramView.ascx" tagname="PublicProgramView" tagprefix="uc3" %>

<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc1:ProgramTabs ID="ProgramTabs1" runat="server" />
<uc2:ProgramBanner ID="ProgramBanner1" runat="server" />
<uc3:PublicProgramView ID="PublicProgramView1" runat="server" />

</asp:Content>
