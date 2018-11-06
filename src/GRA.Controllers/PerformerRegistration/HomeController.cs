using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Controllers.ViewModel.PerformerRegistration.Home;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.PerformerRegistration
{
    [Area("PerformerRegistration")]
    [Authorize(Policy = Policy.AccessPerformerRegistration)]
    public class HomeController : Base.Controller
    {
        private const int MaxUploadMB = 25;
        private const int MBSize = 1024 * 1024;

        private readonly ILogger<HomeController> _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        private readonly ICodeSanitizer _codeSanitizer;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            PerformerSchedulingService performerSchedulingService,
            SiteService siteService,
            UserService userService,
            ICodeSanitizer codeSanitizer) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticationService = authenticationService 
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _codeSanitizer = codeSanitizer 
                ?? throw new ArgumentNullException(nameof(codeSanitizer));
            PageTitle = "Performer Registration";
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction("Index", "Home", new { Area = string.Empty });
            }

            var hasPermission = UserHasPermission(Permission.AccessPerformerRegistration);

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true);

            if (hasPermission 
                && (performer != null || schedulingStage == PsSchedulingStage.RegistrationOpen))
            {
                return RedirectToAction(nameof(Dashboard));
            }

            var viewModel = new IndexViewModel
            {
                HasPermission = hasPermission,
                Settings = settings,
                SchedulingStage = schedulingStage
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthorizationCode(IndexViewModel viewmodel)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = "/PerformerRegistration"
                });
            }

            if (ModelState.IsValid)
            {
                string sanitized = _codeSanitizer.Sanitize(viewmodel.AuthorizationCode, 255);

                try
                {
                    string role
                        = await _userService.ActivateAuthorizationCode(sanitized);

                    if (!string.IsNullOrEmpty(role))
                    {
                        var auth = await _authenticationService
                            .RevalidateUserAsync(GetId(ClaimType.UserId));
                        auth.AuthenticationMessage = $"Code applied, you are now a member of the role: <strong>{role}</strong>.";
                        await LoginUserAsync(auth);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ShowAlertDanger("Invalid code. This request was logged.");
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to activate code: ", gex);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Information()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true);

            if (performer?.RegistrationCompleted == false)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var systems = (await _siteService.GetSystemList()).ToList();

            var viewModel = new InformationViewModel()
            {
                Performer = performer,
                Systems = systems.ToList(),
                BranchCount = systems.Sum(_ => _.Branches.Count()),
                MaxUploadMB = MaxUploadMB
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
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
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
                    else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                    {
                        ModelState.AddModelError("Images", $"Please limit uploads to a max of {MaxUploadMB}MB.");
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
            model.MaxUploadMB = MaxUploadMB;
            model.Systems = systems;

            PageTitle = "Performer Information";
            return View(model);
        }

        public async Task<IActionResult> Schedule()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeSchedule: true);

            if (performer == null)
            {
                return RedirectToAction(nameof(Information));
            }
            else if (performer.SetSchedule && performer.RegistrationCompleted == false)
            {
                return RedirectToAction(nameof(Program));
            }

            var viewModel = new ScheduleViewModel
            {
                StartDate = settings.ScheduleStartDate.Value,
                EndDate = settings.ScheduleEndDate.Value,
                BlackoutDates = (await _performerSchedulingService.GetBlackoutDatesAsync())
                    .ToList(),
                EditingSchedule = performer.RegistrationCompleted
            };

            if (performer.Schedule != null)
            {
                viewModel.ScheduleDates = performer.Schedule.ToList();
            }

            PageTitle = "Schedule";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(ScheduleViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeSchedule: true);

            if (performer == null)
            {
                return RedirectToAction(nameof(Information));
            }
            else if (performer.SetSchedule && performer.RegistrationCompleted == false)
            {
                return RedirectToAction(nameof(Program));
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
                    await _performerSchedulingService.EditPerformerScheduleAsync(performer.Id,
                        schedule);

                    if (performer.RegistrationCompleted == false)
                    {
                        return RedirectToAction(nameof(Program));
                    }
                    else
                    {
                        ShowAlertSuccess("Schedule saved!");
                        return RedirectToAction(nameof(Dashboard));
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Error saving schedule: ", gex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error submitting schedule for user {userId}: {ex}", ex);
                ShowAlertDanger($"There was an error with your schedule, please try submitting " +
                    $"it again or contact {settings.ContactEmail} for assistance.");
            }

            model.BlackoutDates = (await _performerSchedulingService.GetBlackoutDatesAsync())
                .ToList();
            model.EditingSchedule = performer.RegistrationCompleted;
            model.StartDate = settings.ScheduleStartDate.Value;
            model.EndDate = settings.ScheduleEndDate.Value;

            if (performer.Schedule != null)
            {
                model.ScheduleDates = performer.Schedule.ToList();
            }

            PageTitle = "Schedule";
            return View(model);
        }

        public async Task<IActionResult> Program(int? id = null)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

            if (performer != null && performer.SetSchedule == false)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageList = new SelectList(ageGroups, "Id", "Name");
            if (ageList.Count() == 1)
            {
                ageList.First().Selected = true;
            }

            var viewModel = new ProgramViewModel()
            {
                AgeList = ageList,
                MaxUploadMB = MaxUploadMB,
                RegistrationCompleted = performer.RegistrationCompleted
            };

            if (id.HasValue)
            {
                string unableToView = string.Empty;
                try
                {
                    var program = await _performerSchedulingService.GetProgramByIdAsync(id.Value);
                    if (program != null)
                    {
                        unableToView = "Program could not be found.";
                    }
                    else
                    {
                        viewModel.AgeSelection = program.AgeGroups.Select(_ => _.Id).ToList();
                        viewModel.EditingProgram = true;
                        viewModel.Program = program;
                    }
                }
                catch (GraException gex)
                {
                    unableToView = gex.Message;
                }
                if (!string.IsNullOrWhiteSpace(unableToView))
                {
                    ShowAlertDanger("Unable to view Program: ", unableToView);
                    return RedirectToAction(nameof(Dashboard));
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Program(ProgramViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

            if (performer != null && performer.SetSchedule == false)
            {
                return RedirectToAction(nameof(Schedule));
            }

            if (model.EditingProgram)
            {
                string unableToSave = string.Empty;
                try
                {
                    var program = await _performerSchedulingService.GetProgramByIdAsync(
                        model.Program.Id);
                    if (program == null)
                    {
                        unableToSave = "Program could not be found.";
                    }
                }
                catch (GraException gex)
                {
                    unableToSave = gex.Message;
                }
                if (!string.IsNullOrWhiteSpace(unableToSave))
                {
                    ShowAlertDanger("Unable to save Program: ", unableToSave);
                    return RedirectToAction(nameof(Dashboard));
                }
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

            if (model.Images != null && model.Images.Count > 0)
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
                if (model.EditingProgram)
                {
                    var program = await _performerSchedulingService.UpdateProgramAsync(
                        model.Program, ageSelection);

                    ShowAlertSuccess("Program saved!");
                    return RedirectToAction(nameof(ProgramDetails), new { id = program.Id });
                }
                else
                {
                    model.Program.PerformerId = performer.Id;
                    var program = await _performerSchedulingService.AddProgramAsync(model.Program,
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
                }

                if (performer.RegistrationCompleted == false)
                {
                    await _performerSchedulingService.SetPerformerRegistrationCompelted(
                        performer.Id);
                    ShowAlertSuccess("Registration completed!");
                }
                else
                {
                    ShowAlertSuccess("Program added!");
                }

                return RedirectToAction(nameof(Dashboard));
            }

            var ageList = new SelectList(ageGroups, "Id", "Name");
            if (ageList.Count() == 1)
            {
                ageList.First().Selected = true;
            }
            model.AgeList = ageList;
            model.MaxUploadMB = MaxUploadMB;
            return View(model);
        }

        public async Task<IActionResult> Dashboard()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var viewModel = new DashboardViewModel
            {
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                BranchAvailability = performer.Branches.Select(_ => _.Id).ToList(),
                IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen,
                Performer = performer,
                ReferencesPath = _pathResolver.ResolveContentPath(performer.ReferencesFilename),
                Settings = settings,
                Systems = await _siteService.GetSystemList()
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

            return View(viewModel);
        }

        public async Task<IActionResult> Images()
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var performerImages = performer.Images.ToList();
            performerImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            var viewModel = new PerformerImagesViewModel()
            {
                IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen,
                MaxUploadMB = MaxUploadMB,
                PerformerImages = performerImages
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Images(PerformerImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
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
                                performer.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                        }
                    }
                }
                TempData[TempDataKey.AlertSuccess] = "Image(s) added!";
                return RedirectToAction(nameof(Images));
            }

            var performerImages = performer.Images.ToList();
            performerImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            model.IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen;
            model.MaxUploadMB = MaxUploadMB;
            model.PerformerImages = performerImages;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ImagesDelete(PerformerImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemovePerformerImageAsync(imageId);
                }
                TempData[TempDataKey.AlertSuccess] = "Image(s) deleted!";
            }
            return RedirectToAction(nameof(Images));
        }

        [HttpPost]
        public async Task<IActionResult> ProgramDelete(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var program = await _performerSchedulingService.GetProgramByIdAsync(id);

            if (program != null)
            {
                await _performerSchedulingService.RemoveProgramAsync(id);

                TempData[TempDataKey.AlertSuccess] = $"Program \"{program.Title}\" has been deleted!";
            }
            else
            {
                TempData[TempDataKey.AlertWarning] = $"Unable to delete the program \"{program.Title}\".";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> ProgramDetails(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var program = await _performerSchedulingService.GetProgramByIdAsync(id);

            if (program == null)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            var viewModel = new ProgramDetailsViewModel()
            {
                IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen,
                Program = program
            };

            if (program.ProgramImages?.Count > 0)
            {
                viewModel.Image = _pathResolver.ResolveContentPath(
                    program.ProgramImages.First().Filename);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ProgramImages(int id)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage < PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var program = await _performerSchedulingService.GetProgramByIdAsync(id);

            if (program == null)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            var programImages = program.ProgramImages.ToList();
            programImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            var viewModel = new ProgramImagesViewModel()
            {
                IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen,
                MaxUploadMB = MaxUploadMB,
                ProgramImages = programImages,
                ProgramId = program.Id,
                ProgramTitle = program.Title
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramImages(ProgramImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var program = await _performerSchedulingService.GetProgramByIdAsync(model.ProgramId);

            if (program == null)
            {
                return RedirectToAction(nameof(Dashboard));
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
                                program.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                        }
                    }
                }
                TempData[TempDataKey.AlertSuccess] = "Image(s) added!";
                return RedirectToAction(nameof(ProgramImages), new { id = program.Id });
            }

            var programImages = program.ProgramImages.ToList();
            programImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            model.IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen;
            model.MaxUploadMB = MaxUploadMB;
            model.ProgramTitle = program.Title;
            model.ProgramImages = programImages;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramImagesDelete(ProgramImagesViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId,
                includeBranches: true, includePrograms: true, includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var program = await _performerSchedulingService.GetProgramByIdAsync(model.ProgramId);

            if (program == null)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            var imageIds = Newtonsoft.Json.JsonConvert
                .DeserializeObject<List<int>>(model.ImagesToDelete);
            if (imageIds.Count > 0)
            {
                foreach (var imageId in imageIds)
                {
                    await _performerSchedulingService.RemovePerformerImageAsync(imageId);
                }
                TempData[TempDataKey.AlertSuccess] = "Image(s) deleted!";
            }
            return RedirectToAction(nameof(ProgramImages), new { id = program.Id });
        }
    }
}
