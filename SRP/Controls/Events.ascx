<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Events.ascx.cs" Inherits="GRA.SRP.Controls.Events" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>
<asp:Panel ID="pnlList" runat="server" Visible="true" DefaultButton="btnFilter" EnableViewState="false">
    <div class="row">
        <div class="col-sm-12">
            <span class="h1">
                <asp:Literal runat="server" Text="events-title"></asp:Literal></span>
        </div>
    </div>
    <div class="row margin-halfem-top">
        <div class="col-sm-12">
            <asp:Literal runat="server" Text="events-instructions"></asp:Literal>
        </div>
    </div>
    <div class="row hidden-print margin-1em-top margin-1em-bottom">
        <div class="col-xs-12 col-sm-2">
            <div class="margin-halfem-top">
                <label for="<%=StartDate.ClientID %>">Start Date:</label>
                <asp:TextBox
                    ID="StartDate"
                    runat="server"
                    Width="7em"
                    CssClass="datepicker form-control"></asp:TextBox>
            </div>
            <div class="margin-halfem-top">
                <label for="<%=EndDate.ClientID %>">End Date:</label>
                <asp:TextBox
                    ID="EndDate"
                    runat="server"
                    Width="7em"
                    CssClass="datepicker form-control"></asp:TextBox>
            </div>
        </div>
        <div class="col-xs-12 col-sm-10">
            <div class="margin-halfem-top">
                <label for="<%=SystemId.ClientID %>">Library system:</label>
                <asp:DropDownList
                    ID="SystemId"
                    runat="server"
                    DataSourceID="DistrictDataSource"
                    DataTextField="Description"
                    DataValueField="CID"
                    AppendDataBoundItems="True"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="SystemId_SelectedIndexChanged"
                    CssClass="form-control">
                    <asp:ListItem Value="0">All library systems</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="margin-halfem-top">
                <label for="<%=BranchId.ClientID %>">Branch/library:</label>
                <asp:DropDownList
                    ID="BranchId"
                    runat="server"
                    DataSourceID="BranchDataSource"
                    DataTextField="Description"
                    DataValueField="CID"
                    AppendDataBoundItems="True"
                    CssClass="form-control">
                    <asp:ListItem Value="0">All libraries/branches</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-xs-12 margin-halfem-top">
            <label for="<%=SearchText.ClientID %>">Search:</label>
            <asp:TextBox
                ID="SearchText"
                MaxLength="255"
                runat="server"
                placeholder="Enter text here to search events, try a word or phrase"
                CssClass="form-control"></asp:TextBox>
        </div>
    </div>
    <div class="row hidden-print margin-1em-top margin-1em-bottom">
        <div class="col-xs-12">
            <div class="pull-right margin-halfem-top" style="display: inline-block;">
                <button class="btn btn-default" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>
                <asp:Button ID="ThisWeek" runat="server" Text="events-thisweek-button" CssClass="btn btn-default hidden-print" OnClick="ThisWeek_Click" />
                <asp:Button ID="btnFilter" runat="server" Text="events-filter-button"
                    OnClick="btnFilter_Click" CssClass="btn btn-success hidden-print" />
                <asp:Button ID="btnClear" runat="server" Text="events-filter-clear-button" OnClick="btnClear_Click" CssClass="btn btn-primary hidden-print" />
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-xs-12 hidden-print alert alert-success" runat="server" id="WhatsShowingPanel">
            <asp:Literal ID="WhatsShowing" runat="server"></asp:Literal>
        </div>
        <div class="col-xs-12 visible-print-block">
            <asp:Literal ID="WhatsShowingPrint" runat="server"></asp:Literal>
        </div>
    </div>
</asp:Panel>

<table class="table table-striped">
    <thead>
        <tr>
            <th>
                <asp:Literal runat="server" Text="events-header-what"></asp:Literal></th>
            <th>
                <asp:Literal runat="server" Text="events-header-when"></asp:Literal></th>
            <th>
                <asp:Literal runat="server" Text="events-header-where"></asp:Literal></th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater runat="server"
            ID="rptr"
            OnItemDataBound="rptr_ItemDataBound"
            EnableViewState="false">
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:Literal runat="server" ID="Microdata" />
                        <a href='<%# Eval("EID", "~/Events/Details.aspx?EventId={0}") %>'
                            runat="server"
                            enableviewstate="false"
                            onclick='<%# Eval("EID", "return ShowEventInfo({0});") %>'><%# Eval("EventTitle") %></a>
                    </td>
                    <td>
                        <%# DisplayEventDateTime(Eval("EventDate") as DateTime?) %>
                    </td>
                    <td>
                        <asp:Literal ID="BranchName" runat="server" Text='<%# Eval("Branch")%>'></asp:Literal></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr runat="server" visible="<%#rptr.Items.Count == 0 %>" enableviewstate="false">
                    <td colspan="3">
                        <strong><%=this.NoneAvailableText %></strong>
                    </td>
                </tr>
            </FooterTemplate>
        </asp:Repeater>
    </tbody>
</table>

<div id="eventPopupPanel" style="display: none;" class="modal fade">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <span class="lead">
                    <asp:Literal runat="server" Text="events-prompt"></asp:Literal>
                    <span class="modal-title" id="eventPopupTitle"></span></span>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-sm-12 margin-1em-bottom">
                        This event takes place on
                        <strong><span id="eventPopupWhen"></span><span id="eventPopupWhere"></span></strong>.
                    </div>
                </div>
                <div class="row" id="eventPopupDetailsPanel">
                    <div class="col-sm-12 margin-1em-bottom">Event details:</div>
                    <div class="col-sm-12">
                        <blockquote>
                            <span id="eventPopupDescription"></span>
                        </blockquote>
                    </div>
                </div>
                <div class="row margin-1em-bottom" id="eventPopupCustom1Panel">
                    <span id="eventPopupCustomLabel1"></span>:
                    <span id="eventPopupCustomValue1"></span>
                </div>
                <div class="row margin-1em-bottom" id="eventPopupCustom2Panel">
                    <span id="eventPopupCustomLabel2"></span>:
                    <span id="eventPopupCustomValue2"></span>
                </div>
                <div class="row margin-1em-bottom" id="eventPopupCustom3Panel">
                    <span id="eventPopupCustomLabel3"></span>:
                    <span id="eventPopupCustomValue3"></span>
                </div>
                <div class="row" id="eventPopupLinkPanel">
                    <div class="col-sm-12 margin-1em-bottom">
                        See more details: <a id="eventPopupLink" href="#" target="_blank"></a>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="pull-right clearfix">
                    <button class="btn btn-default" type="button" id="eventPopupShare">
                        <span class="glyphicon glyphicon-share"></span>
                        Share this</button>
                    <button class="btn btn-default" type="button" id="eventPopupPrint"><span class="glyphicon glyphicon-print"></span></button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
</div>

<asp:ObjectDataSource ID="DistrictDataSource" runat="server"
    SelectMethod="GetFilteredDistrictDDValues"
    TypeName="GRA.SRP.DAL.LibraryCrosswalk">
    <SelectParameters>
        <asp:Parameter DefaultValue="" Name="city" Type="String" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="BranchDataSource" runat="server"
    SelectMethod="GetFilteredBranchDDValues"
    TypeName="GRA.SRP.DAL.LibraryCrosswalk">
    <SelectParameters>
        <asp:ControlParameter ControlID="SystemId" DefaultValue="0" Name="districtID"
            PropertyName="Text" Type="Int32" />
        <asp:Parameter DefaultValue="" Name="city" Type="String" />
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
                        $('#eventPopupWhere').html(" at " + data.Where);
                    }
                    if (data.ExternalLink) {
                        $('#eventPopupLinkPanel').show();
                        $('#eventPopupLink').attr("href", data.ExternalLink);
                        $('#eventPopupLink').html(data.Title);
                    } else {
                        $('#eventPopupLinkPanel').hide();
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
                    $('#eventPopupShare').click(function () {
                        location.href = "<%=Request.ApplicationPath%>Events/Details.aspx?EventId=" + eventId;
                    });

                    $('#eventPopupPanel').modal('show');
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                window.location = "Details.aspx?EventId=" + eventId;
            });
            return false;
        }
</script>
