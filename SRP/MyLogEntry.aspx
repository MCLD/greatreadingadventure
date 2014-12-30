<%@ Page Title="Game Log" Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true"
    CodeBehind="MyLogEntry.aspx.cs" Inherits="STG.SRP.MyLogEntry" 
    EnableEventValidation="false" 
    %>


<%@ Register src="~/Controls/GameLoggingControl.ascx" tagname="GameLoggingControl" tagprefix="uc1" %>


<asp:Content ID="TopOfHeader" runat="server" ContentPlaceHolderID="TopOfHeader">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

<uc1:GameLoggingControl ID="GameLoggingControl1" runat="server" />
    
</asp:Content>


