<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="CurrentSessions.aspx.cs" 
Inherits="GRA.SRP.ControlRoom.Modules.Security.CurrentSessions" 
%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    <asp:GridView ID="gv" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
        DataKeys="UIDLH"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        PageSize="30"
        Width="100%"      
        >
        <Columns>

		    <asp:BoundField ReadOnly="True" HeaderText="UIDLH" DataField="UIDLH" SortExpression="UIDLH" Visible="False"/>
		    <asp:BoundField ReadOnly="True" HeaderText="UID" DataField="UID" SortExpression="UID" Visible="False"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Name" DataField="Name" SortExpression="Name" Visible="True"  HeaderStyle-HorizontalAlign="Left"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Username" DataField="Username" SortExpression="Username" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Session Start" DataField="StartDateTime" SortExpression="StartDateTime" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Session End" DataField="EndDateTime" SortExpression="EndDateTime" Visible="False"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Session ID" DataField="SessionsID" SortExpression="SessionsID" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
		    <asp:BoundField ReadOnly="True" HeaderText="IP" DataField="IP" SortExpression="IP" Visible="True"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Machine Name" DataField="MachineName" SortExpression="MachineName" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Browser" DataField="Browser" SortExpression="Browser" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
		    <asp:BoundField ReadOnly="True" HeaderText="Organization" DataField="Tenant" SortExpression="Tenant" Visible="True" HeaderStyle-HorizontalAlign="Left"/>

        </Columns>
    </asp:GridView>
    
    
    </asp:Content>
