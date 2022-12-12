using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class ErrorController : Base.UserController
    {
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
            // 0 means we came from UseExceptionHandler
            Response.StatusCode = id > 0 ? id : 500;

            if (Response.StatusCode == 404)
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
