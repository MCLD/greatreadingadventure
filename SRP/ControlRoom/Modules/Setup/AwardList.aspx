<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="AwardList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.AwardList" %>

<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <asp:Panel runat="server" DefaultButton="SearchButton" class="row" Style="margin-bottom: 1em;" ID="SearchPanel">
            <div class="col-xs-4">
                <asp:TextBox ID="SearchText"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Search for an award"></asp:TextBox>
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
                <asp:ObjectDataSource ID="odsDDBranch" runat="server"
                    SelectMethod="GetAlByTypeName"
                    TypeName="GRA.SRP.DAL.Codes">
                    <SelectParameters>
                        <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
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


    <asp:ObjectDataSource ID="odsData"
        runat="server"
        SelectMethod="GetFiltered"
        TypeName="GRA.SRP.DAL.Award">
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


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="True" PageSize="20"
        DataKeys="AID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated"
        OnSorting="GvSorting"
        OnRowCommand="GvRowCommand">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" ToolTip="Edit Record"
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("AID") %>'
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnRun" runat="server" AlternateText="Apply Award" ToolTip="Apply Award"
                        CausesValidation="False" CommandName="ApplyAward" CommandArgument='<%# Bind("AID") %>'
                        ImageUrl="~/ControlRoom/Images/run.png" Width="20px"
                        OnClientClick="return confirm('Are you sure you want to apply this award?\r\nThis may take quite a while...');" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("AID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="AID" DataField="AID"
                SortExpression="AID" Visible="False" ItemStyle-Wrap="False"
                ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField ReadOnly="True" HeaderText="AwardName"
                DataField="AwardName" SortExpression="AwardName" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField SortExpression="BadgeName" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="Badge">
                <ItemTemplate>
                    <%# Eval("BadgeName")%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="NumPoints" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="NumPoints">
                <ItemTemplate>
                    <%# FormatHelper.ToInt((int)Eval("NumPoints"))%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="Program" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="Program">
                <ItemTemplate>
                    <%# Eval("Program")%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:TemplateField SortExpression="Branch" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="Branch">
                <ItemTemplate>
                    <%# Eval("Branch")%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>



            <asp:BoundField ReadOnly="True" HeaderText="District"
                DataField="DistrictName" SortExpression="DistrictName" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="SchoolName"
                DataField="SchName" SortExpression="SchName" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="Badge List">
                <ItemTemplate>
                    <%# (Eval("BadgeList").ToString().Length > 0 ? "Yes" : "No")%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

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
</asp:Content>

