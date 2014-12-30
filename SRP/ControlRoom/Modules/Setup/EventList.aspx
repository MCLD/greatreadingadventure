﻿<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" 
    AutoEventWireup="true" CodeBehind="EventList.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.EventList" 
    Theme="STG"
%>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table>
<thead>
    <th colspan="7">
    </th>
</thead>
<tr>
<td><b>Start Date:</b> </td>
<td>
                <asp:TextBox ID="StartDate" runat="server" Width="75px"                         
                    Text=''></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceStartDate" runat="server" TargetControlID="StartDate" DefaultView="Days">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meStartDate" runat="server"  
                    UserDateFormat="MonthDayYear" TargetControlID="StartDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>  
</td>
<td><b>End Date:</b> </td>
<td>
                <asp:TextBox ID="EndDate" runat="server" Width="75px"                         
                    Text=''></asp:TextBox>
                <ajaxToolkit:CalendarExtender ID="ceEndDate" runat="server" TargetControlID="EndDate">
                </ajaxToolkit:CalendarExtender>
                <ajaxToolkit:MaskedEditExtender ID="meEndDate" runat="server" 
                    UserDateFormat="MonthDayYear" TargetControlID="EndDate" MaskType="Date" Mask="99/99/9999">
                </ajaxToolkit:MaskedEditExtender>  
</td>

<td><b>Branch:</b> </td>
<td>
                <asp:DropDownList ID="BranchId" runat="server" DataSourceID="odsDDBranch" DataTextField="Code" DataValueField="CID" 
                    AppendDataBoundItems="True" Width="200px"
                 >
                    <asp:ListItem Value="0" Text=""></asp:ListItem>
                </asp:DropDownList>
</td>
<td>
    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click" CssClass="btn-lg btn-green"/>
        &nbsp;
    <asp:Button ID="btnClear" runat="server" Text="Clear/All" onclick="btnClear_Click"  CssClass="btn-lg btn-blue"
         />

</td>
</tr>
</table>


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        DataKeys="EID"
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
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("EID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("EID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="EID" DataField="EID" 
                SortExpression="EID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>          


			<asp:BoundField ReadOnly="True" HeaderText="Event Title" 
                DataField="EventTitle" SortExpression="EventTitle" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

            <asp:TemplateField   SortExpression="EventDate" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="EventDate">              
                <ItemTemplate> 
                    <%# Eval("EventDate") == DBNull.Value ? "N/A" : ((DateTime)Eval("EventDate")).ToNormalDate()%>
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 			 


			<asp:BoundField ReadOnly="True" HeaderText="Event Time" 
                DataField="EventTime" SortExpression="EventTime" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="Secret Code" 
                DataField="SecretCode" SortExpression="SecretCode" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="Branch" 
                DataField="Branch" SortExpression="Branch" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
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

   <asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAdminSearch" 
        TypeName="STG.SRP.DAL.Event">
        <SelectParameters>
            <asp:ControlParameter ControlID="StartDate" DefaultValue="" Name="startDate" 
                PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="EndDate" Name="endDate" PropertyName="Text" 
                Type="String" />
            <asp:ControlParameter ControlID="BranchId" DefaultValue="0" Name="branchID" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server" 
        SelectMethod="GetAlByTypeName" 
        TypeName="STG.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue = "Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>

