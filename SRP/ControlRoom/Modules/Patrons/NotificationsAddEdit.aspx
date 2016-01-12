<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="NotificationsAddEdit.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Patrons.NotificationsAddEdit" %>

<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<%@ Register Src="~/ControlRoom/Controls/PatronContext.ascx" TagName="PatronContext" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <uc1:PatronContext ID="pcCtl" runat="server" />

    <asp:ValidationSummary ID="ValidationSummaryMain" runat="server"
        BorderColor="Black" BorderStyle="Solid" BorderWidth="1px"
        CssClass="ValidationSummary" Font-Bold="True" Font-Size="11px"
        HeaderText="There are errors, and no action was taken" Font-Names="Verdana" />


    <asp:DetailsView ID="dv" runat="server" DataSourceID="odsData"
        OnItemCommand="DvItemCommand" OnDataBinding="dv_DataBinding"
        OnDataBound="dv_DataBound"
        Width="100%">
        <Fields>

            <asp:BoundField DataField="NID" HeaderText="NID: " SortExpression="NID" ReadOnly="True" InsertVisible="False" Visible="false">
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="From: " SortExpression="From" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <%# DisplayFrom((GRA.SRP.DAL.Notifications)Container.DataItem) %>
                </EditItemTemplate>
                <InsertItemTemplate>
                </InsertItemTemplate>
                <ItemTemplate>
                    <%# DisplayFrom((GRA.SRP.DAL.Notifications)Container.DataItem) %>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="To: " SortExpression="To" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <%# DisplayTo((GRA.SRP.DAL.Notifications)Container.DataItem) %>
                </EditItemTemplate>
                <InsertItemTemplate>
                </InsertItemTemplate>
                <ItemTemplate>
                    <%# DisplayTo((GRA.SRP.DAL.Notifications)Container.DataItem) %>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Subject: " SortExpression="Subject" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <%# Eval("Subject") %>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <asp:TextBox ID="Subject" runat="server" Text='' Width="500px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSubject" runat="server"
                        ControlToValidate="Subject" Display="Dynamic" ErrorMessage="Subject is required"
                        SetFocusOnError="True" Font-Bold="True"><font color='red'> * Required </font></asp:RequiredFieldValidator>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Subject" runat="server" Text='<%# Eval("Subject") %>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle Font-Bold="True" HorizontalAlign="Right" VerticalAlign="Top" />
                <ItemStyle Width="100%" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Body: " SortExpression="Body" HeaderStyle-Wrap="False">
                <EditItemTemplate>
                    <%# Server.HtmlDecode(Eval("Body").ToString()) %>
                </EditItemTemplate>
                <InsertItemTemplate>
                    <textarea id="Body" runat="server" class="gra-editor"></textarea>
                </InsertItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Body" runat="server" Text='<%# Eval("Body") %>'></asp:Label>
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

                </EditItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>

    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="FetchObject"
        TypeName="GRA.SRP.DAL.Notifications">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="NID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>

