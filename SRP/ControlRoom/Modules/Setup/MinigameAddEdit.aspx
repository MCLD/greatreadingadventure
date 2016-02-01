<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="MinigameAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MinigameAddEdit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        OnItemCommand="DvItemCommand" OnDataBinding="dv_DataBinding"
        OnDataBound="dv_DataBound"
        Width="100%">
        <Fields>

            <asp:BoundField DataField="MGID" HeaderText="MGID: " SortExpression="MGID" ReadOnly="True" InsertVisible="False">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>


            <asp:TemplateField HeaderText="Adventure Type: " SortExpression="MiniGameType" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:DropDownList ID="MiniGameType" runat="server" Width="600px">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                        <asp:ListItem Value="7" Text="Choose Your Adventure"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Code Breaker"></asp:ListItem>
                        <%--<asp:ListItem Value="6" Text="Hidden Picture"></asp:ListItem>--%>
                        <%--<asp:ListItem Value="5" Text="Matching Game"></asp:ListItem>--%>
                        <asp:ListItem Value="2" Text="Mix-And-Match"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Online Book"></asp:ListItem>
                        <%--<asp:ListItem Value="4" Text="Word Match"></asp:ListItem>--%>
                    </asp:DropDownList>

                    <asp:RequiredFieldValidator ID="rfvMiniGameType" runat="server"
                        ControlToValidate="MiniGameType" Display="Dynamic" ErrorMessage="Adventure Type is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="MiniGameType" runat="server" Text='<%# Eval("MiniGameType") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Middle" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Admin Name: " SortExpression="AdminName" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <asp:TextBox ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>' ReadOnly="False"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAdminName" runat="server"
                        ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="AdminName is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="AdminName" runat="server" Text=''></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAdminName" runat="server"
                        ControlToValidate="AdminName" Display="Dynamic" ErrorMessage="AdminName is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="AdminName" runat="server" Text='<%# Eval("AdminName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Middle" Width="200px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Game Name: " SortExpression="GameName" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <asp:TextBox ID="GameName" runat="server" Text='<%# Eval("GameName") %>' ReadOnly="False"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGameName" runat="server"
                        ControlToValidate="GameName" Display="Dynamic" ErrorMessage="GameName is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="GameName" runat="server" Text=''></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvGameName" runat="server"
                        ControlToValidate="GameName" Display="Dynamic" ErrorMessage="GameName is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="GameName" runat="server" Text='<%# Eval("GameName") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Middle" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Is Active? " SortExpression="isActive" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <asp:CheckBox ID="isActive" runat="server" Checked='<%# (bool)Eval("isActive") %>' ReadOnly="False"></asp:CheckBox>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:CheckBox ID="isActive" runat="server" ReadOnly="False"></asp:CheckBox>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="isActive" runat="server" Text='<%# Eval("isActive") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Middle" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="# Points: " SortExpression="NumberPoints" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <asp:TextBox ID="NumberPoints" runat="server" Text='<%# ((int) Eval("NumberPoints") ==0 ? "" : Eval("NumberPoints")) %>'
                        ReadOnly="False" Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNumberPoints" runat="server"
                        ControlToValidate="NumberPoints" Display="Dynamic" ErrorMessage="# Points is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revNumberPoints"
                        ControlToValidate="NumberPoints"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'># Points must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * # Points must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvNumberPoints"
                        ControlToValidate="NumberPoints"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'># Points must be from 0 to 9999!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * # Points must be from 0 to 9999! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="NumberPoints" runat="server" Text='' Width="50px" CssClass="align-right"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvNumberPoints" runat="server"
                        ControlToValidate="NumberPoints" Display="Dynamic" ErrorMessage="# Points is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revNumberPoints"
                        ControlToValidate="NumberPoints"
                        ValidationExpression="\d+"
                        Display="Dynamic"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'># Points must be numeric.</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * # Points must be numeric. </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                    <asp:RangeValidator ID="rvNumberPoints"
                        ControlToValidate="NumberPoints"
                        MinimumValue="0"
                        MaximumValue="9999"
                        Display="Dynamic"
                        Type="Integer"
                        EnableClientScript="true"
                        ErrorMessage="<font color='red'># Points must be from 0 to 9999!</font>"
                        runat="server"
                        Font-Bold="True" Font-Italic="True"
                        Text="<font color='red'> * # Points must be from 0 to 9999! </font>"
                        EnableTheming="True"
                        SetFocusOnError="True" />
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="NumberPoints" runat="server" Text='<%# Eval("NumberPoints") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Middle" />
            </asp:TemplateField>


            <asp:TemplateField HeaderText="AwardedBadgeID: " SortExpression="AwardedBadgeID" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <asp:DropDownList ID="AwardedBadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                        AppendDataBoundItems="True" Width="600px">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="AwardedBadgeIDLbl" runat="server" Text='<%# Eval("AwardBadgeID") %>' Visible="False"></asp:Label>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:DropDownList ID="AwardedBadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                        AppendDataBoundItems="True" Width="600px">
                        <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                    </asp:DropDownList>
                </InsertItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Middle" />
            </asp:TemplateField>


            <asp:BoundField DataField="LastModDate" HeaderText=" Modified Date: " HeaderStyle-Wrap="False"
                SortExpression="LastModDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="LastModUser" HeaderText="Modified By: " HeaderStyle-Wrap="False"
                SortExpression="LastModUser" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="AddedDate" HeaderText="Added Date: " HeaderStyle-Wrap="False"
                SortExpression="AddedDate" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

            <asp:BoundField DataField="AddedUser" HeaderText="Added By: " HeaderStyle-Wrap="False"
                SortExpression="AddedUser" InsertVisible="False" ReadOnly="True">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:BoundField>

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
                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="FetchObject"
        TypeName="GRA.SRP.DAL.Minigame">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="MGID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBadges" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Badge"></asp:ObjectDataSource>

</asp:Content>

