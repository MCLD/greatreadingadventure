using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Categories;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageCategories)]
    public class CategoriesController : Base.MCController
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly CategoryService _categoryService;
        public CategoriesController(ILogger<CategoriesController> logger,
            ServiceFacade.Controller context,
            CategoryService categoryService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _categoryService = categoryService ?? throw new
                ArgumentNullException(nameof(categoryService));
            PageTitle = "Categories";
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var categoryList = await _categoryService.GetPaginatedListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
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

            var viewModel = new CategoryListViewModel()
            {
                Categories = categoryList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryListViewModel model)
        {
            try
            {
                await _categoryService.AddAsync(model.Category);
                ShowAlertSuccess($"Added Category \"{model.Category.Name}\"!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Category: ", gex);
            }

            return RedirectToAction("Index", new
            {
                search = model.Search,
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryListViewModel model)
        {
            try
            {
                await _categoryService.EditAsync(model.Category);
                ShowAlertSuccess($"Category \"{model.Category.Name}\" updated!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit Category: ", gex);
            }

            return RedirectToAction("Index", new
            {
                search = model.Search,
                page = model.PaginateModel.CurrentPage
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CategoryListViewModel model)
        {
            try
            {
                await _categoryService.RemoveAsync(model.Category.Id);
                ShowAlertSuccess($"Category \"{model.Category.Name}\" removed!");
            }
            catch(GraException gex)
            {
                ShowAlertDanger("Unable to remove Category: ", gex);
            }

            return RedirectToAction("Index", new
            {
                search = model.Search,
                page = model.PaginateModel.CurrentPage
            });
        }
    }
}
