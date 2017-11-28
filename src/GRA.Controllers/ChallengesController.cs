using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    public class ChallengesController : Base.UserController
    {
        private readonly ILogger<ChallengesController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        public readonly ActivityService _activityService;
        private readonly CategoryService _categoryService;
        private readonly ChallengeService _challengeService;
        private readonly SiteService _siteService;
        public ChallengesController(ILogger<ChallengesController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            CategoryService categoryService,
            ChallengeService challengeService,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _categoryService = Require.IsNotNull(categoryService, nameof(categoryService));
            _challengeService = Require.IsNotNull(challengeService, nameof(challengeService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "Challenges";
        }

        public async Task<IActionResult> Index(string Search, string Categories, bool Favorites = false, int page = 1)
        {
            int siteId = GetCurrentSiteId();

            ChallengeFilter filter = new ChallengeFilter(page);
            if (!string.IsNullOrWhiteSpace(Search))
            {
                filter.Search = Search;
            }
            if (!string.IsNullOrWhiteSpace(Categories))
            {
                var categoryIds = new List<int>();
                foreach (var category in Categories.Split(','))
                {
                    int result;
                    if (int.TryParse(category, out result))
                    {
                        categoryIds.Add(result);
                    }
                }
                filter.CategoryIds = categoryIds;
            }
            if (Favorites == true && AuthUser.Identity.IsAuthenticated)
            {
                filter.Favorites = true;
            }
            var challengeList = await _challengeService.GetPaginatedChallengeListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = challengeList.Count,
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

            var isActive = (siteStage == SiteStage.ProgramOpen
                || siteStage == SiteStage.ProgramEnded);

            var categoryList = await _categoryService.GetListAsync(true);

            ChallengesListViewModel viewModel = new ChallengesListViewModel()
            {
                Challenges = challengeList.Data.ToList(),
                PaginateModel = paginateModel,
                Search = Search,
                Categories = Categories,
                Favorites = Favorites,
                IsActive = isActive,
                IsLoggedIn = AuthUser.Identity.IsAuthenticated,
                CategoryIds = filter.CategoryIds,
                CategoryList = new SelectList(categoryList, "Id", "Name")
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSingleFavorite(int challengeId, bool favorite)
        {
            var challengeList = new List<Challenge>()
            {
                new Challenge()
                {
                    Id = challengeId,
                    IsFavorited = favorite
                }
            };
            var serviceResult = await _activityService.UpdateFavoriteChallenges(challengeList);

            return Json(new
            {
                success = serviceResult.Status == ServiceResultStatus.Success,
                message = serviceResult.Message,
                favorite = favorite
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateFavorites(ChallengesListViewModel model)
        {
            var serviceResult = await _activityService.UpdateFavoriteChallenges(model.Challenges);
            if (serviceResult.Status == ServiceResultStatus.Warning
                        && !string.IsNullOrWhiteSpace(serviceResult.Message))
            {
                ShowAlertWarning(serviceResult.Message);
            }
            int? page = null;
            if (model.PaginateModel.CurrentPage > 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction("Index", new
            {
                page = page,
                Search = model.Search,
                Categories = model.Categories
            });
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
                + $"{(challenge.PointsAwarded > 1 ? "Points" : "Point")}</strong>";

            if (challenge.BadgeId.HasValue)
            {
                viewModel.Details += " and <strong>a badge</strong>.";
            }

            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            foreach (var task in challenge.Tasks)
            {
                TaskDetailViewModel taskModel = new TaskDetailViewModel()
                {
                    Id = task.Id,
                    IsCompleted = task.IsCompleted ?? false,
                    TaskType = task.ChallengeTaskType.ToString(),
                    Url = task.Url
                };
                var title = task.Title;
                if (!string.IsNullOrWhiteSpace(task.Url))
                {
                    title = $"<a href=\"{task.Url}\" target=\"_blank\">{title}</a>";
                }
                if (task.ChallengeTaskType.ToString() == "Book")
                {
                    string description = $"Read <strong><em>{title}</em></strong>";
                    if (!string.IsNullOrWhiteSpace(task.Author))
                    {
                        description += $" by <strong>{task.Author}</strong>";
                    }
                    taskModel.Description = description;
                }
                else
                {
                    taskModel.Description = CommonMark.CommonMarkConverter.Convert(task.Title);
                }
                if (!string.IsNullOrWhiteSpace(task.Filename))
                {
                    var contentPath = _pathResolver.ResolveContentPath(task.Filename);
                    taskModel.FilePath = $"{siteUrl}/{contentPath}";
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
