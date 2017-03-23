using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class FlightController : Base.MCController
    {
        private readonly ILogger<FlightController> _logger;
        private readonly ActivityService _activityService;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly VendorCodeService _vendorCodeService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FlightController(ILogger<FlightController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            DynamicAvatarService dynamicAvatarService,
            VendorCodeService vendorCodeService,
            IHostingEnvironment hostingEnvironment)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService, nameof(dynamicAvatarService));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            _hostingEnvironment = Require.IsNotNull(hostingEnvironment, nameof(hostingEnvironment));
            PageTitle = "Flight Director";
        }

        public IActionResult Index()
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = "/MissionControl"
                });
            }

            if (!UserHasPermission(Permission.AccessFlightController))
            {
                // not authorized for Mission Control, redirect to authorization code
                return RedirectToRoute(new
                {
                    area = "MissionControl",
                    controller = "Home",
                    action = "AuthorizationCode"
                });
            }
            return View();
        }

        public async Task<IActionResult> CreateVendorCodesAsync()
        {
            var code = await _vendorCodeService.AddTypeAsync(new VendorCodeType
            {
                Description = "Free Book Code",
                MailSubject = "Here's your Free Book Code!",
                Mail = "Congratulations, you've earned a free book! Your free book code is {Code}!",
            });

            var sw = new Stopwatch();
            sw.Start();
            var generatedCount = await _vendorCodeService.GenerateVendorCodesAsync(code.Id, 50);
            sw.Stop();

            AlertSuccess = $"Generated {generatedCount} codes in {sw.Elapsed.TotalSeconds} seconds of type: {code.Description}";

            return View("Index");
        }

        public async Task<IActionResult> RedeemSecretCodeAsync()
        {
            var userContext = _userContextProvider.GetContext();
            try
            {
                await _activityService.LogSecretCodeAsync((int)userContext.ActiveUserId,
                    "secretcode");
            }
            catch (GraException gex)
            {
                AlertWarning = gex.Message;
            }
            return RedirectToRoute(new
            {
                area = string.Empty,
                controller = "Home",
                action = "Index"
            });
        }

        public async Task<IActionResult> SetupDynamicAvatars()
        {
            int siteId = GetCurrentSiteId();
            string path = Path.Combine(Directory.GetParent(_hostingEnvironment.WebRootPath).FullName,
                "assets");

            if (!Directory.Exists(path))
            {
                AlertDanger = $"Asset directory not found at: {path}";
                return View("Index");
            }

            path = Path.Combine(path, "dynamicavatars");
            if (!Directory.Exists(path))
            {
                AlertDanger = $"Asset directory not found at: {path}";
                return View("Index");
            }

            string[] allowedExtensions = { ".png", ".jpg", ".gif" };

            var avatars = new Dictionary<int, DynamicAvatar>();

            int layerCount = 0;
            int elementCount = 0;
            foreach (var layerDirectory in Directory.EnumerateDirectories(path))
            {
                layerCount++;
                string layerString = Path.GetFileNameWithoutExtension(layerDirectory).Substring(5);
                int layerPosition = Convert.ToInt32(layerString);

                var layer = new DynamicAvatarLayer
                {
                    Name = $"Layer {layerPosition}",
                    Position = layerPosition
                };

                layer = await _dynamicAvatarService.AddLayerAsync(layer);

                var destinationRoot = Path.Combine($"site{siteId}", "dynamicavatars", $"layer{layer.Id}");
                var destinationPath = _pathResolver.ResolveContentFilePath(destinationRoot);

                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                int elementNumber = 0;
                foreach (var avatarElementPath in Directory.EnumerateFiles(layerDirectory))
                {
                    var extension = Path.GetExtension(avatarElementPath).ToLower();

                    // Mac often adds hidden files which we don't want to create avatars for
                    if (!allowedExtensions.Contains(extension))
                        continue;

                    if (!avatars.ContainsKey(elementNumber))
                    {
                        var newAvatar = new DynamicAvatar();
                        newAvatar.Name = $"Avatar {elementNumber}";
                        newAvatar.Position = elementNumber;

                        newAvatar = await _dynamicAvatarService.AddAvatarAsync(newAvatar);
                        avatars.Add(elementNumber, newAvatar);
                    }

                    var avatar = avatars[elementNumber];

                    var element = new DynamicAvatarElement
                    {
                        DynamicAvatarId = avatar.Id,
                        DynamicAvatarLayerId = layer.Id,
                    };

                    element = await _dynamicAvatarService.AddElementAsync(element);

                    System.IO.File.Copy(avatarElementPath,
                        Path.Combine(destinationPath, $"{element.Id}{extension}"));

                    elementCount++;
                    elementNumber++;
                }
            }
            AlertSuccess = $"Inserted {elementCount} elements on {layerCount} layers.";
            return View("Index");
        }
    }
}
