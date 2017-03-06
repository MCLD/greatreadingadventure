using GRA.Controllers.ViewModel.MissionControl.Challenges;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewAllChallenges)]
    public class ChallengesController : Base.MCController
    {
        private const string NewTask = "NewTask";
        private const string EditTask = "EditTask";
        private const string TempEditChallenge = "TempEditChallenge";

        private readonly ILogger<ChallengesController> _logger;
        private readonly BadgeService _badgeService;
        private readonly ChallengeService _challengeService;
        private readonly SiteService _siteService;
        public ChallengesController(ILogger<ChallengesController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            ChallengeService challengeService,
            SiteService siteService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _badgeService = Require.IsNotNull(badgeService, nameof(badgeService));
            _challengeService = Require.IsNotNull(challengeService, nameof(challengeService));
            _siteService = Require.IsNotNull(siteService, nameof(SiteService));
            PageTitle = "Challenges";
        }

        public async Task<IActionResult> Index(string Search, int? Program, int? System, int? Branch,
            bool? Mine, int page = 1)
        {
            try
            {
                var viewModel = await GetChallengeList(Search, Program, System, Branch, Mine, page);
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
            catch (Exception ex)
            {
                _logger.LogError($"Invalid challenge filter by User {GetId(ClaimType.UserId)}: {ex}");
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.ActivateChallenges)]
        public async Task<IActionResult> Pending(string Search, int? System, int? Branch,
            int? Program, bool? Mine, int page = 1)
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
                var viewModel = await GetChallengeList(Search, Program, System, Branch, Mine, page, true);

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
                _logger.LogError($"Invalid challenge filter by User {GetId(ClaimType.UserId)}: {ex}");
                ShowAlertDanger("Invalid filter parameters.");
                return RedirectToAction("Pending");
            }
        }

        private async Task<ChallengesListViewModel> GetChallengeList(string Search, int? Program,
            int? System, int? Branch, bool? Mine, int page = 1, bool pending = false)
        {
            BaseFilter filter = new BaseFilter(page);
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
                    challenge.BadgeFilename = _pathResolver.ResolveContentPath(challenge.BadgeFilename);
                }
            }

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = challengeList.Count,
                CurrentPage = (filter.Skip.Value / filter.Take.Value) + 1,
                ItemsPerPage = filter.Take.Value
            };

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);
            ChallengesListViewModel viewModel = new ChallengesListViewModel()
            {
                Challenges = challengeList.Data,
                PaginateModel = paginateModel,
                Search = filter.Search,
                System = System,
                Branch = Branch,
                Program = Program,
                Mine = Mine,
                CanAddChallenges = UserHasPermission(Permission.AddChallenges),
                CanDeleteChallenges = UserHasPermission(Permission.RemoveChallenges),
                CanEditChallenges = UserHasPermission(Permission.EditChallenges),
                SystemList = systemList,
                ProgramList = await _siteService.GetProgramList()
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
                    .Where(_ => _.Id == GetId(ClaimType.SystemId)).SingleOrDefault().Name;
                }
            }
            else if (Branch.HasValue)
            {
                var branch = await _siteService.GetBranchByIdAsync(viewModel.Branch.Value);
                viewModel.BranchName = branch.Name;
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == branch.SystemId).SingleOrDefault().Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (System.HasValue)
            {
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == viewModel.System.Value).SingleOrDefault().Name;
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
                    viewModel.ProgramName =
                        (await _siteService.GetProgramByIdAsync(Program.Value)).Name;
                }
                else
                {
                    viewModel.ProgramName = "Not Limited";
                }
            }

            return viewModel;
        }

        [Authorize(Policy = Policy.AddChallenges)]
        public async Task<IActionResult> Create()
        {
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            PageTitle = "Create Challenge";


            ChallengesDetailViewModel viewModel = new ChallengesDetailViewModel()
            {
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true
            };

            viewModel = await GetDetailLists(viewModel);
            if (site.MaxPointsPerChallengeTask.HasValue)
            {
                viewModel.MaxPointsMessage = $"(Up to {site.MaxPointsPerChallengeTask.Value} points per required task)";
            }

            return View(viewModel);
        }

        [Authorize(Policy = Policy.AddChallenges)]
        [HttpPost]
        public async Task<IActionResult> Create(ChallengesDetailViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (site.MaxPointsPerChallengeTask.HasValue && model.Challenge.TasksToComplete.HasValue
                && model.Challenge.TasksToComplete != 0)
            {
                double pointsPerChallenge = (double)model.Challenge.PointsAwarded / model.Challenge.TasksToComplete.Value;
                if (pointsPerChallenge > site.MaxPointsPerChallengeTask)
                {
                    ModelState.AddModelError("Challenge.PointsAwarded", $"Too many points awarded.");
                }
            }
            if (model.BadgeUploadImage != null
                && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker))
            {
                if (Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".png")
                {
                    ModelState.AddModelError("BadgeUploadImage", "Please use a .jpg or .png image");
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
                        byte[] badgeBytes;
                        string filename;
                        if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                            (model.BadgeUploadImage != null || model.UseBadgeMaker))
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
                        Badge newBadge = new Badge()
                        {
                            Filename = filename
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        challenge.BadgeId = badge.Id;
                    }
                    challenge = await _challengeService.AddChallengeAsync(challenge);
                    AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully created";
                    return RedirectToAction("Edit", new { id = challenge.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to add challenge: ", gex.Message);
                }
            }
            PageTitle = "Create Challenge";
            model = await GetDetailLists(model);

            return View(model);
        }

        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> Edit(int id)
        {
            var site = await GetCurrentSiteAsync();
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            Challenge challenge = new Challenge();
            try
            {
                challenge = await _challengeService.MCGetChallengeDetailsAsync(id);
                if (TempData.ContainsKey(TempEditChallenge))
                {
                    var storedChallenge = Newtonsoft.Json.JsonConvert
                        .DeserializeObject<Challenge>((string)TempData[TempEditChallenge]);

                    challenge.Name = storedChallenge.Name;
                    challenge.Description = storedChallenge.Description;
                    challenge.PointsAwarded = storedChallenge.PointsAwarded;
                    challenge.TasksToComplete = storedChallenge.TasksToComplete;
                    challenge.LimitToSystemId = storedChallenge.LimitToSystemId;
                    challenge.LimitToBranchId = storedChallenge.LimitToBranchId;
                    challenge.LimitToProgramId = storedChallenge.LimitToProgramId;
                }
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view challenge: ", gex);
                return RedirectToAction("Index");
            }

            if (challenge.TasksToComplete > challenge.Tasks.Count())
            {
                AlertInfo = "The challenge does not have enough tasks to be completable";
            }

            bool canActivate = challenge.IsValid
                && !challenge.IsActive
                && (UserHasPermission(Permission.ActivateAllChallenges)
                    || (UserHasPermission(Permission.ActivateSystemChallenges)
                        && challenge.RelatedSystemId == GetId(ClaimType.SystemId)));

            ChallengesDetailViewModel viewModel = new ChallengesDetailViewModel()
            {
                Challenge = challenge,
                TaskTypes = Enum.GetNames(typeof(ChallengeTaskType))
                    .Select(m => new SelectListItem { Text = m, Value = m }).ToList(),
                CanActivate = canActivate,
                CanViewTriggers = UserHasPermission(Permission.ManageTriggers),
                DependentTriggers = await _challengeService.GetDependentsAsync(challenge.Id),
                BadgeMakerUrl = GetBadgeMakerUrl(siteUrl, site.FromEmailAddress),
                UseBadgeMaker = true
            };

            if (site.MaxPointsPerChallengeTask.HasValue)
            {
                viewModel.MaxPointsMessage = $"(Up to {site.MaxPointsPerChallengeTask.Value} points per required task)";
            }

            if (challenge.BadgeId != null)
            {
                var challengeBadge = await _badgeService.GetByIdAsync((int)challenge.BadgeId);
                if (challengeBadge != null)
                {
                    viewModel.BadgePath = _pathResolver.ResolveContentPath(challengeBadge.Filename);
                }
            }

            if (TempData.ContainsKey(NewTask))
            {
                TempData.Remove(NewTask);
                TempData.Remove(EditTask);
                viewModel.AddTask = true;
            }
            else if (TempData.ContainsKey(EditTask))
            {
                viewModel.Task = await _challengeService
                    .GetTaskAsync((int)TempData[EditTask]);
            }
            PageTitle = $"Edit Challenge - {viewModel.Challenge.Name}";

            viewModel = await GetDetailLists(viewModel);

            return View("Edit", viewModel);
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> Edit(ChallengesDetailViewModel model, string Submit)
        {
            var site = await GetCurrentSiteAsync();
            if (site.MaxPointsPerChallengeTask.HasValue && model.Challenge.TasksToComplete.HasValue
                && model.Challenge.TasksToComplete != 0)
            {
                double pointsPerChallenge = (double)model.Challenge.PointsAwarded / model.Challenge.TasksToComplete.Value;
                if (pointsPerChallenge > site.MaxPointsPerChallengeTask)
                {
                    ModelState.AddModelError("Challenge.PointsAwarded", $"Too many points awarded.");
                }
            }
            if (model.BadgeUploadImage != null
                && (string.IsNullOrWhiteSpace(model.BadgeMakerImage) || !model.UseBadgeMaker))
            {
                if (Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(model.BadgeUploadImage.FileName).ToLower() != ".png")
                {
                    ModelState.AddModelError("BadgeUploadImage", "Please use a .jpg or .png image");
                }
            }
            if (ModelState.IsValid)
            {
                var challenge = model.Challenge;
                if (model.BadgeUploadImage != null
                        || !string.IsNullOrWhiteSpace(model.BadgeMakerImage))
                {
                    byte[] badgeBytes;
                    string filename;
                    if (!string.IsNullOrWhiteSpace(model.BadgeMakerImage) &&
                        (model.BadgeUploadImage != null || model.UseBadgeMaker))
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
                    if (challenge.BadgeId == null)
                    {
                        Badge newBadge = new Badge()
                        {
                            Filename = filename
                        };
                        var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                        challenge.BadgeId = badge.Id;
                    }
                    else
                    {
                        var existing = await _badgeService
                                    .GetByIdAsync((int)challenge.BadgeId);
                        existing.Filename = Path.GetFileName(model.BadgePath);
                        await _badgeService.ReplaceBadgeFileAsync(existing, badgeBytes);
                    }
                }
                try
                {
                    var savedChallenge = await _challengeService.EditChallengeAsync(challenge);
                    AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully modified";
                    if (Submit == "Activate"
                        && (UserHasPermission(Permission.ActivateAllChallenges)
                            || (UserHasPermission(Permission.ActivateSystemChallenges)
                                && challenge.RelatedSystemId == GetId(ClaimType.SystemId))))
                    {
                        if (savedChallenge.IsValid)
                        {
                            await _challengeService.ActivateChallengeAsync(savedChallenge);
                            AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully modified and activated";
                        }
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

                return View(model);
            }
        }

        private async Task<ChallengesDetailViewModel> GetDetailLists(ChallengesDetailViewModel model)
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

            return model;
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

        #region Task methods
        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public IActionResult CloseTask(ChallengesDetailViewModel viewModel)
        {
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public IActionResult OpenAddTask(ChallengesDetailViewModel viewModel)
        {
            TempData[NewTask] = true;
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> AddTask(ChallengesDetailViewModel viewModel)
        {
            foreach (string key in ModelState.Keys.Where(m => m.StartsWith("Challenge.")).ToList())
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                viewModel.Task.ChallengeId = viewModel.Challenge.Id;
                await _challengeService.AddTaskAsync(viewModel.Task);
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
        public IActionResult OpenModifyTask(ChallengesDetailViewModel viewModel, int taskId)
        {
            TempData[EditTask] = taskId;
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> ModifyTask(ChallengesDetailViewModel viewModel)
        {
            foreach (string key in ModelState.Keys.Where(m => m.StartsWith("Challenge.")).ToList())
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                await _challengeService.EditTaskAsync(viewModel.Task);
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
        public async Task<IActionResult> DeleteTask(ChallengesDetailViewModel viewModel, int id)
        {
            await _challengeService.RemoveTaskAsync(id);
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult>
            DecreaseTaskSort(ChallengesDetailViewModel viewModel, int id)
        {
            await _challengeService.DecreaseTaskPositionAsync(id);
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult>
            IncreaseTaskSort(ChallengesDetailViewModel viewModel, int id)
        {
            await _challengeService.IncreaseTaskPositionAsync(id);
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }
        #endregion
    }
}
