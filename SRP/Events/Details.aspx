<%@ Page Language="C#" MasterPageFile="~/Layout/SRP.Master" AutoEventWireup="true" CodeBehind="Details.aspx.cs" Inherits="GRA.SRP.Events.Details" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-sm-10 col-sm-offset-1 col-md-8 col-md-offset-2">
            <asp:Panel runat="server" ID="eventDetails" class="panel panel-default">
                <div class="panel-heading">
                    <span class="lead">
                        <asp:Label runat="server" Text="events-prompt"></asp:Label>
                        <asp:Label runat="server" ID="eventTitle"></asp:Label></span>
                </div>
                <div class="panel-body">
                    <p>
                        This event takes place on
                        <strong>
                            <asp:Label runat="server" ID="eventWhen"></asp:Label></strong><asp:Label runat="server" ID="atLabel"> at </asp:Label><strong><asp:Label runat="server" ID="eventWhere"></asp:Label><asp:Hyperlink runat="server" ID="eventWhereLink" Target="_blank" title="Show me this location's Web page in a new window"></asp:Hyperlink><asp:Hyperlink runat="server" ID="eventWhereMapLink" Target="_blank" Visible="false" CssClass="event-branch-detail-glyphicon hidden-print" title="Show me a map of this location in a new window"><span class="glyphicon glyphicon-map-marker"></span></asp:Hyperlink></strong>.
                    </p>
                    <p>Event details:</p>
                    <blockquote>
                        <asp:Label runat="server" ID="eventDescription"></asp:Label>
                    </blockquote>
                    <asp:Panel runat="server" CssClass="margin-1em-top" ID="eventCustom1Panel">
                        <asp:Label runat="server" ID="eventCustomLabel1"></asp:Label>:
                    <asp:Label runat="server" ID="eventCustomValue1"></asp:Label>
                    </asp:Panel>
                    <asp:Panel runat="server" CssClass="margin-1em-top" ID="eventCustom2Panel">
                        <asp:Label runat="server" ID="eventCustomLabel2"></asp:Label>: 
                    <asp:Label runat="server" ID="eventCustomValue2"></asp:Label>
                    </asp:Panel>
                    <asp:Panel runat="server" CssClass="margin-1em-top" ID="eventCustom3Panel">
                        <asp:Label runat="server" ID="eventCustomLabel3"></asp:Label>: 
                    <asp:Label runat="server" ID="eventCustomValue3"></asp:Label>
                    </asp:Panel>
                    <asp:Panel runat="server" CssClass="row margin-1em-top" ID="eventLinkPanel">
                        <div class="col-xs-12">
                            <span class="hidden-print">See more details:
                                <asp:HyperLink runat="server" ID="ExternalLink" Target="_blank"></asp:HyperLink></span>
                            <span class="visible-print">See more details: <small>
                                <asp:Literal runat="server" ID="ExternalLinkPrint"></asp:Literal></small></span>
                        </div>
                    </asp:Panel>
                </div>
                <div class="panel-footer clearfix hidden-print">
                    <div class="pull-right">
                        <button class="btn btn-default" type="button" onclick="window.print();"><span class="glyphicon glyphicon-print"></span></button>
                        <asp:HyperLink runat="server" ID="eventBackLink" CssClass="btn btn-default">Back</asp:HyperLink>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <asp:Label runat="server" ID="Microdata" CssClass="hidden"></asp:Label>
    <script>
        if(<%=this.PrintPage%> == true) {
            window.print();
        }
    </script>
</asp:Content>
