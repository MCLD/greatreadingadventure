<%@ Page Language="C#" MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true" CodeBehind="SurveyResults.aspx.cs" Inherits="GRA.SRP.ControlRoom.Modules.Setup.SurveyResults" %>

<%@ Import Namespace="GRA.SRP.DAL" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>


<%@ Register Src="../../Controls/SurveyAnswerStatsCtl.ascx" TagName="SurveyAnswerStatsCtl" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script>
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load("visualization", "1", {
            packages: ["corechart"]
        });
    </script>
    <script>

        function showhide(link, table) {
            //alert(link);
            if ($("#" + table).css('display') == 'none') {
                $("#" + table).toggle();
                $("." + link).html("hide");
            }
            else {
                $("#" + table).toggle();
                $("." + link).html("show");
            }

        }
    </script>
    <style type="text/css">
        .style2 {
            width: 50%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%" style="border: double 3px #A3C0E8;">
        <tr>
            <th class="style2"><b>Test/Survey: </b></th>
            <th>&nbsp;</th>
        </tr>
        <tr>
            <td class="style2">
                <asp:DropDownList ID="SID" runat="server" DataSourceID="odsDDSurveys"
                    DataTextField="Name" DataValueField="SID"
                    AppendDataBoundItems="True" Width="97%" AutoPostBack="True" OnSelectedIndexChanged="SID_SelectedIndexChanged">
                    <asp:ListItem Value="0" Text="[Select a Test/Survey]"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:CheckBox ID="chkGraphs" runat="server" Text="Show Graphs" />
            </td>
        </tr>
        <tr>
            <th class="style2"><b>Test/SurveySource  </b></th>
            <th><b>School  </b></th>
        </tr>
        <tr>
            <td class="style2">
                <asp:DropDownList ID="ddSource" runat="server"
                    AppendDataBoundItems="True" Width="97%">
                    <asp:ListItem Value="0" Text="[Select a Source for Additional Filtering]"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="ddSchool" runat="server" DataSourceID="odsDDSchool"
                    DataTextField="Code" DataValueField="CID"
                    AppendDataBoundItems="True" Width="97%">
                    <asp:ListItem Value="0" Text="[All Defined]"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>


        <tr>
            <td class="style2" colspan="2">&nbsp;
        <asp:Button ID="btnFilter" runat="server" Text="Filter"
            OnClick="btnFilter_Click" Width="150px" CssClass="btn-lg btn-green" />

                &nbsp;<asp:Button ID="btnClear" runat="server" Text="Clear"
                    OnClick="btnClear_Click" Width="150px" CssClass="btn-lg btn-orange" />

                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        &nbsp;<asp:Button ID="btnExport" runat="server" Text="Export"
            Width="150px" CssClass="btn-lg btn-gray" OnClick="btnExport_Click" />
            </td>
        </tr>



    </table>



    <asp:ObjectDataSource ID="odsDDSurveys" runat="server"
        SelectMethod="GetAll"
        TypeName="GRA.SRP.DAL.Survey"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="odsDDSchool" runat="server"
        SelectMethod="GetAlByTypeName"
        TypeName="GRA.SRP.DAL.Codes">
        <SelectParameters>
            <asp:Parameter Name="Name" DefaultValue="School" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:Panel ID="pnlResults" runat="server" Visible="False">
        <br />
        <asp:Label ID="lblInfo" runat="server" Font-Size="Large" ForeColor="#CC0000"></asp:Label>
        <hr />
        <br />

        <table width="100%" border="0">
            <asp:Repeater ID="rptr" runat="server">
                <ItemTemplate>
                    <asp:Label ID="SID" runat="server" Text='<%# Eval("SID") %>' Visible="false"></asp:Label>
                    <asp:Label ID="QID" runat="server" Text='<%# Eval("QID") %>' Visible="false"></asp:Label>
                    <asp:Label ID="SQMLID" runat="server" Text='<%# Eval("SQMLID") %>' Visible="false"></asp:Label>
                    <asp:Label ID="QType" runat="server" Text='<%# Eval("QType") %>' Visible="false"></asp:Label>
                    <asp:Label ID="QNumber" runat="server" Text='<%# Eval("QNumber") %>' Visible="false"></asp:Label>

                    <uc1:SurveyAnswerStatsCtl ID="SurveyAnswerStatsCtl1" runat="server"
                        SID='<%# Eval("SID") %>' QID='<%# Eval("QID") %>' SQMLID='<%# Eval("SQMLID") %>'
                        QType='<%# Eval("QType") %>' QNumber='<%# Eval("QNumber") %>' ShowQText='<%# Eval("ShowQText") %>'
                        Graphs='<%# chkGraphs.Checked %>'
                        Source='<%# ddSource.SelectedValue %>' />
                </ItemTemplate>
            </asp:Repeater>

        </table>





    </asp:Panel>

</asp:Content>
