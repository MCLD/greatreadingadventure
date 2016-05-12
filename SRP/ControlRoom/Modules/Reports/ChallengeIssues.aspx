<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="ChallengeIssues.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.ChallengeIssues" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row hidden-print">
            <div class="col-xs-12">
                <asp:LinkButton runat="server"
                    CssClass="btn btn-info"
                    OnClick="ShowReport_Click"
                    ForeColor="White">
                    <span class="glyphicon glyphicon-th-list"></span>
                    View
                </asp:LinkButton>
                <asp:LinkButton runat="server"
                    CssClass="btn btn-success"
                    OnClick="DownloadReport_Click"
                    ForeColor="White">
                    <span class="glyphicon glyphicon-download"></span>
                    Download
                </asp:LinkButton>
            </div>
        </div>
        <div class="row" style="margin-top: 2em;">
            <div class="col-xs-6 col-xs-offset-3">
                <asp:Panel runat="server" ID="AlertPanel" Visible="false" role="alert" CssClass="alert alert-danger">
                    <span class="glyphicon glyphicon-alert"></span>
                    <asp:Label runat="server" ID="AlertMessage"></asp:Label>
                </asp:Panel>
            </div>
        </div>
        <asp:Panel class="row margin-1em-top" EnableViewState="false" ID="ReportPanel" runat="server">
            <div class="col-xs-12 lead">
                Possible Event Issues
            </div>
            <div class="col-xs-12">
                <table class="table table-condensed table-striped table-bordered table-hover">
                    <tr class="info">
                        <th>Event Name</th>
                        <th>Event Date</th>
                        <th>Added by</th>
                        <th>Modified by</th>
                        <th>Possible issue</th>
                    </tr>
                    <asp:Repeater runat="server"
                        ID="EventRepeater"
                        EnableViewState="false">
                        <HeaderTemplate>
                            <tr class="danger" runat="server" id="NoEventsFound" visible="false" enableviewstate="true">
                                <td colspan="3">No events found.</td>
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#Eval("EventTitle") %></td>
                                <td><%#Eval("EventDate", "{0:M/d/yy h:mm tt}")%></td>
                                <td><%#Eval("AddedUser") %></td>
                                <td><%#Eval("LastModUser") %></td>
                                <td><%#Eval("Issue") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </asp:Panel>
    </div>

</asp:Content>
