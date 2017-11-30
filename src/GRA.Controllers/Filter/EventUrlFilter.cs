using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GRA.Controllers.Filter
{
    public class EventUrlFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            if (httpContext.Items.Keys.Contains(ItemKey.ExternalEventListUrl))
            {
                if (context.RouteData.Values["area"]?.ToString() == "MissionControl")
                {
                    var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                    var controller = (Base.Controller)context.Controller;
                    context.Result = controller.RedirectToAction("Index", "Home", new { area = "MissionControl" });
                    return;
                }
                else
                {
                    httpContext.Response.Redirect(httpContext.Items[ItemKey.ExternalEventListUrl].ToString());
                }
            }
            await next();
        }
    }
}
