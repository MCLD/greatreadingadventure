<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="MyAccount.aspx.cs"
    Inherits="GRA.SRP.ControlRoom.Modules.PortalUser.MyAccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row">
            <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
                BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
                CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
                HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />
            <asp:DetailsView ID="dv" runat="server" DataSourceID="odsCMSUser"
                AutoGenerateRows="False" CssClass="form-inline"
                OnItemCommand="DvItemCommand" Width="100%">
                <Fields>
                    <asp:BoundField DataField="UID" HeaderText="Id:" SortExpression="UID" ReadOnly="True"
                        InsertVisible="False" Visible="False">
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>

                    <asp:TemplateField HeaderText="User Name: " SortExpression="Username" HeaderStyle-Width="15%" HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" AccessibleHeaderText="">
                        <EditItemTemplate>
                            <asp:Label ID="Username" runat="server" Text='<%# Bind("Username") %>' CssClass="form-control"></asp:Label>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Username" runat="server" Text='<%# Bind("Username") %>' CssClass="form-control"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="First Name: " HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" SortExpression="Firstname">
                        <EditItemTemplate>
                            <asp:TextBox ID="Firstname" runat="server" Text='<%# Bind("Firstname") %>' CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv1" runat="server"
                                ControlToValidate="Firstname" Display="Dynamic" ErrorMessage="First name is required"
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="Firstname" runat="server" Text='<%# Bind("Firstname") %>' ReadOnly="True" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv1" runat="server"
                                ControlToValidate="Firstname" Display="Dynamic" ErrorMessage="First name is required"
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </ItemTemplate>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Last Name: " HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" SortExpression="Lastname">
                        <EditItemTemplate>
                            <asp:TextBox ID="Lastname" runat="server" Text='<%# Bind("Lastname") %>' CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv2" runat="server"
                                ControlToValidate="Lastname" Display="Dynamic" ErrorMessage="Last name is required"
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="Lastname" runat="server" Text='<%# Bind("Lastname") %>' ReadOnly="True" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv2" runat="server"
                                ControlToValidate="Lastname" Display="Dynamic" ErrorMessage="Last name is required"
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>
                        </ItemTemplate>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Email: " HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" SortExpression="Emailaddress">
                        <EditItemTemplate>
                            <asp:TextBox ID="Emailaddress" runat="server" Text='<%# Bind("Emailaddress") %>' CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfv3" runat="server"
                                ControlToValidate="Emailaddress" Display="Dynamic" ErrorMessage="Email is required"
                                SetFocusOnError="True" Font-Bold="True">* Required</asp:RequiredFieldValidator>

                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                ControlToValidate="Emailaddress" Display="Dynamic"
                                ErrorMessage="Email address is not valid."
                                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"> * Invalid</asp:RegularExpressionValidator>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="Emailaddress" runat="server" Text='<%# Bind("Emailaddress") %>' ReadOnly="True" CssClass="form-control"></asp:TextBox>
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


                    <asp:TemplateField HeaderText="Title: " HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" SortExpression="Title">
                        <EditItemTemplate>
                            <asp:TextBox ID="Title" runat="server" Text='<%# Bind("Title") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="Title" runat="server" Text='<%# Bind("Title") %>' ReadOnly="True" CssClass="form-control"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Division: " HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" SortExpression="Division">
                        <EditItemTemplate>
                            <asp:TextBox ID="Division" runat="server" Text='<%# Bind("Division") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="Division" runat="server" Text='<%# Bind("Division") %>' ReadOnly="True" CssClass="form-control"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Department: " HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" SortExpression="Department">
                        <EditItemTemplate>
                            <asp:TextBox ID="Department" runat="server" Text='<%# Bind("Department") %>' CssClass="form-control"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="Department" runat="server" Text='<%# Bind("Department") %>' ReadOnly="True" CssClass="form-control"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Mail signature: " HeaderStyle-CssClass="control-label" ControlStyle-Width="99%" SortExpression="MailSignature">
                        <EditItemTemplate>
                            <asp:TextBox ID="MailSignature" runat="server" Text='<%# Bind("MailSignature") %>'  CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </EditItemTemplate>
                        <ItemTemplate>
                            <asp:TextBox ID="MailSignature" runat="server" Text='<%# Bind("MailSignature") %>' ReadOnly="True" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:TemplateField>


                    <asp:BoundField DataField="LastPasswordReset" HeaderText="Last Password Reset: "
                        SortExpression="LastPasswordReset" InsertVisible="False" ReadOnly="True" HeaderStyle-Height="3em" HeaderStyle-CssClass="control-label">
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>

                    <asp:BoundField DataField="LastModDate" HeaderStyle-CssClass="control-label" HeaderText="Last Modified: "
                        SortExpression="Last Mod:" InsertVisible="False" ReadOnly="True" HeaderStyle-Height="2em">
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="LastModUser" HeaderStyle-CssClass="control-label" HeaderText="Last Modified By:"
                        SortExpression="Last Mod By:" ReadOnly="True" InsertVisible="False" HeaderStyle-Height="2em">
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="AddedDate" HeaderStyle-CssClass="control-label" HeaderText="Added: "
                        SortExpression="Added: " ReadOnly="True" InsertVisible="False" HeaderStyle-Height="2em">
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:BoundField DataField="AddedUser" HeaderStyle-CssClass="control-label" HeaderText="Added By: "
                        SortExpression="Added By: " ReadOnly="True" InsertVisible="False" HeaderStyle-Height="2em">
                        <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                    </asp:BoundField>
                    <asp:TemplateField ShowHeader="False">
                        <ItemTemplate>
                            <asp:ImageButton ID="btnBack" runat="server"
                                CausesValidation="false"
                                CommandName="Back"
                                ImageUrl="~/ControlRoom/Images/back.png"
                                Height="25"
                                ToolTip="Back/Cancel"
                                AlternateText="Back/Cancel" />
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:ImageButton ID="btnBack" runat="server"
                                CausesValidation="false"
                                CommandName="Back"
                                ImageUrl="~/ControlRoom/Images/back.png"
                                Height="25"
                                ToolTip="Back/Cancel"
                                AlternateText="Back/Cancel" />
                            &nbsp;
                        &nbsp;
                    <asp:ImageButton ID="btnRefresh" runat="server"
                        CausesValidation="false"
                        CommandName="Refresh"
                        ImageUrl="~/ControlRoom/Images/refresh.png"
                        Height="25"
                        ToolTip="Refresh Record"
                        AlternateText="Refresh Record" />
                            &nbsp;
                    <asp:ImageButton ID="btnSave" runat="server"
                        CausesValidation="True"
                        CommandName="Save"
                        ImageUrl="~/ControlRoom/Images/save.png"
                        Height="25"
                        ToolTip="Save"
                        AlternateText="Save" />
                            &nbsp;
                    <asp:ImageButton ID="btnSaveback" runat="server"
                        CausesValidation="True"
                        CommandName="Saveandback"
                        ImageUrl="~/ControlRoom/Images/saveback.png"
                        Height="25"
                        ToolTip="Save and return"
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
                        ToolTip="Change Password"
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
        </div>
    </div>
</asp:Content>
