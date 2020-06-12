using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace GRA.Controllers.Filter
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SessionTimeoutFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly IStringLocalizer<Resources.Shared> _sharedLocalizer;

        public SessionTimeoutFilterAttribute(IStringLocalizer<Resources.Shared> sharedLocalizer)
        {
            _sharedLocalizer = sharedLocalizer
                ?? throw new ArgumentNullException(nameof(sharedLocalizer));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // check if a user's session has gone away but they are still authenticated
            if (context.HttpContext.Session.GetInt32(SessionKey.ActiveUserId) == null
                && context.HttpContext.User.Identity.IsAuthenticated)
            {
                var controller = (Base.Controller)context.Controller;

                // by default redirect to Home/Index
                string redirectController = HomeController.Name;

                // if the user logged in recently we can redirect them to the authentication page
                var claim = context.HttpContext
                    .User
                    .Claims?
                    .FirstOrDefault(_ => _.Type == ClaimType.AuthenticatedAt);

                if (DateTime.TryParseExact(claim?.Value,
                    "O",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime authenticatedAt)
                    && (DateTime.Now - authenticatedAt).TotalHours < 2)
                {
                    redirectController = SignInController.Name;

                    controller.TempData[TempDataKey.AlertWarning]
                        = _sharedLocalizer[Annotations.Validate.SessionExpired].ToString();

                    var controllerActionDescriptor
                        = context.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor.ControllerTypeInfo
                            .IsDefined(typeof(Attributes.PreventAjaxRedirect))
                        && controllerActionDescriptor.MethodInfo
                            .IsDefined(typeof(Attributes.PreventAjaxRedirect)))
                    {
                        context.Result = controller.StatusCode(StatusCodes.Status401Unauthorized);
                    }
                }

                await controller.LogoutUserAsync();

                context.Result = controller.RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = redirectController,
                    action = redirectController == HomeController.Name
                        ? nameof(HomeController.Index)
                        : nameof(SignInController.Index)
                });
            }
            else
            {
                await next();
            }
        }
    }
}
