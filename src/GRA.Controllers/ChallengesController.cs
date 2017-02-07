using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class ChallengesController : Base.UserController
    {
        private readonly ILogger<ChallengesController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        public readonly ActivityService _activityService;
        private readonly ChallengeService _challengeService;
        public ChallengesController(ILogger<ChallengesController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            ChallengeService challengeService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _challengeService = Require.IsNotNull(challengeService, nameof(challengeService));
            PageTitle = "Challenges";
        }

        public async Task<IActionResult> Index(string Search, int page = 1)
        {
            int siteId = GetCurrentSiteId();
            int take = 15;
            int skip = take * (page - 1);

            var challengeList = await _challengeService
                .GetPaginatedChallengeListAsync(skip, take, Search);

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

            foreach (var challenge in challengeList.Data)
            {
                if (!string.IsNullOrEmpty(challenge.BadgeFilename))
                {
                    challenge.BadgeFilename = _pathResolver.ResolveContentPath(challenge.BadgeFilename);
                }
                if (challenge.IsCompleted == true)
                {
                    challenge.Status = "Completed!";
                }
            }

            var siteStage = GetSiteStage();

            var isActive = AuthUser.Identity.IsAuthenticated && (siteStage == SiteStage.ProgramOpen
                || siteStage == SiteStage.ProgramEnded);

            ChallengesListViewModel viewModel = new ChallengesListViewModel()
            {
                Challenges = challengeList.Data,
                PaginateModel = paginateModel,
                Search = Search,
                IsActive = isActive
        };

            if (!string.IsNullOrWhiteSpace(Search))
            {
                HttpContext.Session.SetString(SessionKey.ChallengeSearch, Search);
            }
            else
            {
                HttpContext.Session.Remove(SessionKey.ChallengeSearch);
            }
            HttpContext.Session.SetInt32(SessionKey.ChallengePage, page);

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            Challenge challenge = new Domain.Model.Challenge();
            try
            {
                challenge = await _challengeService.GetChallengeDetailsAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view challenge: ", gex);
                return RedirectToAction("Index");
            }
            var siteStage = GetSiteStage();

            if (!string.IsNullOrEmpty(challenge.BadgeFilename))
            {
                challenge.BadgeFilename = _pathResolver.ResolveContentPath(challenge.BadgeFilename);
            }

            bool isActive = AuthUser.Identity.IsAuthenticated && siteStage == SiteStage.ProgramOpen;
            bool showCompleted = siteStage == SiteStage.ProgramOpen
                || siteStage == SiteStage.ProgramEnded;

            ChallengeDetailViewModel viewModel = new ChallengeDetailViewModel()
            {
                Challenge = challenge,
                BadgePath = challenge.BadgeFilename,
                IsActive = isActive,
                ShowCompleted = showCompleted,
                Tasks = new List<TaskDetailViewModel>()
            };

            viewModel.Details = $"Completing <strong>{challenge.TasksToComplete} "
                + $"{(challenge.TasksToComplete > 1 ? "Tasks" : "Task")}</strong> will earn: "
                + $"<strong>{challenge.PointsAwarded} "
                + $"{(challenge.PointsAwarded > 1 ? "Points" : "Point")}</strong> and "
                + "<strong>a badge</strong>.";

            foreach (var task in challenge.Tasks)
            {
                TaskDetailViewModel taskModel = new TaskDetailViewModel()
                {
                    Id = task.Id,
                    IsCompleted = task.IsCompleted ?? false,
                    TaskType = task.ChallengeTaskType.ToString(),
                    Url = task.Url
                };
                if (task.ChallengeTaskType.ToString() == "Book")
                {
                    string description = $"Read <strong><em>{task.Title}</em></strong>";
                    if (!string.IsNullOrWhiteSpace(task.Author))
                    {
                        description += $" by <strong>{task.Author}</strong>";
                    }
                    taskModel.Description = description;
                }
                else
                {
                    taskModel.Description = task.Title;
                }
                viewModel.Tasks.Add(taskModel);
            }
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CompleteTasks(ChallengeDetailViewModel model)
        {
            List<ChallengeTask> tasks = _mapper.Map<List<ChallengeTask>>(model.Tasks);
            try
            {
                var completed = await _activityService.UpdateChallengeTasksAsync(model.Challenge.Id, tasks);
                if (!completed)
                {
                    var challenge
                        = await _challengeService.GetChallengeDetailsAsync(model.Challenge.Id);
                    if (challenge.TasksToComplete != null
                        && challenge.TasksToComplete > 0)
                    {
                        int tasksCompleted = model.Tasks.Where(_ => _.IsCompleted == true).Count();
                        int percentage = tasksCompleted * 100 / (int)challenge.TasksToComplete;
                        ShowAlertSuccess($"Your status has been saved. You have completed {percentage}% of the required tasks for the challenge: {challenge.Name}!");
                    }
                    else
                    {
                        ShowAlertSuccess("Your status has been saved!");
                    }
                }
            }
            catch (GraException gex)
            {
                AlertInfo = gex.Message;
            }
            return RedirectToAction("Detail", new { id = model.Challenge.Id });
        }
    }
}
