<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/MasterPages/CMSSecure.Master" AutoEventWireup="true" 
    CodeBehind="UserAudit.aspx.cs" 
    Inherits="STG.CMS.Portal.ControlRoom.Module.Security.UserAudit" 
	Theme="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MetaTagsInc" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblUID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsCMSUser" runat="server" 
        SelectMethod="Fetch" TypeName="STG.CMS.Portal.Core.CMSUser">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblUID" Name="pUid" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsCMSUser" 
         OnItemCommand="DvItemCommand" 
        Width="100%"
        >
        <Fields>
            <asp:TemplateField InsertVisible="False" ShowHeader="False">
                <ItemTemplate>

<table style="width: 100%; " cellpadding="3" cellspacing="0">
    <tr>
        <td align="right" valign="top"><b>Username:</b></td><td  align="left" valign="top"><%# Eval("Username") %></td>
        <td align="right" valign="top"><b>Email:</b></td><td  align="left" valign="top"><%# Eval("EmailAddress") %></td>
    </tr>
    <tr>
        <td align="right" valign="top"><b>Name:</b></td><td  align="left" valign="top"><%# Eval("FirstName") %> <%# Eval("LastName") %></td>
        <td align="right" valign="top"><b>Is Active:</b></td><td  align="left" valign="top"><%# Eval("IsActive") %></td>
    </tr>
    <tr>
        <td></td>
        <td align="left" valign="top" colspan="3"><%# Eval("Title") %>, <%# Eval("Department") %>, <%# Eval("Division") %></td>
    </tr>
    <tr>
        <td align="right" valign="top"><b>Must Reset Password:</b></td><td  align="left" valign="top"><%# Eval("MustResetPassword") %> </td>
        <td align="right" valign="top"><b>Last Password Reset:</b></td><td  align="left" valign="top"><%# Eval("LastPasswordReset") %></td>
    </tr>
    <tr>
        <td align="right" valign="top"><b>Added Date:</b></td><td  align="left" valign="top"><%# Eval("AddedDate") %> </td>
        <td align="right" valign="top"><b>Added By:</b></td><td  align="left" valign="top"><%# Eval("AddedUser") %></td>
    </tr>
    <tr>
        <td align="right" valign="top"><b>Last Mod Date:</b></td><td  align="left" valign="top"><%# Eval("LastModDate") %> </td>
        <td align="right" valign="top"><b>Last Mod By:</b></td><td  align="left" valign="top"><%# Eval("LastModUser") %></td>
    </tr>

    <tr>
        <td align="right" valign="top"><b>User Groups:</b></td>
        <td align="left" valign="top" colspan="3">
        
            <asp:ObjectDataSource ID="odsUserGroups" runat="server" 
                SelectMethod="GetUserGroupList" TypeName="STG.CMS.Portal.Core.CMSUser">
                <SelectParameters>
                    <asp:ControlParameter ControlID="lblUID" DefaultValue="0" Name="UID" 
                        PropertyName="Text" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
            <div style="width: 100%; border: solid 0px red; border: solid 1px #dddddd;  ">       
                <asp:GridView ID="gvUserGroups" ShowHeader=false  runat="server" DataSourceID="odsUserGroups" AutoGenerateColumns="false" Width="100%">
                    <Columns>
                        <asp:TemplateField ShowHeader="false">
                        <ItemTemplate>
                            <b><%# Eval("GroupName") %></b>
                            <br />
                            <%# Eval("GroupDescription") %>
                        </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>       
        
        </td>
    </tr>
    
    <tr>
        <td align="right" valign="top"><b>User Folders:</b></td>
        <td align="left" valign="top" colspan="3">
        
            <asp:ObjectDataSource ID="odsUserFolders" runat="server" 
                SelectMethod="GetUserFolderAuditList" TypeName="STG.CMS.Portal.Core.CMSUser">
                <SelectParameters>
                    <asp:ControlParameter ControlID="lblUID" DefaultValue="0" Name="UID" 
                        PropertyName="Text" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
            <div style="width: 100%; border: solid 0px red; border: solid 1px #dddddd;  ">       
                <asp:GridView ID="gvFolders" ShowHeader=false  runat="server" DataSourceID="odsUserFolders" AutoGenerateColumns="false" Width="100%">
                    <Columns>
                        <asp:TemplateField ShowHeader="false" ItemStyle-Width="30%">
                            <ItemTemplate  >
                                <b><%# Eval("Folder") %></b>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <ItemTemplate>
                                <%# GetType(Eval("Type").ToString(), Eval("Name").ToString())   %></b>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>       
        
        </td>
    </tr>

    <tr>
        <td align="right" valign="top"><b>User Permissions:</b></td>
        <td align="left" valign="top" colspan="3">
        
            <asp:ObjectDataSource ID="odsUserPermissions" runat="server" 
                SelectMethod="GetUserPermissionsAuditList" TypeName="STG.CMS.Portal.Core.CMSUser">
                <SelectParameters>
                    <asp:ControlParameter ControlID="lblUID" DefaultValue="0" Name="UID" 
                        PropertyName="Text" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
            <div style="width: 100%; border: solid 0px red; border: solid 1px #dddddd;  ">       
                <asp:GridView ID="GridView1" ShowHeader=false  runat="server" DataSourceID="odsUserPermissions" AutoGenerateColumns="false" Width="100%">
                    <Columns>
                        <asp:TemplateField ShowHeader="false"  ItemStyle-Width="30%">
                            <ItemTemplate>
                                <b><%# Eval("PermissionName") %></b>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ShowHeader="false">
                            <ItemTemplate>
                                <%# GetType(Eval("Type").ToString(), Eval("Name").ToString())   %></b>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>       
        
        </td>
    </tr>    
    
    <tr>
        
        <td align="left" valign="top" colspan="4">
    
                &nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit User" Tooltip="Edit User" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" AlternateText="Refresh" Tooltip="Refresh" 
                        CausesValidation="False" CommandName="Refresh" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/refresh.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnHistory" runat="server" AlternateText="Login History" Tooltip="Login History" 
                        CausesValidation="False" CommandName="LoginHistory" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/history.png" Width="20px" />
                    &nbsp;&nbsp;
                    &nbsp;&nbsp;
                    <asp:ImageButton ID="btnList" runat="server" AlternateText="User List" Tooltip="User List" 
                        CausesValidation="False" CommandName="UserList" CommandArgument='<%# Eval("UID") %>' 
                        ImageUrl="~/ControlRoom/Images/user_list.png" Width="20px"/>
                   &nbsp;    
    
        </td>
    </tr>
</table>

                
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>    
    
</asp:Content>
