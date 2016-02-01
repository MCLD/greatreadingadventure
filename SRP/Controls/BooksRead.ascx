<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BooksRead.ascx.cs" Inherits="GRA.SRP.Controls.BooksRead" %>
<div class="row hidden-print">
    <div class="col-sm-12">
        <span class="h1">
            <asp:Label runat="server" Text="books-read-title"></asp:Label></span>
    </div>
</div>

<div class="row margin-1em-top hidden-print margin-halfem-top">
    <div class="col-xs-12 col-sm-7 col-md-6 col-md-offset-1">
        <asp:Panel ID="familyMemberSelector" runat="server" Visible="false" CssClass="form-inline margin-halfem-bottom">
            <asp:Label Text="activity-history-other-family" ID="ActivityHistoryOtherFamily" runat="server"></asp:Label>
            <asp:DropDownList ID="PID" runat="server" AppendDataBoundItems="True" AutoPostBack="true" OnSelectedIndexChanged="Ddl_SelectedIndexChanged" CssClass="form-control">
            </asp:DropDownList>
        </asp:Panel>
    </div>
    <div class="col-xs-12 col-sm-5 col-md-4">
        <div class="pull-right margin-halfem-bottom">
            <button class="btn btn-default" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>

            <asp:HyperLink runat="server" NavigateUrl="~/Account/ActivityHistory.aspx" CssClass="btn btn-default"><span class="glyphicon glyphicon-folder-open margin-halfem-right"></span> Activity History</asp:HyperLink>
        </div>
    </div>
</div>

<div class="row visible-print-block">
    <span class="lead">Books read by <strong><asp:Label runat="server" ID="whoRead"></asp:Label></strong>:</span>
</div>

<div class="row margin-1em-top">
    <div class="col-xs-12 col-md-10 col-md-offset-1">
        <asp:Panel ID="booksPanel" runat="server" Visible="true">
            <table class="table table-compact table-alternating">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Author</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="rptr">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%#Eval("Title") %>
                                </td>
                                <td>
                                    <%#Eval("Author") %>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </asp:Panel>
        <asp:Label Text="books-read-none" runat="server" ID="noBooksLabel" Visible="false"></asp:Label>
    </div>
</div>
