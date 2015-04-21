<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NotificationCounterControl.ascx.cs" Inherits="STG.SRP.Controls.NotificationCounterControl" %>


    <div class="secondary-nav" style="margin-topXX: -20px!important; ">
        <ul class="nav">
            <li><a href="/MyNotifications.aspx" 
                    >
                    <table><tr>
                        <td  width="128" height="90" style="background-image: url(/images/mail.png); background-position:0px -20px;" align="center" valign="top">
                        
                        <center>
                        <br /><asp:Label ID="Count1" runat="server" Text="10" Width="20px"></asp:Label>
                        </center>
                        </td><td class="tx">You have <asp:Label ID="Count2" runat="server" Text="10"></asp:Label> notifications <br /><small> </small></td></tr></table>
                     
                    </a></li>
        </ul>
    </div>


