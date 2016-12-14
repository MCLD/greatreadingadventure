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
    [Authorize(Policy = Policy.EditChallenges)]
    public class ChallengesController : Base.Controller
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
            int take = 15;
            int skip = take * (page - 1);

            var challengeList = await _challengeService
                .GetPaginatedChallengeListAsync(skip, take);

            ChallengesListViewModel viewModel = new ChallengesListViewModel();

            viewModel.Challenges = challengeList.Data;

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = challengeList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }
            viewModel.PaginateModel = paginateModel;

            return View(viewModel);
        }

        public IActionResult Create()
        {
            PageTitle = "Create Challenge";
            return View("Create");
        }

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
                                Filename = Path.GetFileName(model.BadgePath),
                                IsActive = true,
                                Name = challenge.Name
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

        public async Task<IActionResult> Edit(int id)
        {
            Challenge challenge = new Challenge();
            if (TempData.ContainsKey(TempEditChallenge))
            {
                challenge = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<Challenge>((string)TempData[TempEditChallenge]);

                var tasks = await _challengeService.GetChallengeTasksAsync(id);
                challenge.Tasks = tasks.ToList();
            }
            else
            {
                challenge = await _challengeService.GetChallengeDetailsAsync(id);
            }
            if (challenge == null)
            {
                AlertDanger = "The requested challenge could not be accessed or does not exist";
                return RedirectToAction("Index");
            }

            ChallengesDetailViewModel viewModel = new ChallengesDetailViewModel()
            {
                Challenge = challenge,
                TaskTypes = Enum.GetNames(typeof(ChallengeTaskType))
                    .Select(m => new SelectListItem { Text = m, Value = m }).ToList()
            };

            if (challenge.BadgeId != null)
            {
                var challengeBadge = await _badgeService.GetByIdAsync((int)challenge.BadgeId);
                if (challengeBadge != null)
                {
                    viewModel.BadgePath = challengeBadge.Filename;
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

        [HttpPost]
        public async Task<IActionResult> Edit(ChallengesDetailViewModel model)
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
                                    IsActive = true,
                                    Name = challenge.Name
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

                await _challengeService.EditChallengeAsync(challenge);
                AlertSuccess = $"Challenge '<strong>{challenge.Name}</strong>' was successfully modified";
                return RedirectToAction("Index");
            }
            else
            {
                var tasks = await _challengeService.GetChallengeTasksAsync(model.Challenge.Id);
                model.Challenge.Tasks = tasks.ToList();
                PageTitle = $"Edit Challenge - {model.Challenge.Name}";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _challengeService.RemoveChallengeAsync(id);
            return RedirectToAction("Index");
        }

        #region Task methods
        [HttpPost]
        public IActionResult CloseTask(ChallengesDetailViewModel viewModel)
        {
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        public IActionResult OpenAddTask(ChallengesDetailViewModel viewModel)
        {
            TempData[NewTask] = true;
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

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

        [HttpPost]
        public IActionResult OpenModifyTask(ChallengesDetailViewModel viewModel, int taskId)
        {
            TempData[EditTask] = taskId;
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

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

        [HttpPost]
        public async Task<IActionResult> DeleteTask(ChallengesDetailViewModel viewModel, int id)
        {
            await _challengeService.RemoveTaskAsync(id);
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        public async Task<IActionResult>
            DecreaseTaskSort(ChallengesDetailViewModel viewModel, int id)
        {
            await _challengeService.DecreaseTaskPositionAsync(id);
            TempData[TempEditChallenge] = Newtonsoft.Json.JsonConvert
                .SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

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