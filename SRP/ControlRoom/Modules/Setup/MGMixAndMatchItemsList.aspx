<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="MGMixAndMatchItemsList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.MGMixAndMatchItemsList" %>

<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblMGID" runat="server" Text="" Visible="False"></asp:Label>
    <asp:Label ID="lblMMID" runat="server" Text="" Visible="False"></asp:Label>

    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.MGMixAndMatchItems">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblMGID" Name="MGID" DefaultValue="0"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <h1>Mini Game:
        <asp:Label ID="AdminName" runat="server" Text=""></asp:Label>
    </h1>

    <h2>The mix and match game requires three images to work properly.</h2>

    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="True" PageSize="25" Width="100%"
        DataKeys="MMIID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated"
        OnSorting="GvSorting"
        OnRowCommand="GvRowCommand">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" ToolTip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1"
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />

                    &nbsp;&nbsp;

                    <asp:ImageButton ID="btnBack" runat="server"
                        CausesValidation="false"
                        CommandName="back"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="23"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
                </HeaderTemplate>
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" ToolTip="Edit Record"
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("MMIID") %>'
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("MMIID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="200px"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="MMIID" DataField="MMIID"
                SortExpression="MMIID" Visible="False" ItemStyle-Wrap="False"
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>



            <asp:BoundField ReadOnly="True" HeaderText="Easy Label"
                DataField="EasyLabel" SortExpression="EasyLabel" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="75%"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="MediumLabel"
                DataField="MediumLabel" SortExpression="MediumLabel" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="HardLabel"
                DataField="HardLabel" SortExpression="HardLabel" Visible="False"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>


            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server" ImageUrl='<%# String.Format("~/Images/Games/MixMatch/sm_{0}.png?{1}", Eval("MMIID").ToString(), DateTime.Now.ToString()) %>' Width="24px" />
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="300px" HorizontalAlign="Center"></ItemStyle>
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

                &nbsp;&nbsp;&nbsp;&nbsp;

                    <asp:ImageButton ID="btnBack" runat="server"
                        CausesValidation="false"
                        CommandName="back"
                        ImageUrl="~/ControlRoom/Images/back.png"
                        Height="23"
                        Text="Back/Cancel" ToolTip="Back/Cancel"
                        AlternateText="Back/Cancel" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

