using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GRA.Controllers.Filter
{
    public class NotificationFilter : Attribute, IAsyncResultFilter
    {
        private readonly UserService _userService;

        public NotificationFilter(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var controllerActionDescriptor
                    = context.ActionDescriptor as ControllerActionDescriptor;
                if (!controllerActionDescriptor.ControllerTypeInfo
                        .IsDefined(typeof(Attributes.SuppressNotifications))
                    && !controllerActionDescriptor.MethodInfo
                        .IsDefined(typeof(Attributes.SuppressNotifications)))
                {
                    var notifications = await _userService.GetNotificationsForUser();
                    if (notifications.Any())
                    {
                        context.HttpContext.Items[ItemKey.NotificationsList] = notifications;
                    }

                    await next();

                    if (context.HttpContext.Items[ItemKey.NotificationsDisplayed] as bool? == true)
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
