using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Pages;
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
    [Authorize(Policy = Policy.ManagePages)]
    public class PagesController : Base.MCController
    {
        private readonly ILogger<PagesController> _logger;
        private readonly PageService _pageService;
        private readonly LanguageService _languageService;
        private readonly SiteService _siteService;

        public PagesController(ILogger<PagesController> logger,
            ServiceFacade.Controller context,
            PageService pageService,
            LanguageService languageService,
            SiteService siteService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = "Page management";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter(page);

            var headerList = await _pageService.GetPaginatedHeaderListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = headerList.Count,
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

            var viewModel = new PagesListViewModel
            {
                PageHeaders = headerList.Data.ToList(),
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PagesListViewModel model)
        {
            var checkStub = new Regex(@"^[\w\-]*$");
            if (!checkStub.IsMatch(model.PageHeader.Stub))
            {
                ModelState.AddModelError("PageHeader.Stub", "Invalid stub, only letters, numbers, hypens and underscores are allowed");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var pageHeader = await _pageService.AddPageHeaderAsync(model.PageHeader);
                    ShowAlertSuccess($"Page \"{pageHeader.PageName}\" created!");
                    return RedirectToAction(nameof(Detail), new { id = pageHeader.Id });
                }
                catch (GraException gex)
                {
                    AlertInfo = gex.Message;
                }
            }
            else
            {
                var errorMessages = new StringBuilder("<ul>");
                foreach (var error in ModelState.Values.SelectMany(_ => _.Errors))
                {
                    errorMessages.Append("<li>").Append(error.ErrorMessage).Append("</li>");
                }
                errorMessages.Append("</ul>");

                ShowAlertDanger("Could add the page to the following error(s):",
                    errorMessages.ToString());
            }

            return RedirectToAction(nameof(Index), new { page = model.PaginateModel.CurrentPage });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PagesListViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pageHeader = await _pageService.EditPageHeaderAsync(model.PageHeader);
                ShowAlertSuccess($"Page \"{pageHeader.PageName}\" updated!");
            }
            else
            {
                var errorMessages = new StringBuilder("<ul>");
                foreach (var error in ModelState.Values.SelectMany(_ => _.Errors))
                {
                    errorMessages.Append("<li>").Append(error.ErrorMessage).Append("</li>");
                }
                errorMessages.Append("</ul>");

                ShowAlertDanger("Could add the page to the following error(s):",
                    errorMessages.ToString());
            }

            return RedirectToAction(nameof(Index), new { page = model.PaginateModel.CurrentPage });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(PagesListViewModel model)
        {
            await _pageService.DeletePageHeaderAsync(model.PageHeader.Id);
            ShowAlertSuccess($"Page \"{model.PageHeader.PageName}\" removed!");
            return RedirectToAction(nameof(Index), new { page = model.PaginateModel.CurrentPage });
        }

        public async Task<IActionResult> Detail(int id, string language)
        {
            var header = await _pageService.GetHeaderByIdAsync(id);

            var languages = await _languageService.GetActiveAsync();

            var selectedLanguage = languages
                .FirstOrDefault(_ => _.Name.Equals(language, StringComparison.OrdinalIgnoreCase))
                ?? languages.Single(_ => _.IsDefault);

            var page = await _pageService.GetByHeaderAndLanguageAsync(id, selectedLanguage.Id);

            var viewModel = new PageDetailViewModel
            {
                Page = page,
                HeaderId = header.Id,
                HeaderName = header.PageName,
                HeaderStub = header.Stub,
                NewPage = page == null,
                LanguageList = new SelectList(languages, "Name", "Description",
                    selectedLanguage.Name),
                SelectedLanguageId = selectedLanguage.Id
            };

            if (page?.IsPublished == true)
            {
                var baseUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
                var action = Url.Action("Index", "Info", new
                {
                    area = "",
                    culture = selectedLanguage.Name,
                    id = header.Stub
                });

                viewModel.PageUrl = baseUrl + action;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(PageDetailViewModel model)
        {
            var language = await _languageService.GetActiveByIdAsync(model.SelectedLanguageId);

            var currentPage = await _pageService.GetByHeaderAndLanguageAsync(
                    model.HeaderId, language.Id);

            if (ModelState.IsValid)
            {
                var page = model.Page;
                page.LanguageId = language.Id;
                page.PageHeaderId = model.HeaderId;

                if (currentPage == null)
                {
                    await _pageService.AddPageAsync(page);

                    ShowAlertSuccess("Page content added!");
                }
                else
                {
                    await _pageService.EditPageAsync(page);

                    ShowAlertSuccess("Page content updated!");
                }

                return RedirectToAction(nameof(Detail), new
                {
                    id = model.HeaderId,
                    language = language.Name
                });
            }

            var header = await _pageService.GetHeaderByIdAsync(model.HeaderId);
            var languages = await _languageService.GetActiveAsync();

            model.HeaderName = header.PageName;
            model.HeaderStub = header.Stub;
            model.LanguageList = new SelectList(languages, "Name", "Description", language.Name);

            if (currentPage?.IsPublished == true)
            {
                var baseUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
                var action = Url.Action("Index", "Info", new
                {
                    area = "",
                    culture = language.Name,
                    id = header.Stub
                });

                model.PageUrl = baseUrl + action;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePage(PageDetailViewModel model)
        {
            var page = await _pageService.GetByHeaderAndLanguageAsync(model.HeaderId,
                model.SelectedLanguageId);

            await _pageService.DeletePageAsync(page.Id);

            var language = await _languageService.GetActiveByIdAsync(model.SelectedLanguageId);

            ShowAlertSuccess($"{language.Description} page removed!");
            return RedirectToAction(nameof(Detail),
                new
                {
                    id = model.HeaderId,
                    language = language.Name
                });
        }

        public async Task<IActionResult> Preview(int headerId, int languageId)
        {
            try
            {
                var page = await _pageService.GetByHeaderAndLanguageAsync(headerId, languageId);
                var language = await _languageService.GetActiveByIdAsync(languageId);

                PageTitle = $"Preview - {page.Title}";

                var viewModel = new PagePreviewViewModel
                {
                    HeaderId = headerId,
                    Language = language.Name,
                    Content = CommonMark.CommonMarkConverter.Convert(page.Content),
                    Stub = page.PageStub
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
