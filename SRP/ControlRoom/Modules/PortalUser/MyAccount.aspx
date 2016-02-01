<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="MyAccount.aspx.cs" 
Inherits="GRA.SRP.ControlRoom.Modules.PortalUser.MyAccount" 
%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server" 
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px" 
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px" 
        HeaderText="There are errors, and no action was taken"  Font-Names="Verdana"  
        />
    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsCMSUser" 
        Height="50px"  AutoGenerateRows="False" 
        onitemcommand="DvItemCommand">
        <Fields>
            <asp:BoundField DataField="UID" HeaderText="Id:" SortExpression="UID" ReadOnly="True" 
                    InsertVisible="False" Visible="False" >
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>
            
            <asp:TemplateField HeaderText="User Name: " SortExpression="Username"  ItemStyle-Width="600px" AccessibleHeaderText="">
                <EditItemTemplate  >
                    <asp:Label ID="Username" runat="server" Text='<%# Bind("Username") %>'></asp:Label>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Username" runat="server" Text='<%# Bind("Username") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="First Name: " SortExpression="Firstname">
                <EditItemTemplate>
                    <asp:Textbox ID="Firstname" runat="server" Text='<%# Bind("Firstname") %>'></asp:Textbox>
                    <asp:RequiredFieldValidator ID="rfv1" runat="server" 
                        ControlToValidate="Firstname" Display="Dynamic" ErrorMessage="First name is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Textbox ID="Firstname" runat="server" Text='<%# Bind("Firstname") %>' ReadOnly="True"></asp:Textbox>
                    <asp:RequiredFieldValidator ID="rfv1" runat="server" 
                        ControlToValidate="Firstname" Display="Dynamic" ErrorMessage="First name is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Last Name: " SortExpression="Lastname">
                <EditItemTemplate>
                    <asp:Textbox ID="Lastname" runat="server" Text='<%# Bind("Lastname") %>'></asp:Textbox>
                    <asp:RequiredFieldValidator ID="rfv2" runat="server" 
                        ControlToValidate="Lastname" Display="Dynamic" ErrorMessage="Last name is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Textbox ID="Lastname" runat="server" Text='<%# Bind("Lastname") %>' ReadOnly="True"></asp:Textbox>
                    <asp:RequiredFieldValidator ID="rfv2" runat="server" 
                        ControlToValidate="Lastname" Display="Dynamic" ErrorMessage="Last name is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Email: " SortExpression="Emailaddress">
                <EditItemTemplate>
                    <asp:TextBox ID="Emailaddress" runat="server" Text='<%# Bind("Emailaddress") %>'></asp:Textbox>
                    <asp:RequiredFieldValidator ID="rfv3" runat="server" 
                        ControlToValidate="Emailaddress" Display="Dynamic" ErrorMessage="Email is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ControlToValidate="Emailaddress" Display="Dynamic" 
                        ErrorMessage="Email address is not valid." 
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"> * Invalid</asp:RegularExpressionValidator>                        
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Textbox ID="Emailaddress" runat="server" Text='<%# Bind("Emailaddress") %>' ReadOnly="True"></asp:Textbox>
                    <asp:RequiredFieldValidator ID="rfv3" runat="server" 
                        ControlToValidate="Emailaddress" Display="Dynamic" ErrorMessage="Email is required" 
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                        ControlToValidate="Emailaddress" Display="Dynamic" 
                        ErrorMessage="Email address is not valid." 
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"> * Invalid</asp:RegularExpressionValidator>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>


            <asp:TemplateField HeaderText="Title: " SortExpression="Title">
                <EditItemTemplate>
                    <asp:Textbox ID="Title" runat="server" Text='<%# Bind("Title") %>'></asp:Textbox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Textbox ID="Title" runat="server" Text='<%# Bind("Title") %>' ReadOnly="True"></asp:Textbox>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Division: " SortExpression="Division">
                <EditItemTemplate>
                    <asp:Textbox ID="Division" runat="server" Text='<%# Bind("Division") %>'></asp:Textbox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Textbox ID="Division" runat="server" Text='<%# Bind("Division") %>' ReadOnly="True"></asp:Textbox>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Department: " SortExpression="Department">
                <EditItemTemplate>
                    <asp:Textbox ID="Department" runat="server" Text='<%# Bind("Department") %>'></asp:Textbox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Textbox ID="Department" runat="server" Text='<%# Bind("Department") %>' ReadOnly="True"></asp:Textbox>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:TemplateField>
            
            <asp:BoundField DataField="LastPasswordReset" HeaderText="Last Password Reset: " 
                SortExpression="LastPasswordReset" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>

            <asp:BoundField DataField="LastModDate" HeaderText="Last Modified: " 
                SortExpression="Last Mod:" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>
            <asp:BoundField DataField="LastModUser" HeaderText="Last Modified By:" 
                SortExpression="Last Mod By:" ReadOnly="True" InsertVisible="False" >
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>
            <asp:BoundField DataField="AddedDate" HeaderText="Added: " 
                SortExpression="Added: " ReadOnly="True" InsertVisible="False" >
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>
            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " 
                SortExpression="Added By: " ReadOnly="True" InsertVisible="False" >
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />    
            </asp:BoundField>
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
                        &nbsp;
                        &nbsp;
                        &nbsp;
                        &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="ImageButton1" runat="server" 
                        CausesValidation="False" 
                        CommandName="Password" 
                        ImageUrl="~/ControlRoom/RibbonImages/UserSecurity.png" 
                        Height="25"
                        Tooltip="Change Password" 
                        AlternateText="Change Password" />                                              


                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <asp:Label ID="lblUID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsCMSUser" runat="server" 
        SelectMethod="Fetch" TypeName="GRA.SRP.Core.Utilities.SRPUser">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblUID" Name="pUid" PropertyName="Text" 
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
