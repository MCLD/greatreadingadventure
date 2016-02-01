<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Events.ascx.cs" Inherits="GRA.SRP.Controls.Events" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<asp:Panel ID="pnlList" runat="server" Visible="true">

    <div class="row hidden-print">
        <div class="col-sm-12">
            <span class="h1">
                <asp:Label runat="server" Text="events-title"></asp:Label></span>
        </div>
    </div>
    <div class="row hidden-print">
        <div class="col-sm-12 form-inline margin-halfem-top">
            <label for="StartDate" class="margin-halfem-bottom">Start Date:</label>
            <asp:TextBox
                ID="StartDate"
                runat="server"
                Width="7em"
                CssClass="datepicker margin-halfem-right form-control"></asp:TextBox>
            <label for="EndDate" class="margin-halfem-bottom">End Date:</label>
            <asp:TextBox
                ID="EndDate"
                runat="server"
                Width="7em"
                CssClass="datepicker margin-halfem-right form-control"></asp:TextBox>
            <label for="BranchId" class="margin-halfem-bottom">Branch/library:</label>
            <asp:DropDownList
                ID="BranchId"
                runat="server"
                DataSourceID="odsDDBranch"
                DataTextField="Description"
                DataValueField="CID"
                AppendDataBoundItems="True"
                CssClass="margin-1em-right form-control margin-halfem-right">
                <asp:ListItem Value="0">All libraries/branches</asp:ListItem>
            </asp:DropDownList>
            <div class="margin-halfem-top margin-halfem-bottom" style="display: inline-block;">
                <button class="btn btn-default margin-halfem-right" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>
                <asp:Button ID="btnFilter" runat="server" Text="events-filter-button"
                    OnClick="btnFilter_Click" CssClass="btn btn-default hidden-print margin-halfem-right" />
                <asp:Button ID="btnClear" runat="server" Text="events-filter-clear-button" OnClick="btnClear_Click" CssClass="btn btn-default hidden-print" />
            </div>
        </div>
    </div>
    <div class="row visible-print-block margin-1em-top margin-1em-bottom">
        <div class="col-xs-12">
            <asp:Label runat="server" Text="events-title" CssClass="lead"></asp:Label>
        </div>

        <asp:Label ID="whatsShowing" runat="server" CssClass="col-xs-12"></asp:Label>
    </div>
    <table class="table table-striped">
        <thead>
            <tr>
                <th>
                    <asp:Label runat="server" Text="events-header-what"></asp:Label></th>
                <th>
                    <asp:Label runat="server" Text="events-header-when"></asp:Label></th>
                <th>
                    <asp:Label runat="server" Text="events-header-where"></asp:Label></th>
            </tr>
        </thead>
        <tbody>
            <asp:Repeater runat="server" ID="rptr" OnItemCommand="rptr_ItemCommand">
                <ItemTemplate>
                    <tr>
                        <td><a href='<%# Eval("EID", "~/Events/Details.aspx?EventId={0}") %>'
                            runat="server"
                            onclick='<%# Eval("EID", "return ShowEventInfo({0});") %>'><%# Eval("EventTitle") %></a>
                        </td>
                        <td>
                            <%# DisplayEventDateTime(Eval("EventDate") as DateTime?,
                                                     Eval("EventTime").ToString(),
                                                     Eval("EndDate") as DateTime?,
                                                     Eval("EndTime").ToString()) %>
                        </td>
                        <td><%# Eval("Branch")%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr runat="server" visible="<%#rptr.Items.Count == 0 %>">
                        <td colspan="3">
                            <strong><%=this.NoneAvailableText %></strong>
                        </td>
                    </tr>
                </FooterTemplate>
            </asp:Repeater>
        </tbody>
    </table>
</asp:Panel>

<div id="eventPopupPanel" style="display: none;" class="modal fade">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <span class="lead">
                    <asp:Label runat="server" Text="events-prompt"></asp:Label>
                    <span class="modal-title" id="eventPopupTitle"></span></span>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-sm-12">
                        This event takes place on
                        <strong><span id="eventPopupWhen"></span><span id="eventPopupWhere"></span></strong>.
                    </div>
                    <div class="col-sm-12 margin-1em-top">
                        <span id="eventPopupShortDescription"></span>
                    </div>
                </div>
                <div class="row" id="eventPopupDetailsPanel">
                    <div class="col-sm-12 margin-1em-top">Event details:</div>
                    <div class="col-sm-10 col-sm-offset-1 margin-1em-top">
                        <span id="eventPopupDescription"></span>
                    </div>
                </div>
                <div class="row margin-1em-top" id="eventPopupCustom1Panel">
                    <span id="eventPopupCustomLabel1"></span>: 
                    <span id="eventPopupCustomValue1"></span>
                </div>
                <div class="row margin-1em-top" id="eventPopupCustom2Panel">
                    <span id="eventPopupCustomLabel2"></span>: 
                    <span id="eventPopupCustomValue2"></span>
                </div>
                <div class="row margin-1em-top" id="eventPopupCustom3Panel">
                    <span id="eventPopupCustomLabel3"></span>: 
                    <span id="eventPopupCustomValue3"></span>
                </div>
            </div>
            <div class="modal-footer">
                <div class="pull-right clearfix">
                    <button class="btn btn-default" type="button" id="eventPopupPrint"><span class="glyphicon glyphicon-print"></span></button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</div>

<asp:ObjectDataSource ID="odsDDBranch" runat="server"
    SelectMethod="GetAlByTypeName"
    TypeName="GRA.SRP.DAL.Codes">
    <SelectParameters>
        <asp:Parameter Name="Name" DefaultValue="Branch" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<script>
    $(function () {
        $(".datepicker").datepick({
            changeMonth: false,
            showOtherMonths: true,
            selectOtherMonths: true,
            showSpeed: 'fast',
            minDate: '<%=this.FirstAvailableDate%>',
            maxDate: '<%=this.LastAvailableDate%>'
        });
    });

    function ShowEventInfo(eventId) {
        var jqxhr = $.ajax('<%=Request.ApplicationPath%>Handlers/GetEventInfo.ashx?EventId=' + eventId)
            .done(function (data, textStatus, jqXHR) {
                if (!data.Success) {
                    alert("Couldn't show event information: " + data.ErrorMessage);
                } else {
                    $('#eventPopupTitle').text(data.Title);
                    $('#eventPopupWhen').text(data.When);
                    if (data.Where) {
                        $('#eventPopupWhere').text(" at " + data.Where);
                    }
                    $('#eventPopupShortDescription').text(data.ShortDescription);
                    if (data.Description) {
                        $('#eventPopupDetailsPanel').show();
                        $('#eventPopupDescription').html(data.Description);
                    } else {
                        $('#eventPopupDetailsPanel').hide();
                    }
                    if (data.CustomLabel1 && data.CustomValue1) {
                        $('#eventPopupCustom1Panel').show();
                        $('#eventPopupCustomLabel1').text(data.CustomLabel1);
                        $('#eventPopupCustomValue1').text(data.CustomValue1);
                    } else {
                        $('#eventPopupCustom1Panel').hide();
                    }
                    if (data.CustomLabel2 && data.CustomValue2) {
                        $('#eventPopupCustom2Panel').show();
                        $('#eventPopupCustomLabel2').text(data.CustomLabel2);
                        $('#eventPopupCustomValue2').text(data.CustomValue2);
                    } else {
                        $('#eventPopupCustom2Panel').hide();
                    }
                    if (data.CustomLabel3 && data.CustomValue3) {
                        $('#eventPopupCustom3Panel').show();
                        $('#eventPopupCustomLabel3').text(data.CustomLabel3);
                        $('#eventPopupCustomValue3').text(data.CustomValue3);
                    } else {
                        $('#eventPopupCustom3Panel').hide();
                    }

                    $('#eventPopupPrint').click(function () {
                        location.href = "<%=Request.ApplicationPath%>Events/Details.aspx?EventId=" + eventId + "&Print=1";
                    });

                    $('#eventPopupPanel').modal('show');
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                alert("Couldn't contact server to get event details: " + errorThrown);
            });
            return false;
        }
</script>
