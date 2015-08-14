<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" 
	CodeBehind="TenantGroupList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Tenant.TenantGroupList" 
%>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    Organization:         
        <asp:DropDownList ID="TenID" runat="server" DataSourceID="odsDDTen" 
            DataTextField="AdminName" DataValueField="TenID" 
            AppendDataBoundItems="True"  Width="50%"
            >
            <asp:ListItem Value="0" Text="[Select An Organization"></asp:ListItem>
        </asp:DropDownList>
        &nbsp;
        <asp:Button ID="Button1" runat="server" Text="Filter" 
        onclick="Button1_Click" />
        <hr />
  <asp:ObjectDataSource ID="odsDDTen" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.Core.Utilities.Tenant">
   </asp:ObjectDataSource>

    <asp:GridView ID="gv" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        >
        <Columns>
           <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                &nbsp;&nbsp;<asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                                        
                </HeaderTemplate>                
                <ItemTemplate>
                &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("GID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("GID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>

<ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>

					<asp:BoundField ReadOnly="True" HeaderText="Group Id" DataField="GID" 
                SortExpression="GID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
<ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
					<asp:BoundField ReadOnly="True" HeaderText="Group Name" 
                DataField="GroupName" SortExpression="GroupName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
					<ControlStyle Width="150px" />
<ItemStyle VerticalAlign="Top" Wrap="False" Width="200px"></ItemStyle>
            </asp:BoundField>
					<asp:BoundField ReadOnly="True" HeaderText="Description" 
                DataField="GroupDescription" SortExpression="GroupDescription" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" HtmlEncode="False">
					<ControlStyle Width="500px" />
<ItemStyle VerticalAlign="Top" Wrap="False" Width="500px"></ItemStyle>
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
            No records were found.&nbsp;                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>




