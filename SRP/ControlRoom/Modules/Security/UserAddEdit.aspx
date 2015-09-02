<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="UserAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Security.UserAddEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsSRPUser" Width="100%"
        OnItemCommand="DvItemCommand">
        <Fields>

            <asp:BoundField DataField="UID" HeaderText="User Id: " SortExpression="UID" ReadOnly="True" InsertVisible="False">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="Username: " SortExpression="Username">
                <EditItemTemplate>
                    <asp:TextBox ID="Username" runat="server" Text='<%# Bind("Username") %>' ReadOnly="True"></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="Username" runat="server" Text='<%# Bind("Username") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                        ControlToValidate="Username" Display="Dynamic" ErrorMessage="Username is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>

                    <asp:RegularExpressionValidator ID="revStrength1" runat="server"
                        ControlToValidate="Username" Display="Dynamic" ErrorMessage="Username must be at least 7 characters long and have no spaces"
                        ValidationExpression="^(?=.*[A-Za-z0-9])(?!.*[^A-Za-z0-9])(?!.*\s).{8,50}" SetFocusOnError="False" Font-Bold="True"> * Strength
                    </asp:RegularExpressionValidator>

                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Username1" runat="server" Text='<%# Bind("Username") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" Width="150px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Password: " SortExpression="Password">
                <EditItemTemplate>
                    <a href="ChangePassword.aspx">Change this user's password</a>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                        ControlToValidate="Password" Display="Dynamic" ErrorMessage="Password is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revStrength" runat="server"
                        ControlToValidate="Password" Display="Dynamic" ErrorMessage="Password must be at least 7 characters long, and contain one alpha and one numeric character."
                        ValidationExpression="(?=^.{7,}$)(?=.*\d)(?=.*[a-zA-Z]+)(?![.\n]).*$" SetFocusOnError="False" Font-Bold="True"> * Strength
                    </asp:RegularExpressionValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="First Name: " SortExpression="FirstName">
                <EditItemTemplate>
                    <asp:TextBox ID="FirstName" runat="server" Text='<%# Bind("FirstName") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                        ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First Name is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="FirstName" runat="server" Text='<%# Bind("FirstName") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server"
                        ControlToValidate="FirstName" Display="Dynamic" ErrorMessage="First Name is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="FirstName" runat="server" Text='<%# Bind("FirstName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Last Name: " SortExpression="LastName">
                <EditItemTemplate>
                    <asp:TextBox ID="LastName" runat="server" Text='<%# Bind("LastName") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
                        ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last Name is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="LastName" runat="server" Text='<%# Bind("LastName") %>'></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server"
                        ControlToValidate="LastName" Display="Dynamic" ErrorMessage="Last Name is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LastName" runat="server" Text='<%# Bind("LastName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Email Address: " SortExpression="EmailAddress">
                <EditItemTemplate>
                    <asp:TextBox ID="EmailAddress" runat="server" Text='<%# Bind("EmailAddress") %>' MaxLength="128" Rows="4"
                        TextMode="SingleLine" Width="250px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server"
                        ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email Address is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                        ControlToValidate="Emailaddress" Display="Dynamic"
                        ErrorMessage="Email address is not valid."
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"> * Invalid</asp:RegularExpressionValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="EmailAddress" runat="server" Text='<%# Bind("EmailAddress") %>' MaxLength="128" Rows="4"
                        TextMode="SingleLine" Width="250px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server"
                        ControlToValidate="EmailAddress" Display="Dynamic" ErrorMessage="Email Address is required"
                        SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                        ControlToValidate="Emailaddress" Display="Dynamic"
                        ErrorMessage="Email address is not valid."
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"> * Invalid</asp:RegularExpressionValidator>

                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="EmailAddress" runat="server" Text='<%# Bind("EmailAddress") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Division: " SortExpression="Division">
                <EditItemTemplate>
                    <asp:TextBox ID="Division" runat="server" Text='<%# Bind("Division") %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="Division" runat="server" Text='<%# Bind("Division") %>'></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Division" runat="server" Text='<%# Bind("Division") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Department: " SortExpression="Department">
                <EditItemTemplate>
                    <asp:TextBox ID="Department" runat="server" Text='<%# Bind("Department") %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="Department" runat="server" Text='<%# Bind("Department") %>'></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Department" runat="server" Text='<%# Bind("Department") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Title: " SortExpression="Title">
                <EditItemTemplate>
                    <asp:TextBox ID="Title" runat="server" Text='<%# Bind("Title") %>'></asp:TextBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="Title" runat="server" Text='<%# Bind("Title") %>'></asp:TextBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Title" runat="server" Text='<%# Bind("Title") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Is Active: " SortExpression="IsActive">
                <EditItemTemplate>
                    <asp:CheckBox ID="IsActive" runat="server" Checked='<%# (bool)Eval("IsActive") %>'></asp:CheckBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="IsActive" runat="server" Checked='True'></asp:CheckBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="IsActive" runat="server" Text='<%# Bind("IsActive") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Must Reset Password: " SortExpression="MustResetPassword">
                <EditItemTemplate>
                    <asp:CheckBox ID="MustResetPassword" runat="server" Checked='<%# (bool)Eval("MustResetPassword") %>'></asp:CheckBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="MustResetPassword" runat="server" Checked='True'></asp:CheckBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="MustResetPassword" runat="server" Text='<%# Bind("MustResetPassword") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:BoundField DataField="IsDeleted" HeaderText="Is Deleted: "
                SortExpression="IsDeleted" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="Last Password Reset: " SortExpression="LastPasswordReset">
                <EditItemTemplate>
                    <asp:Label ID="LastPasswordReset" runat="server" Text='<%# Bind("LastPasswordReset") %>'></asp:Label>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:Label ID="LastPasswordReset" runat="server" Text='<%# Bind("LastPasswordReset") %>'></asp:Label>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LastPasswordReset" runat="server" Text='<%# Bind("LastPasswordReset") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:BoundField DataField="TenID" HeaderText="Tenant: " Visible="False"
                SortExpression="TenID" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="DeletedDate" HeaderText="Deleted Date: "
                SortExpression="DeletedDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: "
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: "
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: "
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: "
                SortExpression="AddedUser" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>


            <asp:TemplateField InsertVisible="False" ShowHeader="False">
                <EditItemTemplate>

                    <table style="width: 100%" cellpadding="3px;">
                        <tr>
                            <td align="center" width="300px"><b>User Groups </b></td>
                        </tr>
                        <tr>
                            <td valign="top">
                                <asp:ObjectDataSource ID="odsUserGroups" runat="server"
                                    SelectMethod="GetGroupList" TypeName="GRA.SRP.Core.Utilities.SRPUser">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="lblUID" DefaultValue="0" Name="UID"
                                            PropertyName="Text" Type="Int32" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>

                                <div style="height: 150px; width: 100%; overflow: auto; border: solid 0px red; border: solid 1px #dddddd;">

                                    <asp:GridView ID="gvUserGroups" ShowHeader="false" runat="server" DataSourceID="odsUserGroups" AutoGenerateColumns="false" Width="100%">
                                        <Columns>
                                            <asp:TemplateField ShowHeader="false">
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="isMember" Checked='<%# (((int)Eval("Checked")).ToString()=="1"?true:false) %>' runat="server" />
                                                    <%# Eval("GroupName")%>
                                                    <asp:Label ID="GID" runat="server" Text='<%# Eval("GID") %>' Visible="False"></asp:Label>
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
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </ItemTemplate>
                <InsertItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server"
                        CausesValidation="false"
                        CommandName="Back"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                    &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server"
                        CausesValidation="True"
                        CommandName="Add"
                        ImageUrl="~/ControlRoom/Images/add.png"
                        Height="25"
                        Text="Add" ToolTip="Add"
                        AlternateText="Add" />
                    &nbsp;
                    <asp:ImageButton ID="btnAddback" runat="server"
                        CausesValidation="True"
                        CommandName="Addandback"
                        ImageUrl="~/ControlRoom/Images/saveback.png"
                        Height="25"
                        Text="Add and return" ToolTip="Add and return"
                        AlternateText="Add and return" />
                </InsertItemTemplate>
                <EditItemTemplate>
                    <asp:ImageButton ID="btnBack" runat="server"
                        CausesValidation="false"
                        CommandName="Back"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="25"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                    &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server"
                        CausesValidation="false"
                        CommandName="Refresh"
                        ImageUrl="~/ControlRoom/Images/refresh.png"
                        Height="25"
                        Text="Refresh Record" ToolTip="Refresh Record"
                        AlternateText="Refresh Record" />
                    &nbsp;
                    <asp:ImageButton ID="btnSave" runat="server"
                        CausesValidation="True"
                        CommandName="Save"
                        ImageUrl="~/ControlRoom/Images/save.png"
                        Height="25"
                        Text="Save" ToolTip="Save"
                        AlternateText="Save" />
                    &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server"
                        CausesValidation="True"
                        CommandName="Saveandback"
                        ImageUrl="~/ControlRoom/Images/saveback.png"
                        Height="25"
                        Text="Save and return" ToolTip="Save and return"
                        AlternateText="Save and return" />
                    &nbsp;&nbsp;
                    &nbsp;&nbsp;
                    <asp:ImageButton ID="ImageButton1" runat="server"
                        CausesValidation="false"
                        CommandName="LoginHistory"
                        ImageUrl="~/ControlRoom/Images/history.png"
                        Height="25"
                        Text="Login History" ToolTip="Login History"
                        AlternateText="LoginHistory" />
                    &nbsp;
                    <asp:ImageButton ID="ImageButton2" runat="server"
                        CausesValidation="false"
                        CommandName="AuditUser"
                        ImageUrl="~/ControlRoom/Images/audit.png"
                        Height="25"
                        Text="Audit User" ToolTip="Audit User"
                        AlternateText="Audit User"
                        Visible="False" />

                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>


    <asp:Label ID="lblUID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsSRPUser" runat="server"
        SelectMethod="Fetch" TypeName="GRA.SRP.Core.Utilities.SRPUser">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblUID" Name="pUid"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>