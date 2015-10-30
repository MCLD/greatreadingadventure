<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChallengesCtl.ascx.cs" Inherits="GRA.SRP.Controls.ChallengesCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<div class="row">
    <div class="col-sm-12">
        <span class="h1">
            <asp:Label runat="server" Text="Reading Lists Title"></asp:Label></span>
    </div>
    <div class="col-sm-12">
        <asp:Label runat="server" Text="Reading Lists Instructions"></asp:Label>
    </div>
</div>
<div class="row">
    <div class="col-sm-12">
        <p>
            <asp:Label ID="lblNoLists" runat="server" Text="Reading Lists No Lists" Visible="false"></asp:Label>
        </p>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Challenge</th>
                    <th>Steps completed toward goal</th>
                    <th>Points<br />earned</th>
                    <th>Badge<br />earned</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="rptr" OnItemCommand="rptr_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:LinkButton runat="server" CommandArgument='<%# Eval("BLID") %>'><%# Eval("ListName") %></asp:LinkButton>
                            </td>
                            <td>
                                <div class="progress">
                                    <div class="progress-bar" role="progressbar" aria-valuenow="<%#Eval("NumBooksCompleted") %>"
                                        aria-valuemin="0" aria-valuemax="<%#Eval("NumBooksToComplete") %>" style="width: <%#ComputePercent(Eval("NumBooksCompleted"), Eval("NumBooksToComplete")) %>%;">
                                        <span class="hidden-xs"><%#ProgressDisplay(Eval("NumBooksCompleted"), Eval("NumBooksToComplete"))%></span>
                                        <span class="hidden-sm hidden-md hidden-lg"><%#ComputePercent(Eval("NumBooksCompleted"), Eval("NumBooksToComplete")) %>%</span>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <%#Eval("AwardPoints") %>
                            </td>
                            <td>
                                <%#ShowBadge(Eval("AwardBadgeID")) %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
</div>

<asp:Panel ID="pnlDetail" runat="server" Visible="false" Font-Underline="False" CssClass="modal fade">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <span class="lead">Challenge:
                <asp:Label ID="lblTitle" runat="server" Visible="false"></asp:Label></span>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-xs-9 col-sm-10">
                        <asp:Label ID="lblDesc" runat="server" Visible="false"></asp:Label>
                        <asp:Label ID="lblPoints" runat="server" Visible="false"></asp:Label>
                    </div>
                    <div class="col-xs-3 col-sm-2" style="min-width: 64px;">
                        <asp:Label ID="BadgeImage" runat="server" CssClass="pull-right"></asp:Label>

                    </div>
                </div>
                <table class="table table-striped" style="margin-top: 1em;">
                    <tr>
                        <th>Complete</th>
                        <th>Task</th>
                    </tr>
                    <asp:Repeater runat="server" ID="rptr2" OnItemCommand="rptr_ItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td class="pull-center">
                                    <asp:CheckBox ID="chkRead" runat="server" Checked='<%# Eval("HasRead") %>' />
                                    <asp:Label ID="PBLBID" runat="server" Visible="false" Text='<%# Eval("PBLBID") %>'></asp:Label>
                                    <asp:Label ID="BLBID" runat="server" Visible="false" Text='<%# Eval("BLBID") %>'></asp:Label>
                                    <asp:Label ID="BLID" runat="server" Visible="false" Text='<%# Eval("BLID") %>'></asp:Label>
                                </td>

                                <td>
                                    <div class="row">
                                        <div class="col-xs-3 col-sm-1">
                                            <%# Eval("ISBN").ToString().Length > 0 ? string.Format("<a href='javascript: Cover({0});'><img class='cover' src='http://covers.openlibrary.org/b/isbn/{0}-S.jpg' style='width: 32px; margin-right: 10px;  ' align=left/></a>", Eval("ISBN")) : ""%>
                                        </div>
                                        <div class="col-xs-9 col-sm-11">
                                            Read
                                    <strong>
                                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("URL") %>' Target="_blank"
                                            Visible='<%# Eval("URL").ToString().Trim().Length > 0 %>' Font-Underline='<%# Eval("URL").ToString().Trim().Length > 0 %>'><%# Eval("Title") %></asp:HyperLink>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("Title") %>'
                                            Visible='<%# Eval("URL").ToString().Trim().Length == 0 %>'></asp:Label></strong>
                                            by <strong><%# Eval("Author") %></strong>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>

                </table>
            </div>
            <div class="modal-footer">
                <div class="pull-right clearfix">
                    <asp:Button ID="btnClose" runat="server" Text="Reading Lists btn Close" OnClick="btnClose_Click" CssClass="btn btn-standard" />
                    <asp:Button ID="btnSave" runat="server" Text="Reading Lists btn Save" OnClick="btnSave_Click" CssClass="btn btn-standard" />
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
<script>
    $(function () {
        if ("<%=this.ShowModal%>" == "True") {
            var elems = $("div[id$='pnlDetail']");
            elems.each(function (index) {
                $(this).modal('show');
            });
        }
    });
</script>
