using GRA.Controllers.ViewModel.MissionControl.Pages;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewUnpublishedPages)]
    public class PagesController : Base.MCController
    {
        private readonly ILogger<PagesController> _logger;
        private readonly PageService _pageService;
        public PagesController(ILogger<PagesController> logger,
            ServiceFacade.Controller context,
            PageService pageService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _pageService = Require.IsNotNull(pageService, nameof(pageService));
            PageTitle = "Pages";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var pageList = await _pageService.GetPaginatedPageListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = pageList.Count,
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

            PagesListViewModel viewModel = new PagesListViewModel()
            {
                Pages = pageList.Data.ToList(),
                PaginateModel = paginateModel,
                CanAddPage = UserHasPermission(Permission.AddPages),
                CanDeletePage = UserHasPermission(Permission.DeletePages)
            };

            return View(viewModel);
        }

        [Authorize(Policy.AddPages)]
        public IActionResult Create()
        {
            PageTitle = "Create Page";
            return View();
        }

        [Authorize(Policy.AddPages)]
        [HttpPost]
        public async Task<IActionResult> Create(PagesEditViewModel model)
        {
            Regex checkStub = new Regex(@"^[\w\-]*$");
            if (!checkStub.IsMatch(model.Page.Stub))
            {
                ModelState.AddModelError("Page.Stub", "Invalid stub, only letters, numbers, hypens and underscores are allowed");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _pageService.AddPageAsync(model.Page);
                    AlertSuccess = $"Page '{model.Page.Title}' created";
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    AlertInfo = gex.Message;
                }
            }
            PageTitle = "Create Page";
            return View(model);
        }

        public async Task<IActionResult> Edit(string stub)
        {
            try
            {
                PagesEditViewModel viewModel = new PagesEditViewModel()
                {
                    Page = await _pageService.GetByStubAsync(stub, false),
                    CanEdit = UserHasPermission(Permission.EditPages)
                };
                PageTitle = "Edit Page";
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view page: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy.EditPages)]
        [HttpPost]
        public async Task<IActionResult> Edit(PagesEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _pageService.EditPageAsync(model.Page);
                AlertSuccess = $"Page '{model.Page.Title}' edited";
                return RedirectToAction("Index");
            }
            PageTitle = "Edit Page";
            return View(model);
        }

        [Authorize(Policy.DeletePages)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _pageService.DeletePageAsync(id);
            AlertSuccess = "Page deleted";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Preview(string stub)
        {
            try
            {
                var page = await _pageService.GetByStubAsync(stub, false);
                PageTitle = $"Preview - {page.Title}";

                PagePreviewViewModel viewModel = new PagePreviewViewModel()
                {
                    Content = CommonMark.CommonMarkConverter.Convert(page.Content),
                    Stub = stub
                };
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view page: ", gex);
                return RedirectToAction("Index");
            }
        }
    }
}
