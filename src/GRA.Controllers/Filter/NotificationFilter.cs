using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace GRA.Controllers.Filter
{
    public class NotificationFilter : Attribute, IAsyncResultFilter
    {
        private const int MaxNotifications = 3;

        private readonly UserService _userService;
        public NotificationFilter(UserService userService)
        {
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (!controllerActionDescriptor.ControllerTypeInfo
                        .IsDefined(typeof(Attributes.SuppressNotifications))
                    && !controllerActionDescriptor.MethodInfo
                        .IsDefined(typeof(Attributes.SuppressNotifications)))
                {
                    var notifications = await _userService.GetNotificationsForUser();
                    if (notifications.Any())
                    {
                        httpContext.Items[ItemKey.NotificationsList] = notifications;
                    }

                    await next();

                    if (httpContext.Items[ItemKey.NotificationsDisplayed] != null
                        && (bool)httpContext.Items[ItemKey.NotificationsDisplayed] == true)
                    {
                        await _userService.ClearNotificationsForUser();
                    }
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
