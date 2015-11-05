<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityHistCtl.ascx.cs" Inherits="GRA.SRP.Controls.ActivityHistCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<div class="row">
    <div class="col-sm-12">
        <span class="h1">
            <asp:Label ID="Label2" runat="server" Text="activity-history-title"></asp:Label></span>
    </div>
</div>

<div class="row margin-1em-top">
    <div class="col-xs-12 col-md-7 col-md-offset-1">
        <asp:Panel ID="pnlFilter" runat="server" Visible="false">
            <asp:Label Text="activity-history-other-family" ID="ActivityHistoryOtherFamily" runat="server"></asp:Label>
            <asp:DropDownList ID="PID" runat="server" AppendDataBoundItems="True" AutoPostBack="true" OnSelectedIndexChanged="Ddl_SelectedIndexChanged">
            </asp:DropDownList>
        </asp:Panel>
    </div>
    <div class="col-xs-12 col-md-3">
        <div class="pull-right">
            <div class="btn-group">
                <button class="btn btn-default btn-xs dropdown-toggle" type="button" data-toggle="dropdown" runat="server">
                    <%= this.FilterButtonText %> <span class="caret"></span>
                </button>
                <ul class="dropdown-menu dropdown-menu-right">
                    <li>
                        <asp:LinkButton id="allActivitiesFilter" runat="server" Text="All activitites" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                    <li class="award-reading">
                        <asp:LinkButton runat="server" Text="Reading" OnClick="FilterActivitiesClick" CssClass="award-reading-text"></asp:LinkButton></li>
                    <li class="award-adventure">
                        <asp:LinkButton runat="server" Text="Adventures" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                    <li class="award-challenge">
                        <asp:LinkButton runat="server" Text="Challenges" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                    <li class="award-event">
                        <asp:LinkButton runat="server" Text="Events" OnClick="FilterActivitiesClick"></asp:LinkButton></li>
                </ul>
            </div>
        </div>
    </div>
</div>

<div class="row margin-1em-top">
    <div class="col-xs-12 col-md-10 col-md-offset-1">
        <span runat="server" id="noActivitiesLabel" visible="false">You currently have no logged activities.</span>
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
