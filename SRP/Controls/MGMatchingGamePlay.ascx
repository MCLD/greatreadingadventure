<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MGMatchingGamePlay.ascx.cs" Inherits="GRA.SRP.Controls.MGMatchingGamePlay" %>
<asp:Label ID="MAGID" runat="server" Text="" Visible="false"></asp:Label>
<asp:Label ID="Difficulty" runat="server" Text="1" Visible="false"></asp:Label>


<asp:Label ID="Label1" runat="server" Text="0" Visible="false"></asp:Label>
<asp:Label ID="Label2" runat="server" Text="0" Visible="false"></asp:Label>


<div class="row">
    <div class="span3"></div>
	<div class="span6">
        <center style="height: 70px;">
            <span id="Message" class="gameMessage" style="font-weight: bold; font-size: larger;"></span>
            <div id="CompletedGameButtons" style="display: none;">
                    <asp:Button ID="btnContinue" runat="server" Text="Continue"  CssClass="btn e"  
                        Width="150px" onclick="btnContinue_Click"/>  
                    <br /><br />        
            </div>
        </center>

        <div style="border: 5px solid silver;" >
        
            <table width="100%" cellspacing="0" cellpadding="0" border="0" id="GameBoard" style="position: relative; z-index: 0; ">
                <%=Grid %>    
            </table>

            <table width="100%" cellspacing="0" cellpadding="0" border="1" id="BoardGrid" 
                    style="position:absolute; z-index: 200; 
            ">
                <%=PlainGrid %>                                                             
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
    var matchList = <%=GameMatches.ToString() %>;
    var done = <%=MatchTracking.ToString() %>;
    var remaining = size * size / 2;


    var ClickCount = 0;
    var Click1ID= string.Empty;
    var Click2ID= string.Empty;
    var Click1TDID= string.Empty;
    var Click2TDID= string.Empty;
    var Cell1= string.Empty;
    var Cell2= string.Empty;
    var ClickCompleteWithNoMatch = false;

    $(document).ready(function () {
        var h2 = Math.round($("#GameBoard").height() / size);
        var w2 = Math.round($("#GameBoard").width() / size);
        
        for (i = 1 ; i <= size*size ; i++) {
            //$("#Td" + i).html(dict[i-1]);
            $("#Td" + i).bind('click', { param1: matchList[i-1],  param2: "#Td" + i, param3: (i-1)}, onSquareClicked);
        }

        //setClickTile();
        

        $(".BoardSquare color").height(h2);
        $(".BoardSquare color").width(w2);
        $(window).trigger('resize');
        
    });

    
    /*
    setClickTile = function (value) {
       

        var rnd1 = Math.floor(Math.random()*9);                 // 0 - 8 --> whwere to start iterating
        var rnd2 = Math.floor(Math.random()*remaining) ;        // 0 to remaining - how many are still covered
        
        do 
        {
            rnd1 = Math.floor(Math.random()*9); 
        }
        while (done[rnd1]);
        $("#CurrWord").html(dict[rnd1]);

    }
    */

    onSquareClicked = function (value) {

        if (done[value.data.param3] == true) return; // clicked on a cell that was already uncovered and matched
        
        ClickCount = ClickCount + 1;

        if (ClickCount == 1) {  // Just clicked the first tile/item
            if (ClickCompleteWithNoMatch) {
                $(Click1TDID).html("").addClass("color");
                $(Click2TDID).html("").addClass("color");
            }

            Click1ID    = value.data.param1;
            Click1TDID  = value.data.param2;
            Cell1       = value.data.param3; 

            $(Click1TDID).html("").removeClass("color");
            $("#Message").html("");
        }


        if (ClickCount == 2) {  // Just clicked the second tile/item
            Click2ID    = value.data.param1;
            Click2TDID  = value.data.param2;
            Cell2       = value.data.param3; 

            $(Click2TDID).html("").removeClass("color");      
            
            if (Click1TDID == Click2TDID) { //clicked the same tile...
                $(Click1TDID).html("").addClass("color");
                ClickCount = 0;
                ClickCompleteWithNoMatch = false;
                return;
            }

            if (Click1ID == Click2ID) { //clicked a match ...
                var checkmark = "<img src='/Images/checkmark.png' border='0'  style='max-width:100%; max-height: 100%;'>"
//                $(Click1TDID).html(checkmark).addClass("color");
//                $(Click2TDID).html(checkmark).addClass("color");
                $(Click1TDID).html(checkmark).removeClass("color");
                $(Click2TDID).html(checkmark).removeClass("color");
                done[Cell1] = true;
                done[Cell2] = true;
                ClickCount = 0;
                remaining = remaining - 1;
                $("#Message").html("");
                if (remaining == 0) {
                    $("#Message").html("You completed the puzzle!");
                    $("#CompletedGameButtons").toggle();
                }
                ClickCompleteWithNoMatch = false;
                return;
            }
            

            if (Click1ID != Click2ID) { //clicked but no match ...
                
                var incorrect = "<img src='/Images/incorrect.png' border='0'   style='max-width:100%; max-height: 100%;'>"
                $(Click1TDID).html(incorrect).removeClass("color");
                $(Click2TDID).html(incorrect).removeClass("color");
                //$(Click1TDID).addClass("color");
                //$(Click2TDID).addClass("color");
                ClickCompleteWithNoMatch = true;
                ClickCount = 0;
                 $("#Message").html("Incorrect, try again!");
                return;
            }
                              
        }        
    }
    

    $(window).on('resize', function () {
        ResizeBoard();
    }).trigger('resize');     

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
        var h = w;//Math.round($("#GameBoard").height());
        var h2 = w2;//Math.round($("#GameBoard").height() / size);
        $("#GameBoard").height(h);


        $("#BoardGrid").height(h);
        $("#BoardGrid").width(w);
        $("#BoardGrid").css('top', $("#GameBoard").offset().top + "px");
        $(".BoardSquare color").height(h2).width(w2);
        $(".BoardSquare").height(h2).width(w2);
    }
</script> 



