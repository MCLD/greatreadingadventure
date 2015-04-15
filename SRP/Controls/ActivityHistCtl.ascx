<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityHistCtl.ascx.cs" Inherits="STG.SRP.Controls.ActivityHistCtl" %>
<%@ Import Namespace="STG.SRP.Utilities.CoreClasses" %>



<script>    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/all.js#xfbml=1&appId=<%=(ConfigurationManager.AppSettings["FBAPPID"] ?? "121002584737306") %>";
        fjs.parentNode.insertBefore(js, fjs);
    } (document, 'script', 'facebook-jssdk'));</script>
    
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
            
                <td width="50px;" valign="top" align="center" style="font-weight: bold; ">
                    Points 
                </td>
                <td width="" valign="top" align="left" style="font-weight: bold; ">
                    Reason
                </td>
                <td width="" valign="bottom" align="left" style="font-weight: bold; ">
                    Date Awarded
                </td>
            
            </tr>  
            <tr><td colspan="3"><hr /></td></tr>   
<asp:Repeater runat="server" ID="rptr" >
    <ItemTemplate>
            
            <tr>
                <td width="" valign="top" align="right" style="padding-right:25px;">
                    <%# ((int)Eval("NumPoints")).ToInt() %>
                </td>
                <td>
                    <%# Eval("AwardReason") %> 
                    <%# ((bool)Eval("isEvent")  ? string.Format(" <b>{0}</b>, secret code <i>{1}</i>",  Eval("EventTitle"),  Eval("EventCode")) : 
                         ((bool)Eval("isBookList")  ? string.Format(" <b>{0}</b>",  Eval("ListName")) :
                         ((bool)Eval("isReading") ? FormatReading(Eval("Author").ToString(), Eval("Title").ToString(), Eval("Review").ToString(), (int)Eval("PRID")) + string.Format("<br>(<b>{0} {1}</b>)", Eval("ReadingAmount"), Eval("ReadingType")) :
                         ((bool)Eval("isGameLevelActivity") ? string.Format(" (<b>{0}</b>)", Eval("GameName")) : 
                         " (Unknown)"))))%>
                </td>
                <td>
                    <%# ((DateTime)Eval("AwardDate")).ToNormalDate() %> 
                </td>
            </tr>              
            <tr><td colspan="3"><hr /></td></tr>                
                
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

 
