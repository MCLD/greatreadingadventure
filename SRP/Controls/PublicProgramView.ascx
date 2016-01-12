<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublicProgramView.ascx.cs" Inherits="GRA.SRP.Classes.PublicProgramView" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<div class="row margin-1em-top">
    <div class="col-sm-3">
        <%= this.CurrentProgram == null ? "" : Server.HtmlDecode(this.CurrentProgram.HTML3) %>
    </div>
    <div class="col-sm-6">
        <%=this.CurrentProgram == null ? "" : Server.HtmlDecode(this.CurrentProgram.HTML1) %>

        <div class="clearfix margin-1em-top margin-1em-bottom">
            <div class="col-sm-6 margin-halfem-top">
                <a href="~/RegisterILS.aspx" runat="server" class="btn btn-info btn-block btn-lg">
                    <asp:Label runat="server" Text="frontpage-button-register"></asp:Label></a>
            </div>
            <div class="col-sm-6 margin-halfem-top">
                <a href="~/Login.aspx" runat="server" class="btn btn-success btn-block btn-lg"
                    onclick="return showLoginPopup();">
                    <asp:Label runat="server" Text="frontpage-button-login"></asp:Label></a>
            </div>
        </div>

        <%=this.CurrentProgram == null ? "" : Server.HtmlDecode(this.CurrentProgram.HTML2) %>
    </div>
    <div class="col-sm-3">
        <%=this.CurrentProgram == null ? "" : Server.HtmlDecode(this.CurrentProgram.HTML4) %>
    </div>

</div>
<div class="row">
    <div class="col-sm-12">
        <%=this.CurrentProgram == null ? "" : Server.HtmlDecode(this.CurrentProgram.HTML5) %>
    </div>
</div>
