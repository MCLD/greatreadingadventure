<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PublicProgramView.ascx.cs" Inherits="STG.SRP.Classes.PublicProgramView" %>
<%@ Import Namespace="STG.SRP.DAL" %>
            <div class="row"> 
                <div class="span12"> 
                    <h1 class="title-divider" style="text-align: center;">
                        <span>&nbsp;&nbsp;&nbsp;&nbsp;<%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).Title) %></span></h1>
                </div>
            </div>                        
            <div class="row">
                <div class="span3"> 
                    <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML3) %>
                </div >
                 <div class="span6"> 
                    <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML1) %>
                    <br />
                    <br />
                    <div style="text-align: center;">
                        <asp:Button ID="Button1" runat="server" width="25%"
                            Text="btnRegister" CssClass="btn register" onclick="btnRegister_Click" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="Button2" runat="server" width="25%"
                            Text="btnLogin" CssClass="btn login" onclick="btnLogin_Click"/>
                    </div>
                    <br />
                    <br />
                    <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML2 )%>
                </div >
                <div class="span3"> 
                    <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML4) %>
                </div >
            </div>
            <div class="row"> 
                <div class="span12"> 
                    <%=(Session["Program"] == null ? "" :((Programs)Session["Program"]).HTML5) %>
                </div>
            </div>   