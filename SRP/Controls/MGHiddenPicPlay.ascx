<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGHiddenPicPlay.ascx.cs" Inherits="GRA.SRP.Controls.MGHiddenPicPlay" %>


<script language="javascript" type="text/javascript">

    function printDiv(divName) {
        var printContents = document.getElementById(divName).innerHTML;
        var originalContents = document.body.innerHTML;

        document.body.innerHTML = printContents;

        //document.location.href = "MyProgram.aspx";

        window.print();

        //document.body.innerHTML = originalContents;

    }


		Popup = {

		init : function () {
			$('a.action_print').bind('click', Popup.printIt);
		},

		printIt : function () {
			var win = window.open('', 'Image', 'resizable=yes,...');
			if (win.document) {
				win.document.writeln('<img src="'+ $(this).attr('href') +'" alt="image" />');
				win.document.close();
				win.focus();
				win.print();
			}

			return false;
		}
	}

	$(document).ready(function () {
		Popup.init();
	});


	// -->


</script>




<asp:Label ID="HPID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>
<asp:Label ID="Dict" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="Label1" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="Label2" runat="server" Text="0" Visible="false"></asp:Label>
<div class="row">
    <div class="span3"></div>
	<div class="span6">
        <center><h2 ID="CurrWord"></h2><br></center>
        <center><span id="Message" style="font-weight: bold; font-size: larger;"></span>
            <div id="PrintButtons" style="display: none;">
                    <asp:HyperLink ID="printLink"  Text="Print" Target="_blank"  CssClass="btn e" Width="150px" runat="server"></asp:HyperLink>
                    <asp:Button ID="btnContinue" runat="server" Text="Continue"  CssClass="btn e" Width="150px" onclick="btnContinue_Click"/>  
                    <br /><br />        
            </div>
        </center>
        <div style="border: 5px solid silver;">
        
        <table width="100%" cellspacing="0" cellpadding="0" border="0" id="GameBoard" style="position: relative; z-index: 0; ">
            <tr>
                <td>
                    <div id="printArea">
                        <asp:Image ID="BoardImg" runat="server" Width="100%"  alt="Game Board" class="bkImg"/>
                    </div>
                </td>
            </tr>
        </table>
        <table width="100%" cellspacing="0" cellpadding="0" border="1" id="BoardGrid" 
                style="position:absolute; z-index: 200; 
        ">
            <%=Grid %>       
                                                      
        </table>
        </div>
        <div class="span3"></div>
	</>
</div>

<style>
    .color { background: white; width: 33%; font-weight: bold; cursor: pointer ; }
    .bkImg { min-height: 700px;}
</style>

<script type="text/javascript">
    var size = <%=BoardSize.ToString() %>;
    var dict = <%=GameDictionary.ToString() %>;// ["a", "b", "c", "d", "e", "f", "g", "h", "i"];  //<%=GameDictionary.ToString() %>
    var done = [false,false,false,false,false,false,false,false,false];
    var remaining = 9;

    $(document).ready(function () {
        var h2 = Math.round($("#GameBoard").height() / size);
        var w2 = Math.round($("#GameBoard").width() / size);
        for (i = 1 ; i <= 9 ; i++) {
            //$("#Td" + i).html(dict[i-1]);
            //$("#Td" + i).bind('click', { param1: dict[i-1],  param2: "#Td" + i, param3: (i-1)}, onSquareClicked);
            var t = getAlternateTermValue(dict[i-1]);
            $("#Td" + i).html(t);
            $("#Td" + i).bind('click', { param1: t,  param2: "#Td" + i, param3: (i-1)}, onSquareClicked);
        }

        setClickTile();

        $(".BoardSquare color").height(h2);
        $(".BoardSquare color").width(w2);
        $(window).trigger('resize');
        
    });

    getMainTermValue = function (value) {
        var s = value.toString();
        if (s.indexOf("=(") > 0) {
            return s.substring(0, s.indexOf("=("));
        }
        else
        {
            return value;
        }
    }

    getAlternateTermValue = function (value) {
        var s = value.toString();
        if (s.indexOf("=(") > 0) {
            s = s.substring(s.indexOf("=("));
            s = s.replace("=(","").replace(")","");
            var l = s.split(",");
            return l[getRandomInt(0,l.length-1)];
        }
        else
        {
            return value;
        }
    }

    setClickTile = function (value) {
        //$("#CurrWord").html(dict[2]);
        var rnd1 = Math.floor(Math.random()*9);                 // 0 - 8 --> where to start iterating
        var rnd2 = Math.floor(Math.random()*remaining) ;        // 0 to remaining - how many are still covered
        
        do{rnd1 = Math.floor(Math.random()*9);}while (done[rnd1]);
        //$("#CurrWord").html(dict[rnd1]);
        var t = getMainTermValue(dict[rnd1]);
        $("#CurrWord").html(t);

    }

    onSquareClicked = function (value) {
        //if (value.data.param1 == $("#CurrWord").html()) {
        if (getMainTermValue(dict[value.data.param3]) == $("#CurrWord").html()) {
            $("#Message").html("");
            $(value.data.param2).html("").removeClass("color");
            $(value.data.param2).off('click');
            done[value.data.param3] = true;
            remaining = remaining - 1;
            if (remaining >= 1)
            { setClickTile(); }
            else
            { 
                $("#CurrWord").html("You completed the puzzle!");
                $("#PrintButtons").toggle();
            }

        }
        else {
            $("#Message").html("Incorrect, try again!");
            setClickTile();
        }
    }

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }


    $(window).on('resize', function () {
        ResizeBoard();
    }).trigger('resize');     //on page load

    jQuery.fn.onPositionChanged = function (trigger, millis) {
        if (millis == null) millis = 100;
        var o = $(this[0]); // our jquery object
        if (o.length < 1) return o;

        var lastPos = null;
        var lastOff = null;
        setInterval(function () {
            if (o == null || o.length < 1) return o; // abort if element is non existend eny more
            if (lastPos == null) lastPos = o.position();
            if (lastOff == null) lastOff = o.offset();
            var newPos = o.position();
            var newOff = o.offset();
            if (lastPos.top != newPos.top || lastPos.left != newPos.left) {
                $(this).trigger('onPositionChanged', { lastPos: lastPos, newPos: newPos });
                if (typeof (trigger) == "function") trigger(lastPos, newPos);
                lastPos = o.position();
            }
            if (lastOff.top != newOff.top || lastOff.left != newOff.left) {
                $(this).trigger('onOffsetChanged', { lastOff: lastOff, newOff: newOff });
                if (typeof (trigger) == "function") trigger(lastOff, newOff);
                lastOff = o.offset();
            }
        }, millis);

        return o;
    };

    $("#GameBoard").onPositionChanged(function () { ResizeBoard(); }, 5);

    function ResizeBoard() {
        var w = Math.round($("#GameBoard").width());
        var w2 = Math.round($("#GameBoard").width() / size);
        var h = Math.round($("#GameBoard").height());
        var h2 = Math.round($("#GameBoard").height() / size);
        $("#GameBoard").height(h);
        $("#<%=BoardImg.ClientID %>").height(h);
        //alert("<%=BoardImg.ClientID %>");
        $("#BoardGrid").height(h);
        $("#BoardGrid").width(w);
        $("#BoardGrid").css('top', $("#GameBoard").offset().top + "px");
        $(".BoardSquare color").height(h2).width(w2);
        $(".BoardSquare").height(h2).width(w2);
    }
</script> 