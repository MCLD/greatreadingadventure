using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmailValidation;
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

        private readonly AuthenticationService _authenticationService;
        private readonly ILogger<HomeController> _logger;
        private readonly PerformerSchedulingService _performerSchedulingService;
        private readonly UserService _userService;

        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            PerformerSchedulingService performerSchedulingService,
            UserService userService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticationService = authenticationService
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _performerSchedulingService = performerSchedulingService
                ?? throw new ArgumentNullException(nameof(performerSchedulingService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Performer Registration";
        }

        public static string Name
        { get { return "Home"; } }

        [AllowAnonymous]
        public IActionResult AuthorizationCode()
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                return RedirectToSignIn();
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthorizationCode(AuthorizationCodeViewModel viewmodel)
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToSignIn();
            }

            if (ModelState.IsValid)
            {
                string sanitized = viewmodel.AuthorizationCode.Trim().ToLowerInvariant();

                try
                {
                    string role
                        = await _userService.ActivateAuthorizationCode(sanitized);

                    if (!string.IsNullOrEmpty(role))
                    {
                        var auth = await _authenticationService
                            .RevalidateUserAsync(GetId(ClaimType.UserId));
                        // TODO globalize
                        auth.Message = $"Code applied, you are a member of the role: {role}.";
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
            return View();
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
                includeBranches: true,
                includeImages: true,
                includePrograms: true,
                includeSchedule: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            if (string.IsNullOrEmpty(settings.VendorIdPrompt))
            {
                settings.VendorIdPrompt = "Vendor ID";
            }

            var viewModel = new DashboardViewModel
            {
                BlackoutDates = await _performerSchedulingService.GetBlackoutDatesAsync(),
                BranchAvailability = performer.Branches.Select(_ => _.Id).ToList(),
                IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen,
                Performer = performer,
                Settings = settings,
                Systems = await _performerSchedulingService
                    .GetSystemListWithoutExcludedBranchesAsync(),
                EnablePerformerInsuranceQuestion = await
                    GetSiteSettingBoolAsync(SiteSettingKey.Performer.EnableInsuranceQuestion)
            };

            if (performer.Images.Count > 0)
            {
                viewModel.ImagePath = _pathResolver.ResolveContentPath(
                    performer.Images[0].Filename);
            }

            if (!string.IsNullOrWhiteSpace(performer.Website)
                && Uri.TryCreate(performer.Website, UriKind.Absolute, out Uri absoluteUri))
            {
                viewModel.Uri = absoluteUri;
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
                includeImages: true);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var performerImages = performer.Images.ToList();
            performerImages.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            var viewModel = new PerformerImagesViewModel
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
                includeImages: true);

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
                if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                {
                    ModelState.AddModelError("Images", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
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
                    using var fileStream = image.OpenReadStream();
                    using var ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    await _performerSchedulingService.AddPerformerImageAsync(
                        performer.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                }
                ShowAlertSuccess("Image(s) added!");
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
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

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
                    await _performerSchedulingService.RemovePerformerImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }
            return RedirectToAction(nameof(Images));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                return RedirectToSignIn();
            }

            if (!UserHasPermission(Permission.AccessPerformerRegistration))
            {
                // not authorized for Performer registration, redirect to authorization code

                return RedirectToAction(nameof(AuthorizationCode));
            }

            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage == PsSchedulingStage.Unavailable)
            {
                return RedirectToAction(nameof(Controllers.HomeController.Index), "Home",
                    new { Area = string.Empty });
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

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();

            if (string.IsNullOrEmpty(settings.VendorIdPrompt))
            {
                settings.VendorIdPrompt = "Vendor ID";
            }

            var viewModel = new InformationViewModel
            {
                Performer = performer,
                Settings = settings,
                Systems = systems,
                BranchCount = systems.Sum(_ => _.Branches.Count),
                MaxUploadMB = MaxUploadMB,
                EnablePerformerInsuranceQuestion = await
                    GetSiteSettingBoolAsync(SiteSettingKey.Performer.EnableInsuranceQuestion)
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
            else
            {
                var user = await _userService.GetDetails(GetActiveUserId());

                viewModel.Performer = new PsPerformer
                {
                    ContactName = user.FullName,
                    Email = user.Email,
                    Phone = user.PhoneNumber
                };
            }

            PageTitle = "Performer Information";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Information(InformationViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var currentPerformer = await _performerSchedulingService
                .GetPerformerByUserIdAsync(userId);

            if (currentPerformer?.RegistrationCompleted == false)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var systems = await _performerSchedulingService
                .GetSystemListWithoutExcludedBranchesAsync();
            var branchIds = systems.SelectMany(_ => _.Branches).Select(_ => _.Id);
            var BranchAvailability = JsonConvert.DeserializeObject
                <List<int>>(model.BranchAvailabilityString)
                .Where(_ => branchIds.Contains(_)).ToList();

            if (BranchAvailability.Count == 0)
            {
                ModelState.AddModelError("BranchAvailability", "Please select the libraries where you are willing to perform.");
            }

            if (!string.IsNullOrWhiteSpace(model.Performer.Email)
                && !EmailValidator.Validate(model.Performer.Email.Trim()))
            {
                ModelState.AddModelError("Performer.Email",
                    string.Format(CultureInfo.InvariantCulture,
                        Annotations.Validate.Email,
                        "Email"));
            }

            if (currentPerformer == null)
            {
                if (model.Images == null)
                {
                    ModelState.AddModelError("Images", "Please attach an image to submit.");
                }
                else if (model.Images?.Count > 0)
                {
                    var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                    if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                    {
                        ModelState.AddModelError("Images", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                    }
                    else if (model.Images.Sum(_ => _.Length) > MaxUploadMB * MBSize)
                    {
                        ModelState.AddModelError("Images", $"Please limit uploads to a max of {MaxUploadMB}MB.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var performer = model.Performer;
                performer.AllBranches = BranchAvailability.Count == branchIds.Count();

                if (currentPerformer?.RegistrationCompleted == true)
                {
                    performer.Id = currentPerformer.Id;
                    performer = await _performerSchedulingService.EditPerformerAsync(performer,
                        BranchAvailability);
                }
                else
                {
                    performer = await _performerSchedulingService.AddPerformerAsync(performer,
                        BranchAvailability);
                }

                if (!performer.RegistrationCompleted)
                {
                    foreach (var image in model.Images)
                    {
                        using var fileStream = image.OpenReadStream();
                        using var ms = new MemoryStream();
                        fileStream.CopyTo(ms);
                        await _performerSchedulingService.AddPerformerImageAsync(
                            performer.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                    }

                    return RedirectToAction(nameof(Schedule));
                }
                else
                {
                    ShowAlertSuccess("Information saved!");
                    return RedirectToAction(nameof(Dashboard));
                }
            }

            model.BranchCount = systems.Sum(_ => _.Branches.Count);
            model.BranchAvailability = BranchAvailability;
            model.MaxUploadMB = MaxUploadMB;
            model.Settings = settings;
            model.Systems = systems;

            PageTitle = "Performer Information";
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

            if (performer == null)
            {
                return RedirectToAction(nameof(Information));
            }
            else if (!performer.SetSchedule)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageList = new SelectList(ageGroups, "Id", "Name");
            if (ageList.Count() == 1)
            {
                ageList.First().Selected = true;
            }

            var viewModel = new ProgramViewModel
            {
                AgeList = ageList,
                MaxUploadMB = MaxUploadMB,
                RegistrationCompleted = performer.RegistrationCompleted,
                SetupSupplementalText = settings.SetupSupplementalText,
                EnablePerformerLivestreamQuestions = await GetSiteSettingBoolAsync(SiteSettingKey
                    .Performer
                    .EnableLivestreamQuestions),
                BackToBackSelection = new SelectList(new[] { GRA.Defaults.BackToBackInterval })
            };

            if (id.HasValue)
            {
                try
                {
                    var program = await _performerSchedulingService.GetProgramByIdAsync(id.Value,
                        includeAgeGroups: true);
                    viewModel.AgeSelection = program.AgeGroups.Select(_ => _.Id).ToList();
                    viewModel.EditingProgram = true;
                    viewModel.Program = program;
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to view Program: ", gex);
                    return RedirectToAction(nameof(Dashboard));
                }
            }

            var (hasIntervalString, intervalString) = await GetSiteSettingStringAsync(SiteSettingKey.Performer.BackToBackInterval);

            if (hasIntervalString)
            {
                var intervalOptions = intervalString.Split(new[] { ',', ' ' },
                                StringSplitOptions.RemoveEmptyEntries);

                viewModel.BackToBackSelection = new SelectList(intervalOptions);
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

            if (performer == null)
            {
                return RedirectToAction(nameof(Information));
            }
            else if (!performer.SetSchedule)
            {
                return RedirectToAction(nameof(Schedule));
            }

            var ageGroups = await _performerSchedulingService.GetAgeGroupsAsync();
            var ageSelection = ageGroups
                .Where(_ => model.AgeSelection?.Contains(_.Id) == true)
                .Select(_ => _.Id)
                .ToList();
            if (ageSelection.Count == 0)
            {
                ModelState.AddModelError("AgeSelection", "Please select age groups.");
            }

            if (model.Images?.Count > 0)
            {
                var extensions = model.Images.Select(_ => Path.GetExtension(_.FileName).ToLower());
                if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                {
                    ModelState.AddModelError("Images", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
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
                    try
                    {
                        var program = await _performerSchedulingService.UpdateProgramAsync(
                            model.Program, ageSelection);

                        ShowAlertSuccess("Program saved!");
                        return RedirectToAction(nameof(ProgramDetails), new { id = program.Id });
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to update Program: ", gex);
                        return RedirectToAction(nameof(Dashboard));
                    }
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
                            using var fileStream = image.OpenReadStream();
                            using var ms = new MemoryStream();
                            fileStream.CopyTo(ms);
                            await _performerSchedulingService.AddProgramImageAsync(
                                program.Id, ms.ToArray(), Path.GetExtension(image.FileName));
                        }
                    }
                }

                if (!performer.RegistrationCompleted)
                {
                    await _performerSchedulingService.SetPerformerRegistrationCompeltedAsync(
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
            model.SetupSupplementalText = settings.SetupSupplementalText;
            model.BackToBackSelection = new SelectList(new[] { GRA.Defaults.BackToBackInterval });

            var (hasIntervalString, intervalString)
                = await GetSiteSettingStringAsync(SiteSettingKey.Performer.BackToBackInterval);
            if (hasIntervalString)
            {
                var intervalOptions = intervalString.Split(new[] { ',', ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                model.BackToBackSelection = new SelectList(intervalOptions);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProgramDelete(DashboardViewModel model)
        {
            var settings = await _performerSchedulingService.GetSettingsAsync();
            var schedulingStage = _performerSchedulingService.GetSchedulingStage(settings);
            if (schedulingStage != PsSchedulingStage.RegistrationOpen)
            {
                return RedirectToAction(nameof(Index));
            }

            var userId = GetId(ClaimType.UserId);
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
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
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            var program = await _performerSchedulingService.GetProgramByIdAsync(id,
                includeAgeGroups: true);

            if (program == null)
            {
                return RedirectToAction(nameof(Dashboard));
            }

            var viewModel = new ProgramDetailsViewModel
            {
                IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen,
                Program = program,
                EnablePerformerLivestreamQuestions = await
                    GetSiteSettingBoolAsync(SiteSettingKey.Performer.EnableLivestreamQuestions)
            };

            if (program.Images?.Count > 0)
            {
                viewModel.Image = _pathResolver.ResolveContentPath(
                    program.Images[0].Filename);
            }

            if (!program.IsApproved)
            {
                ShowAlertWarning($"There are issues with this program and it will not be available for selection. Please contact {settings.ContactEmail} if you have any questions.");
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
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

            if (performer?.RegistrationCompleted != true)
            {
                return RedirectToAction(nameof(Information));
            }

            PsProgram program;
            try
            {
                program = await _performerSchedulingService.GetProgramByIdAsync(id,
                    includeImages: true);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to view program images: ", gex);
                return RedirectToAction(nameof(Dashboard));
            }

            program.Images.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            var viewModel = new ProgramImagesViewModel
            {
                IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen,
                MaxUploadMB = MaxUploadMB,
                ProgramImages = program.Images,
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
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

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
                if (extensions.Any(_ => !ValidImageExtensions.Contains(_)))
                {
                    ModelState.AddModelError("Images", $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
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
                    using var fileStream = image.OpenReadStream();
                    using var ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    await _performerSchedulingService.AddProgramImageAsync(
                        model.ProgramId, ms.ToArray(), Path.GetExtension(image.FileName));
                }
                ShowAlertSuccess("Image(s) added!");
                return RedirectToAction(nameof(ProgramImages), new { id = model.ProgramId });
            }

            PsProgram program;
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

            program.Images.ForEach(_ => _.Filename = _pathResolver.ResolveContentPath(_.Filename));

            model.IsEditable = schedulingStage == PsSchedulingStage.RegistrationOpen;
            model.MaxUploadMB = MaxUploadMB;
            model.ProgramTitle = program.Title;
            model.ProgramImages = program.Images;
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
            var performer = await _performerSchedulingService.GetPerformerByUserIdAsync(userId);

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
                    await _performerSchedulingService.RemoveProgramImageByIdAsync(imageId);
                }
                ShowAlertSuccess("Image(s) deleted!");
            }
            return RedirectToAction(nameof(ProgramImages), new { id = program.Id });
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
            else if (performer.SetSchedule && !performer.RegistrationCompleted)
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
            else if (performer.SetSchedule && !performer.RegistrationCompleted)
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

                    if (!performer.RegistrationCompleted)
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
                _logger.LogError(ex,
                    "Error submitting schedule for user {UserId}: {ErrorMessage}",
                    userId,
                    ex.Message);
                ShowAlertDanger("There was an error with your schedule, please try submitting " +
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
    }
}
