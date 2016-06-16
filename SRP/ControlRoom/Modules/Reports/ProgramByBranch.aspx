<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="ProgramByBranch.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.ProgramByBranch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row margin-1em-bottom">
            <div class="col-sm-12" style="font-size: 1.2em;">
                <p>Description: Excel Workbook summary report of sign-ups and achievers per program, then detailed sheets showing each branch's stats.</p>
                <ul>
                    <li>
                        <p>Selecting a date shows <strong>all data up to the moment selected</strong>.</p>
                    </li>
                    <li>The "pre-logging" box shows report data <strong>up to the Logging Start Date for each program</strong> (effectively pre-registratons).</li>
                </ul>
            </div>
        </div>
        <div class="form-inline hidden-print">
            <div class="form-group">
                <label for="<%=EndDate.ClientID %>" style="font-size: 1.2em;">Report all data up to: </label>
                <div class="input-group date gra-datetime">
                    <asp:TextBox ID="EndDate" runat="server" CssClass="form-control"></asp:TextBox>
                    <span class="input-group-addon">
                        <span class="glyphicon glyphicon-calendar"></span>
                    </span>
                </div>
            </div>
            <div class="form-group margin-1em-left">
                <div class="checkbox">
                    <label>
                        <asp:CheckBox runat="server" ID="PreloggingOnly" />
                        <b>Report on pre-logging registrations only</b>
                    </label>
                    <br />
                    <small>(Ignores any selected date)</small>
                </div>
            </div>
            <div class="form-group margin-1em-left">
                <asp:LinkButton runat="server"
                    CssClass="btn btn-success"
                    OnClick="DownloadReport_Click"
                    ForeColor="White">
                    <span class="glyphicon glyphicon-download"></span>
                    Download
                </asp:LinkButton>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-6 col-xs-offset-3">
                <asp:Panel runat="server" ID="AlertPanel" Visible="false" role="alert" CssClass="alert alert-danger">
                    <span class="glyphicon glyphicon-alert"></span>
                    <asp:Label runat="server" ID="AlertMessage"></asp:Label>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script>
        $(function () {
            $('#<%=PreloggingOnly.ClientID%>').on('click', function () {
                if ($('#<%=PreloggingOnly.ClientID%>').is(':checked')) {
                    $('#<%=EndDate.ClientID %>').prop('disabled', true);
                } else {
                    $('#<%=EndDate.ClientID %>').prop('disabled', false);
                }
            });
        });
    </script>
</asp:Content>
