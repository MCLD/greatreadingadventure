<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Status.ascx.cs" Inherits="GRA.SRP.Controls.Status" %>
<div class="row">
    <div class="col-xs-12">
        <span id="status-when" class="lead"></span>
        <hr style="margin-bottom: 5px !important; margin-top: 5px !important;" />
    </div>
    <div class="col-xs-12"><span id="status-value" class="odometer"></span></div>
    <div class="col-xs-12"><span id="status-what"></span></div>

    <div class="col-xs-12 margin-1em-top">
        <div class="btn-group" data-toggle="buttons">
            <label class="btn btn-default active">
                <input type="radio" name="status-display" autocomplete="off" checked value="0"><span class="glyphicon glyphicon-dashboard"></span>
            </label>
            <label class="btn btn-default">
                <input type="radio" name="status-display" autocomplete="off" value="1"><span class="glyphicon glyphicon-certificate"></span>
            </label>
            <label class="btn btn-default">
                <input type="radio" name="status-display" autocomplete="off" value="2"><span class="glyphicon glyphicon-star"></span>
            </label>
        </div>
    </div>
</div>

<script>
    var statusUpdateId;
    var firstStatusLoad = true;
    var pointsEarned;
    var badgesAwarded;
    var challengesCompleted;
    var dataSince;
    var statusRefreshCount = 0;

    function updateStatus() {
        statusRefreshCount++;
        if (statusRefreshCount > 60) {
            // give up on updates after 30 minutes
            if (statusUpdateId > 0) {
                console.log("Giving up on status updates, refresh the page to see new updates.");
                clearInterval(statusUpdateId);
            }
            return;
        }
        var jqxhr = $.ajax('<%=Request.ApplicationPath%>Handlers/Status.ashx')
            .done(function (data, textStatus, jqXHR) {
                if (data.Success) {
                    pointsEarned = data.PointsEarned;
                    badgesAwarded = data.BadgesAwarded;
                    challengesCompleted = data.ChallengesCompleted;
                    dataSince = data.Since;
                    if (firstStatusLoad) {
                        changeStatusDisplay("0");
                        statusUpdateId = setInterval(updateStatus, 30 * 1000);
                        firstStatusLoad = false;
                    } else {
                        updateCurrentDisplay();
                    }
                }
            })
            .fail(function () {
                clearInterval(statusUpdateId);
            });
    }

    $("input[name=status-display]").change(updateCurrentDisplay);

    function updateCurrentDisplay() {
        changeStatusDisplay($("input[name=status-display]:checked").val());
    }

    function changeStatusDisplay(onItem) {
        var what;
        var value;
        switch (onItem) {
            case "0":
                what = "Points Earned";
                value = pointsEarned;
                onItem++;
                break;
            case "1":
                what = "Badges Awarded";
                value = badgesAwarded;
                onItem++;
                break;
            case "2":
                what = "Challenges Completed";
                value = challengesCompleted;
                onItem = 0;
                break;
        }
        $('#status-what').html(what);
        $('#status-value').html(value);
        $('#status-when').html(dataSince);
    }

    $().ready(function () {
        updateStatus();
    });
</script>
