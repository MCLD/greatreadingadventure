using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class ErrorController : Base.Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger,
            ServiceFacade.Controller context) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("404");
            }
            return View("Error");
        }
    }
}
