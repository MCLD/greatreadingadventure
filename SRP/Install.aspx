<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Install.aspx.cs" Inherits="STG.SRP.Install" validateRequest="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
    <tr>
        <td nowrap><b>SQL Connection String: </b><br />DBO access db user</td>
        <td>
            <asp:TextBox ID="txtDBO" runat="server" TextMode="SingleLine" Width="1200px"></asp:TextBox><br />
            Data Source=(local);Initial Catalog=SRP;User ID=dbouser;Password=dbopass
        </td>
    </tr>
    <tr>
        <td nowrap><b>Runtime SQL Connection String: </b><br />Normal db User, read/write access</td>
        <td>
            <asp:TextBox ID="txtConn" runat="server" TextMode="SingleLine" Width="1200px"></asp:TextBox><br />
            Data Source=(local);Initial Catalog=SRP;User ID=srpuser;Password=srppass
        </td>
    </tr>
    <tr>
        <td nowrap><b>Email host name: </b></td>
        <td>
            <asp:TextBox ID="txtEmailHost" runat="server" TextMode="SingleLine" Width="1200px" Text="(localhost)"></asp:TextBox>        
        </td>
    </tr>
    <tr>
        <td><b>: </b></td>
        <td>
        
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Run Scripts" />
        </td>
    </tr>    
    </table>
        
    </div>
    </form>
</body>
</html>
