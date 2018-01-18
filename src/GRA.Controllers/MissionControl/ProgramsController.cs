using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Programs;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManagePrograms)]
    public class ProgramsController : Base.MCController
    {
        private readonly ILogger<ProgramsController> _logger;
        private readonly BadgeService _badgeService;
        private readonly DailyLiteracyTipService _dailyLiteracyTipService;
        private readonly PointTranslationService _pointTranslationService;
        private readonly SiteService _siteService;
        public ProgramsController(ILogger<ProgramsController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            DailyLiteracyTipService dailyLiteracyTipService,
            PointTranslationService pointTranslationService,
            SiteService siteService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _dailyLiteracyTipService = dailyLiteracyTipService
                ?? throw new ArgumentNullException(nameof(dailyLiteracyTipService));
            _pointTranslationService = pointTranslationService
                ?? throw new ArgumentNullException(nameof(pointTranslationService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = "Programs";
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var programList = await _siteService.GetPaginatedProgramListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = programList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new ProgramListViewModel()
            {
                Programs = programList.Data,
                PaginateModel = paginateModel,
                Search = search
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            PageTitle = "Create Program";

            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            var dailyLiteracyTipList = await _dailyLiteracyTipService.GetListAsync();
            var pointTranslationList = await _pointTranslationService.GetListAsync();
            var viewModel = new ProgramDetailViewModel()
            {
                Action = nameof(Create),
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true,
                DailyLiteracyTipList = new SelectList(dailyLiteracyTipList, "Id", "Name"),
                PointTranslationList = new SelectList(pointTranslationList, "Id", "TranslationName")
            };

            return View("Detail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProgramDetailViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Program.JoinBadgeName)
                || !string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                || model.BadgeUploadImage != null)
            {
                if (string.IsNullOrWhiteSpace(model.Program.JoinBadgeName))
                {
                    ModelState.AddModelError("Program.JoinBadgeName", "Please provide a name for the badge");
                }
                if (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.BadgeUploadImage == null)
                {
                    ModelState.AddModelError("BadgePath", "Please provide an image for the badge.");
                }
                else if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                    && (Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpg"
                        && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpeg"
                        && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".png"))
                {
                    ModelState.AddModelError("BadgeUploadImage", "Please use a .jpg or .png image.");
                }
            }

            if (model.Program.AgeMaximum < model.Program.AgeMinimum)
            {
                ModelState.AddModelError("Program.AgeMaximum", "The maximum age cannot be lower than the minimum age.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.BadgeUploadImage != null
                            || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage == null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            filename = "badge.png";
                        }
                        else
                        {
                            using (var fileStream = model.BadgeUploadImage.OpenReadStream())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);
                                    badgeBytes = ms.ToArray();
                                }
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        var newBadge = new Badge
                        {
                            Filename = filename
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        model.Program.JoinBadgeId = badge.Id;
                        model.Program.JoinBadgeName = model.Program.JoinBadgeName.Trim();
                    }

                    model.Program.AskAge = model.AgeValues >= 1;
                    model.Program.AgeRequired = model.AgeValues == 2;
                    model.Program.AskSchool = model.SchoolValues >= 1;
                    model.Program.SchoolRequired = model.SchoolValues == 2;
                    model.Program.Name = model.Program.Name.Trim();
                    var program = await _siteService.AddProgramAsync(model.Program);
                    ShowAlertSuccess($"Added Program \"{program.Name}\"!");
                    return RedirectToAction(nameof(Edit), new { id = program.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add Program: ", gex);
                }
            }

            var dailyLiteracyTipList = await _dailyLiteracyTipService.GetListAsync();
            var pointTranslationList = await _pointTranslationService.GetListAsync();
            model.DailyLiteracyTipList = new SelectList(dailyLiteracyTipList, "Id", "Name");
            model.PointTranslationList = new SelectList(pointTranslationList, "Id",
                "TranslationName");
            PageTitle = "Create Program";
            return View("Detail", model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                PageTitle = "Edit Program";

                var site = await GetCurrentSiteAsync();
                var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
                var program = await _siteService.GetProgramByIdAsync(id);
                var dailyLiteracyTipList = await _dailyLiteracyTipService.GetListAsync();
                var pointTranslationList = await _pointTranslationService.GetListAsync();
                var viewModel = new ProgramDetailViewModel()
                {
                    Program = program,
                    Action = nameof(Edit),
                    AgeValues = Convert.ToInt32(program.AskAge)
                        + Convert.ToInt32(program.AgeRequired),
                    SchoolValues = Convert.ToInt32(program.AskSchool)
                        + Convert.ToInt32(program.SchoolRequired),
                    BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                    UseBadgeMaker = true,
                    DailyLiteracyTipList = new SelectList(dailyLiteracyTipList, "Id", "Name"),
                    PointTranslationList = new SelectList(pointTranslationList, "Id",
                        "TranslationName")
                };

                if (program.JoinBadgeId.HasValue)
                {
                    var badge = await _badgeService.GetByIdAsync(program.JoinBadgeId.Value);
                    if (badge != null)
                    {
                        viewModel.BadgePath = _pathResolver.ResolveContentPath(badge.Filename);
                    }
                }

                return View("Detail", viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program :", gex);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProgramDetailViewModel model)
        {
            var currentProgram = await _siteService.GetProgramByIdAsync(model.Program.Id);
            if (string.IsNullOrWhiteSpace(model.Program.JoinBadgeName)
                && !string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                && model.BadgeUploadImage != null
                && currentProgram.JoinBadgeId.HasValue)
            {
                ModelState.AddModelError("Program.JoinBadgeName", "Please provide a name for the badge");
            }

            if (!string.IsNullOrWhiteSpace(model.Program.JoinBadgeName)
                && string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                && model.BadgeUploadImage == null
                && currentProgram.JoinBadgeId.HasValue == false)
            {
                ModelState.AddModelError("BadgemakerImage", "Please provide an image for the badge.");
            }

            if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                    && (Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpg"
                        && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpeg"
                        && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".png"))
            {
                ModelState.AddModelError("BadgeUploadImage", "Please use a .jpg or .png image.");
            }

            if (model.Program.AgeMaximum < model.Program.AgeMinimum)
            {
                ModelState.AddModelError("Program.AgeMaximum", "The maximum age cannot be lower than the minimum age.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage == null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            filename = "badge.png";
                        }
                        else
                        {
                            using (var fileStream = model.BadgeUploadImage.OpenReadStream())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);
                                    badgeBytes = ms.ToArray();
                                }
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        if (model.Program.JoinBadgeId.HasValue)
                        {
                            var existing = await _badgeService
                                        .GetByIdAsync(model.Program.JoinBadgeId.Value);
                            existing.Filename = Path.GetFileName(model.BadgePath);
                            await _badgeService.ReplaceBadgeFileAsync(existing, badgeBytes);
                        }
                        else
                        {
                            var newBadge = new Badge
                            {
                                Filename = filename
                            };
                            var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                            model.Program.JoinBadgeId = badge.Id;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(model.Program.JoinBadgeName))
                    {
                        model.Program.JoinBadgeName = model.Program.JoinBadgeName.Trim();
                    }

                    model.Program.AskAge = model.AgeValues >= 1;
                    model.Program.AgeRequired = model.AgeValues == 2;
                    model.Program.AskSchool = model.SchoolValues >= 1;
                    model.Program.SchoolRequired = model.SchoolValues == 2;
                    model.Program.Name = model.Program.Name.Trim();
                    await _siteService.UpdateProgramAsync(model.Program);
                    ShowAlertSuccess($"Saved Program \"{model.Program.Name}\"!");
                    return RedirectToAction(nameof(Edit), new { id = model.Program.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit Program: ", gex);
                }
            }
            var dailyLiteracyTipList = await _dailyLiteracyTipService.GetListAsync();
            var pointTranslationList = await _pointTranslationService.GetListAsync();
            model.DailyLiteracyTipList = new SelectList(dailyLiteracyTipList, "Id", "Name");
            model.PointTranslationList = new SelectList(pointTranslationList, "Id",
                "TranslationName");
            PageTitle = "Edit Program";
            return View("Detail", model);
        }

        public async Task<IActionResult> Delete(ProgramListViewModel model)
        {
            try
            {
                await _siteService.RemoveProgramAsync(model.Program.Id);
                ShowAlertSuccess($"Program \"{model.Program.Name}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Program: ", gex);
            }

            return RedirectToAction(nameof(Index), new
            {
                page = model.PaginateModel.CurrentPage,
                search = model.Search
            });
        }

        public async Task<IActionResult> DecreasePosition(int id)
        {
            try
            {
                await _siteService.DecreaseProgramPositionAsync(id);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error decreasing position for program {id} : {ex}", ex);
                return Json(false);
            }
        }

        public async Task<IActionResult> IncreasePosition(int id)
        {
            try
            {
                await _siteService.IncreaseProgramPositionAsync(id);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error increasing position for program {id} : {ex}", ex);
                return Json(false);
            }
        }
    }
}
