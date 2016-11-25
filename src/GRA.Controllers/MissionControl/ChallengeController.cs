using GRA.Controllers.ViewModel.Challenge;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.AccessMissionControl)]
    public class ChallengeController : Base.Controller
    {
        private readonly ILogger<ChallengeController> logger;
        private readonly ChallengeService challengeService;
        public ChallengeController(ILogger<ChallengeController> logger,
            ServiceFacade.Controller context,
            ChallengeService challengeService)
            : base(context)
        {
            this.logger = logger;

            if (challengeService == null)
            {
                throw new ArgumentNullException(nameof(challengeService));
            }
            this.challengeService = challengeService;
        }

        public async Task<IActionResult> Index(string Search, string FilterBy, int? page)
        {
            int currentPage = page ?? 1;
            int take = 15;
            int skip = take * (currentPage - 1);

            var challengeList = await challengeService.GetPaginatedChallengeListAsync(CurrentUser, skip, take);

            ChallengeListViewModel viewModel = new ChallengeListViewModel();

            viewModel.Challenges = challengeList.Data;

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = challengeList.Count,
                CurrentPage = currentPage,
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
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(Challenge challenge)
        {
            if (ModelState.IsValid)
            {
                int challengeId;

                // todo: fix siteId
                challenge.SiteId = 1;
                challengeId = challengeService.AddChallengeAsync(CurrentUser, challenge).Id;
                AlertSuccess = $"{challenge.Name} was successfully created";
                return RedirectToAction("Edit", new { id = challengeId });
            }
            else
            {
                return View(challenge);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            Challenge challenge = new Challenge();
            if (TempData.ContainsKey("TempEditChallenge"))
            {
                challenge = Newtonsoft.Json.JsonConvert.DeserializeObject<Challenge>((string)TempData["TempEditChallenge"]);
                challenge.Tasks = (await challengeService.GetChallengeTasksAsync(id)).ToList();
            }
            else
            {
                challenge = await challengeService.GetChallengeDetailsAsync(CurrentUser, id);
            }
            if (challenge == null)
            {
                TempData["AlertDanger"] = "The requested challenge could not be accessed or does not exist";
                return RedirectToAction("Index");
            }
            ChallengeDetailViewModel viewModel = new ChallengeDetailViewModel()
            {
                Challenge = challenge,
                TaskTypes = Enum.GetNames(typeof(ChallengeTaskType)).Select(m => new SelectListItem { Text = m, Value = m }).ToList()
            };
            if (TempData.ContainsKey("AddTask"))
            {
                TempData.Remove("AddTask");
                TempData.Remove("EditTask");
                viewModel.AddTask = true;
            }
            else if (TempData.ContainsKey("EditTask"))
            {
                viewModel.Task = await challengeService.GetTaskAsync(CurrentUser, (int)TempData["EditTask"]);
            }
            return View("Edit", viewModel);
        }
        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> Edit(ChallengeDetailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await challengeService.EditChallengeAsync(CurrentUser, viewModel.Challenge);
                AlertSuccess = $"{viewModel.Challenge.Name} was successfully modified";
                return RedirectToAction("Index");
            }
            else
            {
                viewModel.Challenge.Tasks = (await challengeService.GetChallengeTasksAsync(viewModel.Challenge.Id)).ToList();
                return View(viewModel);
            }
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> Delete(int id)
        {
            await challengeService.RemoveChallengeAsync(CurrentUser, id);
            return RedirectToAction("Index");
        }

        #region Task methods
        [HttpPost]
        public IActionResult CloseTask(ChallengeDetailViewModel viewModel)
        {
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public IActionResult OpenAddTask(ChallengeDetailViewModel viewModel)
        {
            TempData["AddTask"] = true;
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> AddTask(ChallengeDetailViewModel viewModel)
        {
            foreach (string key in ModelState.Keys.Where(m => m.StartsWith("Challenge.")).ToList())
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                viewModel.Task.ChallengeId = viewModel.Challenge.Id;
                await challengeService.AddTaskAsync(CurrentUser, viewModel.Task);
            }
            else
            {
                TempData["AddTask"] = true;
            }
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public IActionResult OpenModifyTask(ChallengeDetailViewModel viewModel, int taskId)
        {
            TempData["EditTask"] = taskId;
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);
            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> ModifyTask(ChallengeDetailViewModel viewModel)
        {
            foreach (string key in ModelState.Keys.Where(m => m.StartsWith("Challenge.")).ToList())
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                await challengeService.EditTaskAsync(CurrentUser, viewModel.Task);
            }
            else
            {
                TempData["EditTask"] = viewModel.Task.Id;
            }
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> DeleteTask(ChallengeDetailViewModel viewModel, int id)
        {
            await challengeService.RemoveTaskAsync(CurrentUser, id);
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> DecreaseTaskSort(ChallengeDetailViewModel viewModel, int id)
        {
            await challengeService.DecreaseTaskPositionAsync(CurrentUser, id);
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditChallenges)]
        public async Task<IActionResult> IncreaseTaskSort(ChallengeDetailViewModel viewModel, int id)
        {
            await challengeService.IncreaseTaskPositionAsync(CurrentUser, id);
            TempData["TempEditChallenge"] = Newtonsoft.Json.JsonConvert.SerializeObject(viewModel.Challenge);

            return RedirectToAction("Edit", new { id = viewModel.Challenge.Id });
        }
        #endregion
    }
}