using System;
using System.Collections.Generic;
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
        private readonly ILogger<NewsController> _logger;
        private readonly NewsService _newsService;
        public NewsController(ILogger<NewsController> logger,
            ServiceFacade.Controller context,
            NewsService newsService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _newsService = newsService ?? throw new ArgumentNullException(nameof(newsService));
            PageTitle = "Mission Control News management";
        }

        public async Task<IActionResult> Index(string search, int? category, int page = 1)
        {
            var filter = new BaseFilter(page)
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
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new PostListViewModel
            {
                Posts = postList.Data,
                PaginateModel = paginateModel,
                Search = search,
                CategoryId = category,
                CategoryList = await _newsService.GetCategoriesAsync()
            };

            if (category.HasValue)
            {
                viewModel.Category = await _newsService.GetCategoryByIdAsync(category.Value);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(PostListViewModel model)
        {
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

        public async Task<IActionResult> CreatePost()
        {
            var viewModel = new PostDetailViewModel
            {
                Action = nameof(CreatePost),
                Categories = new SelectList(await _newsService.GetCategoriesAsync(), "Id", "Name")
            };

            return View("PostDetail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postUrl = Url.Action(nameof(HomeController.Index), 
                    "Home",
                    null,
                    HttpContext.Request.Scheme);

                var post = await _newsService.CreatePostAsync(model.Post, postUrl, model.Publish);
                ShowAlertSuccess($"Added Post \"{post.Title}\"!");
                return RedirectToAction(nameof(Index));
            }

            model.Action = nameof(CreatePost);
            model.Categories = new SelectList(await _newsService.GetCategoriesAsync(), "Id", "Name");

            return View("PostDetail", model);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var viewModel = new PostDetailViewModel
            {
                Post = await _newsService.GetPostByIdAsync(id),
                Action = nameof(EditPost),
                Categories = new SelectList(await _newsService.GetCategoriesAsync(), "Id", "Name")
            };

            return View("PostDetail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(PostDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var postUrl = Url.Action(nameof(HomeController.Index),
                    "Home",
                    null,
                    HttpContext.Request.Scheme);

                var post = await _newsService.EditPostAsync(model.Post, postUrl, model.Publish);
                ShowAlertSuccess($"Updated Post \"{post.Title}\"!");
                return RedirectToAction(nameof(Index));
            }

            model.Action = nameof(CreatePost);
            model.Categories = new SelectList(await _newsService.GetCategoriesAsync(), "Id", "Name");

            return View("PostDetail", model);
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
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new CategoryListViewModel
            {
                Categories = categoryList.Data,
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(CategoryListViewModel model)
        {
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

        public IActionResult CreateCategory()
        {
            var viewModel = new CategoryDetailViewModel
            {
                Action = nameof(CreateCategory)
            };

            return View("CategoryDetail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _newsService.CreateCategoryAsync(model.Category);
                ShowAlertSuccess($"Added Category \"{category.Name}\"!");
                return RedirectToAction(nameof(Categories));
            }

            model.Action = nameof(CreateCategory);

            return View("CategoryDetail", model);
        }

        public async Task<IActionResult> EditCategory(int id)
        {
            var viewModel = new CategoryDetailViewModel
            {
                Category = await _newsService.GetCategoryByIdAsync(id),
                Action = nameof(EditCategory)
            };

            return View("CategoryDetail", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _newsService.EditCategoryAsync(model.Category);
                ShowAlertSuccess($"Updated Category \"{category.Name}\"!");
                return RedirectToAction(nameof(Categories));
            }

            model.Action = nameof(EditCategory);

            return View("CategoryDetail", model);
        }
    }
}
