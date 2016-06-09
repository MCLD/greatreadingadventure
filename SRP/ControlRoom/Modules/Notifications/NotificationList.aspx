<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="NotificationList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Notifications.NotificationList" %>

<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .CRListText {
            text-align: left;
            font-size:  0.5em;
        }
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="GetAllUnreadToPatron"
        TypeName="GRA.SRP.DAL.Notifications">
        <SelectParameters>
            <asp:Parameter DefaultValue="0" Name="PID" Type="Int32" />
        </SelectParameters>

    </asp:ObjectDataSource>



    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="False"
        DataKeys="NID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated"
        OnSorting="GvSorting"
        OnRowCommand="GvRowCommand"
        Width="100%">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <!--
                    &nbsp;
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                        -->
                </HeaderTemplate>
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" ToolTip="Edit Record"
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("NID") %>'
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("NID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="NID" DataField="NID"
                SortExpression="NID" Visible="False" ItemStyle-Wrap="False"
                ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField SortExpression="From" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="From">
                <ItemTemplate>
                    <asp:HyperLink runat="server" Target="_blank" NavigateUrl='<%#Eval("PID_From", "~/ControlRoom/Modules/Patrons/PatronDetails.aspx?pid={0}") %>'><%# Eval("FromUsername")%></asp:HyperLink><br />
                    (<%# Eval("FromFistName")%>  <%# Eval("FromLastName")%>)
                </ItemTemplate>
                <ControlStyle />
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Subject/Message" HeaderStyle-Width="650px"
                SortExpression="Subject" Visible="True" ItemStyle-CssClass="CRListText"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:LinkButton runat="server" CommandName="EditRecord" ID="SubjectField" CommandArgument='<%#Bind("NID") %>' CssClass="btn btn-link"><%#DisplaySubjectMessage(Container.DataItem) %></asp:LinkButton>
                </ItemTemplate>
                <ControlStyle />
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:TemplateField>

            <asp:BoundField ReadOnly="True" HeaderText="Date"
                DataField="AddedDate" SortExpression="AddedDate" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Middle">
                <ItemStyle VerticalAlign="Middle" Wrap="False"></ItemStyle>
            </asp:BoundField>


        </Columns>
        <EmptyDataTemplate>
            <div style="width: 600px; padding: 20px; font-weight: bold;">
                No records were found. &nbsp; 
                    <!--<asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />-->
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

