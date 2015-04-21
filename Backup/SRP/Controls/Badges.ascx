<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Badges.ascx.cs" Inherits="STG.SRP.Controls.Badges" %>

<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>

<script language="javascript" type="text/javascript">

    function printDiv(divName) {
        var printContents = document.getElementById(divName).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;

        document.location.href = "MyBadges.aspx";

        window.print();

        //document.body.innerHTML = originalContents;

    }
</script>
<div id="fb-root"></div>
<script>    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=<%=(ConfigurationManager.AppSettings["FBAPPID"] ?? "121002584737306") %>";
        fjs.parentNode.insertBefore(js, fjs);
    } (document, 'script', 'facebook-jssdk'));</script>



<asp:Panel ID="pnlList" runat="server" Visible="true">

<div class="row" style="min-height: 400px;">
	<div class="span12">
        <h1><asp:Label ID="Label1" runat="server" Text="Badges Title"></asp:Label></h1>
        
        <table width="100%" cellpadding="5" cellspacing="0">
            
            <asp:Repeater runat="server" ID="rptr" onitemcommand="rptr_ItemCommand" >
                <ItemTemplate>
            
            <%# (((long)Eval("Rank")) % 4 == 1 ? "<tr>" : "") %>
            
                <td width="25%" valign="bottom" align="center" style="padding-left:20px; padding-right: 20px; border-bottom: 1px solid gray">
                    <h4><%# Eval("Title") %></h4>

                    <div class="fb-share-button" data-href='<%# Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/ShareBadge.aspx?BID=" + Eval("BadgeID") %>' data-type="button"></div>
                    <!--
                    <div class="fb-share-button" data-href='<%# "http://srp.stglink.com/ShareBadge.aspx?BID=" + Eval("BadgeID") %>' data-type="button"></div>
                    
                    <a href="https://www.facebook.com/sharer/sharer.php?u=<%# Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/ShareBadge.aspx?BID=" + Eval("BadgeID") %>" target="_blank">
                      Share on Facebook
                    </a>
                    -->

                    <br /><br />

                    <img src='/images/badges/sm_<%# Eval("BadgeID") %>.png' />
                    <br /><br />

                    <asp:Button ID="btnView" runat="server" Text="View" CommandArgument='<%# Eval("PBID") %>' CssClass="btn e" />
                    <br />&nbsp;
                </td>
            
            <%# (((long)Eval("Rank")) % 4 == 0 ? "</tr>" : "") %>                
                
                
                </ItemTemplate>
            </asp:Repeater>
            <asp:Label ID="NoBadges" runat="server" Text="You have not earned any badges yet." Visible="false"></asp:Label>
        
        
        </table>
	</div> 
</div> 

</asp:Panel>



<asp:Panel ID="pnlDetail" runat="server" Visible="false">



<div class="row" style="min-height: 400px;" >

	<div class="span12">
        
        <div id="printarea">         
            <center>
            <h2><asp:Label ID="lblTitle" runat="server" Text=""></asp:Label></h2>
            
            <br />
            <asp:Image ID="Badge" runat="server" />
            <br /><br />
            </center>
        </div>
              
        <center>
        <br /><br /><br /><br />


        <asp:Button ID="Button1" runat="server" Text="Offers btn Print"  CssClass="btn e" Width="150px" OnClientClick="printDiv('printarea')"/> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
        <asp:Button ID="btnList" runat="server" Text="Go Back" onclick="btnList_Click" CssClass="btn e"  Width="150px"/>
        </center>

	</div> 

</div> 

</asp:Panel>




</ContentTemplate>
</asp:UpdatePanel>