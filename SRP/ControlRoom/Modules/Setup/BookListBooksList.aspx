<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="BookListBooksList.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.BookListBooksList" %>

<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%=ResolveUrl("~/Scripts/jquery-2.2.0.min.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblPK" runat="server" Text="" Visible="False"></asp:Label>
    <asp:ObjectDataSource ID="odsData" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.BookListBooks">
        <SelectParameters>
            <asp:ControlParameter ControlID="lblPK" Name="BLID"
                PropertyName="Text" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <ul style="font-size: larger;">
        <li>If title and author are blank, entering an ISBN and pressing tab will trigger a lookup for title, author, and cover image from <a href="http://openlibrary.org/" target="_blank">openlibrary.org</a>.</li><br />
        <li>For entering challenge tasks rather than books, just leave the ISBN and Author fields blank. You may still put a URL in the URL field to link to a Web page.</li><br />
        <li>ISBN is not required.</li>
    </ul>

    <table width="100%" class="datatable">
        <tr>
            <td colspan="4" style="background-color: #ddd;"><b>Add To Challenge</b></td>
        </tr>
        <tr>
            <td valign="top" width="150px"><b>ISBN</b><br />
                <asp:TextBox ID="ISBN" runat="server" Width="150px" CssClass="book-isbn"></asp:TextBox><br />
                <img src="../../Images/spacer.gif" class="book-image" alt="Cover image on Open Library" /><span class="lookup-message"></span>
            </td>

            <td valign="top" width="300px"><b>Title/Task</b><br />
                <asp:TextBox ID="Title" runat="server" Width="300px" CssClass="book-title"></asp:TextBox>
            </td>
            <td valign="top" width="150px"><b>Author</b><br />
                <asp:TextBox ID="Author" runat="server" Width="150px" CssClass="book-author"></asp:TextBox>
            </td>
            <td valign="top" width="100%"><b>URL</b><br />
                <asp:TextBox ID="URL" runat="server" Width="85%" CssClass="ils-link"></asp:TextBox>
                <a href="#" onclick="return testLink();">Test URL</a>
                <br />
                (The URL should be available to users not logged in to the ILS)
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Button ID="btnSave" runat="server" Text="Add Book/Task"
                    CssClass="btn-sm btn-green" OnClick="btnSave_Click" />

                &nbsp;&nbsp;            
            <asp:Button ID="Button1" runat="server" Text="Back To List"
                CssClass="btn-sm btn-gray" OnClick="Button1_Click" />


            </td>

        </tr>

    </table>
    <asp:GridView ID="gv" runat="server" AllowSorting="True" AutoGenerateColumns="False" AllowPaging="False"
        DataKeys="BLBID"
        DataSourceID="odsData"
        OnRowCreated="GvRowCreated"
        OnSorting="GvSorting"
        OnRowCommand="GvRowCommand"
        Width="100%">
        <Columns>
            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    &nbsp;
                    <asp:ImageButton ID="btnDelete" runat="server" AlternateText="Delete Record" ToolTip="Delete Record"
                        CausesValidation="False" CommandName="DeleteRecord" CommandArgument='<%# Bind("BLBID") %>'
                        ImageUrl="~/ControlRoom/Images/delete.png" Width="20px" OnClientClick="return confirm('Are you sure you want to delete this record?');" />
                    &nbsp;
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:TemplateField>


            <asp:BoundField ReadOnly="True" HeaderText="BLBID" DataField="BLBID"
                SortExpression="BLBID" Visible="False" ItemStyle-Wrap="False"
                ItemStyle-VerticalAlign="Top">
                <ItemStyle VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Title/Task"
                DataField="Title" SortExpression="Title" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:BoundField ReadOnly="True" HeaderText="Author"
                DataField="Author" SortExpression="Author" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>


            <asp:BoundField ReadOnly="True" HeaderText="ISBN"
                DataField="ISBN" SortExpression="ISBN" Visible="True"
                ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                <ControlStyle Width="250px" />
                <ItemStyle Width="250px" VerticalAlign="Top" Wrap="False"></ItemStyle>
            </asp:BoundField>

            <asp:TemplateField SortExpression="URL" Visible="True"
                ItemStyle-Wrap="True" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                HeaderText="URL">
                <ItemTemplate>
                    <%# string.Format("<a href='{0}' target=_blank>{0}<a/>", Eval("URL")) %>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" Wrap="True" Width="50px" HorizontalAlign="Left"></ItemStyle>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-VerticalAlign="Top" HeaderText="Cover">
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
            <div style="width: 600px; padding: 20px; font-weight: bold;">
                No records were found. &nbsp;
            &nbsp;&nbsp;            
            <br />
                <asp:Button ID="Button1" runat="server" Text="Back To List"
                    CssClass="btn-sm btn-gray" OnClick="Button1_Click" />
            </div>
        </EmptyDataTemplate>
    </asp:GridView>


    <script>
        $('.book-isbn').focusout(function () {
            if ($('.book-isbn').val().length > 0) {
                $('.lookup-message').html("Looking up book...");
                var isbn = $('.book-isbn').val().trim();
                var olIsbn = 'ISBN:{0}'.replace('{0}', isbn);
                $('.book-image').attr('src', 'http://covers.openlibrary.org/b/isbn/{0}-S.jpg'.replace('{0}', isbn));

                if ($('.book-title').val().length == 0 && $('.book-author').val().length == 0) {
                    $.ajax({
                        url: 'https://openlibrary.org/api/books',
                        jsonp: 'callback',
                        dataType: 'jsonp',
                        data: {
                            bibkeys: olIsbn,
                            jscmd: 'data'
                        },
                        success: function (result) {
                            if (result[olIsbn]) {
                                if (result[olIsbn].title && result[olIsbn].title.length > 0) {
                                    $('.book-title').val(result[olIsbn].title.trim());
                                }
                                if (result[olIsbn].authors
                                    && result[olIsbn].authors.length > 0) {
                                    $('.book-author').val(result[olIsbn].authors[0].name.trim());
                                }
                                $('.lookup-message').html("");
                            } else {
                                $('.lookup-message').html("ISBN not found in openlibrary.org.");
                            }
                        },
                        failure: function () {
                            $('.lookup-message').html("Openlibrary.org lookup failed.");
                        }
                    });
                } else {
                    $('.lookup-message').html("");
                }

                var ilsLink = '<%=this.IlsLink%>';
                if (ilsLink && ilsLink != '') {
                    $('.ils-link').val(ilsLink.replace('{0}', isbn));
                }
            }
        });
        function testLink() {
            if ($('.ils-link').val().length > 0) {
                window.open($('.ils-link').val(), "_blank");
            } else {
                alert("Please enter a link before testing.");
            }
            return false;
        }
    </script>
</asp:Content>

