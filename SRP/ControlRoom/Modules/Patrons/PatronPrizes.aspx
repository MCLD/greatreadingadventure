<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="PatronPrizes.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronPrizes" %>

<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register Src="../../Controls/PatronContext.ascx" TagName="PatronContext" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:PatronContext ID="pcCtl" runat="server" />


    <asp:GridView ID="gv" runat="server" AllowSorting="False" AutoGenerateColumns="False" AllowPaging="False"
        OnRowCommand="GvRowCommand"
        Width="100%">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>
                <ItemTemplate>
                    <!--&nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("PPID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    -->
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("PPID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="PPID" DataField="PPID"
                SortExpression="PPID" Visible="False" ItemStyle-Wrap="False"
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField SortExpression="PrizeSource" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="PrizeSource">
                <ItemTemplate>
                    <%# (int)Eval("PrizeSource") == 0 ? "Drawing" : (int)Eval("PrizeSource") == 1 ?"Badge" : "Admin"%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="Badge" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="Badge">
                <ItemTemplate>
                    <%#Eval("Badge")%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField SortExpression="DrawingID" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="DrawingID">
                <ItemTemplate>
                    <%# FormatHelper.ToInt((int)Eval("DrawingID"))%>
                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:BoundField ReadOnly="True" HeaderText="PrizeName"
                DataField="PrizeName" SortExpression="PrizeName" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField SortExpression="RedeemedFlag" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="RedeemedFlag">
                <ItemTemplate>
                    <%# (bool)Eval("RedeemedFlag") ? string.Format("<strong>Redeemed</strong> at {0} (by {1})", Eval("LastModDate"), Eval("LastModUser")) : string.Empty%>

                    <strong>
                        <asp:LinkButton ID="LinkButton1" runat="server"
                            CommandName="pickup" CommandArgument='<%# Bind("PPID") %>'
                            Visible='<%#!(bool)Eval("RedeemedFlag") %>'>Available, click to redeem</asp:LinkButton></strong>

                    <asp:LinkButton runat="server" ID="UndoLinkButton"
                        CommandName="undopickup" CommandArgument='<%# Bind("PPID") %>'
                        Visible='<%#(bool)Eval("RedeemedFlag") %>'>[Undo]</asp:LinkButton>

                </ItemTemplate>
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="Modified On"
                DataField="LastModDate" SortExpression="LastModDate" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Modified By"
                DataField="LastModUser" SortExpression="LastModUser" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Added On"
                DataField="AddedDate" SortExpression="AddedDate" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
            <asp:BoundField ReadOnly="True" HeaderText="Added By"
                DataField="AddedUser" SortExpression="AddedUser" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
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

