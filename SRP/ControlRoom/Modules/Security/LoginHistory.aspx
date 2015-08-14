<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="LoginHistory.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Security.LoginHistory"

%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style1
        {
            font-size: medium;
        }
        .style2
        {
            font-size: medium;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table>
    <tr>
        <td align="right" class="style2">Name: </td>
        <td align="left"><asp:Label ID="lblName" runat="server" CssClass="style1"></asp:Label>
            <asp:Label ID="lblUID" runat="server" Text="" Visible="False" CssClass="style1"></asp:Label></td>
        <td align="right" class="style2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Username:&nbsp; </td>
        <td align="left">
            <asp:Label ID="lblUsername" runat="server" 
                CssClass="style1"></asp:Label></td>      
    </tr>
</table>
<br />
    
    
    
    
    
    <asp:GridView ID="gv" runat="server" AllowPaging="True" AllowSorting="True" 
        AutoGenerateColumns="False" Width="100%"
        DataKeys="UIDLH"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"
        PageSize="25" onpageindexchanging="gv_PageIndexChanging"      
        >
        <HeaderStyle HorizontalAlign="left" />
        <Columns>

			<asp:BoundField ReadOnly="True" HeaderText="UIDLH" DataField="UIDLH" 
                SortExpression="UIDLH" Visible="False"/>
			<asp:BoundField ReadOnly="True" HeaderText="UID" DataField="UID" 
                SortExpression="UID" Visible="False"/>
			<asp:BoundField ReadOnly="True" HeaderText="Session Start" DataField="StartDateTime" SortExpression="StartDateTime" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Session End" DataField="EndDateTime" SortExpression="EndDateTime" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Session ID" DataField="SessionsID" SortExpression="SessionsID" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="IP" DataField="IP" SortExpression="IP" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Machine Name" DataField="MachineName" SortExpression="MachineName" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Browser" DataField="Browser" SortExpression="Browser" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found.
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
