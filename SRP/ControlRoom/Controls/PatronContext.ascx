<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronContext.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.PatronContext" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<table width="100%"  style="border:double 3px #A3C0E8;">
<tr>
    <th colspan="6" style="background-COLOR: #A3C0E8!IMPORTANT; color: White!important"><b>Currently Selected Patron</b></th>
</tr>
<tr>
    <th width="100px"><b>First Name: </b></th><td width="200px"><asp:Label ID="txtFirstName" runat="server"></asp:Label></td>
    <th width="100px"><b>Last Name: </b></th><td><asp:Label ID="txtLastName" runat="server"></asp:Label></td>
    <th width="120px"><b>Username/Login ID: </b></th><td><asp:Label ID="txtUsername" runat="server"></asp:Label></td>
</tr>

<tr>
    <th nowrap width="100px"><b>Email Address: </b></th><td colspan="3"><asp:Label ID="txtEmail" runat="server"></asp:Label></td>
    <th><b>DOB: </b></th><td><asp:Label ID="txtDOB" runat="server"></asp:Label></td>
</tr>

<tr>
    <th ><b>Program: </b></th><td colspan="3"><asp:Label ID="txtProgram" runat="server"></asp:Label></td>
    <th><b>Gender: </b></th><td><asp:Label ID="txtGender" runat="server"></asp:Label></td>
</tr>

</table>

