<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master" 
    AutoEventWireup="true" CodeBehind="BookListBooksList.aspx.cs" Inherits="STG.SRP.ControlRoom.Modules.Setup.BookListBooksList" 
    
%>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
  	<asp:ObjectDataSource ID="odsData" runat="server" 
        SelectMethod="GetAll" 
        TypeName="STG.SRP.DAL.BookListBooks">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BLID" 
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
	</asp:ObjectDataSource>

    <table width="100%" class=datatable>
        <tr> <td colspan=4 style="background-color: #ddd;"><b>Add To Book List</b></td></tr>
        <tr>
            <td valign=top width="150px"><b>Author</b><br />
                <asp:TextBox ID="Author" runat="server" width="150px"></asp:TextBox>
            </td>
            <td valign=top width="300px"><b>Title</b><br />
                <asp:TextBox ID="Title" runat="server" width="300px"></asp:TextBox>
            </td>
            <td valign=top width="200px"><b>ISBN</b><br />
                <asp:TextBox ID="ISBN" runat="server" width="200px"></asp:TextBox>
                <br />(will attempt to get cover from OpenLibrary.Org using ISBN) 
            </td>
            <td valign=top width="100%"><b>URL</b><br />
                <asp:TextBox ID="URL" runat="server" width="90%"></asp:TextBox>
                <br />(The URL should be available to users NOT LOGGED INTO THE ILS)
            </td>                                   
        </tr>
        <tr> <td colspan=4>
            <asp:Button ID="btnSave" runat="server" Text="Add Book" 
                CssClass="btn-sm btn-green" onclick="btnSave_Click"/>

&nbsp;&nbsp;            
            <asp:Button ID="Button1" runat="server" Text="Back To List" 
                CssClass="btn-sm btn-gray" onclick="Button1_Click" />


             </td>
        
        </tr>
    
    </table>
    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="BLBID"
        DataSourceID="odsData"
        onrowcreated="GvRowCreated" 
        onsorting="GvSorting" 
        onrowcommand="GvRowCommand"      
        width="100%"
        >
        <Columns>
            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                </HeaderTemplate>                
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" Tooltip="Delete Record" 
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("BLBID") %>' 
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');"/>
                   &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


			<asp:BoundField ReadOnly="True" HeaderText="BLBID" DataField="BLBID" 
                SortExpression="BLBID" Visible="False" ItemStyle-Wrap="False" 
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>           			 

			<asp:BoundField ReadOnly="True" HeaderText="Author" 
                DataField="Author" SortExpression="Author" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="Title" 
                DataField="Title" SortExpression="Title" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

			<asp:BoundField ReadOnly="True" HeaderText="ISBN" 
                DataField="ISBN" SortExpression="ISBN" Visible="True" 
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left">
				 <ControlStyle Width="250px" />
                <ItemStyle    Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField> 

            <asp:TemplateField  SortExpression="URL" Visible="True" 
                ItemStyle-Wrap="True" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign ="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="URL"
                >                         
                <ItemTemplate>
                    <%# string.Format("<a href='{0}' target=_blank>{0}<a/>", Eval("URL")) %>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="True" Width="50px" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField   ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" HeaderText="Cover">                         
                <ItemTemplate>
                    <%# string.Format("<a href='http://openlibrary.org/isbn/{0}' target=_blank><img src='http://covers.openlibrary.org/b/isbn/{0}-S.jpg'/></a>", Eval("ISBN")) %>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False" Width="50px" HorizontalAlign="Center"></ItemStyle>
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
            &nbsp;&nbsp;            
            <br /><asp:Button ID="Button1" runat="server" Text="Back To List" 
                CssClass="btn-sm btn-gray" onclick="Button1_Click" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>

