using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.PerformerRegistration.Home;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.PerformerRegistration
{
    [Area("PerformerRegistration")]
    [Authorize(Policy = Policy.AccessPerformerRegistration)]
    public class HomeController : Base.Controller
    {
        private readonly Regex alphanumericRegex = new Regex("[^a-zA-Z0-9 -]");

        private const int MaxUploadSize = 25 * 1024 * 1024;

        private const string ReferencesFolder = "references";
        private const string KitImagesFolder = "kitimages";
        private const string PerformerImagesFolder = "performerimages";
        private const string ProgramImagesFolder = "programimages";

        private readonly ILogger<HomeController> _logger;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly SiteService _siteService;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            PerformerSchedulingService performerSchedulingService,
            SiteService siteService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = "Performer Registration";
        }

        public async Task<IActionResult> Index()
        {
            var dates = await _performerSchedulingService.GetDatesAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(dates);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction("Index", "Home", new { Area = string.Empty });
            }

            return View();
        }

        public async Task<IActionResult> Information()
        {
            var dates = await _performerSchedulingService.GetDatesAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(dates);
            if (schedulingStage != PsSchedulingStage.PerformerOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                true);

            if (performer?.RegistrationCompleted == false)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var systems = (await _siteService.GetSystemList()).ToList();

            var viewModel = new InformationViewModel()
            {
                Performer = performer,
                Systems = systems.ToList(),
                BranchCount = systems.Sum(_ => _.Branches.Count())
            };

            if (performer != null)
            {
                if (performer.AllBranches)
                {
                    viewModel.BranchAvailability = systems
                        .SelectMany(_ => _.Branches)
                        .Select(_ => _.Id)
                        .ToList();
                }
                else
                {
                    viewModel.BranchAvailability = performer.Branches?.Select(_ => _.Id).ToList();
                }
            }

            PageTitle = "Performer Information";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Information(InformationViewModel model)
        {
            var siteId = GetCurrentSiteId();
            var dates = await _performerSchedulingService.GetDatesAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(dates);
            if (schedulingStage != PsSchedulingStage.PerformerOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var currentPerformer = await _performerSchedulingService
                .GetPerformerByUserIdAsync(userId);

            if (currentPerformer != null && currentPerformer.RegistrationCompleted == false)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var systems = (await _siteService.GetSystemList()).ToList();
            var branchIds = systems.SelectMany(_ => _.Branches).Select(_ => _.Id);
            var BranchAvailability = JsonConvert.DeserializeObject
                <List<int>>(model.BranchAvailabilityString)
                .Where(_ => branchIds.Contains(_)).ToList();

            if (BranchAvailability.Count == 0)
            {
                ModelState.AddModelError("BranchAvailability", "Please select the libraries where you are willing to perform.");
            }

            if (currentPerformer == null)
            {
                if (model.Images == null)
                {
                    ModelState.AddModelError("Images", "Please attach an image to submit.");
                }
                else if (model.Images != null && model.Images.Count > 0)
                {
                    var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                    if (extensions.Any(_ => _ != ".jpg" && _ != ".jpeg" && _ != ".png"))
                    {
                        ModelState.AddModelError("Images", "Please only attach .jpg or .png images.");
                    }
                    else if (model.Images.Sum(_ => _.Length) > MaxUploadSize)
                    {
                        ModelState.AddModelError("Images", "Please limit uploads to a max of 25MB.");
                    }
                }

                if (model.References == null)
                {
                    ModelState.AddModelError("References", "Please attach a list of references to submit.");
                }
            }

            if (model.References != null
                && Path.GetExtension(model.References.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("References", "Please attach a .pdf file.");
            }

            if (ModelState.IsValid)
            {
                var performer = currentPerformer ?? model.Performer;

                if (BranchAvailability.Count == branchIds.Count())
                {
                    performer.AllBranches = true;
                }
                else
                {
                    performer.AllBranches = false;
                }

                if (performer?.RegistrationCompleted == true)
                {
                    performer = await _performerSchedulingService.EditPerformerAsync(performer,
                        BranchAvailability);
                }
                else
                {
                    performer = await _performerSchedulingService.AddPerformerAsync(performer,
                        BranchAvailability);
                }

                if (model.References != null)
                {
                    using (var fileStream = model.References.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            await _performerSchedulingService.AddPerformerReferencesAsync(
                                performer.Id, ms.ToArray(),
                                Path.GetExtension(model.References.FileName));
                        }
                    }
                }

                if (performer.RegistrationCompleted == false)
                {
                    foreach (var image in model.Images)
                    {
                        using (var fileStream = image.OpenReadStream())
                        {
                            using (var ms = new MemoryStream())
                            {
                                fileStream.CopyTo(ms);
                                await _performerSchedulingService.AddPerformerImageAsync(
                                    performer.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                            }
                        }
                    }
                }

                if (performer.RegistrationCompleted == false)
                {
                    return RedirectToAction(nameof(Schedule));
                }
                else
                {
                    TempData[TempDataKey.AlertSuccess] = "Information saved!";
                    return RedirectToAction(nameof(Dashboard));
                }
            }

            model.BranchCount = systems.Sum(_ => _.Branches.Count());
            model.BranchAvailability = BranchAvailability;
            model.Systems = systems;

            PageTitle = "Performer Information";
            return View(model);
        }

        public async Task<IActionResult> Schedule()
        {
            return null;
        }

        public async Task<IActionResult> Dashboard()
        {
            return null;
        }
    }
}
