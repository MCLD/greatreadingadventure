<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master" AutoEventWireup="true" 
    CodeBehind="PatronsByProgram.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronsByProgram" 

%>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit"%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<table width="100%"  style="border:double 3px #A3C0E8;">

<tr>
    <th width="50%"><b>Program</b></th>
    <th>

   <asp:ObjectDataSource ID="odsDDPrograms" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Programs">
    </asp:ObjectDataSource>



    
    </th>
</tr>
<tr>
    <td>
        <asp:DropDownList ID="ProgID" runat="server" DataSourceID="odsDDPrograms" 
            DataTextField="TabName" DataValueField="PID" 
            AppendDataBoundItems="True"  Width="97%"
            >
            <asp:ListItem Value="" Text="[Select a Value]"></asp:ListItem>
        </asp:DropDownList>
    </td>
    <td>



    
    <asp:Button ID="btnFilter" runat="server" Text="Filter" 
        onclick="btnFilter_Click"  Width="150px" CSSClass="btn-lg btn-green"/>

        &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear" 
        onclick="btnClear_Click"  Width="150px" CSSClass="btn-lg btn-orange"/>
    </td>
</tr>

</table>

        &nbsp;<hr />
    <asp:ObjectDataSource ID="odsSearch" runat="server" 
        TypeName="GRA.SRP.DAL.Patron" EnablePaging="True" 
        SelectMethod="GetPaged" 
        OldValuesParameterFormatString="original_{0}" 
        SelectCountMethod="GetTotalPagedCount" 
        SortParameterName="sort" 
        
        
        >
            <SelectParameters>
                <asp:Parameter Name="sort" Type="String" />
                <asp:Parameter Name="startRowIndex" Type="Int32" />
                <asp:Parameter Name="maximumRows" Type="Int32" />
                <asp:Parameter DefaultValue="" Name="searchFirstName" Type="String" />
                <asp:Parameter DefaultValue="" Name="searchLastName" Type="String" />
                <asp:Parameter DefaultValue="" Name="searchUsername" Type="String" />
                <asp:Parameter DefaultValue="" Name="searchEmail" Type="String" />
                <asp:Parameter DefaultValue="" Name="searchDOB" Type="String" />
                <asp:SessionParameter DefaultValue="" Name="searchProgram" 
                    SessionField="PS_Prog" Type="Int32" />
                <asp:Parameter DefaultValue="" Name="searchGender" Type="String" />
            </SelectParameters>
    </asp:ObjectDataSource>



    <asp:GridView ID="gv1" runat="server"  
        AutoGenerateColumns="False" 
        AllowSorting="True"
        AllowPaging="True"
        PageSize="5"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        OnRowCommand="GvRowCommand"
        width="100%"
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
