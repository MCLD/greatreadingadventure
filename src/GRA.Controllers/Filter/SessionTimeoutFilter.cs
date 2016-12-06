using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers.Filter
{
    public class SessionTimeoutFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            if (context.HttpContext.Session.GetInt32(SessionKey.ActiveUserId) == null
                && context.HttpContext.User.Identity.IsAuthenticated)
            {
                // no session but logged in - log out, redirect
                var controller = (Base.Controller)context.Controller;
                await controller.LogoutUserAsync();
                // TODO move this message to a customizable location
                controller.TempData[TempDataKey.AlertWarning] = "Your session has expired. Please sign in again.";

                context.Result = controller.RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    action = "Index"
                });
            }
            else
            {
                await next();
            }
        }
    }
}
