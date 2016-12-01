using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ServiceFacade;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace GRA.Controllers
{
    public class JoinController : Base.Controller
    {
        private readonly ILogger<JoinController> _logger;
        public JoinController(ILogger<JoinController> logger,
            ServiceFacade.Controller context)
                : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            PageTitle = "Join";
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSite(sitePath);
            PageTitle = $"{site.Name} - join now!";
            return View();
        }
    }
}
