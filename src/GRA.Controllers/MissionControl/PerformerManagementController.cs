using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.PerformerManagement;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManagePerformers)]
    public class PerformerManagementController : Base.MCController
    {
        private static readonly int MaxUploadMB = 25;
        private static readonly int MBSize = 1024 * 1024;

        private static readonly int KitsPerPage = 15;
        private static readonly int PerformersPerPage = 15;

        private static readonly DateTime DefaultPerformerScheduleStartTime = DateTime.Parse("8:00 AM");
        private static readonly DateTime DefaultPerformerScheduleEndTime = DateTime.Parse("8:00 PM");

        private readonly ILogger<PerformerManagementController> _logger;
        private readonly PerformerSchedulingService _performerSchedulingService;
        public PerformerManagementController(ILogger<PerformerManagementController> logger,
            ServiceFacade.Controller context,
            PerformerSchedulingService performerSchedulingService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            PageTitle = "Performer Management";
        }

        #region Performers
        public async Task<IActionResult> Index(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var filter = new PerformerSchedulingFilter(page, PerformersPerPage);

            var performerList = await _performerSchedulingService
                .GetPaginatedPerformerListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = performerList.Count,
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
            foreach (var performer in performerList.Data)
            {
                performer.ProgramCount = await _performerSchedulingService
                    .GetPerformerProgramCountAsync(performer.Id);

                if (schedulingStage >= PsSchedulingStage.SchedulingOpen)
                {
                    performer.SelectionsCount = await _performerSchedulingService
                        .GetPerformerSelectionCountAsync(performer.Id);
                }
            }

            var viewModel = new PerformerListViewModel()
            {
                Performers = performerList.Data.ToList(),
                PaginateModel = paginateModel,
                PerformerSchedulingEnbabled = true,
                RegistrationClosed = schedulingStage >= PsSchedulingStage.RegistrationClosed,
                SchedulingStage = schedulingStage
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerDelete(PerformerListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                try
                {
                    await _performerSchedulingService.RemovePerformerAsync(
                        model.PerformerToDelete.Id);
                    ShowAlertSuccess($"Performer \"{model.PerformerToDelete.Name}\" removed!.");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to remove performer: ", gex);
                }
            }
            else
            {
                ShowAlertDanger("Cannot remove performers after scheduling has opened.");
            }

            return RedirectToAction(nameof(Index),
                new { page = model.PaginateModel.CurrentPage });
        }

        public async Task<IActionResult> Performer(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeBranches: true,
                    includeImages: true,
                    includePrograms: true,
                    includeSchedule: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new PerformerViewModel()
            {
                Approve = !performer.IsApproved,
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                BranchAvailability = performer.Branches.Select(_ => _.Id).ToList(),
                Performer = performer,
                ReferencesPath = _pathResolver.ResolveContentPath(performer.ReferencesFilename),
                SchedulingStage = schedulingStage,
                Settings = settings,
                Systems = await _performerSchedulingService
                    .GetSystemListWithoutExcludedBranchesAsync()
            };

            if (performer.Images.Any())
            {
                viewModel.ImagePath = _pathResolver.ResolveContentPath(
                    performer.Images.First().Filename);
            }

            if (!string.IsNullOrWhiteSpace(performer.Website))
            {
                if (Uri.TryCreate(performer.Website, UriKind.Absolute, out Uri absoluteUri))
                {
                    viewModel.Uri = absoluteUri;
                }
            }

            var performerIndexList = await _performerSchedulingService.GetPerformerIndexListAsync();
            var index = performerIndexList.IndexOf(id);
            viewModel.ReturnPage = index / PerformersPerPage + 1;
            if (index != 0)
            {
                viewModel.PrevPerformer = performerIndexList[index - 1];
            }
            if (performerIndexList.Count != index + 1)
            {
                viewModel.NextPerformer = performerIndexList[index + 1];
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerApprove(PerformerViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            try
            {
                await _performerSchedulingService.SetPerformerApprovedAsync(model.Performer.Id, model.Approve);
                ShowAlertSuccess($"Performer {(model.Approve ? "Approved" : "Unapproved")}!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"Unable to {(model.Approve ? "Approve" : "Unapprove")} performer: ", gex);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Performer), new { model.Performer.Id });
        }

        [HttpPost]
        public async Task<IActionResult> ProgramDelete(PerformerViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            try
            {
                await _performerSchedulingService.RemoveProgramAsync(model.ProgramToDelete.Id);
                ShowAlertSuccess($"Program \"{model.ProgramToDelete.Title}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove program: ", gex);
            }

            return RedirectToAction(nameof(Performer), new { id = model.Performer.Id });
        }

        public async Task<IActionResult> PerformerDetails(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeBranches: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit performer: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();

            var viewModel = new PerformerDetailsViewModel()
            {
                Performer = performer,
                Systems = systems,
                BranchCount = systems.Sum(_ => _.Branches.Count())
            };

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

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerDetails(PerformerDetailsViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();
            var branchIds = systems.SelectMany(_ => _.Branches).Select(_ => _.Id);
            var branchAvailability = JsonConvert
                .DeserializeObject<List<int>>(model.BranchAvailabilityString)
                .Where(_ => branchIds.Contains(_))
                .ToList();

            if (branchAvailability.Count == 0)
            {
                ModelState.AddModelError("BranchAvailability", "Please select the libraries where they are willing to perform.");
            }

            if (model.References != null
                && Path.GetExtension(model.References.FileName).ToLower() != ".pdf")
            {
                ModelState.AddModelError("References", "Please attach a .pdf file.");
            }

            if (ModelState.IsValid)
            {
                if (branchAvailability.Count == branchIds.Count())
                {
                    model.Performer.AllBranches = true;
                }
                else
                {
                    model.Performer.AllBranches = false;
                }

                var performer = await _performerSchedulingService.EditPerformerAsync(
                    model.Performer, branchAvailability);

                if (model.References != null)
                {
                    using (var fileStream = model.References.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            await _performerSchedulingService.SetPerformerReferencesAsync(
                                model.Performer.Id, ms.ToArray(),
                                Path.GetExtension(model.References.FileName));
                        }
                    }
                }

                ShowAlertSuccess($"Performer {performer.Name} updated!");
                return RedirectToAction(nameof(Performer), new { id = performer.Id });
            }

            model.BranchCount = systems.Sum(_ => _.Branches.Count());
            model.BranchAvailability = branchAvailability;
            model.Systems = systems;

            return View(model);
        }

        public async Task<IActionResult> PerformerImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view performer images: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new PerformerImagesViewModel()
            {
                PerformerId = performer.Id,
                PerformerName = performer.Name,
                PerformerImages = performer.Images.ToList(),
                MaxUploadMB = MaxUploadMB
            };
            viewModel.PerformerImages.ForEach(_ => _.Filename =
                _pathResolver.ResolveContentPath(_.Filename));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerImages(PerformerImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(
                    model.PerformerId, includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add performer images: ", gex);
                return RedirectToAction(nameof(Index));
            }

            if (model.Images == null)
            {
                ModelState.AddModelError("Images", "Please attach an image to submit.");
            }
            else if (model.Images.Count > 0)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => _ != ".jpg" && _ != ".jpeg" && _ != ".png"))
                {
                    ModelState.AddModelError("Images", "Please only attach .jpg or .png images.");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images", $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var image in model.Images)
                {
                    using (var fileStream = image.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            await _performerSchedulingService.AddPerformerImageAsync(
                                model.PerformerId, ms.ToArray(), Path.GetExtension(image.FileName));
                        }
                    }
                }
                ShowAlertSuccess("Image(s) added!");
                return RedirectToAction(nameof(PerformerImages));
            }

            var performerImages = performer.Images.ToList();
            performerImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            model.MaxUploadMB = MaxUploadMB;
            model.PerformerName = model.PerformerName;
            model.PerformerImages = performerImages;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerImagesDelete(PerformerImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemovePerformerImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }

            return RedirectToAction(nameof(PerformerImages), new { id = model.PerformerId });
        }

        public async Task<IActionResult> Program(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    includeAgeGroups: true,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ProgramViewModel()
            {
                Program = program
            };

            if (program.Images?.Count > 0)
            {
                viewModel.Image = _pathResolver.ResolveContentPath(program.Images.First().Filename);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ProgramAdd(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add program: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageList = new SelectList(ageGroups, "Id", "Name");
            if (ageList.Count() == 1)
            {
                ageList.First().Selected = true;
            }

            var viewModel = new ProgramDetailsViewModel()
            {
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                PerformerId = id,
                PerformerName = performer.Name,
                MaxUploadMB = MaxUploadMB
            };

            return View(nameof(ProgramDetails), viewModel);
        }

        public async Task<IActionResult> ProgramDetails(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit program: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageList = new SelectList(ageGroups, "Id", "Name");

            var viewModel = new ProgramDetailsViewModel()
            {
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                AgeSelection = program.AgeGroups.Select(_ => _.Id).ToList(),
                Program = program
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramDetails(ProgramDetailsViewModel model)
        {
            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageSelection = ageGroups
                .Where(_ => model.AgeSelection?.Contains(_.Id) == true)
                .Select(_ => _.Id)
                .ToList();
            if (ageSelection.Count() == 0)
            {
                ModelState.AddModelError("AgeSelection", "Please select age groups.");
            }

            if (model.Images != null && model.Images.Count > 0 && model.PerformerId.HasValue)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => _ != ".jpg" && _ != ".jpeg" && _ != ".png"))
                {
                    ModelState.AddModelError("Images", "Please only attach .jpg or .png images.");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images", $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }

            if (ModelState.IsValid)
            {
                var program = new PsProgram();
                try
                {
                    if (model.PerformerId.HasValue)
                    {
                        model.Program.PerformerId = model.PerformerId.Value;
                        program = await _performerSchedulingService.AddProgramAsync(model.Program,
                            ageSelection);
                        if (model.Images != null)
                        {
                            foreach (var image in model.Images)
                            {
                                using (var fileStream = image.OpenReadStream())
                                {
                                    using (var ms = new MemoryStream())
                                    {
                                        fileStream.CopyTo(ms);
                                        await _performerSchedulingService.AddProgramImageAsync(
                                            program.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                                    }
                                }
                            }
                        }
                        ShowAlertSuccess($"Program \"{program.Title}\" added!");
                    }
                    else
                    {
                        program = await _performerSchedulingService.UpdateProgramAsync(
                            model.Program, ageSelection);

                        ShowAlertSuccess($"Program {program.Title} updated!");
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit program: ", gex);
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Performer), new { id = program.PerformerId });
            }

            model.AgeList = new SelectList(ageGroups, "Id", "Name");
            if (model.PerformerId.HasValue)
            {
                var performer = await _performerSchedulingService
                        .GetPerformerByIdAsync(model.PerformerId.Value);
                model.PerformerName = performer.Name;
            }
            return View(model);
        }

        public async Task<IActionResult> ProgramImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program images: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ProgramImagesViewModel()
            {
                ProgramId = program.Id,
                ProgramName = program.Title,
                ProgramImages = program.Images.ToList(),
                MaxUploadMB = MaxUploadMB
            };
            viewModel.ProgramImages.ForEach(_ => _.Filename = _pathResolver
                .ResolveContentPath(_.Filename));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramImages(ProgramImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            if (model.Images == null)
            {
                ModelState.AddModelError("Images", "Please attach an image to submit.");
            }
            else if (model.Images.Count > 0)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => _ != ".jpg" && _ != ".jpeg" && _ != ".png"))
                {
                    ModelState.AddModelError("Images", "Please only attach .jpg or .png images.");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images", $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var image in model.Images)
                {
                    using (var fileStream = image.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            await _performerSchedulingService.AddProgramImageAsync(
                                model.ProgramId, ms.ToArray(), Path.GetExtension(image.FileName));
                        }
                    }
                }
                ShowAlertSuccess("Image(s) added!");
                return RedirectToAction(nameof(ProgramImages), new { id = model.ProgramId });
            }

            var program = new PsProgram();
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(model.ProgramId,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program images: ", gex);
                return RedirectToAction(nameof(Index));
            }

            model.ProgramName = program.Title;
            model.ProgramImages = program.Images.ToList();
            model.ProgramImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(
                _.Filename));
            model.MaxUploadMB = MaxUploadMB;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramImagesDelete(ProgramImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemoveProgramImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }

            return RedirectToAction(nameof(ProgramImages), new { id = model.ProgramId });
        }

        public async Task<IActionResult> PerformerSchedule(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(id,
                    includeSchedule: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update schedule: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new PerformerScheduleViewModel()
            {
                Performer = performer,
                StartDate = settings.ScheduleStartDate.Value,
                EndDate = settings.ScheduleEndDate.Value,
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                ScheduleDates = performer.Schedule.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PerformerSchedule(PerformerScheduleViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var schedule = new List<PsScheduleDate>();
            try
            {
                schedule = JsonConvert.DeserializeObject<List<PsScheduleDate>>(model.JsonSchedule);

                foreach (var date in schedule)
                {
                    date.ParsedDate = DateTime.Parse($"{date.Year}-{date.Month}-{date.Date}");
                    if (Enum.TryParse(date.Availability, out PsScheduleDateStatus status))
                    {
                        date.Status = status;
                    }
                    else
                    {
                        date.Status = PsScheduleDateStatus.Available;
                    }
                }

                try
                {
                    await _performerSchedulingService.EditPerformerScheduleAsync(model.Performer.Id,
                        schedule);
                    ShowAlertSuccess($"Schedule for \"{model.Performer.Name}\" updated!");
                    return RedirectToAction(nameof(Performer), new { id = model.Performer.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Error updating schedule: ", gex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting schedule for performer {model.Performer.Id} by user {GetId(ClaimType.UserId)}: {ex}", ex);
                ShowAlertDanger("An error occured while updating the schedule.");
            }

            var performer = new PsPerformer();
            try
            {
                performer = await _performerSchedulingService.GetPerformerByIdAsync(
                    model.Performer.Id, includeSchedule: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update schedule: ", gex);
                return RedirectToAction(nameof(Index));
            }

            model.Performer = performer;
            model.BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync();
            model.StartDate = settings.ScheduleStartDate.Value;
            model.EndDate = settings.ScheduleEndDate.Value;
            model.ScheduleDates = performer.Schedule.ToList();
            return View(model);
        }
        #endregion

        #region Performer Selections
        public async Task<IActionResult> PerformerSelections(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = await _performerSchedulingService.GetPerformerByIdAsync(id);

            var branchSelections = await _performerSchedulingService
                .GetPerformerBranchSelectionsAsync(performer.Id);

            var branchSelectionDates = branchSelections
                .GroupBy(_ => _.RequestedStartTime.Date)
                .Select(_ => _.ToList())
                .ToList();

            var viewModel = new PerformerSelectionsViewModel
            {
                Performer = performer,
                BranchSelectionDates = branchSelectionDates,
                DefaultPerformerScheduleStartTime = DefaultPerformerScheduleStartTime,
                DefaultPerformerScheduleEndTime = DefaultPerformerScheduleEndTime
            };

            var performerIndexList = await _performerSchedulingService.GetPerformerIndexListAsync();
            var index = performerIndexList.IndexOf(id);
            viewModel.ReturnPage = index / PerformersPerPage + 1;
            if (index != 0)
            {
                viewModel.PrevPerformer = performerIndexList[index - 1];
            }
            if (performerIndexList.Count != index + 1)
            {
                viewModel.NextPerformer = performerIndexList[index + 1];
            }

            return View(viewModel);
        }

        public async Task<IActionResult> GetPerformerCalendar(int performerId, int branchSelectionId)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var performer = await _performerSchedulingService.GetPerformerByIdAsync(performerId,
                includeSchedule: true);

            var branchSelections = await _performerSchedulingService
                .GetPerformerBranchSelectionsAsync(performer.Id);
            var branchSelection = branchSelections.Where(_ => _.Id == branchSelectionId).Single();

            var viewModel = new PerformerCalendarViewModel()
            {
                BranchSelection = branchSelection,
                Settings = settings,
                Performer = performer,
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                BookedDates = branchSelections.Select(_ => _.RequestedStartTime)
            };

            var dayScheduleViewModel = new DayScheduleViewModel()
            {
                BranchSelections = branchSelections
                    .Where(_ => _.RequestedStartTime.Date == branchSelection.RequestedStartTime.Date)
                    .OrderBy(_ => _.RequestedStartTime)
                    .ToList()
            };

            var performerDaySchedule = performer.Schedule
                .Where(_ => _.Date == branchSelection.RequestedStartTime.Date)
                .SingleOrDefault();

            var startTime = performerDaySchedule?.StartTime ?? DefaultPerformerScheduleStartTime;
            var endTime = performerDaySchedule?.EndTime ?? DefaultPerformerScheduleEndTime;

            var earliestSelection = branchSelections
                .OrderBy(_ => _.ScheduleStartTime)
                .FirstOrDefault();
            var latestSelection = branchSelections
                .OrderByDescending(_ => _.ScheduleStartTime)
                .FirstOrDefault();

            if (earliestSelection == null
                || startTime.TimeOfDay < earliestSelection.ScheduleStartTime.TimeOfDay)
            {
                dayScheduleViewModel.StartTime = startTime.ToShortTimeString();
            }
            if (latestSelection == null ||
                endTime.TimeOfDay > latestSelection.ScheduleStartTime
                    .AddMinutes(latestSelection.ScheduleDuration).TimeOfDay)
            {
                dayScheduleViewModel.EndTime = endTime.ToShortTimeString();
            }

            viewModel.DayScheduleModel = dayScheduleViewModel;

            return PartialView("_performerCalendarPartial", viewModel);
        }

        public async Task<IActionResult> GetPerformerDaySchedule(int performerId, DateTime date)
        {
            var performerSchedule = await _performerSchedulingService.GetPerformerDateScheduleAsync(
                performerId, date.Date);

            var branchSelections = (await _performerSchedulingService
                .GetPerformerBranchSelectionsAsync(performerId, date)).ToList();

            var viewModel = new DayScheduleViewModel()
            {
                BranchSelections = branchSelections
            };
            var startTime = performerSchedule?.StartTime ?? DefaultPerformerScheduleStartTime;
            var endTime = performerSchedule?.EndTime ?? DefaultPerformerScheduleEndTime;

            var earliestSelection = branchSelections
                .OrderBy(_ => _.ScheduleStartTime)
                .FirstOrDefault();
            var latestSelection = branchSelections
                .OrderByDescending(_ => _.ScheduleStartTime)
                .FirstOrDefault();

            if (earliestSelection == null
                || startTime.TimeOfDay < earliestSelection.ScheduleStartTime.TimeOfDay)
            {
                viewModel.StartTime = startTime.ToShortTimeString();
            }
            if (latestSelection == null ||
                endTime.TimeOfDay > latestSelection.ScheduleStartTime
                    .AddMinutes(latestSelection.ScheduleDuration).TimeOfDay)
            {
                viewModel.EndTime = endTime.ToShortTimeString();
            }

            return PartialView("_DaySchedulePartial", viewModel);
        }

        public async Task<JsonResult> CheckProgramTimeAvailability(int programId, DateTime date,
            bool backToBack)
        {
            try
            {
                var result = await _performerSchedulingService
                    .ValidateScheduleTimeAsync(programId, date, backToBack);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    return Json(new
                    {
                        success = true,
                        message = result
                    });
                }
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditBranchProgramSelection(PsBranchSelection branchSelection)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return Json(new
                {
                    success = false,
                    message = "Program selection has not yet started."
                });
            }

            try
            {
                await _performerSchedulingService.UpdateBranchProgramSelectionAsync(branchSelection);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            _logger.LogInformation($"Selection {branchSelection.Id} edited");

            ShowAlertSuccess("Program selection edited!");
            return Json(new
            {
                success = true
            });
        }

        [HttpPost]
        public async Task<JsonResult> SetSecretCode(int id, string secretCode)
        {
            secretCode = secretCode?.Trim().ToLower();

            try
            {
                await _performerSchedulingService.SetSelectionSecretCodeAsync(id,
                    secretCode);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    secretCode,
                    message = gex.Message
                });
            }

            return Json(new
            {
                success = true,
                secretCode
            });
        }
        #endregion

        #region Kits
        public async Task<IActionResult> Kits(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var filter = new BaseFilter(page, KitsPerPage);

            var kitList = await _performerSchedulingService.GetPaginatedKitListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = kitList.Count,
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

            if (schedulingStage >= PsSchedulingStage.SchedulingOpen)
            {
                foreach (var kit in kitList.Data)
                {

                    kit.SelectionsCount = await _performerSchedulingService
                        .GetKitSelectionCountAsync(kit.Id);
                }
            }

            var viewModel = new KitListViewModel
            {
                Kits = kitList.Data.ToList(),
                PaginateModel = paginateModel,
                PerformerSchedulingEnbabled = true,
                SchedulingStage = schedulingStage
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> KitDelete(KitListViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                try
                {
                    await _performerSchedulingService.RemoveKitAsync(
                        model.KitToDelete.Id);
                    ShowAlertSuccess($"Kit \"{model.KitToDelete.Name}\" removed!.");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to remove kit: ", gex);
                }
            }
            else
            {
                ShowAlertDanger("Cannot remove kit after scheduling has opened.");
            }

            return RedirectToAction(nameof(Kits), new { page = model.PaginateModel.CurrentPage });
        }

        public async Task<IActionResult> Kit(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeAgeGroups: true,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view kit: ", gex);
                return RedirectToAction(nameof(Kits));
            }

            var viewModel = new KitViewModel()
            {
                Kit = kit,
                SchedulingStage = schedulingStage
            };

            if (kit.Images.Any())
            {
                viewModel.ImagePath = _pathResolver.ResolveContentPath(
                    kit.Images.First().Filename);
            }

            if (!string.IsNullOrWhiteSpace(kit.Website))
            {
                if (Uri.TryCreate(kit.Website, UriKind.Absolute, out Uri absoluteUri))
                {
                    viewModel.Uri = absoluteUri;
                }
            }

            var kitIndexList = await _performerSchedulingService.GetKitIndexListAsync();
            var index = kitIndexList.IndexOf(id);
            viewModel.ReturnPage = index / KitsPerPage + 1;
            if (index != 0)
            {
                viewModel.PrevKit = kitIndexList[index - 1];
            }
            if (kitIndexList.Count != index + 1)
            {
                viewModel.NextKit = kitIndexList[index + 1];
            }

            return View(viewModel);
        }

        public async Task<IActionResult> KitAdd()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();

            var viewModel = new KitDetailsViewModel()
            {
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                MaxUploadMB = MaxUploadMB,
                NewKit = true
            };

            return View(nameof(KitDetails), viewModel);
        }

        public async Task<IActionResult> KitDetails(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update kit: ", gex);
                return RedirectToAction(nameof(Kits));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();

            var viewModel = new KitDetailsViewModel()
            {
                AgeList = new SelectList(ageGroups, "Id", "Name"),
                AgeSelection = kit.AgeGroups.Select(_ => _.Id).ToList(),
                Kit = kit
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> KitDetails(KitDetailsViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageSelection = ageGroups
                .Where(_ => model.AgeSelection?.Contains(_.Id) == true)
                .Select(_ => _.Id)
                .ToList();
            if (ageSelection.Count() == 0)
            {
                ModelState.AddModelError("AgeSelection", "Please select age groups.");
            }

            if (model.NewKit)
            {
                if (model.Images == null)
                {
                    ModelState.AddModelError("Images", "Please attach an image to submit.");
                }
                else
                {
                    var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                    if (extensions.Any(_ => _ != ".jpg" && _ != ".jpeg" && _ != ".png"))
                    {
                        ModelState.AddModelError("Images", "Please only attach .jpg or .png images.");
                    }
                    else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                    {
                        ModelState.AddModelError("Images", $"Please limit uploads to a max of {MaxUploadMB}MB.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var kit = new PsKit();
                    if (model.NewKit)
                    {
                        kit = await _performerSchedulingService.AddKitAsync(model.Kit, ageSelection);

                        foreach (var image in model.Images)
                        {
                            using (var fileStream = image.OpenReadStream())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    fileStream.CopyTo(ms);
                                    await _performerSchedulingService.AddKitImageAsync(
                                        kit.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                                }
                            }
                        }

                        ShowAlertSuccess($"Kit \"{kit.Name}\" added!");
                    }
                    else
                    {
                        kit = await _performerSchedulingService.UpdateKitAsync(model.Kit, ageSelection);
                        ShowAlertSuccess($"Kit \"{kit.Name}\" updated!");
                    }
                    return RedirectToAction(nameof(Kit), new { id = kit.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger($"Unable to {(model.NewKit ? "add" : "update")} kit: ", gex);
                }
            }

            model.AgeList = new SelectList(ageGroups, "Id", "Name");
            if (model.NewKit)
            {
                model.MaxUploadMB = MaxUploadMB;
            }
            return View(model);
        }

        public async Task<IActionResult> KitImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view kit images: ", gex);
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new KitImagesViewModel()
            {
                KitId = kit.Id,
                KitName = kit.Name,
                KitImages = kit.Images.ToList(),
                MaxUploadMB = MaxUploadMB
            };
            viewModel.KitImages.ForEach(_ => _.Filename =
                _pathResolver.ResolveContentPath(_.Filename));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> KitImages(KitImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(model.KitId,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add kit images: ", gex);
                return RedirectToAction(nameof(Index));
            }

            if (model.Images == null)
            {
                ModelState.AddModelError("Images", "Please attach an image to submit.");
            }
            else if (model.Images.Count > 0)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => _ != ".jpg" && _ != ".jpeg" && _ != ".png"))
                {
                    ModelState.AddModelError("Images", "Please only attach .jpg or .png images.");
                }
                else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                {
                    ModelState.AddModelError("Images", $"Please limit uploads to a max of {MaxUploadMB}MB.");
                }
            }
            if (ModelState.IsValid)
            {
                foreach (var image in model.Images)
                {
                    using (var fileStream = image.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            await _performerSchedulingService.AddKitImageAsync(
                                kit.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                        }
                    }
                }
                ShowAlertSuccess("Image(s) added!");
                return RedirectToAction(nameof(KitImages));
            }

            var kitImages = kit.Images.ToList();
            kitImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));


            model.KitName = kit.Name;
            model.KitImages = kitImages;
            model.MaxUploadMB = MaxUploadMB;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> KitImagesDelete(KitImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemoveKitImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }

            return RedirectToAction(nameof(KitImages), new { id = model.KitId });
        }
        #endregion

        #region Kit Selections
        public async Task<IActionResult> KitSelections(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(id, includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"Could not view kit selections: ", gex.Message);
                return RedirectToAction(nameof(Kits));
            }

            var kits = await _performerSchedulingService.GetAllKitsAsync();
            var branchSelections = await _performerSchedulingService.GetKitBranchSelectionsAsync(id);

            var viewModel = new KitSelectionsViewModel()
            {
                Kit = kit,
                BranchSelections = branchSelections,
                KitList = new SelectList(kits, "Id", "Name", kit.Id),
                AgeGroupList = new SelectList(kit.AgeGroups, "Id", "Name")
            };

            var kitIndexList = await _performerSchedulingService.GetKitIndexListAsync();
            var index = kitIndexList.IndexOf(id);
            viewModel.ReturnPage = index / KitsPerPage + 1;
            if (index != 0)
            {
                viewModel.PrevKit = kitIndexList[index - 1];
            }
            if (kitIndexList.Count != index + 1)
            {
                viewModel.NextKit = kitIndexList[index + 1];
            }

            return View(viewModel);
        }

        public async Task<JsonResult> GetKitAgeGroups(int kitId)
        {
            var kit = new PsKit();
            try
            {
                kit = await _performerSchedulingService.GetKitByIdAsync(kitId,
                    includeAgeGroups: true);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            return Json(new
            {
                success = true,
                data = new SelectList(kit.AgeGroups.Select(_ => _.Id), "Id", "Name")
            });
        }

        public async Task<JsonResult> CheckBranchAgeGroup(int ageGroupId, int branchId,
            int? currentSelectionId = null)
        {
            var alreadySelected = await _performerSchedulingService
                .BranchAgeGroupAlreadySelectedAsync(ageGroupId, branchId, currentSelectionId);
            if (alreadySelected)
            {
                return Json(new
                {
                    success = false,
                    message = "That branch already has a selection for that age group."
                });
            }
            return Json(new
            {
                success = true,
                message = "Age group available."
            });
        }

        [HttpPost]
        public async Task<JsonResult> EditBranchKitSelection(PsBranchSelection branchSelection)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.SchedulingOpen)
            {
                return Json(new
                {
                    success = false,
                    message = "Kit selection has not yet started."
                });
            }

            if (branchSelection.KitId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No kit selected."
                });
            }

            try
            {
                await _performerSchedulingService.UpdateBranchKitSelectionAsync(branchSelection);
            }
            catch (GraException gex)
            {
                return Json(new
                {
                    success = false,
                    message = gex.Message
                });
            }

            _logger.LogInformation($"Selection {branchSelection.Id} edited");

            ShowAlertSuccess("Kit selection updated!");
            return Json(new
            {
                success = true
            });
        }
        #endregion

        public async Task<IActionResult> Schedule()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var viewModel = new ScheduleDetailViewModel
            {
                Settings = settings,
                PerformerSchedulingEnbabled = _performerSchedulingService
                    .GetSchedulingStage(settings) != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(PsSettings settings)
        {
            if (settings.RegistrationClosed < settings.RegistrationOpen)
            {
                ModelState.AddModelError("Settings.RegistrationClosed", "The Registration Closed date cannot be before the Registration Open date.");
            }
            if (settings.SchedulingPreview < settings.RegistrationClosed)
            {
                ModelState.AddModelError("Settings.SchedulingPreview", "The Schedule Preview date cannot be before the Registration Closed date.");
            }
            if (settings.SchedulingOpen < settings.SchedulingPreview)
            {
                ModelState.AddModelError("Settings.SchedulingOpen", "The Schedule Open date cannot be before the Schedule Preview date.");
            }
            if (settings.SchedulingClosed < settings.SchedulingOpen)
            {
                ModelState.AddModelError("Settings.SchedulingClosed", "The Schedule Closed date cannot be before the Schedule Open date.");
            }
            if (settings.SchedulePosted < settings.SchedulingClosed)
            {
                ModelState.AddModelError("Settings.SchedulePosted", "The Schedule Posted date cannot be before the Schedule Closed date.");
            }
            if (settings.ScheduleEndDate < settings.ScheduleStartDate)
            {
                ModelState.AddModelError("Settings.ScheduleEndDate", "The Schedule End date cannot be before the Schedule Start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _performerSchedulingService.UpdateSettingsAsync(settings);
                    ShowAlertSuccess($"Schedule updated!");
                    return RedirectToAction(nameof(Schedule));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }

            var viewModel = new ScheduleDetailViewModel
            {
                Settings = settings,
                PerformerSchedulingEnbabled = _performerSchedulingService.GetSchedulingStage(settings)
                    != PsSchedulingStage.Unavailable
            };
            return View(viewModel);
        }

        public async Task<IActionResult> BlackoutDates(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService.GetSchedulingStage(settings)
                    != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var filter = new BaseFilter(page);

            var blackoutDateList = await _performerSchedulingService.GetPaginatedBlackoutDatesAsync(
                filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = blackoutDateList.Count,
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

            var viewModel = new BlackoutDatesListViewModel
            {
                BlackoutDates = blackoutDateList.Data,
                PaginateModel = paginateModel,
                PerformerSchedulingEnbabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBlackoutDate(BlackoutDatesListViewModel model)
        {
            try
            {
                var blackoutDate = await _performerSchedulingService.AddBlackoutDateAsync(
                    model.BlackoutDate);
                ShowAlertSuccess($"Added Blackout Date \"{blackoutDate.Reason}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Blackout Date: ", gex);
            }

            return RedirectToAction(nameof(BlackoutDates), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlackoutDate(BlackoutDatesListViewModel model)
        {
            try
            {
                await _performerSchedulingService.RemoveBlackoutDateAsync(model.BlackoutDate.Id);
                ShowAlertSuccess($"Blackout Date \"{model.BlackoutDate.Reason}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Blackout Date: ", gex);
            }

            return RedirectToAction(nameof(BlackoutDates), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        public async Task<IActionResult> AgeGroups(int page = 1)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var performerSchedulingEnabled = _performerSchedulingService.GetSchedulingStage(settings)
                    != PsSchedulingStage.Unavailable;
            if (!performerSchedulingEnabled)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var filter = new BaseFilter(page);

            var ageGroupList = await _performerSchedulingService.GetPaginatedAgeGroupsAsync(
                filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = ageGroupList.Count,
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

            var viewModel = new AgeGroupsListViewModel
            {
                AgeGroups = ageGroupList.Data,
                PaginateModel = paginateModel,
                PerformerSchedulingEnbabled = performerSchedulingEnabled
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddAgeGroup(AgeGroupsListViewModel model)
        {
            try
            {
                var ageGroup = await _performerSchedulingService.AddAgeGroupAsync(model.AgeGroup);
                ShowAlertSuccess($"Added Age Group \"{ageGroup.Name}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Age Group: ", gex);
            }

            return RedirectToAction(nameof(AgeGroups), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAgeGroup(AgeGroupsListViewModel model)
        {
            try
            {
                await _performerSchedulingService.RemoveAgeGroupAsync(model.AgeGroup.Id);
                ShowAlertSuccess($"Age Group \"{model.AgeGroup.Name}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Age Group: ", gex);
            }

            return RedirectToAction(nameof(AgeGroups), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }
    }
}
