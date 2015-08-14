<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="ClosingPush.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Settings.ClosingPush" 
    
%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
Starting at: <asp:TextBox ID="txtStart" runat="server">0</asp:TextBox>

<asp:Button ID="Button1" runat="server" Text="Give Points" 
        onclick="Button1_Click" />



</asp:Content>
