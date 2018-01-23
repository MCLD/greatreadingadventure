using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.AccessFlightController)]
    public class FlightController : Base.MCController
    {
        private readonly ILogger<FlightController> _logger;
        private readonly ActivityService _activityService;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly VendorCodeService _vendorCodeService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FlightController(ILogger<FlightController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            DynamicAvatarService dynamicAvatarService,
            QuestionnaireService questionnaireService,
            VendorCodeService vendorCodeService,
            IHostingEnvironment hostingEnvironment)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService, nameof(dynamicAvatarService));
            _questionnaireService = Require.IsNotNull(questionnaireService,
                nameof(questionnaireService));
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

        [HttpPost]
        public async Task<IActionResult> CreateVendorCodesAsync(int numberOfCodes)
        {
            var allCodes = await _vendorCodeService.GetTypeAllAsync();
            var code = allCodes.FirstOrDefault();
            if (code == null)
            {
                code = await _vendorCodeService.AddTypeAsync(new VendorCodeType
                {
                    Description = "Free Book Code",
                    DonationOptionSubject = "Choose whether to receive your free book or donate it to a child",
                    DonationOptionMail = "If you'd like to redeem your free book code, please visit <a href=\"/Profile/\">your profile</a> and select the redeem option. If you're not interested in redeeming it, you can select the option to donate it to a child.",
                    MailSubject = "Here's your Free Book Code!",
                    Mail = $"Congratulations, you've earned a free book! Your free book code is: {TemplateToken.VendorCodeToken}!",
                    DonationMessage = "Your free book has been donated.Thank you!!!",
                    DonationSubject = "Thank you for donating your free book!",
                    DonationMail = "Thanks so much for the donation of your book.",
                    Url = "http://freebook/?Code={Code}"
                });
            }
            var sw = new Stopwatch();
            sw.Start();
            var generatedCount = await _vendorCodeService.GenerateVendorCodesAsync(code.Id, numberOfCodes);
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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int siteId = GetCurrentSiteId();

            string assetPath = Path.Combine(
                Directory.GetParent(_hostingEnvironment.WebRootPath).FullName, "assets");

            if (!Directory.Exists(assetPath))
            {
                AlertDanger = $"Asset directory not found at: {assetPath}";
                return View("Index");
            }

            assetPath = Path.Combine(assetPath, "dynamicavatars");
            if (!Directory.Exists(assetPath))
            {
                AlertDanger = $"Asset directory not found at: {assetPath}";
                return View("Index");
            }

            IEnumerable<DynamicAvatarLayer> avatarList;
            var jsonPath = Path.Combine(assetPath, "default avatars.json");
            using (StreamReader file = System.IO.File.OpenText(jsonPath))
            {
                var jsonString = await file.ReadToEndAsync();
                avatarList = JsonConvert.DeserializeObject<IEnumerable<DynamicAvatarLayer>>(jsonString);
            }

            _logger.LogInformation($"Found {avatarList.Count()} DynamicAvatarLayer objects in avatar file");

            var time = _dateTimeProvider.Now;

            int totalFilesCopied = 0;

            foreach (var layer in avatarList)
            {
                int layerFilesCopied = 0;
                var userId = GetId(ClaimType.UserId);
                if (layer.DynamicAvatarColors != null)
                {
                    foreach (var color in layer.DynamicAvatarColors)
                    {
                        color.CreatedAt = time;
                        color.CreatedBy = userId;
                    }
                }
                foreach (var item in layer.DynamicAvatarItems)
                {
                    item.CreatedAt = time;
                    item.CreatedBy = userId;
                }
                var addedLayer = await _dynamicAvatarService.AddLayerAsync(layer);

                var layerAssetPath = Path.Combine(assetPath, addedLayer.Name);
                var destinationRoot = Path.Combine($"site{siteId}", "dynamicavatars", $"layer{addedLayer.Id}");
                var destinationPath = _pathResolver.ResolveContentFilePath(destinationRoot);
                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                List<DynamicAvatarElement> elementList = new List<DynamicAvatarElement>();
                _logger.LogInformation($"Processing {addedLayer.DynamicAvatarItems.Count()} items in {layer.Name}...");

                foreach (var item in addedLayer.DynamicAvatarItems)
                {
                    var itemRoot = Path.Combine(destinationRoot, $"item{item.Id}");
                    var itemPath = Path.Combine(destinationPath, $"item{item.Id}");
                    if (!Directory.Exists(itemPath))
                    {
                        Directory.CreateDirectory(itemPath);
                    }
                    if (addedLayer.DynamicAvatarColors.Count > 0)
                    {
                        foreach (var color in addedLayer.DynamicAvatarColors)
                        {
                            var element = new DynamicAvatarElement()
                            {
                                DynamicAvatarItemId = item.Id,
                                DynamicAvatarColorId = color.Id,
                                Filename = Path.Combine(itemRoot, $"{item.Id}_{color.Id}.png")
                            };
                            elementList.Add(element);
                            System.IO.File.Copy(
                                Path.Combine(layerAssetPath, $"{item.Name} {color.Color}.png"),
                                Path.Combine(itemPath, $"{item.Id}_{color.Id}.png"));
                            layerFilesCopied++;
                        }
                    }
                    else
                    {
                        var element = new DynamicAvatarElement()
                        {
                            DynamicAvatarItemId = item.Id,
                            Filename = Path.Combine(itemRoot, $"{item.Id}.png")
                        };
                        elementList.Add(element);
                        System.IO.File.Copy(Path.Combine(layerAssetPath, $"{item.Name}.png"),
                            Path.Combine(itemPath, $"{item.Id}.png"));
                        layerFilesCopied++;
                    }
                }
                await _dynamicAvatarService.AddElementListAsync(elementList);
                totalFilesCopied += layerFilesCopied;
                _logger.LogInformation($"Copied {layerFilesCopied} items for {layer.Name}");
            }
            _logger.LogInformation($"Copied {totalFilesCopied} items for all layers.");

            var bundleJsonPath = Path.Combine(assetPath, "default bundles.json");
            if (System.IO.File.Exists(bundleJsonPath))
            {
                IEnumerable<DynamicAvatarBundle> bundleList;
                using (StreamReader file = System.IO.File.OpenText(bundleJsonPath))
                {
                    var jsonString = await file.ReadToEndAsync();
                    bundleList = JsonConvert.DeserializeObject<IEnumerable<DynamicAvatarBundle>>(jsonString);
                }

                foreach (var bundle in bundleList)
                {
                    _logger.LogInformation($"Processing bundle {bundle.Name}...");
                    List<int> items = bundle.DynamicAvatarItems.Select(_ => _.Id).ToList();
                    bundle.DynamicAvatarItems = null;
                    var newBundle = await _dynamicAvatarService.AddBundleAsync(bundle, items);
                }
            }

            sw.Stop();
            string loaded = $"Default dynamic avatars added in {sw.Elapsed.TotalSeconds} seconds.";
            _logger.LogInformation(loaded);
            ShowAlertSuccess(loaded);
            return View("Index");
        }

        public async Task<IActionResult> AttachDefaultThumbnails()
        {
            int siteId = GetCurrentSiteId();

            string assetPath = Path.Combine(
                Directory.GetParent(_hostingEnvironment.WebRootPath).FullName, "assets");
            assetPath = Path.Combine(assetPath, "thumbnails");

            var layers = await _dynamicAvatarService.GetLayersAsync();
            foreach (var layer in layers)
            {
                if (layer.Name != "Body")
                {
                    var layerAssetPath = Path.Combine(assetPath, layer.Name);
                    var destinationRoot = Path.Combine($"site{siteId}", "dynamicavatars", $"layer{layer.Id}");
                    var destinationPath = _pathResolver.ResolveContentFilePath(destinationRoot);

                    var items = await _dynamicAvatarService.GetItemsByLayerAsync(layer.Id);

                    foreach (var item in items)
                    {
                        var itemRoot = Path.Combine(destinationRoot, $"item{item.Id}");
                        var itemPath = Path.Combine(destinationPath, $"item{item.Id}");

                        var thumbnailImage = Path.Combine(layerAssetPath, $"{item.Name}.jpg");
                        System.IO.File.Copy(Path.Combine(thumbnailImage),
                            Path.Combine(itemPath, "Thumbnail.jpg"));

                        item.Thumbnail = Path.Combine(itemRoot, "Thumbnail.jpg");
                        await _dynamicAvatarService.UpdateItemAsync(item);
                    }
                }
            }

            ShowAlertSuccess("Default avatar thumbnails have been successfully added.");
            return View("Index");
        }

        public async Task<IActionResult> AddQuestionnaireAsync()
        {
            var questionnaire = new Questionnaire
            {
                IsLocked = false,
                IsDeleted = false,
                Name = "Test questionnaire",
                Questions = new List<Question>()
            };

            var question = new Question
            {
                Name = "Name question",
                Text = "What is your name?",
                Answers = new List<Answer>()
            };

            question.Answers.Add(new Answer
            {
                Text = "Sir Lancelot"
            });
            question.Answers.Add(new Answer
            {
                Text = "Sir Robin"
            });
            question.Answers.Add(new Answer
            {
                Text = "King Arthur"
            });

            questionnaire.Questions.Add(question);

            await _questionnaireService.AddAsync(questionnaire);

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReloadSiteCacheAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var sites = await _siteLookupService.ReloadSiteCacheAsync();
            sw.Stop();
            ShowAlertSuccess($"Sites flushed from cache, reloaded in {sw.ElapsedMilliseconds} ms.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }
    }
}
