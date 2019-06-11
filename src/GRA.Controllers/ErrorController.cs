using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class ErrorController : Base.UserController
    {
        private readonly ILogger<ErrorController> _logger;

        public static string Name { get { return "Error"; } }

        public ErrorController(ILogger<ErrorController> logger,
            ServiceFacade.Controller context) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index(int id)
        {
            string path = default;
            string queryString = default;

            var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusFeature != null)
            {
                path = statusFeature.OriginalPath ?? "";
                queryString = statusFeature.OriginalQueryString ?? "";
            }

            string currentUser = UserClaim(ClaimType.UserId);

            string remoteAddress = HttpContext.Connection?.RemoteIpAddress.ToString();

            Response.StatusCode = id;

            if (!string.IsNullOrEmpty(currentUser))
            {
                _logger.LogError(
                    "HTTP Error {Id}: {Protocol} {Method} {Path}{QueryString} remoteAddress={RemoteAddress} currentUser={CurrentUser}",
                    id,
                    Request.Protocol,
                    Request.Method,
                    path,
                    queryString,
                    remoteAddress,
                    currentUser);
            }
            else
            {
                _logger.LogError(
                    "HTTP Error {Id}: {Protocol} {Method} {Path}{QueryString} remoteAddress={RemoteAddress}",
                    id,
                    Request.Protocol,
                    Request.Method,
                    path,
                    queryString,
                    remoteAddress);
            }

            switch (id)
            {
                case 404:
                    return View("404");
                default:
                    return View("Error");
            }
        }
    }
}
