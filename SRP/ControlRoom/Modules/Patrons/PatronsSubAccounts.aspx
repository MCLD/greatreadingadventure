<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
    CodeBehind="PatronsSubAccounts.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronsSubAccounts" 

%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<%@ Register src="../../Controls/PatronContext.ascx" tagname="PatronContext" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <uc1:PatronContext ID="PatronContext1" runat="server" />

    <asp:GridView ID="gv1" runat="server"  
        AutoGenerateColumns="False" 
        AllowSorting="True"
        AllowPaging="False"
        PageSize="5"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        OnRowCommand="GvRowCommand"
        Width="100%"
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
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("PID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("PID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

			<asp:BoundField ReadOnly="True" HeaderText="Patron Id" DataField="PID" 
                SortExpression="PID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>

			<asp:BoundField ReadOnly="True" HeaderText="First Name" 
                    DataField="FirstName" SortExpression="FirstName" Visible="True" 
                    ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="300px" HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

			<asp:BoundField ReadOnly="True" HeaderText="Last Name" 
                    DataField="LastName" SortExpression="LastName" Visible="True" 
                    ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="300px" HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

			<asp:BoundField ReadOnly="True" HeaderText="UserName" 
                    DataField="Username" SortExpression="Username" Visible="True" 
                    ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="300px" HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


			<asp:BoundField ReadOnly="True" HeaderText="Email Address" 
                    DataField="EmailAddress" SortExpression="EmailAddress" Visible="True" 
                    ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="300px" HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField   SortExpression="DOB" Visible="True"  
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left" 
                HeaderText="DOB">              
                <ItemTemplate> 
                    <%# (   Eval("DOB").Equals(DBNull.Value) ? "" : FormatHelper.ToNormalDate((DateTime)Eval("DOB"))   ) %> 
                </ItemTemplate> 
				 <ControlStyle Width="250px" /> 
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle> 
            </asp:TemplateField>	 	

			<asp:BoundField ReadOnly="True" HeaderText="Gender" 
                DataField="Gender" SortExpression="Gender" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" HtmlEncode="False" ItemStyle-HorizontalAlign ="Left"  HeaderStyle-HorizontalAlign="Left">
				<ControlStyle Width="150px" />
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="150px" HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>

			<asp:BoundField ReadOnly="True" HeaderText="Program" 
                DataField="Program" SortExpression="AdminName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" HtmlEncode="False" ItemStyle-HorizontalAlign ="Left"  HeaderStyle-HorizontalAlign="Left">
				<ControlStyle Width="150px" />
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="150px" HorizontalAlign="Left"></ItemStyle>
            </asp:BoundField>


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>



 </asp:Content>
