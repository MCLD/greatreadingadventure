<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FamilyList.ascx.cs" Inherits="GRA.SRP.Controls.FamilyList" %>

<div class="row">
    <div class="col-sm-12 margin-1em-bottom">
        <span class="h1">
            <asp:Label runat="server" Text="family-list-title"></asp:Label></span>
    </div>
</div>

<div class="row margin-1em-bottom">
    <div class="col-sm-12">
        <asp:HyperLink CausesValidation="false"
            CssClass="btn btn-default"
            runat="server"
            NavigateUrl="~/Account/AddChildAccount.aspx">
            <span class="glyphicon glyphicon-plus-sign margin-halfem-right"></span>
            <asp:Label runat="server" Text="myaccount-add-child"></asp:Label>
        </asp:HyperLink>
    </div>
</div>

<table class="table table-striped table-hover">
    <thead>
        <tr>
            <th colspan="2">
                <asp:Label runat="server" Text="family-list-members"></asp:Label></th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="rptr" runat="server" OnItemCommand="rptr_ItemCommand">
            <ItemTemplate>
                <tr>
                    <td style="vertical-align: middle;">
                        <%# Eval("FirstName") + " " + Eval("LastName")%> (<%# Eval("username") %>)
                    </td>
                    <td class="clearfix">
                        <div class="pull-right">
                            <asp:Button runat="server"
                                CommandName="login"
                                CommandArgument='<%# Eval("PID") %>'
                                Text="family-list-login"
                                CssClass="btn btn-default" />
                            <asp:Button runat="server"
                                CommandName="log"
                                CommandArgument='<%# Eval("PID") %>'
                                Text="family-list-log"
                                CssClass="btn btn-default" />
                            <asp:Button runat="server"
                                CommandName="act"
                                CommandArgument='<%# Eval("PID") %>'
                                Text="family-list-manage"
                                CssClass="btn btn-default" />
                            <asp:Button runat="server"
                                CommandName="pwd"
                                CommandArgument='<%# Eval("PID") %>'
                                Text="family-list-password"
                                CssClass="btn btn-default" />
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
