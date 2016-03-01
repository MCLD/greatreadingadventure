<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="BadgeList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.BadgeList" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <asp:Panel runat="server" DefaultButton="SearchButton" class="row" style="margin-bottom: 1em;" ID="SearchPanel">
            <div class="col-xs-4">
                <asp:TextBox ID="SearchText"
                    runat="server"
                    CssClass="form-control"
                    placeholder="Search for a badge"></asp:TextBox>
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
        TypeName="GRA.SRP.DAL.Badge">
        <SelectParameters>
            <asp:ControlParameter
                ControlID="SearchText"
                Name="searchText"
                PropertyName="Text"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <p style="margin-bottom: 0.5em;"><b>Badge Gallery Direct URL: </b><a href='/Badges/Default.aspx?PID=<%=Programs.GetDefaultProgramID()%>' target="_blank"><%= Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/Badges/Default.aspx?PID=" + Programs.GetDefaultProgramID() %></a></p>

    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="BID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated"
        OnSorting="GvSorting"
        OnRowCommand="GvRowCommand">

        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    &nbsp;&nbsp;<asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" ToolTip="Edit Record"
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("BID") %>'
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />

                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("BID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Visible"
                SortExpression="HiddenFromPublic"
                ItemStyle-HorizontalAlign="Center"
                HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <span style="font-size: 1.5em;" class='glyphicon <%# Eval("HiddenFromPublic") as bool? != true 
                            ? "glyphicon-ok-circle text-success"
                            : "glyphicon-ban-circle text-danger" %>'></span>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField ReadOnly="True" HeaderText="Badge Id" DataField="BID"
                SortExpression="BID" Visible="False" ItemStyle-Wrap="False"
                ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Control Room Name"
                DataField="AdminName" SortExpression="AdminName" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="250px"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Public Name"
                DataField="UserName" SortExpression="UserName" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="250px"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <a class="fancybox" rel="group" href="<%# VirtualPathUtility.ToAbsolute(String.Format("~/Images/Badges/{0}.png?{1}", Eval("BID").ToString(), DateTime.Now.Ticks)) %>">
                        <asp:Image ID="Image1" runat="server"
                            ImageUrl='<%# String.Format("~/Images/Badges/sm_{0}.png?{1}", Eval("BID").ToString(), DateTime.Now.Ticks) %>'
                            Width="32" Height="32" /></a>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False" Width="300px" HorizontalAlign="Center"></ItemStyle>
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

    <script type="text/javascript">
        $(document).ready(function () {
            $(".fancybox").fancybox({
                openEffect: 'elastic',
                closeEffect: 'elastic'
            });
        });
    </script>
</asp:Content>

