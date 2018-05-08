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

        public IActionResult Index(int id)
        {
            string originalPath = "unknown";
            var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusFeature != null)
            {
                originalPath = statusFeature.OriginalPath;
            }
            string currentUser = UserClaim(ClaimType.UserId);

            if (id == 404)
            {
                _logger.LogError($"HTTP Error {id}: path={originalPath},site={GetCurrentSiteId()},currentUser={currentUser}");
                return View("404");
            }
            _logger.LogCritical($"HTTP Error {id}: path={originalPath},site={GetCurrentSiteId()},currentUser={currentUser}");
            return View("Error");
        }
    }
}
