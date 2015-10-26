<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublicProgramView.ascx.cs" Inherits="GRA.SRP.Classes.PublicProgramView" %>
<%@ Import Namespace="GRA.SRP.DAL" %>
<div class="row" style="margin-top: 1em;">
    <div class="col-sm-3">
        <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML3) %>
    </div>
    <div class="col-sm-6">
        <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML1) %>

        <div class="text-center" style="margin-top: 2em; margin-bottom: 2em;">
            <asp:Button ID="Button1" runat="server" Width="25%"
                Text="btnRegister" CssClass="btn register" OnClick="btnRegister_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="Button2" runat="server" Width="25%"
                            Text="btnLogin" CssClass="btn login" OnClick="btnLogin_Click" />
        </div>
        <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML2 )%>
    </div>
    <div class="col-sm-3">
        <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML4) %>
    </div>
</div>
<div class="row">
    <div class="col-sm-12">
        <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML5) %>
    </div>
</div>
