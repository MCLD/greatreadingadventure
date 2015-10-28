<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Badges.ascx.cs" Inherits="GRA.SRP.Controls.Badges" %>

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

<asp:Panel ID="pnlList" runat="server" Visible="true">

<div class="row" style="min-height: 400px;">
	<div class="span12">
        <h1><asp:Label ID="Label1" runat="server" Text="Badges"></asp:Label></h1>
        
        <table width="100%" cellpadding="5" cellspacing="0">
            
            <asp:Repeater runat="server" ID="rptr" onitemcommand="rptr_ItemCommand" >
                <ItemTemplate>
            
            <%# (((long)Eval("Rank")) % 4 == 1 ? "<tr>" : "") %>
            
                <td width="25%" valign="bottom" align="center" style="padding-left:20px; padding-right: 20px; border-bottom: 1px solid gray">
                    <h4><%# Eval("Title") %></h4>

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