<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/UnsecuredControl.Master" AutoEventWireup="true" 
CodeBehind="NoAccess.aspx.cs" Inherits="GRA.SRP.ControlRoom.NoAccess" 

%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p class="style1" style="margin: 20px; font-size: 125%;font-weight: bold;">
        <br />
        <br />
        Please contact the system administrator for more information or to modify your 
        security and access profile.</p>

        <p style="margin-left: 40px">

        Shortcuts:

    <br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href = "/ControlRoom/">Go to the Home page</a><br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href = "/ControlRoom/Login.aspx">Login Screen</a>

    </p>

</asp:Content>
