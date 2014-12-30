<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityHistCtl.ascx.cs" Inherits="STG.SRP.Controls.ActivityHistCtl" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
</asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
<ContentTemplate>

<div class="row" style="min-height: 400px;">
<div class="span2"></div>
	<div class="span8">
        <h1><asp:Label ID="Label1" runat="server" Text="Activity History Title"></asp:Label></h1>
        
        <table width="100%" cellpadding="5" cellspacing="0">

<asp:Panel ID="pnlFilter" runat="server" Visible="false">
        <tr><td><hr /></td></tr>

            <tr>
                <td>
                

<table  width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td><b>Family Members:</b> &nbsp;</td>
        <td>
                        <asp:DropDownList ID="PID" runat="server" AppendDataBoundItems="True" >
                        </asp:DropDownList>
        </td>
        <td>
           
<asp:Button ID="btnFilter" runat="server" Text="Activity History btn Filter" 
                 CssClass="btn e" onclick="btnFilter_Click"/>

        </td>
    </tr>
</table>

                
                </td>
            </tr>
</asp:Panel>
            <tr><td><hr /></td></tr>

<tr><td>
<table width="100%">
            <tr>
            
                <td width="200px;" valign="bottom" align="center" style="font-weight: bold; ">
                    # Points Earned 
                </td>
                <td width="" valign="bottom" align="left" style="font-weight: bold; ">
                    Reason
                </td>
                <td width="" valign="bottom" align="left" style="font-weight: bold; ">
                    Date Awarded
                </td>
            
            </tr>  

<asp:Repeater runat="server" ID="rptr" >
    <ItemTemplate>
            
            <tr>
            
                <td width="" valign="bottom" align="right" style="padding-right:75px;">
                    <%# FormatHelper.ToInt((int)Eval("NumPoints")) %>
                    
                </td>
                <td>
                    <%# Eval("AwardReason") %> 
                    <%# ((bool)Eval("isEvent")  ? " (Code: " + Eval("EventCode") + ")" : "")%>
                </td>
                <td>
                    <%# FormatHelper.ToNormalDate((DateTime)Eval("AwardDate")) %> 
                </td>
            
            </tr>              
                
                
    </ItemTemplate>
</asp:Repeater>

</table>
</td></tr>

        
        </table>
	</div> 
</div> 
<asp:Label ID="lblPID" runat="server" Text="" Visible="false"></asp:Label>

<script language="javascript" type="text/javascript">
    $(function () {
        $(".datepicker").datepicker();
    });
</script>

 
</ContentTemplate>
</asp:UpdatePanel>