using System;
using System.IO;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.Share;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class ShareController : Base.UserController
    {
        private readonly ILogger<ShareController> _logger;
        private readonly SiteService _siteService;
        public ShareController(ILogger<ShareController> logger,
            ServiceFacade.Controller context,
            SiteService siteService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = "Share";
        }

        public async Task<IActionResult> Avatar(string id)
        {
            var site = await GetCurrentSiteAsync();

            var filePath = _pathResolver
                .ResolveContentFilePath($"site{site.Id}/useravatars/{id}.png");
            if (System.IO.File.Exists(filePath))
            {
                var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
                var contentPath = _pathResolver
                    .ResolveContentPath($"site{site.Id}/useravatars/{id}.png");
                var imageUrl = Path.Combine(siteUrl, contentPath)
                    .Replace("\\", "/", StringComparison.OrdinalIgnoreCase);
                var viewModel = new ShareAvatarViewModel()
                {
                    ImageUrl = imageUrl,
                    Social = new Domain.Model.Social
                    {
                        Description = site.AvatarCardDescription,
                        ImageLink = imageUrl
                    }
                };
                return View(viewModel);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
