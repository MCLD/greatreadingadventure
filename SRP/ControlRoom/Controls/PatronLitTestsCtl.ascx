<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronLitTestsCtl.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.PatronLitTestsCtl" %>

<table cellpadding="5">
    <tr>
        <td></td>
        <td></td>
        <td valign="middle" align="right"><b>Score</b></td>
        <td valign="middle" align="right"><b>Score (%)</b></td>
        <td valign="middle" align="center"><b>Rank</b></td>
        <td valign="middle" align="center"><b>Date</b></td>
    </tr>

    <tr>
        <td>Test 1 (@ program enrollment)</td>
        <td></td>
        <td valign="middle" align="right"><asp:Label ID="lblT1S" runat="server" Text=""></asp:Label></td>
        <td valign="middle" align="right"><asp:Label ID="lblT1P" runat="server" Text=""></asp:Label></td>
        <td valign="middle" align="center"><asp:Label ID="lblT1R" runat="server" Text=""></asp:Label></td>
        <td valign="middle" align="center"><asp:Label ID="lblT1D" runat="server" Text=""></asp:Label></td>
    </tr>
        <tr>
        <td>Test 2 (@ program completion)</td>
        <td></td>
        <td valign="middle" align="right"><asp:Label ID="lblT2S" runat="server" Text=""></asp:Label></td>
        <td valign="middle" align="right"><asp:Label ID="lblT2P" runat="server" Text=""></asp:Label></td>
        <td valign="middle" align="center"><asp:Label ID="lblT2R" runat="server" Text=""></asp:Label></td>
        <td valign="middle" align="center"><asp:Label ID="lblT2D" runat="server" Text=""></asp:Label></td>
    </tr>
</table>