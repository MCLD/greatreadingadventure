<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="BookListList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.BookListList" %>

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
                    placeholder="Search for a challenge"></asp:TextBox>
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
        TypeName="GRA.SRP.DAL.BookList">
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


    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="BLID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated"
        OnSorting="GvSorting"
        OnRowCommand="GvRowCommand"
        OnRowDataBound="GvRowDataBound"
        Width="100%">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False">
                <HeaderTemplate>
                    &nbsp;&nbsp;<asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>
                <ItemTemplate>
                    &nbsp;
                       <asp:LinkButton ID="ImageButton2" runat="server"
                           CausesValidation="True"
                           CommandName="Saveandbooks"
                           Text="Tasks" ToolTip="Tasks" CommandArgument='<%# Bind("BLID") %>'
                           AlternateText="Tasks"><span class="glyphicon glyphicon-list margin-halfem-bottom" style="font-size: large;"></span></asp:LinkButton>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" ToolTip="Edit Record"
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("BLID") %>'
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("BLID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />

                    &nbsp;
                   <asp:HyperLink runat="server" 
                       NavigateUrl='<%# Eval("BLID","~/Challenges/Details.aspx?ChallengeId={0}") %>'
                       AlternateText="Direct link to this challenge"
                       ToolTip="Direct link to this challenge"
                       Target="_blank"><span class="glyphicon glyphicon-new-window margin-halfem-bottom" style="font-size: large;"></asp:HyperLink>
                </ItemTemplate>
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="BLID" DataField="BLID"
                SortExpression="BLID" Visible="False" ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Admin Name"
                DataField="AdminName" SortExpression="AdminName" Visible="True"
                ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Challenge Name"
                DataField="ListName" SortExpression="ListName" Visible="True"
                ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Points"
                DataField="AwardPoints" SortExpression="AwardPoints" Visible="True"
                ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField SortExpression="NumBooksToComplete" Visible="True"
                ItemStyle-Wrap="False"
                HeaderText="Tasks">
                <ItemTemplate>
                    <%# Eval("NumBooksToComplete") %>/<%# Eval("TotalTasks") %>
                    <%# Eval("TotalTasks") as int? == 0 ? "<span class=\"glyphicon glyphicon-exclamation-sign\" data-toggle=\"tooltip\" title=\"This challenge needs tasks or it will not be visible!\"></span>" : string.Empty %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="ProgName" Visible="True"
                ItemStyle-Wrap="False"
                HeaderText="Program">
                <ItemTemplate>
                    <%# string.IsNullOrWhiteSpace(Eval("ProgName").ToString()) ? "All programs" : Eval("ProgName") %>
                </ItemTemplate>
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="Library" Visible="True"
                ItemStyle-Wrap="False"
                HeaderText="Library">
                <ItemTemplate>
                    <%# Eval("Library")%>
                </ItemTemplate>
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="Modified On"
                DataField="LastModDate" SortExpression="LastModDate" Visible="False"
                ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Modified By"
                DataField="LastModUser" SortExpression="LastModUser" Visible="False"
                ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Added On"
                DataField="AddedDate" SortExpression="AddedDate" Visible="False"
                ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Added By"
                DataField="AddedUser" SortExpression="AddedUser" Visible="False"
                ItemStyle-Wrap="False">
                <ItemStyle Wrap="False"></ItemStyle>
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