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
        }

        public async Task<IActionResult> Index()
        {
            var dates = await _performerSchedulingService.GetDates();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(dates);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction("Index", "Home", new { Area = string.Empty });
            }

            return View();
        }

        public async Task<IActionResult> Information()
        {
            var dates = await _performerSchedulingService.GetDates();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(dates);
            if (schedulingStage != PsSchedulingStage.PerformerOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

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

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Information(InformationViewModel model)
        {
            var siteId = GetCurrentSiteId();
            var dates = await _performerSchedulingService.GetDates();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(dates);
            if (schedulingStage != PsSchedulingStage.PerformerOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

            if (performer != null && performer.RegistrationCompleted == false)
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

            if (model.Images == null && performer.RegistrationCompleted == false)
            {
                ModelState.AddModelError("Images", "Please attach an image to submit.");
            }
            else if (model.Images != null && model.Images.Count > 0
                && performer.RegistrationCompleted == false)
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

            if (model.References == null && performer.RegistrationCompleted == false)
            {
                ModelState.AddModelError("References", "Please attach a list of references to submit.");
            }
            else if (model.References != null
                && Path.GetExtension(model.References.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("References", "Please attach a .pdf file.");
            }

            if (ModelState.IsValid)
            {
                if (performer?.RegistrationCompleted == true)
                {
                    performer = await _performerSchedulingService.EditPerformerAsync(performer);
                }
                else
                {
                    performer = await _performerSchedulingService.AddPerformerAsync(performer);
                }

                performer.BillingAddress = model.Performer.BillingAddress.Trim();
                performer.HasFingerprintCard = model.Performer.HasFingerprintCard;
                performer.Name = model.Performer.Name.Trim();
                performer.Phone = model.Performer.Phone.Trim();
                performer.PhonePreferred = model.Performer.PhonePreferred;
                performer.VendorId = model.Performer.VendorId.Trim();
                performer.Website = model.Performer.Website?.Trim();

                var performerFilename = alphanumericRegex.Replace(performer.Name, "");

                if (performer.RegistrationCompleted == false)
                {
                    performer.UserId = userId;
                }

                if (model.References != null)
                {
                    if (performer.RegistrationCompleted)
                    {
                        System.IO.File.Delete(_pathResolver.ResolveContentFilePath(
                            Path.Combine($"site{siteId}", ReferencesFolder, 
                            performer.ReferencesFilename)));
                    }

                    var referencesFilename = $"{performerFilename}_references" +
                        $"{Path.GetExtension(model.References.FileName)}";
                    while (System.IO.File.Exists(_pathResolver.ResolveContentFilePath(
                        Path.Combine($"site{siteId}", ReferencesFolder, referencesFilename))))
                    {
                        referencesFilename = $"{performerFilename}_references" +
                            $"_{Path.GetRandomFileName().Replace(".", "")}" +
                            $"{Path.GetExtension(model.References.FileName)}";
                    }

                    performer.ReferencesFilename = Path.Combine($"site{siteId}", ReferencesFolder, 
                        referencesFilename);

                    using (var fileStream = model.References.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            var filePath = _pathResolver.ResolveContentFilePath(
                                Constants.ReferencesFolder, referencesFilename);
                            await System.IO.File.WriteAllBytesAsync(filePath, ms.ToArray());
                        }
                    }
                }

                if (BranchAvailability.Count == branchIds.Count())
                {
                    performer.AllBranches = true;
                    if (performer.Branches?.Any() == true)
                    {
                        _context.PerformerBranches.RemoveRange(performer.Branches);
                    }
                }
                else
                {
                    performer.AllBranches = false;
                }

                if (performer.RegistrationCompleted == false)
                {
                    await _context.Performers.AddAsync(performer);
                }
                else
                {
                    _context.Performers.Update(performer);
                }

                if (performer.RegistrationCompleted == false)
                {
                    foreach (var image in model.Images)
                    {
                        var imageFilename = $"{performerFilename}" +
                            $"{Path.GetExtension(image.FileName)}";
                        while (System.IO.File.Exists(_pathResolver.ResolveContentFilePath(
                            Constants.PerformerImagesFolder, imageFilename)))
                        {
                            imageFilename = $"{performerFilename}" +
                                $"_{Path.GetRandomFileName().Replace(".", "")}" +
                                $"{Path.GetExtension(image.FileName)}";
                        }
                        using (var fileStream = image.OpenReadStream())
                        {
                            using (var ms = new MemoryStream())
                            {
                                fileStream.CopyTo(ms);
                                var filePath = _pathResolver.ResolveContentFilePath(
                                    Constants.PerformerImagesFolder, imageFilename);
                                await System.IO.File.WriteAllBytesAsync(filePath, ms.ToArray());
                            }
                        }
                        var performerImage = new PerformerImage()
                        {
                            PerformerId = performer.Id,
                            Filename = imageFilename
                        };
                        await _context.PerformerImages.AddAsync(performerImage);
                    }
                }

                if (performer.AllBranches == false)
                {
                    var branchesToAdd = new List<PerformerBranch>();
                    if (performer.RegistrationCompleted == false)
                    {
                        branchesToAdd = BranchAvailability
                            .Select(_ => new PerformerBranch()
                            {
                                BranchId = _,
                                PerformerId = performer.Id
                            })
                            .ToList();
                    }
                    else
                    {
                        branchesToAdd = BranchAvailability
                            .Except(performer.Branches.Select(_ => _.BranchId))
                            .Select(_ => new PerformerBranch()
                            {
                                BranchId = _,
                                PerformerId = performer.Id
                            })
                            .ToList();
                        var branchesToRemove = performer.Branches
                            .Where(_ => BranchAvailability.Contains(_.BranchId) == false);
                        _context.RemoveRange(branchesToRemove);
                    }
                    await _context.PerformerBranches.AddRangeAsync(branchesToAdd);
                }

                await _context.SaveChangesAsync();
                _memoryCache.Remove(CacheKey.PerformerIndexList);
                _memoryCache.Remove(CacheKey.ProgramIndexList);
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

            return View(model);
        }

        public async Task<IActionResult> Schedule()
        {
            return null;
        }
    }
}
