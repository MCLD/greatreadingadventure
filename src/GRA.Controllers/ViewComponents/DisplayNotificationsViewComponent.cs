using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
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

        private readonly UserService _userService;
        public DisplayNotificationsViewComponent(UserService userService)
        {
            _userService = Require.IsNotNull(userService, nameof(userService));
        }
        public IViewComponentResult Invoke()
        {
            var notifications =
                (List<GRA.Domain.Model.Notification>)HttpContext.Items[ItemKey.NotificationsList];

            var notificationDisplayList = new List<GRA.Domain.Model.Notification>();
            int? totalPointsEarned = 0;
            bool earnedBadge = false;

            foreach (var notification in notifications.Where(m => m.BadgeId != null)
                .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt))
            {
                earnedBadge = true;
                totalPointsEarned += notification.PointsEarned;
                if (notificationDisplayList.Count < MaxNotifications)
                {
                    notificationDisplayList.Add(notification);
                }
            }

            if (notificationDisplayList.Count < MaxNotifications)
            {
                foreach (var notification in notifications.Where(m => m.BadgeId == null)
                    .OrderByDescending(m => m.PointsEarned).ThenByDescending(m => m.CreatedAt))
                {
                    totalPointsEarned += notification.PointsEarned;
                    if (notificationDisplayList.Count < MaxNotifications)
                    {
                        notificationDisplayList.Add(notification);
                    }
                }
            }

            string summaryText = "";
            if (notifications.Count() > 1)
            {
                if (notifications.Count() > MaxNotifications)
                {
                    summaryText = $"and <a href='{Url.Action("History", "Profile")}'>other activities</a> ";
                }

                summaryText += $"for a total of <strong>{totalPointsEarned} points!</strong>";
            }

            DisplayNotificationsViewModel viewModel = new DisplayNotificationsViewModel()
            {
                Notifications = notificationDisplayList,
                SummaryText = summaryText
            };

            HttpContext.Items[ItemKey.NotificationsDisplayed] = true;
            if (earnedBadge == true && false)
            {
                return View("Modal", viewModel);
            }
            else
            {
                return View("Alert", viewModel);
            }
        }
    }
}
