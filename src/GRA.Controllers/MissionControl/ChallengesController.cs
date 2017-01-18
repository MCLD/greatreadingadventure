using GRA.Controllers.ViewModel.MissionControl.Challenges;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
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
        public ChallengesController(ILogger<ChallengesController> logger,
            ServiceFacade.Controller context,
            BadgeService badgeService,
            ChallengeService challengeService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _badgeService = Require.IsNotNull(badgeService, nameof(badgeService));
            _challengeService = Require.IsNotNull(challengeService, nameof(challengeService));
            PageTitle = "Challenges";
        }

        public async Task<IActionResult> Index(string Search, string FilterBy, int page = 1)
        {
            var viewModel = await GetChallengeList(null, Search, page);

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

        [Authorize(Policy = Policy.AddChallenges)]
        public async Task<IActionResult> MyChallenges(string Search, int page = 1)
        {
            PageTitle = "My Challenges";

            var viewModel = await GetChallengeList("User", Search, page);

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

        [Authorize(Policy = Policy.ActivateChallenges)]
        public async Task<IActionResult> Pending(string Search, int page = 1)
        {
            PageTitle = "Pending Challenges";

            var viewModel = await GetChallengeList("Pending", Search, page);

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

        private async Task<ChallengesListViewModel> GetChallengeList(string filter,
            string search, int page)
        {
            int take = 15;
            int skip = take * (page - 1);

            var challengeList = await _challengeService
                .MCGetPaginatedChallengeListAsync(skip, take, search, filter);

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
                CurrentPage = page,
                ItemsPerPage = take
            };

            ChallengesListViewModel viewModel = new ChallengesListViewModel()
            {
                Challenges = challengeList.Data,
                PaginateModel = paginateModel,
                CanAddChallenges = UserHasPermission(Permission.AddChallenges),
                CanDeleteChallenges = UserHasPermission(Permission.RemoveChallenges),
                CanEditChallenges = UserHasPermission(Permission.EditChallenges)
            };

            return viewModel;
        }

        [Authorize(Policy = Policy.AddChallenges)]
        public IActionResult Create()
        {
            PageTitle = "Create Challenge";
            return View("Create");
        }

        [Authorize(Policy = Policy.AddChallenges)]
        [HttpPost]
        public async Task<IActionResult> Create(ChallengesDetailViewModel model)
        {
            if (model.BadgeImage != null)
            {
                if (Path.GetExtension(model.BadgeImage.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(model.BadgeImage.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(model.BadgeImage.FileName).ToLower() != ".png")
                {
                    ModelState.AddModelError("BadgeImage", "Please use a .jpg or .png image");
                }
            }
            if (ModelState.IsValid)
            {
                var challenge = model.Challenge;
                if (model.BadgeImage != null)
                {
                    using (var fileStream = model.BadgeImage.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            var badgeBytes = ms.ToArray();
                            var newBadge = new Badge
                            {
                                Filename = Path.GetFileName(model.BadgeImage.FileName),
                            };
                            var badge = await _badgeService.AddBadgeAsync(newBadge, badgeBytes);
                            challenge.BadgeId = badge.Id;
                        }
                    }
                }
                challenge = await _challengeService.AddChallengeAsync(challenge);
                AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully created";
                return RedirectToAction("Edit", new { id = challenge.Id });
            }
            else
            {
                PageTitle = "Create Challenge";
                return View(model);
            }
        }

        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> Edit(int id)
        {
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
                }
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view challenge: ", gex);
                return RedirectToAction("Index");
            }

            if (challenge.TasksToComplete >  challenge.Tasks.Count())
            {
                AlertInfo = "The challenge does not have enough tasks to be completable";
            }

            bool canActivate = challenge.IsValid
                && !challenge.IsActive
                && UserHasPermission(Permission.ActivateChallenges);


            ChallengesDetailViewModel viewModel = new ChallengesDetailViewModel()
            {
                Challenge = challenge,
                TaskTypes = Enum.GetNames(typeof(ChallengeTaskType))
                    .Select(m => new SelectListItem { Text = m, Value = m }).ToList(),
                CanActivate = canActivate
            };

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
            return View("Edit", viewModel);
        }

        [Authorize(Policy = Policy.EditChallenges)]
        [HttpPost]
        public async Task<IActionResult> Edit(ChallengesDetailViewModel model, string Submit)
        {
            if (model.BadgeImage != null)
            {
                if (Path.GetExtension(model.BadgeImage.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(model.BadgeImage.FileName).ToLower() != ".jpeg"
                    && Path.GetExtension(model.BadgeImage.FileName).ToLower() != ".png")
                {
                    ModelState.AddModelError("BadgeImage", "Please use a .jpg or .png image");
                }
            }
            if (ModelState.IsValid)
            {
                var challenge = model.Challenge;
                if (model.BadgeImage != null)
                {
                    using (var fileStream = model.BadgeImage.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            var badgeBytes = ms.ToArray();
                            if (challenge.BadgeId == null)
                            {
                                var newBadge = new Badge
                                {
                                    Filename = Path.GetFileName(model.BadgeImage.FileName),
                                };
                                var badge = await _badgeService
                                    .AddBadgeAsync(newBadge, badgeBytes);
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
                    }
                }
                try
                {
                    var savedChallenge = await _challengeService.EditChallengeAsync(challenge);
                    AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully modified";
                    if (Submit == "Activate" && UserHasPermission(Permission.ActivateChallenges))
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
                    AlertWarning = gex.Message;
                }
                return RedirectToAction("Edit", new { id = model.Challenge.Id });
            }
            else
            {
                var tasks = await _challengeService.GetChallengeTasksAsync(model.Challenge.Id);
                model.Challenge.Tasks = tasks.ToList();
                PageTitle = $"Edit Challenge - {model.Challenge.Name}";
                return View(model);
            }
        }

        [Authorize(Policy = Policy.RemoveChallenges)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _challengeService.RemoveChallengeAsync(id);
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