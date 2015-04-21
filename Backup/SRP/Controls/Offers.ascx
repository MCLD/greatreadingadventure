<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Offers.ascx.cs" Inherits="STG.SRP.Controls.Offers" %>

<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>

<script language="javascript" type="text/javascript">

    function printDiv(divName) {
        var printContents = document.getElementById(divName).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;
 
        document.location.href = "MyOffers.aspx";
 
        window.print();

        //document.body.innerHTML = originalContents;
        
    }
</script>

<asp:Panel ID="pnlList" runat="server" Visible="true">

<div class="row" style="min-height: 400px;">
	<div class="span12">
        <h1><asp:Label ID="Label1" runat="server" Text="Offers Title"></asp:Label></h1>
        
        <table width="100%" cellpadding="5" cellspacing="0">
            
            <asp:Repeater runat="server" ID="rptr" onitemcommand="rptr_ItemCommand" >
                <ItemTemplate>
            
            <%# (((long)Eval("Rank")) % 4 == 1 ? "<tr>" : "") %>
            
                <td width="25%" valign="bottom" align="center" style="padding-left:20px; padding-right: 20px;padding-bottom: 20px;">
                    <h5><%# Eval("Title") %></h5>
                    <img src='/images/offers/md_<%# Eval("OID") %>.png' />
                    <br /><br />
                    <asp:Button ID="btnView" runat="server" Text="View" CommandArgument='<%# Eval("OID") %>' Visible='<%# !(bool)Eval("ExternalRedirectFlag") %>' CssClass="btn e" />
                    <asp:HyperLink ID="HyperLink1" runat="server" Target="_blank" NavigateUrl='<%# Eval("RedirectURL") %>' Visible='<%# (bool)Eval("ExternalRedirectFlag") %>' CssClass="btn e" >View</asp:HyperLink>
                    <hr />
                </td>
            
            <%# (((long)Eval("Rank")) % 4 == 0 ? "</tr>" : "") %>                
                
                
                </ItemTemplate>
            </asp:Repeater>
        
        
        
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
            <asp:Image ID="Coupon" runat="server" />
            <br /><br />
            <asp:Label ID="Label2" runat="server" Text="Offers Serial"></asp:Label><asp:Label ID="lblSerial" runat="server" Text=""></asp:Label>

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