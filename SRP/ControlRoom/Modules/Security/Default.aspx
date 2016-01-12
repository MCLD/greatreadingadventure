<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" 
Inherits="GRA.SRP.ControlRoom.Modules.Security.Default" 

%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:GridView ID="gv" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" Width ="100%" PageSize="15"  
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"  
        OnPageIndexChanging="GvPageIndexChanging"    
        >
        <Columns>
           <asp:TemplateField>
                <HeaderTemplate>
                    &nbsp;&nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" 
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                                        
                </HeaderTemplate> 
                <HeaderStyle HorizontalAlign="left"/>               
                <ItemTemplate>
                &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnAudit" runat="server" AlternateText="Security Audit" Tooltip="Security Audit" 
                        CausesValidation="False" CommandName="AuditUser" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/audit.png" Width="20px" Visible="False"/>
                    &nbsp;
                    <asp:ImageButton ID="btnHistory" runat="server" AlternateText="Login History" Tooltip="Login History" 
                        CausesValidation="False" CommandName="LoginHistory" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/history.png" Width="20px" />
                    &nbsp;

                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
            </asp:TemplateField>

			<asp:BoundField ReadOnly="True" HeaderText="User Id" DataField="UID" SortExpression="UID" Visible="False" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Username" DataField="Username" SortExpression="Username" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Password" DataField="Password" SortExpression="Password" Visible="False" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="First Name" DataField="FirstName" SortExpression="FirstName" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Last Name" DataField="LastName" SortExpression="LastName" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Email Address" DataField="EmailAddress" SortExpression="EmailAddress" Visible="True" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Division" DataField="Division" SortExpression="Division" Visible="False" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Department" DataField="Department" SortExpression="Department" Visible="False" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Title" DataField="Title" SortExpression="Title" Visible="False" HeaderStyle-HorizontalAlign="Left" />
			<asp:BoundField ReadOnly="True" HeaderText="Is Active" DataField="IsActive" SortExpression="IsActive" Visible="True"  ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Must Reset Password" DataField="MustResetPassword" SortExpression="MustResetPassword" Visible="True" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Is Deleted" DataField="IsDeleted" SortExpression="IsDeleted" Visible="False"/>
			<asp:BoundField ReadOnly="True" HeaderText="Last Password Reset" DataField="LastPasswordReset" SortExpression="LastPasswordReset" Visible="True" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
			<asp:BoundField ReadOnly="True" HeaderText="Deleted Date" DataField="DeletedDate" SortExpression="DeletedDate" Visible="False"/>
			<asp:BoundField ReadOnly="True" HeaderText=" Modified Date" DataField="LastModDate" SortExpression="LastModDate" Visible="False"/>
			<asp:BoundField ReadOnly="True" HeaderText="Modified By" DataField="LastModUser" SortExpression="LastModUser" Visible="False"/>
			<asp:BoundField ReadOnly="True" HeaderText="Added Date" DataField="AddedDate" SortExpression="AddedDate" Visible="False"/>
			<asp:BoundField ReadOnly="True" HeaderText="Added By" DataField="AddedUser" SortExpression="AddedUser" Visible="False"/>

            
        </Columns>
    </asp:GridView>
    </asp:Content>
