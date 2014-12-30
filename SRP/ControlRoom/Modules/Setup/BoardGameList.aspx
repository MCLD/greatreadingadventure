﻿<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="BoardGameList.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.BoardGameList" 
    
%>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.ProgramGame">
	</asp:ObjectDataSource>


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False" Width="100%"
        DataKeys="PGID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("PGID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("PGID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="PGID" DataField="PGID" 
                SortExpression="PGID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          


			<asp:BoundField ReadOnly="True" HeaderText="Game Name" 
                DataField="GameName" SortExpression="GameName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="100%" />
                <ItemStyle    Width="100%" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="Modified On" 
                DataField="LastModDate" SortExpression="LastModDate" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Modified By" 
                DataField="LastModUser" SortExpression="LastModUser" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Added On" 
                DataField="AddedDate" SortExpression="AddedDate" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Added By" 
                DataField="AddedUser" SortExpression="AddedUser" Visible="False" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>			


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

