using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Sites;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSites)]
    public partial class SitesController : Base.MCController
    {
        private readonly EmailService _emailSerivce;
        private readonly ILogger<SitesController> _logger;
        private readonly MapsterMapper.IMapper _mapper;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public SitesController(ILogger<SitesController> logger,
            ServiceFacade.Controller context,
            EmailService emailService,
            SiteService siteService,
            UserService userService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(emailService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(userService);

            _logger = logger;
            _mapper = context.Mapper;
            _emailSerivce = emailService;
            _siteService = siteService;
            _userService = userService;

            PageTitle = "Site management";
        }

        public static string Area
        { get { return "MissionControl"; } }

        public static string Name
        { get { return "Sites"; } }

        public async Task<IActionResult> Configuration(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteConfigurationViewModel>(site);

            var user = await _userService.GetDetails(GetActiveUserId());
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                viewModel.CurrentUserMail = user.Email;
            }

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Configuration(SiteConfigurationViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await _siteLookupService.GetByIdAsync(model.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    _mapper.Map(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Configuration), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            PageTitle = $"Site management - {site.Name}";
            return View(model);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteDetailViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(SiteDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    _mapper.Map(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Detail), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            PageTitle = $"Site management - {model.Name}";
            return View(model);
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter(page);

            var siteList = await _siteService.GetPaginatedListAsync(filter);

            if (siteList.Count == 1)
            {
                return RedirectToAction("Detail", new { id = siteList.Data.First().Id });
            }

            var paginateModel = new PaginateViewModel
            {
                ItemCount = siteList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new SiteListViewModel()
            {
                Sites = siteList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Schedule(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteScheduleViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Schedule(SiteScheduleViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await _siteLookupService.GetByIdAsync(model.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    _mapper.Map(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Schedule), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            PageTitle = $"Site management - {site.Name}";
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> SendTestEmail(string emailAddress)
        {
            try
            {
                var site = await GetCurrentSiteAsync();
                var link = await _siteLookupService.GetSiteLinkAsync(site.Id);

                var details = new DirectEmailDetails(site.Name)
                {
                    DirectEmailSystemId = "TestMessage",
                    IsTest = true,
                    SendingUserId = GetActiveUserId(),
                    ToAddress = emailAddress,
                };

                details.Tags.Add("Sitelink", link.ToString());

                var result = await _emailSerivce.SendDirectAsync(details);

                return Json(new
                {
                    success = result.Successful,
                    message = result.SentResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email: {Message}", ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Settings(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            PageTitle = $"Site management - {site.Name}";

            var settingGroups = SiteSettingDefinitions.DefinitionDictionary
                .GroupBy(_ => _.Value.Category)
                .Select(_ => new SiteSettingGroup()
                {
                    Name = _.Key,
                    SettingInformations = _.Select(i => new SiteSettingInformation()
                    {
                        SiteSetting = site.Settings.SingleOrDefault(s => s.Key == i.Key)
                            ?? new SiteSetting(),
                        Definition = i.Value,
                        Key = i.Key
                    }).ToList()
                })
                .OrderBy(_ => _.Name)
                .ToList();

            var viewModel = new SiteSettingsViewModel()
            {
                Id = site.Id,
                SiteSettingGroups = settingGroups
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SiteSettingsViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await _siteLookupService.GetByIdAsync(model.Id);

            var siteSettings = model.SiteSettingGroups
                .SelectMany(_ => _.SettingInformations.Select(s => s.SiteSetting))
                .Where(_ => !string.IsNullOrWhiteSpace(_.Value))
                .GroupBy(_ => _.Key)
                .Select(_ => _.First());

            var settingKeys = SiteSettingDefinitions.DefinitionDictionary.Keys.ToList();
            var invalidKeys = siteSettings
                .Where(_ => !settingKeys.Contains(_.Key))
                .Select(_ => _.Key);
            if (invalidKeys.Any())
            {
                var keysString = string.Join(", ", invalidKeys);
                _logger.LogError("Invalid site setting key(s): {Keys}", keysString);
                ShowAlertDanger("Invalid site setting.");
                return RedirectToAction(nameof(Settings), new { id = site.Id });
            }

            foreach (var siteSetting in siteSettings)
            {
                siteSetting.SiteId = site.Id;

                if (!string.IsNullOrWhiteSpace(siteSetting.Value))
                {
                    var definition = SiteSettingDefinitions.DefinitionDictionary[siteSetting.Key];
                    if (definition.Format == SiteSettingFormat.Boolean)
                    {
                        siteSetting.Value = "True";
                    }
                    else if (definition.Format == SiteSettingFormat.Integer
                        && !int.TryParse(siteSetting.Value, out int value))
                    {
                        ModelState.AddModelError("", $"Please enter a whole number for {definition.Name}.");
                    }
                    else if (definition.Format == SiteSettingFormat.IntegerCsv && !CsvRegex().IsMatch(siteSetting.Value))
                    {
                        ModelState.AddModelError("", $"Please enter only numbers, commas, and spaces for {definition.Name}.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                await _siteService.UpdateSiteSettingsAsync(site.Id, siteSettings);
                ShowAlertSuccess($"Site '{site.Name}' settings successfully updated!");
                return RedirectToAction(nameof(Settings), new { id = site.Id });
            }

            var settingGroups = SiteSettingDefinitions.DefinitionDictionary
                .GroupBy(_ => _.Value.Category)
                .Select(_ => new SiteSettingGroup()
                {
                    Name = _.Key,
                    SettingInformations = _.Select(i => new SiteSettingInformation()
                    {
                        SiteSetting = siteSettings.SingleOrDefault(s => s.Key == i.Key)
                            ?? new SiteSetting(),
                        Definition = i.Value,
                        Key = i.Key
                    }).ToList()
                })
                .ToList();

            var viewModel = new SiteSettingsViewModel()
            {
                Id = site.Id,
                SiteSettingGroups = settingGroups
            };
            PageTitle = $"Site management - {site.Name}";

            return View(viewModel);
        }

        public async Task<IActionResult> SocialMedia(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteSocialMediaViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SocialMedia(SiteSocialMediaViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await _siteLookupService.GetByIdAsync(model.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    _mapper.Map(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(SocialMedia), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            PageTitle = $"Site management - {site.Name}";
            return View(model);
        }

        [GeneratedRegex("^[0-9, ]*$")]
        private static partial Regex CsvRegex();
    }
}
