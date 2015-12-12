<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Feed.ascx.cs" Inherits="GRA.SRP.Controls.Feed" %>
<div class="row">
    <div class="col-xs-12 text-center">
        <span class="lead">Feed</span>
        <hr style="margin-bottom: 5px !important; margin-top: 5px !important;" />
    </div>
    <div class="col-xs-12" id="activity-feed">
    </div>
</div>

<script>
    var rootPath = '<%=Request.ApplicationPath%>';
    var feedUpdateId = 0;
    var latest = 0;

    $().ready(function () {
        loadFeed();
    });

    function loadFeed() {
        var jqxhr = $.ajax(rootPath + 'Handlers/Feed.ashx?after=' + latest)
            .done(function (data, textStatus, jqXHR) {
                if (data.Success) {
                    if (data.Latest > latest) {
                        var html = [];
                        var newLatest = latest;
                        var addedRows = 0;
                        // new records
                        if (latest == 0) {
                            html.push('<table class="feed-table" id="activity-feed-table">');
                        }
                        $.each(data.Entries, function (index, entry) {
                            if (entry["ID"] > latest) {
                                addedRows++;
                                html.push('<tr><td class="feed-avatar">');
                                html.push('<img Width="24" Height="24" src="' + rootPath + 'Images/Avatars/sm_' + entry.AvatarId + '.png" class="margin-1em-right"/></td>');
                                html.push('<td><strong>' + entry.Username + '</strong>');
                                switch (entry.AwardReasonId) {
                                    case 1:
                                        // badge
                                        html.push(' earned the <a href="' + rootPath + 'Badges/Details.aspx?BadgeId=' + entry.BadgeId + '"');
                                        html.push('onClick="return HideTooltipShowBadgeInfo(this.parentElement, ' + entry.BadgeId + ');">' + entry.AchievementName + ' badge</a>.');
                                        break;
                                    case 2:
                                        // challenge
                                        html.push(' completed the <a href="' + rootPath + 'Challenges/Details.aspx?blid=' + entry.ChallengeId + '">');
                                        html.push(entry.AchievementName + ' challenge</a>');
                                        if (entry.BadgeId && entry.BadgeId > 0) {
                                            html.push(' and <a href="' + rootPath + 'Badges/Details.aspx?BadgeId=' + entry.BadgeId + '"');
                                            html.push('onClick="return HideTooltipShowBadgeInfo(this.parentElement, ' + entry.BadgeId + ');">earned a badge</a>.');
                                        }
                                        break;
                                }
                                html.push('</td></tr>');
                                if (entry["ID"] > newLatest) {
                                    newLatest = entry["ID"];
                                }
                            }
                        });
                        if (latest == 0) {
                            html.push('</table>');
                            $("#activity-feed").after(html.join(""));
                        } else {
                            $('#activity-feed-table > tbody > tr:first').before(html.join(""));
                            for(i = 0; i < addedRows; i++) {
                                $('#activity-feed-table tr:last').remove();
                            }
                        }
                        latest = newLatest;
                        feedUpdateId = setInterval(loadFeed, 30 * 1000);
                    }
                }
            })
            .fail(function () {
                clearInterval(feedUpdateId);
            });
    }
</script>
