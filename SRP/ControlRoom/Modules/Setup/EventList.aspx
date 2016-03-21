<%@ Page Language="C#" MasterPageFile="~/ControlRoom/AJAX.Master"
    AutoEventWireup="true" CodeBehind="EventList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.EventList" %>

<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <asp:Panel runat="server" DefaultButton="SearchButton" class="row" Style="margin-bottom: 1em;" ID="SearchPanel">
            <div class="col-xs-4">
                <asp:TextBox ID="SearchText"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Search for an event"></asp:TextBox>
            </div>
            <div class="col-xs-4">
                <asp:DropDownList ID="BranchId"
                    runat="server"
                    DataSourceID="odsDDBranch"
                    DataTextField="Code"
                    DataValueField="CID"
                    AppendDataBoundItems="True"
                    CssClass="form-control">
                    <asp:ListItem Value="0" Text="Filter by library"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-xs-3">
                <asp:LinkButton runat="server"
                    OnClick="Search"
                    CssClass="btn btn-success"
                    ForeColor="White"
                    ID="SearchButton">
                    <span class="glyphicon glyphicon-search"></span>
                    Search</asp:LinkButton>
                <asp:LinkButton runat="server"
                    OnClick="ClearSearch"
                    CssClass="btn btn-warning"
                    ForeColor="White">
                    <span class="glyphicon glyphicon-erase"></span>
                    Clear Search</asp:LinkButton>
            </div>
        </asp:Panel>
    </div>
    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="true" PageSize="250" Width="100%"
        DataKeys="EID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated"
        OnSorting="GvSorting"
        OnRowCommand="GvRowCommand">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    &nbsp;&nbsp;<asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" ToolTip="Edit Record"
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("EID") %>'
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("EID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    &nbsp;
                    <asp:HyperLink runat="server"
                        NavigateUrl='<%# Eval("EID","~/Events/Details.aspx?EventId={0}") %>'
                        AlternateText="Direct link to this event"
                        ToolTip="Direct link to this event"
                        Visible='<%# Eval("HiddenFromPublic") as bool? != true %>'
                        Target="_blank"><span class="glyphicon glyphicon-new-window margin-halfem-bottom" style="font-size: large;"></asp:HyperLink>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:TemplateField HeaderText="Visible"
                SortExpression="HiddenFromPublic">
                <ItemTemplate>
                    <span style="font-size: 1.5em;" class='glyphicon <%# Eval("HiddenFromPublic") as bool? != true 
                            ? "glyphicon-ok-circle text-success"
                            : "glyphicon-ban-circle text-danger" %>'></span>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Link">
                <ItemTemplate>
                    <asp:HyperLink runat="server" Visible='<%# Eval("ExternalLinkToEvent").ToString().Length > 0 ? true : false %>'
                        NavigateUrl='<%# Eval("ExternalLinkToEvent")%>'
                        Target="_blank"><span class="glyphicon glyphicon-new-window"></span></asp:HyperLink>
                    &nbsp;
                </ItemTemplate>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="EID" DataField="EID"
                SortExpression="EID" Visible="False" ItemStyle-Wrap="False"
                ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Event"
                DataField="EventTitle" SortExpression="EventTitle" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="True"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Date"
                DataField="EventDate" SortExpression="EventDate" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="True"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Secret Code"
                DataField="SecretCode" SortExpression="SecretCode" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle Width="150px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Points Awarded"
                DataField="NumberPoints" SortExpression="NumberPoints" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField ReadOnly="True" HeaderText="Branch/Library"
                DataField="Branch" SortExpression="Branch" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField ReadOnly="True" HeaderText="Modified On"
                DataField="LastModDate" SortExpression="LastModDate" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Modified By"
                DataField="LastModUser" SortExpression="LastModUser" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Added On"
                DataField="AddedDate" SortExpression="AddedDate" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Added By"
                DataField="AddedUser" SortExpression="AddedUser" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight: bold;">
                No records were found. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>

    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="GetFiltered"
        TypeName="GRA.SRP.DAL.Event">
        <SelectParameters>
            <asp:ControlParameter
                ControlID="SearchText"
                Name="searchText"
                PropertyName="Text"
                Type="String" />
            <asp:ControlParameter
                ControlID="BranchId"
                DefaultValue="0"
                Name="branchId"
                PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDBranch" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>

