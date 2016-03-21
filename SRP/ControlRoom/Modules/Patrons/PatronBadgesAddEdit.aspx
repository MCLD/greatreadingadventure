<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master"
    AutoEventWireup="true" CodeBehind="PatronBadgesAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronBadgesAddEdit" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/ControlRoom/Controls/PatronContext.ascx" TagName="PatronContext" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />

    <uc1:PatronContext ID="PatronContext1" runat="server" />

    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        OnItemCommand="DvItemCommand" OnDataBinding="dv_DataBinding"
        OnDataBound="dv_DataBound"
        Width="100%">
        <Fields>
            <asp:BoundField DataField="PBID" HeaderText="PBID: " SortExpression="PBID" ReadOnly="True" InsertVisible="False">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="Badge:" SortExpression="BadgeID" HeaderStyle-Wrap="False">
                <InsertItemTemplate>
                    <table width="100%">
                        <tr>
                            <td width="100%">
                                <asp:DropDownList ID="BadgeID" runat="server" DataSourceID="odsDDBadges" DataTextField="AdminName" DataValueField="BID"
                                    AppendDataBoundItems="True" CssClass="form-control">
                                    <asp:ListItem Value="0" Text="[Select a Value]"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <table style="width: 400px;" class="margin-1em-bottom">
                                    <tr>
                                        <td style="width: 400px;">
                                            <asp:TextBox ID="SearchText"
                                                runat="server"
                                                CssClass="form-control"
                                                placeholder="Search for a badge"></asp:TextBox>
                                        </td>
                                        <td rowspan="2">
                                            <asp:LinkButton runat="server"
                                                OnClick="Search"
                                                CssClass="btn btn-success margin-1em-left"
                                                ForeColor="White"
                                                ID="SearchButton">
                                <span class="glyphicon glyphicon-search"></span>
                                Filter badge list</asp:LinkButton>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="FilterBranchId"
                                                runat="server"
                                                DataSourceID="odsDDBranch"
                                                DataTextField="Code"
                                                DataValueField="CID"
                                                AppendDataBoundItems="True"
                                                CssClass="form-control">
                                                <asp:ListItem Value="0" Text="Filter by library"></asp:ListItem>
                                            </asp:DropDownList>

                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <asp:ObjectDataSource ID="odsDDBranch" runat="server"
                        SelectMethod="GetAlByTypeName"
                        TypeName="GRA.SRP.DAL.Codes">
                        <SelectParameters>
                            <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>


                    <asp:ObjectDataSource ID="odsDDBadges" runat="server"
                        SelectMethod="GetFiltered"
                        TypeName="GRA.SRP.DAL.Badge">
                        <SelectParameters>
                            <asp:ControlParameter
                                ControlID="SearchText"
                                Name="searchText"
                                PropertyName="Text"
                                Type="String" />
                            <asp:ControlParameter
                                ControlID="FilterBranchId"
                                DefaultValue="0"
                                Name="BranchId"
                                PropertyName="SelectedValue"
                                Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="BadgeID" runat="server" Text='<%# Eval("BadgeID") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Date Earned: " SortExpression="DateEarned" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <asp:TextBox ID="DateEarned" runat="server" Text='<%# Eval("DateEarned") %>' ReadOnly="False" CssClass="form-control" Width="10 em"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="ceDateEarned" runat="server" TargetControlID="DateEarned"></ajaxToolkit:CalendarExtender>
                    <ajaxToolkit:MaskedEditExtender ID="meDateEarned" runat="server"
                        UserDateFormat="MonthDayYear" TargetControlID="DateEarned" MaskType="Date" Mask="99/99/9999"></ajaxToolkit:MaskedEditExtender>
                    <asp:RequiredFieldValidator ID="rfvDateEarned" runat="server"
                        ControlToValidate="DateEarned" Display="Dynamic" ErrorMessage="Date Earned is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="DateEarned" runat="server" Text='' CssClass="form-control" Width="10 em"></asp:TextBox>
                    <ajaxToolkit:CalendarExtender ID="ceDateEarned" runat="server" TargetControlID="DateEarned"></ajaxToolkit:CalendarExtender>
                    <ajaxToolkit:MaskedEditExtender ID="meDateEarned" runat="server"
                        UserDateFormat="MonthDayYear" TargetControlID="DateEarned" MaskType="Date" Mask="99/99/9999"></ajaxToolkit:MaskedEditExtender>
                    <asp:RequiredFieldValidator ID="rfvDateEarned" runat="server"
                        ControlToValidate="DateEarned" Display="Dynamic" ErrorMessage="Date Earned is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="DateEarned" runat="server" Text='<%# Eval("DateEarned") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
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
        TypeName="GRA.SRP.DAL.PatronBadges">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="PBID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

