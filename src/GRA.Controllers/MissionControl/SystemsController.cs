﻿using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Systems;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSystems)]
    public class SystemsController : Base.MCController
    {
        private readonly ILogger<SystemsController> _logger;
        private readonly SiteService _siteService;
        private readonly SpatialService _spatialService;

        public SystemsController(ILogger<SystemsController> logger,
            ServiceFacade.Controller context,
            SiteService siteService,
            SpatialService spatialService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _spatialService = spatialService
                ?? throw new ArgumentNullException(nameof(spatialService));
            PageTitle = "System & branch management";
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var systemList = await _siteService.GetPaginatedSystemListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = systemList.Count,
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

            var viewModel = new SystemListViewModel
            {
                Systems = systemList.Data.ToList(),
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSystem(SystemListViewModel model)
        {
            try
            {
                await _siteService.AddSystemAsync(model.System);
                ShowAlertSuccess($"Added System '{model.System.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add System: ", gex);
            }
            return RedirectToAction("Index", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> EditSystem(SystemListViewModel model)
        {
            try
            {
                await _siteService.UpdateSystemAsync(model.System);
                ShowAlertSuccess($"System  '{model.System.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit System: ", gex);
            }
            return RedirectToAction("Index", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSystem(int id, string search)
        {
            try
            {
                await _siteService.RemoveSystemAsync(id);
                AlertSuccess = "System removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete System: ", gex);
            }
            return RedirectToAction("Index", new { search });
        }

        public async Task<IActionResult> Branches(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var branchList = await _siteService.GetPaginatedBranchListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = branchList.Count,
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

            var viewModel = new BranchesListViewModel
            {
                Branches = branchList.Data.ToList(),
                PaginateModel = paginateModel,
                ShowGeolocation = await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey),
                SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name")
            };

            var (IsSet, SetValue) = await _siteLookupService.GetSiteSettingStringAsync(
                GetCurrentSiteId(), SiteSettingKey.Events.GoogleMapsAPIKey);
            viewModel.ShowGeolocation = IsSet;
            viewModel.GoogleMapsAPIKey = SetValue;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddBranch(BranchesListViewModel model)
        {
            try
            {
                model.Branch.Geolocation = null;
                var branch = await _siteService.AddBranchAsync(model.Branch);
                ShowAlertSuccess($"Added Branch '{branch.Name}'");

                if (await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey))
                {
                    var result = await _spatialService
                        .GetGeocodedAddressAsync(branch.Address);
                    if (result.Status == ServiceResultStatus.Success)
                    {
                        branch.Geolocation = result.Data;
                        await _siteService.UpdateBranchAsync(branch);
                    }
                    else if (result.Status == ServiceResultStatus.Warning)
                    {
                        ShowAlertWarning("Unable to set branch geolocation: ", result.Message);
                    }
                    else
                    {
                        ShowAlertDanger("Unable to set branch geolocation: ", result.Message);
                    }
                }
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add Branch: ", gex);
            }
            return RedirectToAction("Branches", new
            {
                search = model.Search
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditBranch(BranchesListViewModel model)
        {
            try
            {
                if (await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey))
                {
                    model.Branch.Geolocation = null;
                    var newAddress = model.Branch.Address?.Trim();
                    var currentBranch = await _siteService.GetBranchByIdAsync(model.Branch.Id);
                    if (string.IsNullOrWhiteSpace(currentBranch.Geolocation)
                        || !string.Equals(currentBranch.Address, newAddress,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        var result = await _spatialService
                            .GetGeocodedAddressAsync(newAddress);
                        if (result.Status == ServiceResultStatus.Success)
                        {
                            model.Branch.Geolocation = result.Data;
                        }
                        else if (result.Status == ServiceResultStatus.Warning)
                        {
                            ShowAlertWarning("Unable to set branch geolocation: ", result.Message);
                        }
                        else
                        {
                            ShowAlertDanger("Unable to set branch geolocation: ", result.Message);
                        }
                    }
                }

                await _siteService.UpdateBranchAsync(model.Branch);
                ShowAlertSuccess($"Branch  '{model.Branch.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit Branch: ", gex);
            }
            return RedirectToAction("Branches", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBranch(int id, string search)
        {
            try
            {
                await _siteService.RemoveBranchAsync(id);
                AlertSuccess = "Branch removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete Branch: ", gex);
            }
            return RedirectToAction("Branches", new { search });
        }

        [HttpPost]
        public async Task<JsonResult> SetBranchGeolocation(int id)
        {
            var success = false;
            var message = string.Empty;

            if (await _siteLookupService.IsSiteSettingSetAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey))
            {
                var branch = await _siteService.GetBranchByIdAsync(id);
                if (string.IsNullOrEmpty(branch.Geolocation))
                {
                    var result = await _spatialService
                            .GetGeocodedAddressAsync(branch.Address);
                    if (result.Status == ServiceResultStatus.Success)
                    {
                        branch.Geolocation = result.Data;
                        await _siteService.UpdateBranchAsync(branch);

                        success = true;
                    }
                    else
                    {
                        message = result.Message;
                    }
                }
                else
                {
                    message = "Geolocation is already set.";
                }
            }
            else
            {
                message = "Geolocation is not set up.";
            }

            return Json(new
            {
                success,
                message
            });
        }
    }
}
