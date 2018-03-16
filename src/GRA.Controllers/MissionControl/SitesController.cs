using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Sites;
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
    [Authorize(Policy = Policy.ManageSites)]
    public class SitesController : Base.MCController
    {
        private readonly ILogger<SitesController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly SiteService _siteService;
        public SitesController(ILogger<SitesController> logger,
            ServiceFacade.Controller context,
            SiteService siteService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = context.Mapper ?? throw new ArgumentNullException(nameof(context.Mapper));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            PageTitle = "Site management";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter(page);

            var siteList = await _siteService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = siteList.Count,
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

            var viewModel = new SiteListViewModel()
            {
                Sites = siteList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
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
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    _mapper.Map<SiteDetailViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Detail), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            PageTitle = PageTitle = $"Site management - {model.Name}";
            return View(model);
        }

        public async Task<IActionResult> Configuration(int id)
        {
            var site = await _siteLookupService.GetByIdAsync(id);
            var viewModel = _mapper.Map<Site, SiteConfigurationViewModel>(site);

            PageTitle = $"Site management - {site.Name}";
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Configuration(SiteConfigurationViewModel model)
        {
            string siteName = null;
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    siteName = site.Name;
                    _mapper.Map<SiteConfigurationViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Configuration), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                var site = await _siteLookupService.GetByIdAsync(model.Id);
                siteName = site.Name;
            }
            PageTitle = PageTitle = $"Site management - {siteName}";
            return View(model);
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
            string siteName = null;
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    siteName = site.Name;
                    _mapper.Map<SiteScheduleViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(Schedule), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                var site = await _siteLookupService.GetByIdAsync(model.Id);
                siteName = site.Name;
            }
            PageTitle = PageTitle = $"Site management - {siteName}";
            return View(model);
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
            string siteName = null;
            if (ModelState.IsValid)
            {
                try
                {
                    var site = await _siteLookupService.GetByIdAsync(model.Id);
                    siteName = site.Name;
                    _mapper.Map<SiteSocialMediaViewModel, Site>(model, site);

                    await _siteService.UpdateAsync(site);
                    ShowAlertSuccess($"Site '{site.Name}' successfully updated!");
                    return RedirectToAction(nameof(SocialMedia), new { id = site.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update site: ", gex);
                }
            }
            if (string.IsNullOrWhiteSpace(siteName))
            {
                var site = await _siteLookupService.GetByIdAsync(model.Id);
                siteName = site.Name;
            }
            PageTitle = PageTitle = $"Site management - {siteName}";
            return View(model);
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
                        SiteSetting = site.Settings.Where(s => s.Key == i.Key).SingleOrDefault() 
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

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SiteSettingsViewModel model)
        {
            var site = await _siteLookupService.GetByIdAsync(model.Id);

            var siteSettings = model.SiteSettingGroups
                .SelectMany(_ => _.SettingInformations.Select(s => s.SiteSetting))
                .Where(_ => !string.IsNullOrWhiteSpace(_.Value))
                .GroupBy(_ => _.Key)
                .Select(_ => _.First());

            var settingKeys = SiteSettingDefinitions.DefinitionDictionary.Keys.ToList();
            var invalidKeys = siteSettings
                .Where(_ => settingKeys.Contains(_.Key) == false)
                .Select(_ => _.Key);
            if (invalidKeys.Any())
            {
                var keysString = string.Join(", ", invalidKeys);
                _logger.LogError($"Invalid site setting key(s): {keysString}");
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
                    else if (definition.Format == SiteSettingFormat.Integer)
                    {
                        if (int.TryParse(siteSetting.Value, out int value) == false)
                        {
                            ModelState.AddModelError("", $"Please enter a whole number for {definition.Name}.");
                        }
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
                        SiteSetting = siteSettings.Where(s => s.Key == i.Key).SingleOrDefault()
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
    }
}
