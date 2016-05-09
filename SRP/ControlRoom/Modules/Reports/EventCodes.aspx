<%@ Page Title="" Language="C#" MasterPageFile="~/ControlRoom/Control.Master" AutoEventWireup="true" CodeBehind="EventCodes.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Reports.EventCodesByBranch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="row hidden-print">
            <div class="col-xs-4">
                <asp:DropDownList ID="LibraryDistrictList"
                    runat="server"
                    DataSourceID="LibraryDistrictData"
                    DataTextField="Code"
                    DataValueField="CID"
                    AppendDataBoundItems="True"
                    CssClass="form-control"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="SelectedDistrict">
                    <asp:ListItem Value="0" Text="All library districts"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-xs-4">
                <asp:DropDownList ID="LibraryBranchList"
                    runat="server"
                    DataSourceID="LibraryBranchData"
                    DataTextField="Code"
                    DataValueField="CID"
                    AppendDataBoundItems="True"
                    CssClass="form-control"
                    AutoPostBack="true">
                    <asp:ListItem Value="0" Text="All library branches"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-xs-4">
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
        <div class="row lead visible-print">
            <div class="col-xs-12">
                <asp:Literal runat="server" ID="PrintHeader" EnableViewState="false"></asp:Literal>
            </div>
        </div>
        <asp:Panel class="row margin-1em-top" EnableViewState="false" ID="ReportPanel" runat="server">
            <div class="col-xs-12">
                <table class="table table-condensed table-striped table-bordered table-hover">
                    <tr class="info">
                        <th>Event Name</th>
                        <th>Event Date</th>
                        <th>Secret Code</th>
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
                                <td><%#Eval("SecretCode") %></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </asp:Panel>
    </div>
    <asp:ObjectDataSource ID="LibraryDistrictData" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="Library District" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="LibraryBranchData" runat="server"
        SelectMethod="GetFilteredBranchDDValues"
        TypeName="GRA.SRP.DAL.LibraryCrosswalk">
        <SelectParameters>
            <asp:ControlParameter
                ControlID="LibraryDistrictList"
                DefaultValue="0"
                Name="districtId"
                PropertyName="Text"
                Type="Int32" />
            <asp:Parameter Name="city" DefaultValue="" />
        </SelectParameters>
    </asp:ObjectDataSource>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
</asp:Content>
