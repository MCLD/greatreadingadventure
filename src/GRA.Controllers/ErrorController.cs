using System;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class ErrorController : Base.UserController
    {
        private const string UnsubscribeInMcLink = "/MissionControl/Home/Unsubscribe/";
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger,
            ServiceFacade.Controller context) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static string Name
        { get { return "Error"; } }

        public IActionResult Index(int id)
        {
            var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // there was an issue where a /MissionControl/ link was being included in unsub URLS,
            // this routes requests to the right place if those links are clicked
            if (statusFeature?.OriginalPath?.StartsWith(UnsubscribeInMcLink) == true
                && statusFeature.OriginalPath.Length > UnsubscribeInMcLink.Length)
            {
                var token = statusFeature.OriginalPath[UnsubscribeInMcLink.Length..];
                return RedirectToAction(nameof(HomeController.Unsubscribe),
                    HomeController.Name,
                    new
                    {
                        Area = "",
                        id = token
                    });
            }

            // 0 means we came from UseExceptionHandler
            Response.StatusCode = id > 0 ? id : 500;

            if (Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                return View("404");
            }

            var errorDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();

            if (errorDetails != null)
            {
                _logger.LogError(errorDetails.Error,
                    "Unhandled exception details: {ErrorMessage}",
                    errorDetails.Error?.Message ?? "no error message");
            }

            return View("Error");
        }
    }
}
