<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CRTopMenu.ascx.cs" Inherits="GRA.SRP.ControlRoom.CRTopMenu" %>

<script>
<!--

    /*By JavaScript Kit
    http://javascriptkit.com
    Credit MUST stay intact for use
    */

    function show2() {
        if (!document.all && !document.getElementById)
            return
        thelement = document.getElementById ? document.getElementById("tick2") : document.all.tick2
        var Digital = new Date()
        var hours = Digital.getHours()
        var minutes = Digital.getMinutes()
        var seconds = Digital.getSeconds()
        var dn = "PM"
        if (hours < 12)
            dn = "AM"
        if (hours > 12)
            hours = hours - 12
        if (hours == 0)
            hours = 12
        if (minutes <= 9)
            minutes = "0" + minutes
        if (seconds <= 9)
            seconds = "0" + seconds
        var ctime = hours + ":" + minutes + ":" + seconds + " " + dn
        thelement.innerHTML = "<b style='font-size:14;color;' class='topmenuclock'>" + ctime + "</b>"
        setTimeout("show2()", 1000)
    }
    window.onload = show2
//-->
</script>
    <div id="cdnavcont">
	    <div id="cdnavheader">
		    <ul>
                <li <% =Request.Url.AbsoluteUri.EndsWith("/ControlRoom/default.aspx") ? "id=\"current\"" : string.Empty %>><asp:HyperLink runat="server" ID="CRHome" NavigateUrl="~/ControlRoom/"><span class="glyphicon glyphicon-home"></span></asp:HyperLink></li>
		        <asp:Repeater ID="rptTabs" runat="server">
                <ItemTemplate>
                <li <%# ((bool)Eval("isSelected") ? "id=\"current\"" : "") %>><a href='<%# Eval("URL") %>'><span><div id="cr<%# NoSpaces(Eval("Name")) %>"><%# Eval("Name") %></div></span></a></li>
                </ItemTemplate>
                </asp:Repeater>
		    </ul>
	    </div>
   	    <div style="float:right">
   	        <!-- Float right -->
        </div>
	    <div style="float:right">&nbsp;&nbsp;<span id=tick2></span>&nbsp;&nbsp;</div>
	    <br style="clear:both;" />
    </div>	
    
    