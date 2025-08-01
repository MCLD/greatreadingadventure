using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageSystems)]
    public class SystemsController : Base.MCController
    {
        private readonly BranchImportExportService _branchImportExportService;
        private readonly JobService _jobService;
        private readonly ILogger<SystemsController> _logger;
        private readonly SiteService _siteService;
        private readonly SpatialService _spatialService;
        private readonly UserService _userService;

        public SystemsController(BranchImportExportService branchImportExportService,
            ILogger<SystemsController> logger,
            JobService jobService,
            ServiceFacade.Controller context,
            SiteService siteService,
            SpatialService spatialService,
            UserService userService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(branchImportExportService);
            ArgumentNullException.ThrowIfNull(jobService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(spatialService);
            ArgumentNullException.ThrowIfNull(userService);

            _branchImportExportService = branchImportExportService;
            _jobService = jobService;
            _logger = logger;
            _siteService = siteService;
            _spatialService = spatialService;
            _userService = userService;

            PageTitle = "System & branch management";
        }

        public static string Name
        { get { return "Systems"; } }

        [HttpPost]
        public async Task<IActionResult> AddBranch(BranchesListViewModel model)
        {
            if (model != null)
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
            }
            return RedirectToAction("Branches", new { search = model?.Search });
        }

        [HttpPost]
        public async Task<IActionResult> AddSystem(SystemListViewModel model)
        {
            if (model != null)
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
            }
            return RedirectToAction("Index", new { search = model?.Search });
        }

        [HttpGet]
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

            (viewModel.ShowGeolocation, viewModel.GoogleMapsAPIKey)
                = await _siteLookupService.GetSiteSettingStringAsync(GetCurrentSiteId(),
                    SiteSettingKey.Events.GoogleMapsAPIKey);

            return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> EditBranch(BranchesListViewModel model)
        {
            if (model != null)
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
                                ShowAlertWarning("Unable to set branch geolocation: ",
                                    result.Message);
                            }
                            else
                            {
                                ShowAlertDanger("Unable to set branch geolocation: ",
                                    result.Message);
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
            }
            return RedirectToAction("Branches", new { search = model?.Search });
        }

        [HttpPost]
        public async Task<IActionResult> EditSystem(SystemListViewModel model)
        {
            if (model != null)
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
            }
            return RedirectToAction("Index", new { search = model?.Search });
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            string date = _dateTimeProvider.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            return File(await _branchImportExportService.ToCsvAsync(),
                "text/csv",
                Utility.FileUtility.EnsureValidFilename($"{date}-Branches.csv"));
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View(new ImportViewModel { SiteId = GetCurrentSiteId() });
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportViewModel viewModel)
        {
            if (viewModel?.FileUpload == null)
            {
                AlertDanger = $"You must upload a file to import of this type(s): {string.Join(", ", ValidFiles.CsvExtensions)}";
                return RedirectToAction(nameof(Import));
            }
            else
            {
                if (!ValidFiles.CsvExtensions
                        .Contains(Path.GetExtension(viewModel.FileUpload.FileName),
                            StringComparer.OrdinalIgnoreCase))
                {
                    AlertDanger = $"File must be one of the following type(s): {string.Join(", ", ValidFiles.CsvExtensions)}";
                    return RedirectToAction(nameof(Import));
                }
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Uploaded branch file for import: {Filename} of size {Filesize}",
                    viewModel.FileUpload.FileName,
                    viewModel.FileUpload.Length);

                var tempFile = _pathResolver.ResolvePrivateTempFilePath();

                await using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    await viewModel.FileUpload.CopyToAsync(fileStream);
                }

                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));

                var jobModel = new JobBranchImport
                {
                    DoImport = viewModel.DoImport,
                    Filename = file,
                    UserId = GetActiveUserId()
                };

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.BranchImport,
                    SerializedParameters = JsonConvert.SerializeObject(jobModel)
                });

                _logger.LogDebug("Redirecting to {ImportType} {Filename} (as temp file {TemporaryFilename})",
                    viewModel.DoImport ? "import" : "test run",
                    viewModel.FileUpload.FileName,
                    tempFile);

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(Import)),
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(Index)),
                    Title = "Loading import..."
                });
            }
            else
            {
                return View(viewModel);
            }
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> RemoveBranch(int id)
        {
            var branchList = new Dictionary<string, int>();
            foreach (var system in await _siteService.GetSystemList())
            {
                foreach (var branch in await _siteService.GetBranches(system.Id))
                {
                    if (branch.Id != id)
                    {
                        branchList.Add($"{system.Name} - {branch.Name}", branch.Id);
                    }
                }
            }

            return View(new RemoveBranchViewModel
            {
                Branch = await _siteService.GetBranchByIdAsync(id),
                BranchList = new SelectList(branchList, "Value", "Key"),
                InUseCount = await _siteService.GetBranchInUseAsync(id),
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBranch(RemoveBranchViewModel removeBranchViewModel)
        {
            int reassigned = 0;
            string newBranchName = null;
            try
            {
                if (removeBranchViewModel?.ReassignBranch.HasValue == true)
                {
                    reassigned = await _userService.ReassignBranchAsync(
                        removeBranchViewModel.BranchId,
                        removeBranchViewModel.ReassignBranch.Value);
                    newBranchName = await _siteService
                        .GetBranchName(removeBranchViewModel.ReassignBranch.Value);
                }
                await _siteService.RemoveBranchAsync(removeBranchViewModel.BranchId);
                AlertSuccess = reassigned > 0
                    ? $"Branch removed, {reassigned} users reassigned to {newBranchName}"
                    : "Branch removed";
            }
            catch (GraException gex)
            {
                if (reassigned > 0)
                {
                    Exception reportingException = gex;
                    while (reportingException.InnerException != null)
                    {
                        reportingException = reportingException.InnerException;
                    }
                    ShowAlertDanger($"{reassigned} users reassigned to {newBranchName} but branch could not be removed: {reportingException.Message}");
                }
                else
                {
                    if (gex.InnerException != null)
                    {
                        ShowAlertDanger("There was an error removing the branch: ",
                            gex.InnerException.Message);
                    }
                    else
                    {
                        ShowAlertDanger("There was an error removing the branch: ", gex);
                    }
                }
            }
            return RedirectToAction(nameof(Branches));
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
