using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Downloads;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.AccessMissionControl)]
    public class DownloadsController : Base.MCController
    {
        private readonly BadgeService _badgeService;
        private readonly ILogger<DownloadsController> _logger;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public DownloadsController(ServiceFacade.Controller context,
            BadgeService badgeService,
            ILogger<DownloadsController> logger,
            SiteService siteService,
            UserService userService) : base(context)
        {
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = Name;
        }

        public static string Name
        { get { return "Downloads"; } }

        public async Task<IActionResult> GetBadgeZipForSystem(int id)
        {
            var system = await _siteService.GetSystemByIdAsync(id);
            if (!UserHasPermission(Permission.ManageSites))
            {
                var branch = await _userService.GetUsersBranch(GetActiveUserId());
                if (branch.SystemId != id)
                {
                    return RedirectNotAuthorized("You may only download badges for your system.");
                }
            }
            var badgeWebPaths = await _badgeService.GetFilesBySystemAsync(id);

            var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var badgeWebPath in badgeWebPaths)
                {
                    var filePath = Path.Combine($"site{GetCurrentSiteId()}",
                        "badges",
                        badgeWebPath[(badgeWebPath.LastIndexOf('/') + 1)..]);

                    var path = _pathResolver.ResolveContentFilePath(filePath);
                    if (System.IO.File.Exists(path))
                    {
                        zip.CreateEntryFromFile(path, Path.GetFileName(path));
                    }
                    else
                    {
                        _logger.LogWarning("Missing badge file for download: {Filename}", badgeWebPath);
                    }
                }
            }
            ms.Seek(0, SeekOrigin.Begin);

            string filename = _dateTimeProvider.Now.ToString("yyyyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture)
                + "-badges-"
                + system.Name.Replace(' ', '-');

            return File(ms, System.Net.Mime.MediaTypeNames.Application.Zip, filename);
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Domain.Model.System> systemList = null;
            if (UserHasPermission(Permission.ManageSites))
            {
                systemList = await _siteService.GetSystemList();
            }
            else
            {
                var branch = await _userService.GetUsersBranch(GetActiveUserId());
                systemList = new[] { await _siteService.GetSystemByIdAsync(branch.SystemId) };
            }

            var viewModel = new IndexViewModel();

            foreach (var system in systemList)
            {
                var count = await _badgeService.GetCountBySystemAsync(system.Id);
                viewModel.SystemBadgeCount.Add(system, count);
            }

            return View(viewModel);
        }
    }
}
