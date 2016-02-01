<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" 
	CodeBehind="GroupsAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Security.GroupsAddEdit" 
%>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="cpAddEdit" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsSRPGroups"  Width="100%"
        onitemcommand="DvItemCommand" >
        <Fields>

        <asp:BoundField DataField="GID" HeaderText="Group Id: " SortExpression="GID" ReadOnly="True" InsertVisible="False">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
        </asp:BoundField>

        <asp:TemplateField HeaderText="Group Name: " SortExpression="GroupName">
		<EditItemTemplate>
                    <asp:TextBox ID="GroupName" runat="server" Text='<%# Bind("GroupName") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGroupName" runat="server" 
                        ControlToValidate="GroupName" Display="Dynamic" ErrorMessage="Group Name is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="GroupName" runat="server" Text='<%# Bind("GroupName") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGroupName" runat="server" 
                        ControlToValidate="GroupName" Display="Dynamic" ErrorMessage="Group Name is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="GroupName" runat="server" Text='<%# Bind("GroupName") %>'></asp:Label>
                </ItemTemplate>
            	<HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" width="100px"/>    
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Description: " SortExpression="GroupDescription">
		<EditItemTemplate>
                    <asp:TextBox ID="GroupDescription" runat="server" Text='<%# Bind("GroupDescription") %>' MaxLength="255" Rows="4" 
                        TextMode="MultiLine" Width="350px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGroupDescription" runat="server" 
                        ControlToValidate="GroupDescription" Display="Dynamic" ErrorMessage="Description is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="GroupDescription" runat="server" Text='<%# Bind("GroupDescription") %>' MaxLength="255" Rows="4" 
                        TextMode="MultiLine" Width="350px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGroupDescription" runat="server" 
                        ControlToValidate="GroupDescription" Display="Dynamic" ErrorMessage="Description is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="GroupDescription" runat="server" Text='<%# Bind("GroupDescription") %>'></asp:Label>
                </ItemTemplate>
            	<HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
        </asp:TemplateField>

            <asp:BoundField DataField="LastModDate" HeaderText="Modified On: " 
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " 
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added On: " 
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " 
                SortExpression="AddedUser" InsertVisible="False" ReadOnly="True">
            <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>


            <asp:TemplateField InsertVisible="False" ShowHeader="False">
                <EditItemTemplate>
            
<table style="width: 100%" cellpadding=3px;>
    <tr>
        <td align="center" width="400px"> <b>Users</b> </td>    
        <td align="center" width="400px"> <b>Permissions</b> </td>    
    </tr>    
    <tr>
        <td valign="top">
            <asp:ObjectDataSource ID="odsGroupUsers" runat="server" 
                SelectMethod="GetUserList" TypeName="SRP_DAL.SRPGroup">
                <SelectParameters>
                    <asp:ControlParameter ControlID="lblGID" DefaultValue="1000" Name="GID" 
                        PropertyName="Text" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
<div style="height: 300px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd;  ">       
            <asp:GridView ID="gvGroupUsers" ShowHeader=false  runat="server" DataSourceID="odsGroupUsers" AutoGenerateColumns="false" Width="100%">
                <Columns>
                    <asp:TemplateField ShowHeader="false">
                    <ItemTemplate>
                        <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("isMember")).ToString()=="1"?true:false) %>' runat="server" />   <%# Eval("Username") %> (<%# Eval("FirstName") %><%# Eval("LastName") %>)
                        <asp:Label ID="UID" runat="server" Text='<%# Eval("UID") %>' Visible="False"></asp:Label>
                    </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
</div>        
        </td>    
        <td valign="top">
            <asp:ObjectDataSource ID="odsGroupPermissions" runat="server" 
                SelectMethod="GetPermissionList" TypeName="SRP_DAL.SRPGroup">
                <SelectParameters>
                    <asp:ControlParameter ControlID="lblGID" DefaultValue="1000" Name="GID" 
                        PropertyName="Text" Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
        
<div style="height: 300px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd; ">
       
            <asp:GridView ID="gvGroupPermissions" ShowHeader=false  runat="server" DataSourceID="odsGroupPermissions" AutoGenerateColumns="false" Width="100%">
                <Columns>
                    <asp:TemplateField ShowHeader="false">
                    <ItemTemplate>
                        <asp:CheckBox ID="isChecked" Checked='<%# (((int)Eval("Checked")).ToString()=="1"?true:false) %>' runat="server" />   <%# Eval("PermissionName") %>
                        <asp:Label ID="PermissionID" runat="server" Text='<%# Eval("PermissionID") %>' Visible="False"></asp:Label>
                    </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>       
</div>             
        </td>    
  
    </tr>    
</table>
                       </EditItemTemplate>
     
            </asp:TemplateField>


            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Tooltip="Back/Cancel"  
                        AlternateText="Back/Cancel" />
                </ItemTemplate>
                <InsertItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Tooltip="Back/Cancel"  
                        AlternateText="Back/Cancel" />
                        &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" 
                        CausesValidation="True" 
                        CommandName="Add" 
                        ImageUrl="~/ControlRoom/Images/add.png" 
                        Height="25"
                        Tooltip="Add" 
                        AlternateText="Add" />                        
                        &nbsp;
                    <asp:ImageButton ID="btnAddback" runat="server" 
                        CausesValidation="True" 
                        CommandName="Addandback" 
                        ImageUrl="~/ControlRoom/Images/saveback.png" 
                        Height="25"
                        Tooltip="Add and return" 
                        AlternateText="Add and return" />                        
                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server" 
                        CausesValidation="false" 
                        CommandName="Back" 
                        ImageUrl="~/ControlRoom/Images/back.png" 
                        Height="25"
                        Tooltip="Back/Cancel" 
                        AlternateText="Back/Cancel" />
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server" 
                        CausesValidation="false" 
                        CommandName="Refresh" 
                        ImageUrl="~/ControlRoom/Images/refresh.png" 
                        Height="25"
                        Tooltip="Refresh Record"  
                        AlternateText="Refresh Record" />   
                        &nbsp;
                    <asp:ImageButton ID="btnSave" runat="server" 
                        CausesValidation="True" 
                        CommandName="Save" 
                        ImageUrl="~/ControlRoom/Images/save.png" 
                        Height="25"
                        Tooltip="Save" 
                        AlternateText="Save"/>     
                        &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server" 
                        CausesValidation="True" 
                        CommandName="Saveandback" 
                        ImageUrl="~/ControlRoom/Images/saveback.png" 
                        Height="25"
                        Tooltip="Save and return" 
                        AlternateText="Save and return" />                                              
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>


    <asp:Label ID="lblGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsSRPGroups" runat="server" 
        SelectMethod="Fetch" TypeName="SRP_DAL.SRPGroup">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblGID" DefaultValue="" Name="gid" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>
