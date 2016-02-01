<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityHistCtl.ascx.cs" Inherits="GRA.SRP.Controls.ActivityHistCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<div class="row hidden-print">
    <div class="col-sm-12">
        <span class="h1">
            <asp:Label ID="Label2" runat="server" Text="activity-history-title"></asp:Label></span>
    </div>
</div>

<div class="row margin-1em-top hidden-print margin-halfem-top">
    <div class="col-xs-12 col-sm-7 col-md-6 col-md-offset-1">
        <asp:Panel ID="pnlFilter" runat="server" Visible="false" CssClass="form-inline margin-halfem-bottom">
            <asp:Label Text="activity-history-other-family" ID="ActivityHistoryOtherFamily" runat="server"></asp:Label>
            <asp:DropDownList ID="PID" runat="server" AppendDataBoundItems="True" AutoPostBack="true" OnSelectedIndexChanged="Ddl_SelectedIndexChanged" CssClass="form-control">
            </asp:DropDownList>
        </asp:Panel>
    </div>
    <div class="col-xs-12 col-sm-5 col-md-4">
        <div class="pull-right margin-halfem-bottom">
            <button class="btn btn-default" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>

            <asp:HyperLink runat="server" NavigateUrl="~/Account/BookList.aspx" CssClass="btn btn-default"><span class="glyphicon glyphicon-bookmark margin-halfem-right"></span> Book List</asp:HyperLink>

            <div class="btn-group">
                <button class="btn btn-default dropdown-toggle" type="button" data-toggle="dropdown" runat="server">
                    <%= this.FilterButtonText %> <span class="caret"></span>
                </button>
                <ul class="dropdown-menu dropdown-menu-right">
                    <li>
                        <asp:LinkButton ID="allActivitiesFilter" runat="server" Text="All activitites" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                    <li class="award-reading">
                        <asp:LinkButton runat="server" Text="Reading" OnClick="FilterActivitiesClick" CssClass="award-reading-text"></asp:LinkButton></li>
                    <li class="award-adventure" runat="server" id="AdventureDropDownItem">
                        <asp:LinkButton runat="server" Text="Adventures" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                    <li class="award-challenge" runat="server" id="ChallengeDropDownItem">
                        <asp:LinkButton runat="server" Text="Challenges" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                    <li class="award-event" runat="server" id="EventsDropDownItem">
                        <asp:LinkButton runat="server" Text="Events" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                </ul>
            </div>
        </div>
    </div>
</div>

<div class="row visible-print-block">
    <span class="lead">
        <asp:Label runat="server" ID="whatShowing"></asp:Label>:</span>
</div>

<div class="row margin-1em-top">
    <div class="col-xs-12 col-md-10 col-md-offset-1">
        <span runat="server" id="noActivitiesLabel" visible="false">No activities logged.</span>
        <asp:Panel ID="activitiesPanel" runat="server" Visible="true">
            <table class="table table-compact table-hover">
                <thead>
                    <tr>
                        <th>Points </th>
                        <th>Reason</th>
                        <th>Date Awarded</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="rptr">
                        <ItemTemplate>
                            <tr class="<%#AwardClass(Eval("AwardReasonCd")) %>">
                                <td>
                                    <%# ((int)Eval("NumPoints")).ToInt() %>
                                </td>
                                <td>
                                    <%# Eval("AwardReason") %><%# ((bool)Eval("isEvent") ? string.Format(" <strong>{0}</strong>, secret code: <em>{1}</em>",  Eval("EventTitle"),  Eval("EventCode")) : 
                         ((bool)Eval("isBookList")  ? string.Format(": <strong>{0}</strong>",  Eval("ListName")) :
                         ((bool)Eval("isReading") ? FormatReading(Eval("Author").ToString(), Eval("Title").ToString(), Eval("Review").ToString(), (int)Eval("PRID")) + string.Format(", <strong>{0} {1}</strong>", Eval("ReadingAmount"), Eval("ReadingType")) :
                         ((bool)Eval("isGameLevelActivity") ? string.Format(": (<strong>{0}</strong>)", Eval("GameName")) : 
                         " (Unknown)"))))%>
                                </td>
                                <td>
                                    <%# ((DateTime)Eval("AwardDate")).ToNormalDate() %> 
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </asp:Panel>
    </div>
</div>

<asp:Label ID="lblPID" runat="server" Text="" Visible="false"></asp:Label>
