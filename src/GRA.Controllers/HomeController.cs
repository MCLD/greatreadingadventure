using GRA.Controllers.ViewModel;
using GRA.Controllers.ViewModel.Home;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class HomeController : Base.UserController
    {
        private const string AuthorMissingTitle = "AuthorMissingTitle";

        private readonly ILogger<HomeController> _logger;
        private readonly ActivityService _activityService;
        private readonly EmailReminderService _emailReminderService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        public HomeController(ILogger<HomeController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            EmailReminderService emailReminderService,
            SiteService siteService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _emailReminderService = Require.IsNotNull(emailReminderService,
                nameof(emailReminderService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
        }

        public async Task<IActionResult> Index(string sitePath = null)
        {
            var site = await GetCurrentSiteAsync(sitePath);
            if (site != null)
            {
                PageTitle = site.Name;
            }
            if (AuthUser.Identity.IsAuthenticated)
            {
                // signed-in users can view the dashboard
                var user = await _userService.GetDetails(GetActiveUserId());
                DashboardViewModel viewModel = new DashboardViewModel()
                {
                    FirstName = user.FirstName,
                    CurrentPointTotal = user.PointsEarned
                };
                if (TempData.ContainsKey(AuthorMissingTitle))
                {
                    viewModel.Author = (string)TempData[AuthorMissingTitle];
                    ModelState.AddModelError("Title", "Please include the Title of the book");
                }

                return View("Dashboard", viewModel);
            }
            else
            {
                // determine the appropriate page to show and show it
                if (site.AccessClosed != null && DateTime.Now >= site.AccessClosed)
                {
                    if (site.AccessClosedPage != null)
                    {
                        // show custom access closed page
                        return View("IndexAccessClosed");
                    }
                    else
                    {
                        return View("IndexAccessClosed");
                    }
                }
                else if (site.ProgramEnds != null && DateTime.Now >= site.ProgramEnds)
                {
                    if (site.ProgramEndedPage != null)
                    {
                        //show custom program ended page
                        return View("IndexProgramEnded");
                    }
                    else
                    {
                        return View("IndexProgramEnded");
                    }
                }
                else if (site.ProgramStarts != null && DateTime.Now > site.ProgramStarts)
                {
                    if (site.ProgramOpenPage != null)
                    {
                        // show custom program open page
                        return View("IndexProgramOpen");
                    }
                    else
                    {
                        return View("IndexProgramOpen");
                    }
                }
                else if (site.RegistrationOpens != null && DateTime.Now > site.RegistrationOpens)
                {
                    if (site.RegistrationOpenPage != null)
                    {
                        // show custom registration open page
                        return View("IndexRegistrationOpen");
                    }
                    else
                    {
                        return View("IndexRegistrationOpen");
                    }
                }
                else
                {
                    if (site.BeforeRegistrationPage != null)
                    {
                        // show custom before registration page
                        return View("IndexNotOpenYet");
                    }
                    else
                    {
                        var viewModel = new NotOpenYetViewModel
                        {
                            Site = await GetCurrentSiteAsync(),
                            SignUpSource = "NotOpenYet"
                        };
                        if (viewModel.Site != null && viewModel.Site.RegistrationOpens != null)
                        {
                            viewModel.RegistrationOpens
                                = ((DateTime)viewModel.Site.RegistrationOpens).ToString("D");
                        }
                        return View("IndexNotOpenYet", viewModel);
                    }
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReminder(NotOpenYetViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Email))
            {
                await _emailReminderService
                    .AddEmailReminderAsync(viewModel.Email, viewModel.SignUpSource);
            }
            AlertInfo = "<span class=\"fa fa-envelope\"></span> Thanks! We'll let you know when you can join the program.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> GetIcs()
        {
            var site = await GetCurrentSiteAsync();
            var filename = new string(site.Name.Where(_ => char.IsLetterOrDigit(_)).ToArray());
            var siteUrl = await _siteService.GetBaseUrl(Request.Scheme, Request.Host.Value);
            var calendarBytes = await _siteService.GetIcsFile(siteUrl);
            return File(calendarBytes, "text/calendar", $"{filename}.ics");
        }

        public async Task<IActionResult> Signout()
        {
            if (AuthUser.Identity.IsAuthenticated)
            {
                await LogoutUserAsync();
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> LogBook(DashboardViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Title)
                && !string.IsNullOrWhiteSpace(viewModel.Author))
            {
                TempData[AuthorMissingTitle] = viewModel.Author;
            }
            else
            {
                var book = new Domain.Model.Book
                {
                    Author = viewModel.Author,
                    Title = viewModel.Title
                };
                await _activityService.LogActivityAsync(GetActiveUserId(), 1, book);
            }
            return RedirectToAction("Index");
        }
    }
}