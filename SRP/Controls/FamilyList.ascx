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
            NavigateUrl="~/Account/AddFamilyMemberAccount.aspx">
            <span class="glyphicon glyphicon-plus-sign margin-halfem-right"></span>
            <asp:Label runat="server" Text="myaccount-add-family-member"></asp:Label>
        </asp:HyperLink>
    </div>
</div>

<table class="table">
    <thead>
        <tr>
            <th colspan="3">
                <asp:Label runat="server" Text="family-list-members"></asp:Label></th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="rptr" runat="server" OnItemCommand="rptr_ItemCommand" OnItemDataBound="rptr_ItemDataBound">
            <ItemTemplate>
                <tr>
                    <td style="vertical-align: middle;">
                        <asp:Image runat="server" ID="avatarImage" width="64" Height="64" />
                    </td>
                    <td style="vertical-align: middle;">
                        <%# Eval("FirstName") + " " + Eval("LastName")%> (<%# Eval("username") %>)
                    </td>
                    <td class="clearfix" style="vertical-align: middle;">
                        <div class="pull-right margin-halfem-top">
                            <asp:Button runat="server"
                                CommandName="login"
                                CommandArgument='<%# Eval("PID") %>'
                                Text="family-list-login"
                                CssClass="btn btn-default margin-halfem-bottom" />
                            <asp:Button runat="server"
                                CommandName="log"
                                CommandArgument='<%# Eval("PID") %>'
                                Text="family-list-log"
                                CssClass="btn btn-default margin-halfem-bottom" />
                            <asp:Button runat="server"
                                CommandName="pwd"
                                CommandArgument='<%# Eval("PID") %>'
                                Text="family-list-password"
                                CssClass="btn btn-default margin-halfem-bottom" />
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
