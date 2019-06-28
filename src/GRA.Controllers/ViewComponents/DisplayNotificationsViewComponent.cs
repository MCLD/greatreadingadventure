using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRA.Abstract;
using GRA.Controllers.ViewModel.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace GRA.Controllers.ViewComponents
{
    [ViewComponent(Name = "DisplayNotifications")]
    public class DisplayNotificationsViewComponent : ViewComponent
    {
        private const int MaxNotifications = 3;

        private readonly IPathResolver _pathResolver;
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;
        private readonly IHtmlLocalizer<GRA.Resources.Shared> _sharedHtmlLocalizer;

        public DisplayNotificationsViewComponent(IPathResolver pathResolver,
            IStringLocalizer<Resources.Shared> sharedLocalizer,
            IHtmlLocalizer<GRA.Resources.Shared> sharedHtmlLocalizer)
        {
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
            _sharedHtmlLocalizer = sharedHtmlLocalizer
                ?? throw new ArgumentNullException(nameof(sharedHtmlLocalizer));
        }

        public IViewComponentResult Invoke()
        {
            var notifications =
                (List<Domain.Model.Notification>)HttpContext.Items[ItemKey.NotificationsList];
            var totalNotifications = notifications.Count;

            var notificationDisplayList = new List<GRA.Domain.Model.Notification>();
            int? totalPointsEarned = 0;
            var earnedBadge = false;

            foreach (var notification in notifications.Where(m => m.IsAchiever).ToList())
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    if (!string.IsNullOrWhiteSpace(notification.BadgeFilename))
                    {
                        notification.BadgeFilename
                            = _pathResolver.ResolveContentPath(notification.BadgeFilename);
                        earnedBadge = true;
                    }
                    notificationDisplayList.Add(notification);
                }
                notifications.Remove(notification);
            }

            foreach (var notification in notifications.Where(m => m.IsJoiner).ToList())
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    if (!string.IsNullOrWhiteSpace(notification.BadgeFilename))
                    {
                        notification.BadgeFilename
                            = _pathResolver.ResolveContentPath(notification.BadgeFilename);
                        earnedBadge = true;
                    }
                    notification.LocalizedText
                        = _sharedHtmlLocalizer[Annotations.Info.SuccessfullyJoined,
                            HttpContext.Items[ItemKey.SiteName]];
                    notification.DisplayIcon = "far fa-thumbs-up";
                    notificationDisplayList.Add(notification);
                }
                notifications.Remove(notification);
            }

            foreach (var notification in notifications.Where(m => m.IsAvatarBundle).ToList())
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    if (!string.IsNullOrWhiteSpace(notification.BadgeFilename))
                    {
                        notification.BadgeFilename
                            = _pathResolver.ResolveContentPath(notification.BadgeFilename);
                        earnedBadge = true;
                    }
                    notification.DisplayIcon = "far fa-thumbs-up";
                    notification.Text = new StringBuilder(notification.Text)
                        .AppendFormat(" <a href=\"{0}\">Check out your new avatar options!</a>",
                            Url.Action(nameof(AvatarController.Index), AvatarController.Name))
                        .ToString();

                    notificationDisplayList.Add(notification);
                }
                notifications.Remove(notification);
            }


            foreach (var notification in notifications
                .Where(m => !string.IsNullOrWhiteSpace(m.BadgeFilename))
                .OrderByDescending(m => m.PointsEarned)
                .ThenByDescending(m => m.CreatedAt).ToList())
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    notification.BadgeFilename
                        = _pathResolver.ResolveContentPath(notification.BadgeFilename);
                    earnedBadge = true;
                    notificationDisplayList.Add(notification);
                }
                notifications.Remove(notification);
            }

            foreach (var notification in notifications
                .OrderByDescending(m => m.PointsEarned)
                .ThenByDescending(m => m.CreatedAt))
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    notificationDisplayList.Add(notification);
                }
            }

            string summaryText = "";
            if (notificationDisplayList.Count > 1 && totalNotifications > MaxNotifications)
            {
                summaryText = string.Format("<a href=\"{0}\">{1}</a>",
                    Url.Action(nameof(ProfileController.History), ProfileController.Name),
                    _sharedLocalizer[Annotations.Interface.AndOtherActivities]);
            }

            var viewModel = new DisplayNotificationsViewModel
            {
                Notifications = notificationDisplayList,
                SummaryText = summaryText
            };

            HttpContext.Items[ItemKey.NotificationsDisplayed] = true;
            if (earnedBadge)
            {
                HttpContext.Items[ItemKey.NotificationsModal] = true;
                return View("Modal", viewModel);
            }
            else
            {
                return View("Alert", viewModel);
            }
        }
    }
}
