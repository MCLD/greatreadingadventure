<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronSearchCtl.ascx.cs" Inherits="GRA.SRP.ControlRoom.Controls.PatronSearchCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<table width="100%"  style="border:double 3px #A3C0E8;">
<tr>
    <th><b>First Name</b></th>
    <th><b>Last Name</b></th>
    <th><b>Username/Login ID</b></th>
</tr>
<tr>
    <td><asp:TextBox ID="txtFirstName" runat="server" Width="95%"></asp:TextBox></td>
    <td><asp:TextBox ID="txtLastName" runat="server" Width="95%"></asp:TextBox></td>
    <td><asp:TextBox ID="txtUsername" runat="server" Width="95%"></asp:TextBox></td>
</tr>

<tr>
    <th colspan="2"><b>Email Address</b></th>
    <th><b>DOB</b></th>
</tr>
<tr>
    <td colspan="2"><asp:TextBox ID="txtEmail" runat="server" Width="97%"></asp:TextBox></td>
    <td>
        <asp:TextBox ID="txtDOB" runat="server" Width="75px"                         
            Text=''></asp:TextBox>
        <ajaxToolkit:CalendarExtender ID="cetxtDOB" runat="server" TargetControlID="txtDOB">
        </ajaxToolkit:CalendarExtender>
        <ajaxToolkit:MaskedEditExtender ID="metxtDOB" runat="server" 
            UserDateFormat="MonthDayYear" TargetControlID="txtDOB" MaskType="Date" Mask="99/99/9999">
        </ajaxToolkit:MaskedEditExtender>  
    </td>
</tr>

<tr>
    <th colspan="2"><b>Program</b></th>
    <th><b>Gender</b></th>
</tr>
<tr>
    <td colspan="2">
        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" 
            DataTextField="TabName" DataValueField="PID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>
        <asp:DropDownList ID="Gender" runat="server" 
            AppendDataBoundItems="True"  Width="95%"
        >
            <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
            <asp:ListItem Value="M" Text="Male"></asp:ListItem>
            <asp:ListItem Value="F" Text="Female"></asp:ListItem>
            <asp:ListItem Value="O" Text="Other"></asp:ListItem>
        </asp:DropDownList>
    </td>
</tr>

</table>

   <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>
