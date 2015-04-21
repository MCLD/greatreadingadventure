﻿<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="PatronNotifications.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Patrons.PatronNotifications" 
    
%>

<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>
<%@ Register src="~/ControlRoom/Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAllToPatron" 
        TypeName="STG.SRP.DAL.Notifications">
        <SelectParameters>
            <asp:SessionParameter DefaultValue="0" Name="PID" SessionField="CURR_PATRON_ID" 
                Type="Int32" />
        </SelectParameters>

	</asp:ObjectDataSource>


    <uc1:PatronContext ID="pcCtl" runat="server" />

    <asp:GridView ID="gv" runat="server" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="NID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"   
        width="100%"   
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("NID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("NID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="NID" DataField="NID" 
                SortExpression="NID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          
			 

            <asp:TemplateField   SortExpression="PID_From" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="PID_From">              
                <ItemTemplate> 
                    <%# ((int)Eval("PID_From")==0 ? "System" : "")%> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 	 

			<asp:BoundField ReadOnly="True" HeaderText="Subject" 
                DataField="Subject" SortExpression="Subject" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="650px" />
                <ItemStyle    Width="650px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="Date" 
                DataField="AddedDate" SortExpression="AddedDate" Visible="True" 
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

