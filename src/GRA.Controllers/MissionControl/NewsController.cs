using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.News;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageNews)]
    public class NewsController : Base.MCController
    {
        private readonly JobService _jobService;
        private readonly ILogger<NewsController> _logger;
        private readonly NewsService _newsService;

        public NewsController(ILogger<NewsController> logger,
            ServiceFacade.Controller context,
            JobService jobService,
            NewsService newsService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            PageTitle = "Mission Control News management";
        }

        public async Task<IActionResult> Categories(int page = 1)
        {
            var filter = new BaseFilter(page);

            var categoryList = await _newsService.GetPaginatedCategoryListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = categoryList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(new { page = paginateModel.LastPage ?? 1 });
            }

            var viewModel = new CategoryListViewModel
            {
                Categories = categoryList.Data,
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        public IActionResult CreateCategory()
        {
            return View("CategoryDetail", new CategoryDetailViewModel
            {
                Action = nameof(CreateCategory)
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryDetailViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Could not create empty category.");
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var category = await _newsService.CreateCategoryAsync(model.Category);
                ShowAlertSuccess($"Added Category \"{category.Name}\"!");
                return RedirectToAction(nameof(Categories));
            }

            model.Action = nameof(CreateCategory);

            return View("CategoryDetail", model);
        }

        public async Task<IActionResult> CreatePost()
        {
            var viewModel = new PostDetailViewModel
            {
                Action = nameof(CreatePost),
                Categories
                    = new SelectList(await _newsService.GetAllCategoriesAsync(), "Id", "Name"),
                NoYes = NoYesSelectList()
            };

            return View("PostDetail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostDetailViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Could not create empty post.");
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var post = await _newsService.CreatePostAsync(model.Post, model.Publish);

                if (model.Publish)
                {
                    return await SendEmailJobAsync(GetJobParameters(post));
                }
                else
                {
                    ShowAlertSuccess($"Added Post \"{post.Title}\"!");
                    return RedirectToAction(nameof(Index));
                }
            }

            model.Action = nameof(CreatePost);
            model.Categories
                = new SelectList(await _newsService.GetAllCategoriesAsync(), "Id", "Name");
            model.NoYes = NoYesSelectList();

            return View("PostDetail", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(CategoryListViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Could not delete empty category.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _newsService.RemoveCategoryAsync(model.Category.Id);
                ShowAlertSuccess($"Removed Category \"{model.Category.Name}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Category: ", gex);
            }

            return RedirectToAction(nameof(Categories), new
            {
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(PostListViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Could not delete empty post.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _newsService.RemovePostAsync(model.Post.Id);
                ShowAlertSuccess($"Removed Post \"{model.Post.Title}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove Post: ", gex);
            }

            return RedirectToAction(nameof(Index), new
            {
                category = model.Category,
                page = model.PaginateModel.CurrentPage,
                search = model.Search
            });
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            return View("CategoryDetail", new CategoryDetailViewModel
            {
                Category = await _newsService.GetCategoryByIdAsync(id),
                Action = nameof(EditCategory)
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryDetailViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Could not edit empty category.");
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var category = await _newsService.EditCategoryAsync(model.Category);
                ShowAlertSuccess($"Updated Category \"{category.Name}\"!");
                return RedirectToAction(nameof(Categories));
            }

            model.Action = nameof(EditCategory);

            return View("CategoryDetail", model);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            return View("PostDetail", new PostDetailViewModel
            {
                Action = nameof(EditPost),
                Categories
                    = new SelectList(await _newsService.GetAllCategoriesAsync(), "Id", "Name"),
                NoYes = NoYesSelectList(),
                Post = await _newsService.GetPostByIdAsync(id)
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostDetailViewModel model)
        {
            if (model == null)
            {
                ShowAlertDanger("Could not edit empty post.");
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                var post = await _newsService.EditPostAsync(model.Post,
                    model.Publish,
                    model.MarkUpdated);

                if (model.Publish)
                {
                    return await SendEmailJobAsync(GetJobParameters(post));
                }
                else
                {
                    ShowAlertSuccess($"Updated Post \"{post.Title}\"!");
                    return RedirectToAction(nameof(Index));
                }
            }

            model.Action = nameof(CreatePost);
            model.Categories
                = new SelectList(await _newsService.GetAllCategoriesAsync(), "Id", "Name");
            model.NoYes = NoYesSelectList();

            return View("PostDetail", model);
        }

        public async Task<IActionResult> Index(string search, int? category, int page = 1)
        {
            var filter = new NewsFilter(page)
            {
                Search = search
            };

            if (category.HasValue)
            {
                filter.CategoryIds = new List<int> { category.Value };
            }

            var postList = await _newsService.GetPaginatedPostListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = postList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(new { page = paginateModel.LastPage ?? 1 });
            }

            var viewModel = new PostListViewModel
            {
                Posts = postList.Data,
                PaginateModel = paginateModel,
                Search = search,
                CategoryId = category,
                CategoryList = await _newsService.GetAllCategoriesAsync()
            };

            if (category.HasValue)
            {
                viewModel.Category = await _newsService.GetCategoryByIdAsync(category.Value);
            }

            return View(viewModel);
        }

        private JobSendNewsEmails GetJobParameters(NewsPost post)
        {
            return new JobSendNewsEmails
            {
                NewsPostId = post.Id,
                PostLink = Url.Action(nameof(HomeController.Index),
                    HomeController.Name,
                    new { category = post.CategoryId },
                    HttpContext.Request.Scheme),
                SiteLink =
                    Url.Action(nameof(Controllers.HomeController.Index),
                    Controllers.HomeController.Name,
                    new { area = "" },
                    HttpContext.Request.Scheme),
                SiteMcLink = Url.Action(nameof(HomeController.Index),
                    HomeController.Name,
                    null,
                    HttpContext.Request.Scheme),
                SiteName = HttpContext.Items[ItemKey.SiteName]?.ToString()
            };
        }

        private async Task<IActionResult> SendEmailJobAsync(JobSendNewsEmails jobDetails)
        {
            var jobToken = await _jobService.CreateJobAsync(new Job
            {
                JobType = JobType.SendNewsEmails,
                SerializedParameters = JsonSerializer.Serialize(jobDetails)
            });

            _logger.LogDebug("Redirecting to send news emails for post {PostId}",
                jobDetails.NewsPostId);

            return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
            {
                CancelUrl = Url.Action(nameof(Index)),
                JobToken = jobToken.ToString(),
                PingSeconds = 5,
                SuccessRedirectUrl = "",
                SuccessUrl = Url.Action(nameof(Index)),
                Title = "Sending emails..."
            });
        }
    }
}
