<%@ Page Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="GRA.SRP.Challenges.Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-8 col-sm-offset-2">
            <asp:Panel runat="server" ID="challengeDetails" CssClass="panel panel-default">
                <div class="panel-heading">
                    <span class="lead">Challenge:
                        <asp:Label runat="server" ID="challengeTitle"></asp:Label></span>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-xs-8 col-sm-9">
                            <asp:Label ID="lblDesc" runat="server"></asp:Label>
                            <asp:Label ID="lblPoints" runat="server"></asp:Label>
                        </div>
                        <div class="col-xs-4 col-sm-3" style="min-width: 64px;">
                            <asp:Label ID="BadgeImage" runat="server" CssClass="pull-right"></asp:Label>
                        </div>
                    </div>
                </div>
                <table class="table table-striped" style="margin-top: 1em;">
                    <tr>
                        <th class="text-center">Complete</th>
                        <th>Task</th>
                    </tr>
                    <asp:Repeater runat="server" ID="rptr" OnItemDataBound="rptr_ItemDataBound">
                        <ItemTemplate>
                            <tr>
                                <td class="text-center challenge-checkbox-container" style="vertical-align: middle;">
                                    <asp:CheckBox ID="chkRead" runat="server" Checked='<%# Eval("HasRead") %>' />
                                    <asp:Label ID="PBLBID" runat="server" Visible="false" Text='<%# Eval("PBLBID") %>'></asp:Label>
                                    <asp:Label ID="BLBID" runat="server" Visible="false" Text='<%# Eval("BLBID") %>'></asp:Label>
                                    <asp:Label ID="BLID" runat="server" Visible="false" Text='<%# Eval("BLID") %>'></asp:Label>
                                </td>

                                <td>
                                    <div class="row">
                                        <asp:Panel runat="server" Visible='<%#Eval("Author").ToString().Length > 0 %>'>
                                            <div class="hidden-xs col-sm-2">
                                                <%# Eval("ISBN").ToString().Length > 0 ? string.Format("<img class='cover' src='http://covers.openlibrary.org/b/isbn/{0}-S.jpg' style='width: 32px; margin-right: 10px;  ' align=left/>", Eval("ISBN")) : ""%>
                                            </div>
                                            <div class="col-xs-12 col-sm-10">
                                                Read
                                    <strong>
                                        <asp:HyperLink runat="server" NavigateUrl='<%# Eval("URL") %>' Target="_blank"
                                            Visible='<%# Eval("URL").ToString().Trim().Length > 0 %>' Font-Underline='<%# Eval("URL").ToString().Trim().Length > 0 %>'><%# Eval("Title") %></asp:HyperLink>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("Title") %>'
                                            Visible='<%# Eval("URL").ToString().Trim().Length == 0 %>'></asp:Label></strong>
                                                by <strong><%# Eval("Author") %></strong>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" Visible='<%#Eval("Author").ToString().Length == 0 %>'>
                                            <div class="col-xs-12 col-sm-12">
                                                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("URL") %>' Target="_blank"
                                                    Visible='<%# Eval("URL").ToString().Trim().Length > 0 %>' Font-Underline='<%# Eval("URL").ToString().Trim().Length > 0 %>'><%# Eval("Title") %></asp:HyperLink>
                                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("Title") %>'
                                                    Visible='<%# Eval("URL").ToString().Trim().Length == 0 %>'></asp:Label>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
                <div class="panel-footer clearfix hidden-print">
                    <div class="pull-right">
                        <button class="btn btn-default" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>
                        <asp:HyperLink runat="server" ID="challengesBackLink" CssClass="btn btn-default"><asp:Label runat="server" Text="challenges-return"></asp:Label></asp:HyperLink>
                        <asp:Button ID="btnSave" runat="server" Text="challenges-save" OnClick="btnSave_Click" CssClass="btn btn-default" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <script>
        if(<%=this.PrintPage%> == true) {
            window.print();
        }
    </script>
</asp:Content>
