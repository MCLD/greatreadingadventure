<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyGameLoggingNavControl.ascx.cs" Inherits="GRA.SRP.Controls.MyGameLoggingNavControl" %>
<div class="secondary-nav" style="margin-top: -20px!important; border: 0px solid black;">
    <ul class="nav">
        <li><a href="~/Adventures/" runat="server">
            <table>
                <tr>
                    <td>
                        <img src="/images/LogReading.png" id="Enter" style="width: 100px; float: left; padding-right: 5px; padding-top: 5px; padding-bottom: 5px;" alt="Reading Log" />
                        <asp:Label ID="lbl" runat="server" Text="Reading Log"></asp:Label>
                    </td>
                </tr>
            </table>
        </a></li>
    </ul>
</div>
