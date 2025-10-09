using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Challenges;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewAllChallenges)]
    public class ChallengesController : Base.MCController
    {
        private const string EditTask = "EditTask";
        private const string NewTask = "NewTask";
        private const string TempEditChallenge = "TempEditChallenge";

        private readonly BadgeService _badgeService;
        private readonly CategoryService _categoryService;
        private readonly ChallengeService _challengeService;
        private readonly EventService _eventService;
        private readonly ILogger<ChallengesController> _logger;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public ChallengesController(ILogger<ChallengesController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            CategoryService categoryService,
            ChallengeService challengeService,
            EventService eventService,
            SiteService siteService,
            UserService userService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(badgeService);
            ArgumentNullException.ThrowIfNull(categoryService);
            ArgumentNullException.ThrowIfNull(challengeService);
            ArgumentNullException.ThrowIfNull(eventService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(userService);

            _badgeService = badgeService;
            _categoryService = categoryService;
            _challengeService = challengeService;
            _eventService = eventService;
            _logger = logger;
            _siteService = siteService;
            _userService = userService;

            PageTitle = "Challenges";
        }

        public static string Name
        { get { return "Challenges"; } }

        [Authorize(Policy = Policy.AddChallenges)]
        public async Task<IActionResult> Create()
        {
            var site = await GetCurrentSiteAsync();
            PageTitle = "Create Challenge";

            var viewModel = new ChallengesDetailViewModel
            {
                IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits),
                MaxPointLimit = await _challengeService.GetMaximumAllowedPointsAsync(site.Id)
            };

            viewModel = await GetDetailLists(viewModel);
            if (viewModel.MaxPointLimit.HasValue)
            {
                viewModel.MaxPointsMessage
                    = $"(Up to {viewModel.MaxPointLimit.Value} points per required task)";
            }

            return View(viewModel);
        }

        [Authorize(Policy = Policy.AddChallenges)]
        [HttpPost]
        public async Task<IActionResult> Create(ChallengesDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            byte[] badgeBytes = null;

            model.IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits);
            model.MaxPointLimit = await _challengeService
                .GetMaximumAllowedPointsAsync(GetCurrentSiteId());
            if (!model.IgnorePointLimits
                && model.Challenge.TasksToComplete.HasValue
                && model.Challenge.TasksToComplete != 0)
            {
                double pointsPerChallenge =
                    (double)model.Challenge.PointsAwarded / model.Challenge.TasksToComplete.Value;
                if (pointsPerChallenge > model.MaxPointLimit)
                {
                    ModelState.AddModelError("Challenge.PointsAwarded",
                        $"A challenge with {model.Challenge.TasksToComplete} tasks may award a maximum of {model.MaxPointLimit * model.Challenge.TasksToComplete} points.");
                }
            }
            if (string.IsNullOrWhiteSpace(model.BadgeAltText))
            {
                if ((!string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.UseBadgeMaker) ||
                    (model.BadgeUploadImage != null && !model.UseBadgeMaker))
                {
                    ModelState.AddModelError("BadgeAltText",
                        "The badge's alternative text is required.");
                }
            }
            else
            {
                if ((model.BadgeUploadImage == null && !model.UseBadgeMaker)
                    || (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && model.UseBadgeMaker))
                {
                    ModelState.AddModelError("BadgePath", "A badge is required.");
                }
                if (model.BadgeUploadImage != null
                    && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) && !model.UseBadgeMaker))
                {
                    if (!ValidImageExtensions.Contains(Path
                        .GetExtension(model.BadgeUploadImage.FileName),
                            StringComparer.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("BadgeUploadImage",
                            $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                    }
                }
                if (model.BadgeUploadImage != null)
                {
                    try
                    {
                        await using (var ms = new MemoryStream())
                        {
                            await model.BadgeUploadImage.CopyToAsync(ms);
                            badgeBytes = ms.ToArray();
                        }
                        await _badgeService.ValidateBadgeImageAsync(badgeBytes);
                    }
                    catch (GraException gex)
                    {
                        ModelState.AddModelError("BadgeUploadImage", gex.Message);
                    }
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var challenge = model.Challenge;
                    if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                    {
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                            && (model.BadgeUploadImage == null || model.UseBadgeMaker))
                        {
                            var badgeString = model.BadgeMakerImage.Split(',').Last();
                            badgeBytes = Convert.FromBase64String(badgeString);
                            filename = "badge.png";
                        }
                        else
                        {
                            if (badgeBytes == null)
                            {
                                await using var ms = new MemoryStream();
                                await model.BadgeUploadImage.CopyToAsync(ms);
                                badgeBytes = ms.ToArray();
                            }
                            filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                        }
                        var newBadge = new Badge
                        {
                            Filename = filename,
                            AltText = model.BadgeAltText
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        challenge.BadgeId = badge.Id;
                    }
                    var serviceResult = await _challengeService.AddChallengeAsync(challenge);
                    if (serviceResult.Status == ServiceResultStatus.Warning
                        && !string.IsNullOrWhiteSpace(serviceResult.Message))
                    {
                        ShowAlertWarning(serviceResult.Message);
                    }
                    AlertSuccess = $"Challenge '<strong>{serviceResult.Data.Name}</strong>' was successfully created";
                    return RedirectToAction("Edit", new { id = serviceResult.Data.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to add challenge: ", gex.Message);
                }
            }

            PageTitle = "Create Challenge";
            model = await GetDetailLists(model);

            if (model.MaxPointLimit.HasValue)
            {
                model.MaxPointsMessage
                    = $"(Up to {model.MaxPointLimit.Value} points per required task)";
            }

            return View(model);
        }

        [Authorize(Policy = Policy.RemoveChallenges)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _challengeService.RemoveChallengeAsync(id);
                ShowAlertSuccess("Challenge deleted.");
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete challenge: ", gex.Message);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> Edit(int id)
        {
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteLookupService.GetSiteLinkAsync(site.Id);
            Challenge challenge;
            try
            {
                challenge = await _challengeService.MCGetChallengeDetailsAsync(id);

                foreach (var task in challenge.Tasks)
                {
                    if (task.ChallengeTaskType == ChallengeTaskType.Action)
                    {
                        task.Description = CommonMark.CommonMarkConverter.Convert(task.Title);
                    }
                    if (!string.IsNullOrWhiteSpace(task.Filename))
                    {
                        var contentPath = _pathResolver.ResolveContentPath(task.Filename);
                        task.Filename = $"{siteUrl}{contentPath}";
                    }
                }
                if (TempData.TryGetValue(TempEditChallenge, out object value))
                {
                    var storedChallenge = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<Challenge>((string)value);

                    challenge.Name = storedChallenge.Name;
                    challenge.Description = storedChallenge.Description;
                    challenge.PointsAwarded = storedChallenge.PointsAwarded;
                    challenge.TasksToComplete = storedChallenge.TasksToComplete;
                    challenge.LimitToSystemId = storedChallenge.LimitToSystemId;
                    challenge.LimitToBranchId = storedChallenge.LimitToBranchId;
                    challenge.AssociatedProgramId = storedChallenge.AssociatedProgramId;
                    challenge.CategoryIds = storedChallenge.CategoryIds;
                }
                else
                {
                    challenge.CategoryIds = challenge.Categories.Select(_ => _.Id).ToList();
                }
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view challenge: ", gex);
                return RedirectToAction("Index");
            }

            if (challenge.TasksToComplete > challenge.Tasks.Count())
            {
                var taskLabel = challenge.TasksToComplete != 1 ? "s" : "";
                AlertInfo = $"This challenge cannot currently be completed, it requires <strong>{challenge.TasksToComplete}</strong> task{taskLabel} to be complete but only has <strong>{challenge.Tasks.Count()}</strong> assigned.";
            }

            bool canActivate = challenge.IsValid
                && !challenge.IsActive
                && (UserHasPermission(Permission.ActivateAllChallenges)
                    || (UserHasPermission(Permission.ActivateSystemChallenges)
                        && challenge.RelatedSystemId == GetId(ClaimType.SystemId)));

            var viewModel = new ChallengesDetailViewModel
            {
                Challenge = challenge,
                CanActivate = canActivate,
                CanEditGroups = UserHasPermission(Permission.EditChallengeGroups),
                CanManageEvents = UserHasPermission(Permission.ManageEvents),
                CanViewParticipants = UserHasPermission(Permission.ViewParticipantDetails),
                CanViewTriggers = UserHasPermission(Permission.ManageTriggers),
                CreatedByName = await _userService.GetUsersNameByIdAsync(challenge.CreatedBy),
                DependentTriggers = await _challengeService.GetDependentsAsync(challenge.Id),
                Groups = await _challengeService.GetGroupsByChallengeId(challenge.Id),
                RelatedEvents = await _eventService.GetByChallengeIdAsync(challenge.Id),
                IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits),
                MaxPointLimit = await _challengeService.GetMaximumAllowedPointsAsync(site.Id)
            };
            if (viewModel.MaxPointLimit.HasValue)
            {
                viewModel.MaxPointsMessage
                    = $"(Up to {viewModel.MaxPointLimit.Value} points per required task)";
            }
            if (challenge.TasksToComplete.HasValue
                && viewModel.MaxPointLimit.HasValue)
            {
                double pointsPerChallenge = (double)viewModel.Challenge.PointsAwarded
                    / viewModel.Challenge.TasksToComplete.Value;
                if (pointsPerChallenge > viewModel.MaxPointLimit)
                {
                    viewModel.MaxPointsWarningMessage = $"This Challenge exceeds the maximum of {viewModel.MaxPointLimit.Value} points per required task - only Administrators can edit it.";
                }
            }

            if (challenge.BadgeId != null)
            {
                var challengeBadge = await _badgeService.GetByIdAsync((int)challenge.BadgeId);
                if (challengeBadge != null)
                {
                    viewModel.BadgePath = _pathResolver.ResolveContentPath(challengeBadge.Filename);
                    viewModel.BadgeAltText = challengeBadge.AltText;
                }
            }

            if (TempData.ContainsKey(NewTask))
            {
                TempData.Remove(NewTask);
                TempData.Remove(EditTask);
                viewModel.AddTask = true;
            }
            else if (TempData.TryGetValue(EditTask, out object value))
            {
                viewModel.Task = await _challengeService
                    .GetTaskAsync((int)value);
                if (!string.IsNullOrWhiteSpace(viewModel.Task.Filename))
                {
                    var contentPath = _pathResolver.ResolveContentPath(viewModel.Task.Filename);
                    viewModel.TaskFilePath = $"{siteUrl}{contentPath}";
                }
            }
            PageTitle = $"Edit Challenge - {viewModel.Challenge.Name}";

            viewModel = await GetDetailLists(viewModel);

            return View("Edit", viewModel);
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> Edit(ChallengesDetailViewModel model, string Submit)
        {
            ArgumentNullException.ThrowIfNull(model);

            byte[] badgeBytes = null;

            model.IgnorePointLimits = UserHasPermission(Permission.IgnorePointLimits);
            model.MaxPointLimit = await _challengeService
                .GetMaximumAllowedPointsAsync(GetCurrentSiteId());
            if (model.Challenge.TasksToComplete.HasValue
                && model.Challenge.TasksToComplete != 0
                && model.Challenge.PointsAwarded != 0
                && !model.IgnorePointLimits)
            {
                double pointsPerChallenge =
                    (double)model.Challenge.PointsAwarded / model.Challenge.TasksToComplete.Value;
                if (pointsPerChallenge > model.MaxPointLimit)
                {
                    ModelState.AddModelError("Challenge.PointsAwarded",
                        $"A challenge with {model.Challenge.TasksToComplete} tasks may award a maximum of {model.MaxPointLimit.Value * model.Challenge.TasksToComplete} points.");
                }
            }

            var existingBadge = model.Challenge.BadgeId.HasValue ?
                await _badgeService.GetByIdAsync(model.Challenge.BadgeId.Value) : null;
            if (string.IsNullOrWhiteSpace(model.BadgeAltText))
            {
                if ((model.BadgeUploadImage != null)
                    || (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                    || existingBadge != null)
                {
                    ModelState.AddModelError("BadgeAltText",
                        "The badge's alternative text is required.");
                }
            }
            else if ((model.BadgeUploadImage == null)
                && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker)
                && ((!string.IsNullOrWhiteSpace(model.BadgeMakerImage) && !model.UseBadgeMaker)
                    || existingBadge == null))
            {
                ModelState.AddModelError("BadgePath", "A badge is required.");
            }

            if (model.BadgeUploadImage != null
                && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker))
            {
                if (!ValidImageExtensions.Contains(Path
                    .GetExtension(model.BadgeUploadImage.FileName),
                        StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("BadgeUploadImage",
                        $"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                }
                if (model.BadgeUploadImage != null)
                {
                    try
                    {
                        await using (var ms = new MemoryStream())
                        {
                            await model.BadgeUploadImage.CopyToAsync(ms);
                            badgeBytes = ms.ToArray();
                        }
                        await _badgeService.ValidateBadgeImageAsync(badgeBytes);
                    }
                    catch (GraException gex)
                    {
                        ModelState.AddModelError("BadgeUploadImage", gex.Message);
                    }
                }
            }
            if (ModelState.IsValid)
            {
                var challenge = model.Challenge;
                if (model.BadgeUploadImage != null
                    || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                {
                    string filename;
                    if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage)
                        && (model.BadgeUploadImage == null || model.UseBadgeMaker))
                    {
                        var badgeString = model.BadgeMakerImage.Split(',').Last();
                        badgeBytes = Convert.FromBase64String(badgeString);
                        filename = "badge.png";
                    }
                    else
                    {
                        if (badgeBytes == null)
                        {
                            await using var ms = new MemoryStream();
                            await model.BadgeUploadImage.CopyToAsync(ms);
                            badgeBytes = ms.ToArray();
                        }
                        filename = Path.GetFileName(model.BadgeUploadImage.FileName);
                    }
                    if (challenge.BadgeId == null)
                    {
                        var newBadge = new Badge
                        {
                            Filename = filename,
                            AltText = model.BadgeAltText
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        challenge.BadgeId = badge.Id;
                    }
                    else
                    {
                        var existing = await _badgeService
                                    .GetByIdAsync((int)challenge.BadgeId);
                        if (!string.Equals(existing.AltText, model.BadgeAltText,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            existing.AltText = model.BadgeAltText;
                            await _badgeService.ReplaceBadgeFileAsync(existing, null, null);
                        }
                        existing.Filename = Path.GetFileName(model.BadgePath);
                        existing.AltText = model.BadgeAltText;
                        await _badgeService.ReplaceBadgeFileAsync(existing,
                            badgeBytes,
                            filename);
                    }
                }
                else if (model.Challenge.BadgeId.HasValue)
                {
                    var existing = await _badgeService
                        .GetByIdAsync((int)challenge.BadgeId);
                    if (!string.Equals(existing.AltText, model.BadgeAltText,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        existing.AltText = model.BadgeAltText;
                        await _badgeService.ReplaceBadgeFileAsync(existing, null, null);
                    }
                }
                try
                {
                    var serviceResult = await _challengeService.EditChallengeAsync(challenge);
                    if (serviceResult.Status == ServiceResultStatus.Warning
                        && !string.IsNullOrWhiteSpace(serviceResult.Message))
                    {
                        ShowAlertWarning(serviceResult.Message);
                    }
                    AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully modified";

                    var hasPermissions = UserHasPermission(Permission.ActivateAllChallenges)
                            || UserHasPermission(Permission.ActivateSystemChallenges);

                    if (Submit == "Activate"
                        && hasPermissions
                        && challenge.RelatedSystemId == GetId(ClaimType.SystemId)
                        && serviceResult.Data.IsValid)
                    {
                        await _challengeService.ActivateChallengeAsync(serviceResult.Data);
                        AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully modified and activated";
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to edit challenge: ", gex.Message);
                }
                return RedirectToAction("Edit", new { id = model.Challenge.Id });
            }
            else
            {
                var tasks = await _challengeService.GetChallengeTasksAsync(model.Challenge.Id);
                model.Challenge.Tasks = tasks.ToList();
                PageTitle = $"Edit Challenge - {model.Challenge.Name}";
                model = await GetDetailLists(model);
                model.DependentTriggers = await _challengeService
                    .GetDependentsAsync(model.Challenge.Id);
                model.Groups = await _challengeService.GetGroupsByChallengeId(model.Challenge.Id);
                model.RelatedEvents = await _eventService.GetByChallengeIdAsync(model.Challenge.Id);

                if (model.MaxPointLimit.HasValue)
                {
                    model.MaxPointsMessage
                        = $"(Up to {model.MaxPointLimit.Value} points per required task)";

                    var currentChallenge = await _challengeService
                        .MCGetChallengeDetailsAsync(model.Challenge.Id);
                    if (currentChallenge.TasksToComplete.HasValue)
                    {
                        double pointsPerChallenge = (double)currentChallenge.PointsAwarded
                            / currentChallenge.TasksToComplete.Value;
                        if (pointsPerChallenge > model.MaxPointLimit.Value)
                        {
                            model.MaxPointsWarningMessage = $"This Challenge exceeds the maximum of {model.MaxPointLimit.Value} points per required task - only Administrators can edit it.";
                        }
                    }
                }
                return View(model);
            }
        }

        public async Task<IActionResult> Index(string Search,
            int[] CategoryIds,
            int? Program,
            int? System,
            int? Branch,
            bool? Mine,
            string Ordering,
            int page = 1)
        {
            try
            {
                var viewModel = await GetChallengeList(Search,
                    CategoryIds,
                    Program,
                    System,
                    Branch,
                    Mine,
                    Ordering,
                    page);
                viewModel.ShowSystem = true;

                if (viewModel.PaginateModel.MaxPage > 0
                    && viewModel.PaginateModel.CurrentPage > viewModel.PaginateModel.MaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = viewModel.PaginateModel.LastPage ?? 1
                        });
                }
                return View("Index", viewModel);
            }
            catch (GraException ex)
            {
                _logger.LogError(ex,
                    "Invalid challenge filter by User {UserId}: {ErrorMessage}",
                    GetId(ClaimType.UserId),
                    ex.Message);
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.ActivateChallenges)]
        public async Task<IActionResult> Pending(string Search,
            int[] CategoryIds,
            int? System,
            int? Branch,
            int? Program,
            bool? Mine,
            ChallengeFilter.OrderingOption Ordering = ChallengeFilter.OrderingOption.Name,
            int page = 1)
        {
            try
            {
                PageTitle = "Pending Challenges";

                if (!UserHasPermission(Permission.ActivateAllChallenges))
                {
                    System = GetId(ClaimType.SystemId);
                    if (Branch.HasValue && !(await _siteService.ValidateBranch(
                        Branch.Value, System.Value)))
                    {
                        Branch = GetId(ClaimType.BranchId);
                    }
                }
                var viewModel = await GetChallengeList(Search,
                    CategoryIds,
                    Program,
                    System,
                    Branch,
                    Mine,
                    Ordering.ToString(),
                    page,
                    true);

                viewModel.ShowSystem = UserHasPermission(Permission.ActivateAllChallenges);
                if (viewModel.PaginateModel.MaxPage > 0
                    && viewModel.PaginateModel.CurrentPage > viewModel.PaginateModel.MaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = viewModel.PaginateModel.LastPage ?? 1
                        });
                }

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Invalid challenge filter by User {UserId}: {ErrorMessage}",
                    GetId(ClaimType.UserId),
                    ex.Message);
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("Pending");
            }
        }

        private async Task<ChallengesListViewModel> GetChallengeList(string Search,
            int[] CategoryIds,
            int? Program,
            int? System,
            int? Branch,
            bool? Mine,
            string Ordering,
            int page = 1,
            bool pending = false)
        {
            var filter = new ChallengeFilter(page)
            {
                CategoryIds = CategoryIds
            };

            if (!string.IsNullOrEmpty(Ordering)
                && Enum.TryParse(typeof(ChallengeFilter.OrderingOption),
                    Ordering,
                    out var typedOrdering))
            {
                filter.Ordering = (ChallengeFilter.OrderingOption)typedOrdering;
            }
            else
            {
                filter.Ordering = ChallengeFilter.OrderingOption.Name;
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                filter.Search = Search;
            }

            if (System.HasValue)
            {
                filter.SystemIds = new List<int>() { System.Value };
            }
            if (Branch.HasValue)
            {
                filter.BranchIds = new List<int>() { Branch.Value };
            }
            if (Program.HasValue)
            {
                if (Program.Value == 0)
                {
                    filter.ProgramIds = new List<int?>() { null };
                }
                else
                {
                    filter.ProgramIds = new List<int?>() { Program.Value };
                }
            }
            if (Mine == true)
            {
                filter.UserIds = new List<int>() { GetId(ClaimType.UserId) };
            }
            if (pending)
            {
                filter.IsActive = false;
            }
            var challengeList = await _challengeService
                .MCGetPaginatedChallengeListAsync(filter);

            foreach (var challenge in challengeList.Data)
            {
                if (!string.IsNullOrEmpty(challenge.BadgeFilename))
                {
                    challenge.BadgeFilename
                        = _pathResolver.ResolveContentPath(challenge.BadgeFilename);
                }
            }

            var paginateModel = new PaginateViewModel
            {
                ItemCount = challengeList.Count,
                CurrentPage = (filter.Skip.Value / filter.Take.Value) + 1,
                ItemsPerPage = filter.Take.Value
            };

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            var categoryList = await _categoryService.GetListAsync();
            var viewModel = new ChallengesListViewModel
            {
                Branch = Branch,
                CanAddChallenges = UserHasPermission(Permission.AddChallenges),
                CanDeleteChallenges = UserHasPermission(Permission.RemoveChallenges),
                CanEditChallenges = UserHasPermission(Permission.EditChallenges),
                CategoryIds = filter.CategoryIds,
                CategoryList = new SelectList(categoryList, "Id", "Name"),
                Challenges = challengeList.Data,
                Mine = Mine,
                Ordering = filter.Ordering,
                PaginateModel = paginateModel,
                Program = Program,
                ProgramList = await _siteService.GetProgramList(),
                Search = filter.Search,
                System = System,
                SystemList = systemList
            };
            if (Mine == true)
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Mine";
                if (pending && !UserHasPermission(Permission.ActivateAllChallenges))
                {
                    viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == GetId(ClaimType.SystemId))?.Name;
                }
            }
            else if (Branch.HasValue)
            {
                var branch = await _siteService.GetBranchByIdAsync(viewModel.Branch.Value);
                viewModel.BranchName = branch.Name;
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == branch.SystemId)?.Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (System.HasValue)
            {
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == viewModel.System.Value)?.Name;
                viewModel.BranchList = (await _siteService.GetBranches(viewModel.System.Value))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "System";
            }
            else
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "All";
            }

            if (Program.HasValue)
            {
                if (Program.Value > 0)
                {
                    var program = await _siteService.GetProgramByIdAsync(Program.Value);
                    viewModel.ProgramName = $"Limited to {program.Name}";
                }
                else
                {
                    viewModel.ProgramName = "Not Limited to a Program";
                }
            }
            else
            {
                viewModel.ProgramName = "All Programs";
            }

            return viewModel;
        }

        private async Task<ChallengesDetailViewModel>
            GetDetailLists(ChallengesDetailViewModel model)
        {
            var systemList = (await _siteService.GetSystemList())
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId))
                    .ThenBy(_ => _.Name);
            model.SystemList = new SelectList(systemList, "Id", "Name");

            var programList = (await _siteService.GetProgramList());
            model.ProgramList = new SelectList(programList, "Id", "Name");

            if (model.Challenge?.LimitToSystemId.HasValue == true)
            {
                var branchList = (await _siteService
                    .GetBranches(model.Challenge.LimitToSystemId.Value))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                model.BranchList = new SelectList(branchList, "Id", "Name");
            }
            else
            {
                var branchList = (await _siteService.GetAllBranches())
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                model.BranchList = new SelectList(branchList, "Id", "Name");
            }

            var categoryList = await _categoryService.GetListAsync();
            model.CategoryList = new SelectList(categoryList, "Id", "Name");

            return model;
        }

        #region Task methods

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> AddTask(ChallengesDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (!string.IsNullOrWhiteSpace(viewModel.Task.Url))
            {
                try
                {
                    viewModel.Task.Url = new UriBuilder(
                        viewModel.Task.Url).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    ShowAlertDanger("Invalid URL");
                    return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
                }
            }

            foreach (string key in ModelState
                .Keys
                .Where(m => m.StartsWith("Challenge.", StringComparison.OrdinalIgnoreCase))
                .ToList())
            {
                ModelState.Remove(key);
            }

            if (viewModel.TaskUploadFile != null && !ValidFiles.UploadExtensions.Contains(Path
                    .GetExtension(viewModel.TaskUploadFile.FileName),
                        StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("BadgeUploadImage",
                    $"File upload must be one of the following types: {string.Join(", ", ValidFiles.UploadExtensions)}");
            }

            if (ModelState.IsValid)
            {
                byte[] fileBytes = null;
                if (viewModel.Task.ChallengeTaskType == ChallengeTaskType.Action)
                {
                    viewModel.Task.Author = null;
                    viewModel.Task.Isbn = null;
                    viewModel.Task.Url = null;
                }
                if (viewModel.TaskUploadFile != null)
                {
                    viewModel.Task.Filename = viewModel.TaskUploadFile.FileName;
                    await using var fileStream = viewModel.TaskUploadFile.OpenReadStream();
                    await using var ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                viewModel.Task.ChallengeId = viewModel.Challenge.Id;
                await _challengeService.AddTaskAsync(viewModel.Task, fileBytes);
            }
            else
            {
                TempData[NewTask] = true;
            }
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public IActionResult CloseTask(ChallengesDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> DecreaseTaskSort(int id)
        {
            try
            {
                await _challengeService.DecreaseTaskPositionAsync(id);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error decreasing task sort for task {TaskId}: {ErrorMessage}",
                    id,
                    ex.Message);
                return Json(false);
            }
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> DeleteTask(ChallengesDetailViewModel viewModel, int id)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            try
            {
                await _challengeService.RemoveTaskAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete task: ", gex);
            }

            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> IncreaseTaskSort(int id)
        {
            try
            {
                await _challengeService.IncreaseTaskPositionAsync(id);
                return Json(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error decreasing task sort for task {TaskId}: {ErrorMessage}",
                    id,
                    ex.Message);
                return Json(false);
            }
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> ModifyTask(ChallengesDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (!string.IsNullOrWhiteSpace(viewModel.Task.Url))
            {
                try
                {
                    viewModel.Task.Url = new UriBuilder(
                        viewModel.Task.Url).Uri.AbsoluteUri;
                }
                catch (Exception)
                {
                    ShowAlertDanger("Invalid URL");
                    return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
                }
            }

            foreach (string key in ModelState
                .Keys
                .Where(m => m.StartsWith("Challenge.", StringComparison.OrdinalIgnoreCase))
                .ToList())
            {
                ModelState.Remove(key);
            }

            if (viewModel.TaskUploadFile != null
                && !ValidFiles.UploadExtensions.Contains(Path
                    .GetExtension(viewModel.TaskUploadFile.FileName),
                        StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("BadgeUploadImage",
                    $"File upload must be one of the following types: {string.Join(", ", ValidFiles.UploadExtensions)}");
            }

            if (ModelState.IsValid)
            {
                byte[] fileBytes = null;
                if (viewModel.Task.ChallengeTaskType == ChallengeTaskType.Action)
                {
                    viewModel.Task.Author = null;
                    viewModel.Task.Isbn = null;
                }
                if (viewModel.TaskUploadFile != null)
                {
                    viewModel.Task.Filename = viewModel.TaskUploadFile.FileName;
                    await using var fileStream = viewModel.TaskUploadFile.OpenReadStream();
                    await using var ms = new MemoryStream();
                    fileStream.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
                await _challengeService.EditTaskAsync(viewModel.Task, fileBytes);
            }
            else
            {
                TempData[EditTask] = viewModel.Task.Id;
            }
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public IActionResult OpenAddTask(ChallengesDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            TempData[NewTask] = true;
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public IActionResult OpenModifyTask(ChallengesDetailViewModel viewModel, int taskId)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            TempData[EditTask] = taskId;
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        #endregion Task methods

        #region Challenge Group methods

        [Authorize(Policy = Policy.AddChallengeGroups)]
        public IActionResult CreateGroup()
        {
            PageTitle = "Create Challenge Group";
            var viewModel = new ChallengeGroupDetailViewModel()
            {
                Action = nameof(CreateGroup)
            };

            return View("GroupDetail", viewModel);
        }

        [Authorize(Policy = Policy.AddChallengeGroups)]
        [HttpPost]
        public async Task<IActionResult> CreateGroup(ChallengeGroupDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var challengeIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.ChallengeIds))
            {
                challengeIds = model?.ChallengeIds
                .Split(',')
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(int.Parse)
                .ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceResult = await _challengeService.AddGroupAsync(model.ChallengeGroup,
                        challengeIds);
                    if (serviceResult.Status == ServiceResultStatus.Warning
                        && !string.IsNullOrWhiteSpace(serviceResult.Message))
                    {
                        ShowAlertWarning(serviceResult.Message);
                    }
                    ShowAlertSuccess($"Added Challenge Group \"{model.ChallengeGroup.Name}\"!");
                    return RedirectToAction(nameof(EditGroup), new { id = serviceResult.Data.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add Challenge Group: ", gex);
                }
            }

            model.ChallengeGroup.Challenges = await _challengeService.GetByIdsAsync(challengeIds);
            foreach (var challenge in model.ChallengeGroup.Challenges)
            {
                if (!string.IsNullOrWhiteSpace(challenge.BadgeFilename))
                {
                    challenge.BadgeFilename = _pathResolver.ResolveContentPath(
                        challenge.BadgeFilename);
                }
            }

            PageTitle = "Create Challenge Group";
            return View("GroupDetail", model);
        }

        [Authorize(Policy = Policy.EditChallengeGroups)]
        public async Task<IActionResult> DeleteGroup(ChallengeGroupListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                await _challengeService.RemoveGroupAsync(model.ChallengeGroup.Id);
                ShowAlertSuccess($"Category \"{model.ChallengeGroup.Name}\" removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Challenge Group: ", gex);
            }

            return RedirectToAction("Groups", new
            {
                search = model.Search,
                page = model.PaginateModel.CurrentPage
            });
        }

        [Authorize(Policy = Policy.EditChallengeGroups)]
        public async Task<IActionResult> EditGroup(int id)
        {
            PageTitle = "Edit Challenge Group";
            var challengeGroup = await _challengeService.GetGroupByIdAsync(id);
            var baseUrl = await _siteLookupService.GetSiteLinkAsync(GetCurrentSiteId());
            var urlPath = Url.Action(nameof(Controllers.ChallengesController.List),
                Controllers.ChallengesController.Name,
                new
                {
                    area = "",
                    id = challengeGroup.Stub
                });

            var viewModel = new ChallengeGroupDetailViewModel()
            {
                ChallengeGroup = challengeGroup,
                ChallengeIds = string.Join(",", challengeGroup.Challenges.Select(_ => _.Id)),
                Action = nameof(EditGroup),
                RelatedEvents = await _eventService.GetByChallengeGroupIdAsync(challengeGroup.Id),
                CanManageEvents = UserHasPermission(Permission.ManageEvents),
                GroupUrl = new Uri(baseUrl, urlPath).ToString()
            };

            foreach (var challenge in viewModel.ChallengeGroup.Challenges)
            {
                if (!string.IsNullOrWhiteSpace(challenge.BadgeFilename))
                {
                    challenge.BadgeFilename = _pathResolver.ResolveContentPath(
                        challenge.BadgeFilename);
                }
            }

            return View("GroupDetail", viewModel);
        }

        [Authorize(Policy = Policy.EditChallengeGroups)]
        [HttpPost]
        public async Task<IActionResult> EditGroup(ChallengeGroupDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var challengeIds = new List<int>();
            if (!string.IsNullOrWhiteSpace(model.ChallengeIds))
            {
                challengeIds = model?.ChallengeIds
                .Split(',')
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .Select(int.Parse)
                .ToList();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceResult = await _challengeService.EditGroupAsync(model.ChallengeGroup,
                        challengeIds);
                    if (serviceResult.Status == ServiceResultStatus.Warning
                        && !string.IsNullOrWhiteSpace(serviceResult.Message))
                    {
                        ShowAlertWarning(serviceResult.Message);
                    }
                    ShowAlertSuccess($"Saved Challenge Group \"{model.ChallengeGroup.Name}\"!");
                    return RedirectToAction(nameof(EditGroup), new { id = serviceResult.Data.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit Challenge Group: ", gex);
                }
            }

            model.ChallengeGroup.Challenges = await _challengeService.GetByIdsAsync(challengeIds);
            model.RelatedEvents = await _eventService.GetByChallengeGroupIdAsync(
                model.ChallengeGroup.Id);
            model.CanManageEvents = UserHasPermission(Permission.ManageEvents);
            foreach (var challenge in model.ChallengeGroup.Challenges)
            {
                if (!string.IsNullOrWhiteSpace(challenge.BadgeFilename))
                {
                    challenge.BadgeFilename = _pathResolver.ResolveContentPath(
                        challenge.BadgeFilename);
                }
            }

            PageTitle = "Edit Challenge Group";
            return View("GroupDetail", model);
        }

        public async Task<IActionResult> Groups(string search, int page = 1)
        {
            PageTitle = "Challenge Groups";
            var filter = new ChallengeGroupFilter(page)
            {
                Search = search
            };

            var groupList = await _challengeService.GetPaginatedGroupListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = groupList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new ChallengeGroupListViewModel()
            {
                ChallengeGroups = groupList.Data,
                PaginateModel = paginateModel,
                Search = search,
                CanAddGroups = UserHasPermission(Permission.AddChallengeGroups),
                CanEditGroups = UserHasPermission(Permission.EditChallengeGroups)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> StubInUse(string stub)
        {
            return Json(await _challengeService.StubInUseAsync(stub));
        }

        #endregion Challenge Group methods

        #region Featured Challenge Group methods

        [HttpPost]
        [Authorize(Policy = Policy.ManageFeaturedChallengeGroups)]
        public async Task<IActionResult> FeaturedChangeSort(int id, bool increase, int? page)
        {
            if (!UserHasPermission(Permission.ManageFeaturedChallengeGroups))
            {
                ShowAlertDanger("Permission denied.");
            }
            else
            {
                try
                {
                    await _challengeService.UpdateFeaturedGroupSortAsync(id, increase);
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(gex.Message);
                }
            }
            return RedirectToAction(nameof(FeaturedGroups), page);
        }

        [Authorize(Policy = Policy.ManageFeaturedChallengeGroups)]
        public async Task<IActionResult> FeaturedCreate()
        {
            PageTitle = "Create Featured Group";

            var viewModel = new FeaturedGroupDetailsViewModel
            {
                ChallengeGroupList = new SelectList(await _challengeService.GetGroupListAsync(),
                    "Id",
                    "Name"),
                NewFeaturedGroup = true
            };

            return View(nameof(FeaturedDetails), viewModel);
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageFeaturedChallengeGroups)]
        public async Task<IActionResult> FeaturedDelete(FeaturedGroupListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            try
            {
                await _challengeService.RemoveFeaturedGroupAsync(model.FeaturedGroup.Id);
                ShowAlertSuccess($"Removed featured challenge group '<strong>{model.FeaturedGroup.Name}</strong>'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove featured challenge group: ", gex);
            }

            return RedirectToAction(nameof(FeaturedGroups), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [Authorize(Policy = Policy.ManageFeaturedChallengeGroups)]
        public async Task<IActionResult> FeaturedDetails(int id)
        {
            PageTitle = "Edit Featured Group";

            var viewModel = new FeaturedGroupDetailsViewModel
            {
                ChallengeGroupList = new SelectList(await _challengeService.GetGroupListAsync(),
                    "Id",
                    "Name"),
                FeaturedGroup = await _challengeService.GetFeaturedGroupByIdAsync(id)
            };

            viewModel.FeaturedGroupText = viewModel.FeaturedGroup.FeaturedGroupText;

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageFeaturedChallengeGroups)]
        public async Task<IActionResult> FeaturedDetails(FeaturedGroupDetailsViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            byte[] imageBytes = null;

            if (model.FeaturedGroup.StartDate >= model.FeaturedGroup.EndDate)
            {
                ModelState.AddModelError("FeaturedGroup.EndDate",
                    "The End date cannot be before the Start date.");
            }

            if (model.NewFeaturedGroup)
            {
                if (model.UploadedImage == null)
                {
                    ModelState.AddModelError(nameof(model.UploadedImage),
                        "Please attach an image to submit.");
                }
                else
                {
                    try
                    {
                        await using var memoryStream = new MemoryStream();
                        await model.UploadedImage.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                        await _badgeService.ValidateBadgeImageAsync(imageBytes);
                    }
                    catch (GraException gex)
                    {
                        ModelState.AddModelError(nameof(model.UploadedImage), gex.Message);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                FeaturedChallengeGroup featuredGroup;
                if (model.NewFeaturedGroup)
                {
                    featuredGroup = await _challengeService.AddFeaturedGroupAsync(
                        model.FeaturedGroup,
                        model.FeaturedGroupText,
                        model.UploadedImage.FileName,
                        imageBytes);

                    ShowAlertSuccess($"Added featured challenge group '<strong>{featuredGroup.Name}</strong>'");
                }
                else
                {
                    featuredGroup = await _challengeService.EditFeaturedGroupAsync(
                        model.FeaturedGroup,
                        model.FeaturedGroupText);

                    ShowAlertSuccess($"Updated featured challenge group '<strong>{model.FeaturedGroup.Name}</strong>'");
                }

                return RedirectToAction(nameof(FeaturedDetails), new { id = featuredGroup.Id });
            }

            model.ChallengeGroupList = new SelectList(await _challengeService.GetGroupListAsync(),
                    "Id",
                    "Name");

            if (model.NewFeaturedGroup)
            {
                PageTitle = "Create Featured Group";
            }
            else
            {
                PageTitle = "Edit Featured Group";
            }

            return View(model);
        }

        public async Task<IActionResult> FeaturedGroups(int page)
        {
            PageTitle = "Featured Groups";

            if (page == default)
            {
                page = 1;
            }

            var filter = new BaseFilter(page);

            var featuredGroupList = await _challengeService
                .GetPaginatedFeaturedGroupListAsync(filter);

            var viewModel = new FeaturedGroupListViewModel
            {
                CanManageFeaturedGroups
                    = UserHasPermission(Permission.ManageFeaturedChallengeGroups),
                CurrentPage = page,
                FeaturedGroups = featuredGroupList.Data,
                ItemCount = featuredGroupList.Count,
                ItemsPerPage = filter.Take.Value,
                Now = _dateTimeProvider.Now,
            };

            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = viewModel.LastPage ?? 1
                    });
            }

            return View(viewModel);
        }

        public async Task<IActionResult> ReplaceFeaturedImage(FeaturedGroupDetailsViewModel model)
        {
            if (model?.UploadedImage == null)
            {
                ShowAlertDanger("No replacement image was submitted.");
                return RedirectToAction(nameof(FeaturedDetails),
                    new { id = model.FeaturedGroupId });
            }

            if (!ValidImageExtensions.Contains(Path
                .GetExtension(model.UploadedImage.FileName),
                    StringComparer.OrdinalIgnoreCase))
            {
                ShowAlertDanger($"Image must be one of the following types: {string.Join(", ", ValidImageExtensions)}");
                return RedirectToAction(nameof(FeaturedDetails),
                    new { id = model.FeaturedGroupId });
            }

            byte[] imageBytes = null;
            try
            {
                await using var memoryStream = new MemoryStream();
                await model.UploadedImage.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
                await _badgeService.ValidateBadgeImageAsync(imageBytes);

                await _challengeService.ReplaceFeaturedImageAsync(model.FeaturedGroupId,
                    model.UploadedImage.FileName,
                    imageBytes);
            }
            catch (GraException gex)
            {
                ShowAlertDanger(gex.Message);
            }

            return RedirectToAction(nameof(FeaturedDetails), new { id = model.FeaturedGroupId });
        }

        #endregion Featured Challenge Group methods
    }
}
