<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronContext.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.PatronContext" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<table style="width: 100%;" class="table table-condensed">
    <tr>
        <th>First Name:</th>
        <td>
            <asp:Label ID="txtFirstName" runat="server"></asp:Label></td>
        <th>Last Name: </th>
        <td>
            <asp:Label ID="txtLastName" runat="server"></asp:Label></td>
        <th>Username/Login ID: </th>
        <td>
            <asp:HyperLink runat="server" Target="_blank" ID="PatronNavigation"><asp:Label ID="txtUsername" runat="server"></asp:Label></asp:HyperLink>
            </td>
    </tr>

    <tr>
        <th>Program: </th>
        <td>
            <asp:Label ID="txtProgram" runat="server"></asp:Label></td>
        <th>Current Point Total:</th>
        <td>
            <asp:Label ID="PointTotal" runat="server"></asp:Label>
        </td>
        <th>Email Address: </th>
        <td>
            <asp:Label ID="txtEmail" runat="server"></asp:Label></td>

    </tr>

    <tr>
        <th>Registered:</th>
        <td>
            <asp:Label ID="Registered" runat="server"></asp:Label></td>
        <th><span runat="server" id="DobLabel">DOB: </span></th>
        <td>
            <asp:Label ID="txtDOB" runat="server"></asp:Label></td>
        <th><span runat="server" id="GenderLabel">Gender: </span></th>
        <td>
            <asp:Label ID="txtGender" runat="server"></asp:Label></td>
    </tr>
</table>

