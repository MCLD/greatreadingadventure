<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="BadgeList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.BadgeList" 

%>
<%@ Import Namespace="GRA.SRP.DAL" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<hr />
<b>Badge Gallery Direct URL: </b> <a href='/Badges/Default.aspx?PID=<%=Programs.GetDefaultProgramID()%>' target=_blank><%= Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/Badges/Default.aspx?PID=" + Programs.GetDefaultProgramID() %></a>
<hr />
<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="GRA.SRP.DAL.Badge">

    </asp:ObjectDataSource>

    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="BID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        >

        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    &nbsp;&nbsp;<asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnEdit" runat="server" AlternateText="Edit Record" Tooltip="Edit Record" 
                        CausesValidation="False" CommandName="EditRecord" CommandArgument='<%# Bind("BID") %>'  
                        ImageUrl="~/ControlRoom/Images/edit.png" Width="20px" />

                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("BID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>

			<asp:BoundField ReadOnly="True" HeaderText="Badge Id" DataField="BID" 
                SortExpression="BID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Control Room Name" 
                DataField="AdminName" SortExpression="AdminName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
			    <ControlStyle Width="250px" />
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="250px"></ItemStyle>
            </asp:BoundField>
			<asp:BoundField ReadOnly="True" HeaderText="Patron Name" 
                DataField="UserName" SortExpression="UserName" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
			    <ControlStyle Width="250px" />
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="250px"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                    
                </HeaderTemplate>                
                <ItemTemplate>
                    <asp:Image ID="Image1" runat="server" 
                        ImageUrl='<%# String.Format("~/Images/Badges/sm_{0}.png?{1}", Eval("BID").ToString(), DateTime.Now.ToString()) %>' 
                        Width="32" Height="32"
                    />
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
            <div style="width: 600px; padding: 20px; font-weight:bold; ">
            No records were found. &nbsp; 
                    <asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Record" Tooltip="Add Record"
                        CausesValidation="False" CommandName="AddRecord" CommandArgument="-1" 
                        ImageUrl="~/ControlRoom/Images/add.png" Width="20px" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

