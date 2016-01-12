<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatronNotificationsCtl.ascx.cs" Inherits="GRA.SRP.Controls.PatronNotificationsCtl" %>
<%@ Import Namespace="GRA.SRP.Utilities.CoreClasses" %>

<div class="row">
    <div class="col-sm-12 hidden-print">
        <span class="h1">
            <asp:Label ID="Label1" runat="server" Text="notifications-title"></asp:Label></span>
    </div>
</div>

<asp:Panel ID="pnlList" runat="server" Visible="true">
    <div class="row">
        <div class="col-sm-12 hidden-print margin-halfem-top">
            <button runat="server" onserverclick="btnAsk_Click" class="btn btn-default margin-halfem-bottom">
                <span class="glyphicon glyphicon-pencil margin-halfem-right"></span>
                Write a message</button>
        </div>

        <div class="col-sm-12 margin-1em-top">
            <p>Do you have questions about the reading program or prizes? Ask here by writing a message!</p>
        </div>

        <%if(this.UserHasMessages) { %>
        <div class="col-sm-12 margin-1em-top">
            <table class="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th style="font-size: larger;">Date
                        </th>
                        <th style="font-size: larger;">Message
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="rptr" OnItemCommand="rptr_ItemCommand">
                        <ItemTemplate>
                            <tr <%#SuccessIfTrue(Eval("isUnread")) %>>
                                <td style="font-size: larger;">
                                    <asp:LinkButton ID="LinkButton2" runat="server" Text='<%# FormatHelper.ToNormalDate((DateTime)Eval("AddedDate")) %>' CommandArgument='<%# Eval("NID") %>'
                                        Font-Bold='<%# Eval("isUnread") %>'></asp:LinkButton>
                                </td>
                                <td style="font-size: larger;">
                                    <asp:LinkButton ID="LinkButton1" runat="server" Text='<%# Eval("Subject") %>' CommandArgument='<%# Eval("NID") %>'
                                        Font-Bold='<%# Eval("isUnread") %>'></asp:LinkButton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <% } else { %>
        <div class="col-sm-12 margin-1em-top">
            <p><em>You currently have no messages.</em></p>
        </div>
        <% } %>
    </div>
</asp:Panel>


<asp:Panel ID="pnlDetail" runat="server" Visible="false">
    <div class="row">
        <div class="col-sm-12 hidden-print margin-halfem-top">
            <button runat="server" onserverclick="btnList_Click" class="btn btn-default margin-halfem-bottom">
                <span class="glyphicon glyphicon-th-list margin-halfem-right"></span>
                Go back</button>
            <button runat="server" onserverclick="btnDelete_Click" class="btn btn-danger margin-halfem-bottom">
                <span class="glyphicon glyphicon-remove margin-halfem-right"></span>
                Delete
            </button>
            <button class="btn btn-default  margin-halfem-bottom" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>
            <button runat="server" onserverclick="btnAsk_Click" class="btn btn-default  margin-halfem-bottom">
                <span class="glyphicon glyphicon-pencil margin-halfem-right"></span>
                Write another message</button>
        </div>
        <div class="col-sm-12">
            <hr class="hidden-print" />
            <p>
                <strong>From:</strong>
                <asp:Label ID="lblFrom" runat="server"></asp:Label>
            </p>
            <p>
                <strong>Subject:</strong>
                <asp:Label ID="lblTitle" runat="server"></asp:Label>
            </p>
            <p>
                <strong>Received:</strong>
                <asp:Label ID="lblReceived" runat="server"></asp:Label>
            </p>
            <asp:Label ID="lblBody" runat="server"></asp:Label>
            <asp:Label ID="NID" runat="server" Visible="false"></asp:Label>
            <hr class="hidden-print" />
        </div>
    </div>
</asp:Panel>

<asp:Panel ID="pnlAsk" runat="server" Visible="false">
    <div class="row">
        <div class="col-sm-12 hidden-print margin-halfem-top">
            <button runat="server" onserverclick="btnList_Click" class="btn btn-default margin-halfem-bottom">
                <span class="glyphicon glyphicon-th-list margin-halfem-right"></span>
                Go back</button>
            <button runat="server" onserverclick="btnAskSubmit_Click" class="btn btn-success margin-halfem-bottom">
                <span class="glyphicon glyphicon-send margin-halfem-right"></span>
                Send message</button>
        </div>
        <div class="col-sm-12">
            <div class="lead margin-1em-top margin-1em-bottom">
                <asp:Label ID="Label2" runat="server" Text="notifications-send-message"></asp:Label>
            </div>
            <div class="form-group <%= this.SubjectHasError %>">
                <asp:Label AssociatedControlID="txtSubject" runat="server" CssClass="control-label">Subject</asp:Label>
                <%if(!string.IsNullOrWhiteSpace(this.SubjectHasError)) { %>
                <span class="help-block"><strong>Please enter a subject for your message.</strong></span>
                <% } %>
                <asp:TextBox ID="txtSubject" runat="server" Text="" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-group margin-1em-top <%= this.BodyHasError %>">
                <asp:Label AssociatedControlID="txtBody" runat="server" CssClass="control-label">Question</asp:Label>
                <%if(!string.IsNullOrWhiteSpace(this.BodyHasError)) { %>
                <span class="help-block"><strong>Please enter your question below.</strong></span>
                <% } %>
                <asp:TextBox ID="txtBody" runat="server" Text="" Rows="10"
                    TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
            </div>
        </div>
    </div>
</asp:Panel>
