using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers.Filter
{
    public class SessionTimeoutFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Session.GetInt32(SessionKey.ActiveUserId) == null
                && context.HttpContext.User.Identity.IsAuthenticated)
            {
                // no session but logged in - log out, redirect
                var controller = (Controllers.Base.Controller)context.Controller;
                Task.WaitAll(controller.LogoutUserAsync());
                // TODO move this message to a customizable location
                controller.TempData[TempDataKey.AlertWarning] = "Your session has expired. Please sign in again.";

                context.Result = controller.RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    action = "Index"
                });
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
