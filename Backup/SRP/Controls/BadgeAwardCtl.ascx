<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BadgeAwardCtl.ascx.cs" Inherits="STG.SRP.Controls.BadgeAwardCtl" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<div class="row" style="min-height: 400px;">
    <div class="span2"></div>
    <div class="span8">
        <h1><asp:Label ID="Title" runat="server" Text=""></asp:Label></h1>


        <table width="100%">
            <tr>
<asp:Repeater runat="server" ID="rptr" >
    <ItemTemplate>
            
            <tr>
            
           
                   
                <td align="center">
                    <h3><%# Eval("UserName") %></h3><br />
                    <div class="fb-share-button" data-href='<%# Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/ShareBadge.aspx?BID=" + Eval("BID") %>' data-type="button"></div>
                    <br /><br />

                    <img src='/images/badges/<%# Eval("BID") %>.png' />
                    <br /><br />
                    <asp:Label ID="Msg" runat="server" Text='<%# Eval("CustomEarnedMessage") %>'></asp:Label>
                    
                    <hr />
                </td>
            
            </tr>              
                
                
    </ItemTemplate>
</asp:Repeater>

</tr></table>

        <center>
        <asp:Button ID="btnContinue" runat="server" Text="Continue"  CssClass="btn d" 
                Width="200px" onclick="btnContinue_Click" />
        </center>

    </div>
</div>