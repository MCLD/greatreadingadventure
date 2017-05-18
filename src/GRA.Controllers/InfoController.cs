using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class InfoController : Base.UserController
    {
        private readonly ILogger<InfoController> _logger;
        private readonly PageService _pageService;
        public InfoController(ILogger<InfoController> logger,
            ServiceFacade.Controller context,
            PageService pageService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _pageService = Require.IsNotNull(pageService, nameof(pageService));
        }

        public async Task<IActionResult> Index(string stub)
        {
            try
            {
                var page = await _pageService.GetByStubAsync(stub, true);
                PageTitle = page.Title;
                return View("Index", CommonMark.CommonMarkConverter.Convert(page.Content));
            }
            catch (GraException)
            {
                return StatusCode(404);
            }
        }
    }
}
