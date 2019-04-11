using System;
using System.Threading.Tasks;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
        }

        public async Task<IActionResult> Index(string id)
        {
            try
            {
                var page = await _pageService.GetByStubAsync(id);
                PageTitle = page.Title;
                page.Content = CommonMark.CommonMarkConverter.Convert(page.Content);
                return View(page);
            }
            catch (GraException)
            {
                return StatusCode(404);
            }
        }
    }
}
