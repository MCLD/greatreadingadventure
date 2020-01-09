using System;
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
        private readonly ActivityService _activityService;
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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = context.Mapper;
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
            _categoryService = categoryService
                ?? throw new ArgumentNullException(nameof(categoryService));
            _challengeService = challengeService
                ?? throw new ArgumentNullException(nameof(challengeService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = _sharedLocalizer[Annotations.Title.Challenges];
        }

        public async Task<IActionResult> Index(string Search = null,
            int? Program = null,
            string Categories = null,
            string Group = null,
            bool Favorites = false,
            int page = 1,
            System.Net.HttpStatusCode httpStatus = System.Net.HttpStatusCode.OK)
        {
            var filter = new ChallengeFilter(page);
            if (!string.IsNullOrWhiteSpace(Search))
            {
                filter.Search = Search;
            }
            if (Program.HasValue)
            {
                filter.ProgramIds = new List<int?> { Program };
            }
            if (!string.IsNullOrWhiteSpace(Categories))
            {
                var categoryIds = new List<int>();
                foreach (var category in Categories.Split(','))
                {
                    if (int.TryParse(category, out int result))
                    {
                        categoryIds.Add(result);
                    }
                }
                filter.CategoryIds = categoryIds;
            }
            if (Favorites && AuthUser.Identity.IsAuthenticated)
            {
                filter.Favorites = true;
            }

            ChallengeGroup challengeGroup = null;
            if (!string.IsNullOrWhiteSpace(Group))
            {
                challengeGroup = await _challengeService.GetActiveGroupByStubAsync(Group);
                if (challengeGroup != null)
                {
                    filter.GroupId = challengeGroup.Id;
                }
                PageTitle
                    = _sharedLocalizer[Annotations.Title.ChallengeGroup, challengeGroup.Name];
            }

            var challengeList = await _challengeService.GetPaginatedChallengeListAsync(filter);

            var paginateModel = new PaginateViewModel
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
                    challenge.Status = _sharedLocalizer[Annotations.Interface.Completed];
                }
            }

            var siteStage = GetSiteStage();

            var isActive = siteStage == SiteStage.ProgramOpen
                || siteStage == SiteStage.ProgramEnded;

            var categoryList = await _categoryService.GetListAsync(true);

            var viewModel = new ChallengesListViewModel
            {
                Challenges = challengeList.Data.ToList(),
                ChallengeGroup = challengeGroup,
                PaginateModel = paginateModel,
                Search = Search,
                Program = Program,
                Categories = Categories,
                Favorites = Favorites,
                IsActive = isActive,
                IsLoggedIn = AuthUser.Identity.IsAuthenticated,
                CategoryIds = filter.CategoryIds,
                CategoryList = new SelectList(categoryList, "Id", "Name"),
                ProgramList = new SelectList(await _siteService.GetProgramList(), "Id", "Name")
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

            if(httpStatus != System.Net.HttpStatusCode.OK)
            {
                Response.StatusCode = (int)httpStatus;
            }
            return View(nameof(Index), viewModel);
        }

        public async Task<IActionResult> List(string id, int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var challengeGroup = await _challengeService.GetActiveGroupByStubAsync(id);
                if (challengeGroup != null)
                {
                    return await Index(Group: id, page: page);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateSingleFavorite(int challengeId, bool favorite)
        {
            var serviceResult = new ServiceResult();
            try
            {
                var challengeList = new List<Challenge>
                {
                    new Challenge
                    {
                        Id = challengeId,
                        IsFavorited = favorite
                    }
                };
                serviceResult = await _activityService.UpdateFavoriteChallenges(challengeList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error updaing user favorite challenges: {Message}",
                    ex.Message);
                serviceResult.Status = ServiceResultStatus.Error;
                serviceResult.Message = "An error occured while trying to update the challenge.";
            }
            return Json(new
            {
                success = serviceResult.Status == ServiceResultStatus.Success,
                message = serviceResult.Message,
                favorite
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
            return RedirectToAction(nameof(Index), new
            {
                page,
                model.Search,
                model.Program,
                model.Categories,
                model.Favorites,
                Group = model.ChallengeGroup?.Stub
            });
        }

        public async Task<IActionResult> Detail(int id)
        {
            Challenge challenge = null;
            try
            {
                challenge = await _challengeService.GetChallengeDetailsAsync(id);
            }
            catch (GraException gex)
            {
                ShowAlertWarning(gex.Message);
                return await Index(httpStatus: System.Net.HttpStatusCode.NotFound);
            }
            var siteStage = GetSiteStage();

            if (!string.IsNullOrEmpty(challenge.BadgeFilename))
            {
                challenge.BadgeFilename = _pathResolver.ResolveContentPath(challenge.BadgeFilename);
            }

            bool isActive = AuthUser.Identity.IsAuthenticated && siteStage == SiteStage.ProgramOpen;
            bool showCompleted = siteStage == SiteStage.ProgramOpen
                || siteStage == SiteStage.ProgramEnded;

            var viewModel = new ChallengeDetailViewModel
            {
                Challenge = challenge,
                BadgePath = challenge.BadgeFilename,
                IsActive = isActive,
                IsLoggedIn = AuthUser.Identity.IsAuthenticated,
                ShowCompleted = showCompleted,
                Tasks = new List<TaskDetailViewModel>(),
                IsBadgeEarning = challenge.BadgeId.HasValue,
                PointCountAndDescription = challenge.PointsAwarded == 1
                    ? _sharedLocalizer[Annotations.Info.PointSingular, challenge.PointsAwarded]
                    : _sharedLocalizer[Annotations.Info.PointsPlural, challenge.PointsAwarded],
                TaskCountAndDescription = challenge.TasksToComplete == 1
                    ? _sharedLocalizer[Annotations.Info.TaskSingular, challenge.TasksToComplete]
                    : _sharedLocalizer[Annotations.Info.TasksPlural, challenge.TasksToComplete]
            };

            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            foreach (var task in challenge.Tasks)
            {
                var taskModel = new TaskDetailViewModel
                {
                    Id = task.Id,
                    IsCompleted = task.IsCompleted ?? false,
                    TaskType = task.ChallengeTaskType.ToString(),
                    Url = task.Url,
                    Title = task.Title,
                    Author = task.Author
                };
                if(taskModel.TaskType != "Book")
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
            PageTitle = _sharedLocalizer[Annotations.Title.ChallengeDetails, challenge.Name];
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
                        int tasksCompleted = model.Tasks.Count(_ => _.IsCompleted);
                        int percentage = tasksCompleted * 100 / (int)challenge.TasksToComplete;
                        ShowAlertSuccess(_sharedLocalizer[Annotations.Info.StatusSavedPercentage,
                            percentage,
                            challenge.Name]);
                    }
                    else
                    {
                        ShowAlertSuccess(_sharedLocalizer[Annotations.Info.StatusSaved]);
                    }
                }
            }
            catch (GraException gex)
            {
                AlertInfo = gex.Message;
            }
            return RedirectToAction(nameof(Detail), new { id = model.Challenge.Id });
        }
    }
}
