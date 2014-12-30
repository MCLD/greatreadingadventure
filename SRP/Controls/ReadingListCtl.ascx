<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReadingListCtl.ascx.cs" Inherits="STG.SRP.Controls.ReadingListCtl" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<script language="javascript" type="text/javascript">

    function printDiv(divName) {
        var printContents = document.getElementById(divName).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;

        window.print();

        //document.location.href = "Events.aspx";

        document.body.innerHTML = originalContents;

    }
</script>
<div id="printarea"> 
    <div class="row">
	    <div class="span12">
            <h1><asp:Label ID="Label1" runat="server" Text="Reading Lists Title"></asp:Label></h1>

            <asp:Label ID="Label2" runat="server" Text="Reading Lists Instructions"></asp:Label>
	    </div> 
    </div> 
    <div class="row" style="min-height: 400px;">
	    <div class="span4">
            <br />
            <ul>
             <asp:Repeater runat="server" ID="rptr"  onitemcommand="rptr_ItemCommand">
                <ItemTemplate>
                    <li>
                        <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument='<%# Eval("BLID") %>'><%# Eval("ListName") %></asp:LinkButton>
                    </li>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Label ID="lblNoLists" runat="server" Text="Reading Lists No Lists" Visible="false"></asp:Label>
            </ul>
	    </div> 

        <div class="span8">
            <asp:Panel ID="pnlDetail" runat="server" Visible="false" Font-Underline="False">
            <h2><asp:Label ID="lblTitle" runat="server" Visible="false"></asp:Label></h2>
            <asp:Label ID="lblMessage" runat="server" Visible="False" Font-Bold="True" 
                    ForeColor="#CC0000">Your selections have been saved.<br /><br /></asp:Label>
            <asp:Label ID="lblDesc" runat="server" Visible="false"></asp:Label>
            <table width="100%" border=0 cellpadding=5>
                <tr>
                    <td></td><td  nowrap><b>Title</b></td><td  nowrap><b>Author</b></td><td  nowrap><b>ISBN</b></td>
                </tr>
                <asp:Repeater runat="server" ID="rptr2"  onitemcommand="rptr_ItemCommand">
                    <ItemTemplate>
                        <tr>
                            <td valign="top" align="center" style="padding-bottom: 7px;">
                                <asp:CheckBox ID="chkRead" runat="server" Checked='<%# Eval("HasRead") %>'/>
                                <asp:Label ID="PBLBID" runat="server" Visible="false" Text='<%# Eval("PBLBID") %>'></asp:Label>
                                <asp:Label ID="BLBID" runat="server" Visible="false" Text='<%# Eval("BLBID") %>'></asp:Label>
                                <asp:Label ID="BLID" runat="server" Visible="false" Text='<%# Eval("BLID") %>'></asp:Label>
                            </td>
                            <td valign="middle">
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("URL") %>' Target="_blank" 
                                        Visible='<%# Eval("URL").ToString().Trim().Length > 0 %>' Font-Underline='<%# Eval("URL").ToString().Trim().Length > 0 %>'><%# Eval("Title") %></asp:HyperLink>
                                <asp:Label ID="Label3" runat="server" Text='<%# Eval("Title") %>' 
                                        Visible='<%# Eval("URL").ToString().Trim().Length == 0 %>'></asp:Label>
                                </td><td nowrap><%# Eval("Author") %></td><td  nowrap><%# Eval("ISBN") %></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </table>
                <asp:Button ID="btnPrint" Visible="false" runat="server" Text="Reading Lists btn Print"  CssClass="btn e" Width="150px" OnClientClick="printDiv('printarea')"/> &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
                <asp:Button ID="btnSave" Visible="false"  runat="server" Text="Reading Lists btn Save" onclick="btnSave_Click" CssClass="btn e"  Width="150px"/>
            </asp:Panel>
	    </div> 

    </div> 
</div>

</ContentTemplate>
</asp:UpdatePanel>