<%@ Page
    Language="C#"
    MasterPageFile="~/ControlRoom/Control.Master"
    AutoEventWireup="true"
    CodeBehind="PatronProgramRewardCodes.aspx.cs"
    Inherits="GRA.SRP.ControlRoom.Modules.Patrons.PatronProgramRewardCodes" %>

<%@ Register Src="~/ControlRoom/Controls/PatronContext.ascx" TagPrefix="uc1" TagName="PatronContext" %>


<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <uc1:PatronContext runat="server" ID="PatronContext" />
    <div class="container-fluid">
        <div class="row">
            <asp:Panel runat="server" id="NoCode" Visible="false" CssClass="alert alert-danger col-sm-8 col-sm-offset-2" style="font-size: 1.5em;">
                <span class="glyphicon glyphicon-alert"></span>&nbsp&nbsp;No program reward codes found for this patron.
            </asp:Panel>

            <div class="col-xs-12">
                <asp:Repeater runat="server" ID="EarnedCodeRepeater">
                    <HeaderTemplate>
                        <table class="table table-condensed table-hover table-bordered">
                            <tr style="background-color: #C1DBFA;">
                                <th>Date awarded</th>
                                <th>Code</th>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><%# Eval("DateUsed") %></td>
                            <td><%# Eval("ShortCode") %></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
