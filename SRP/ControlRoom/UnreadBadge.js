if (window.jQuery) {
    $().ready(function () {
        if ($("#crMail")) {
            var updateUnreadId;
            var unreadUpdateCount = 0;
            $('#crMail').css('white-space', 'no-wrap');
            $('#crMail').html($('#crMail').html() + ' <div id="unreadBadge" style="display: none;" class="label label-danger label-as-badge text-primary"></div>')
            function updateUnread() {
                unreadUpdateCount++;
                if (unreadUpdateCount > 120) {
                    console.log('Giving up on unread mail count updates, refresh the page to see new updates.');
                    clearInterval(unreadUpdateId);
                    $('#unreadBadge').html('?');
                    $('#unreadBadge').show();
                    $('#unreadBadge').removeClass('animated flash');
                    $('#crMail').removeClass('text-danger');
                }
                var jqxhrMail = $.ajax(graUnreadLookupUrl)
                    .done(function (data, textStatus, jqXHR) {
                        if (typeof data === "undefined") {
                            console.log('Invalid response for unread mail count.');
                        } else {
                            if (data != '0') {
                                console.log('Unread message count #' + unreadUpdateCount + ' updated count to ' + data);
                                $('#unreadBadge').removeClass('animated flash');
                                $('#unreadBadge').html(data);
                                $('#unreadBadge').show();
                                $('#unreadBadge').addClass('animated flash');
                                $('#crMail').addClass('text-danger');
                            }
                            else {
                                $('#unreadBadge').hide();
                                $('#crMail').removeClass('text-danger');
                            }
                        }
                    })
                    .fail(function () {
                        clearInterval(unreadUpdateId);
                        $('#unreadBadge').html('?');
                        $('#unreadBadge').show();
                        $('#unreadBadge').removeClass('animated flash');
                        $('#crMail').removeClass('text-danger');
                    });
            }
            var unreadUpdateId = setInterval(updateUnread, 60 * 1000);
            updateUnread();
        }
    });
}
else
{
    console.log('jQuery is not loaded, not loading unread badge update script.');
}