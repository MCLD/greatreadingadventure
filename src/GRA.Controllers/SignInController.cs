using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class SignInController : Base.Controller
    {
        private readonly ILogger<SignInController> _logger;
        public SignInController(ILogger<SignInController> logger,
            ServiceFacade.Controller context)
                : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            PageTitle = "Sign In";
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSite(sitePath);
            PageTitle = $"Sign In to {site.Name}";
            return View();
        }
    }
}
