using GRA.Abstract;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.ViewComponents
{
    [ViewComponent(Name = "DisplayNotifications")]
    public class DisplayNotificationsViewComponent : ViewComponent
    {
        private const int MaxNotifications = 3;

        private readonly IPathResolver _pathResolver;
        private readonly UserService _userService;
        public DisplayNotificationsViewComponent(IPathResolver pathResolver,
            UserService userService)
        {
            _pathResolver = pathResolver;
            _userService = Require.IsNotNull(userService, nameof(userService));
        }
        public IViewComponentResult Invoke()
        {
            var notifications =
                (List<GRA.Domain.Model.Notification>)HttpContext.Items[ItemKey.NotificationsList];

            var totalNotifications = notifications.Count;

            var notificationDisplayList = new List<GRA.Domain.Model.Notification>();
            int? totalPointsEarned = 0;
            bool earnedBadge = false;

            foreach (var notification in notifications.Where(m => m.IsAchiever).ToList())
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    if (!string.IsNullOrWhiteSpace(notification.BadgeFilename))
                    {
                        notification.BadgeFilename = _pathResolver.ResolveContentPath(notification.BadgeFilename);
                        earnedBadge = true;
                    }
                    notificationDisplayList.Add(notification);
                }
                notifications.Remove(notification);
            }

            foreach (var notification in notifications.Where(m => m.IsJoining).ToList())
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    if (!string.IsNullOrWhiteSpace(notification.BadgeFilename))
                    {
                        notification.BadgeFilename = _pathResolver.ResolveContentPath(notification.BadgeFilename);
                        earnedBadge = true;
                    }
                    notificationDisplayList.Add(notification);
                }
                notifications.Remove(notification);
            }

            foreach (var notification in notifications
                .Where(m => !string.IsNullOrWhiteSpace(m.BadgeFilename))
                .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt).ToList())
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    notification.BadgeFilename = _pathResolver.ResolveContentPath(notification.BadgeFilename);
                    earnedBadge = true;
                    notificationDisplayList.Add(notification);
                }
                notifications.Remove(notification);
            }

            foreach (var notification in notifications
                .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt))
            {
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    notificationDisplayList.Add(notification);
                }
            }

            string summaryText = "";
            if (notificationDisplayList.Count() > 1)
            {
                if (totalNotifications > MaxNotifications)
                {
                    summaryText = $"...and <a href='{Url.Action("History", "Profile")}'>other activities</a>!";
                }
            }

            DisplayNotificationsViewModel viewModel = new DisplayNotificationsViewModel()
            {
                Notifications = notificationDisplayList,
                SummaryText = summaryText
            };

            HttpContext.Items[ItemKey.NotificationsDisplayed] = true;
            if (earnedBadge == true)
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
