using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GRA.Controllers.Filter
{
    public class UnauthenticatedFilter : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result
                    = new RedirectToActionResult(nameof(HomeController.Index), "Home", null);
            }
        }
    }
}