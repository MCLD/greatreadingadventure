<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyPointsControl.ascx.cs" Inherits="GRA.SRP.Controls.MyPointsControl" %>

<span class="points-display animated bounceInLeft">I have <strong><asp:Label ID="lblPoints" runat="server" Text="Label"></asp:Label> points.</strong></span>
<br /><asp:Label ID="lblNextLevel" runat="server" Text=""></asp:Label>
<br />

<div id="divGoalProgress" runat="server" >
    <span><asp:Label ID="lblGoal" runat="server" Text="Reading Goal"></asp:Label></span>
    <div class="progress" style="margin-left: 8px; margin-right: 8px">
        <div ID="divGoalProgressBar" runat="server" class="progress-bar" role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 60%; min-width: 2em;">
            <asp:Label ID="lblPercentGoal" runat="server" Text="Label"></asp:Label>
        </div>
    </div>
</div>

See my <asp:HyperLink runat="server" NavigateUrl="~/Account/ActivityHistory.aspx">activity history</asp:HyperLink>.