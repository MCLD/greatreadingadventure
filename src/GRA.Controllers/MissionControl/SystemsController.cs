using System;
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
        private readonly ILogger<SystemsController> _logger;
        private readonly BranchImportExportService _branchImportExportService;
        private readonly SiteService _siteService;
        private readonly SpatialService _spatialService;
        private readonly JobService _jobService;

        public const string Name = "Systems";

        public SystemsController(ILogger<SystemsController> logger,
            ServiceFacade.Controller context,
            BranchImportExportService branchImportExportService,
            JobService jobService,
            SiteService siteService,
            SpatialService spatialService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _branchImportExportService = branchImportExportService
                ?? throw new ArgumentNullException(nameof(branchImportExportService));
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportViewModel viewModel)
        {
            if (viewModel?.FileUpload == null)
            {
                AlertDanger = $"You must upload a file to import of this type(s): {string.Join(", ", ValidCsvExtensions)}";
                return RedirectToAction(nameof(Import));
            }
            else
            {
                if (!ValidCsvExtensions
                    .Contains(Path.GetExtension(viewModel.FileUpload.FileName).ToUpperInvariant()))
                {
                    AlertDanger = $"File must be one of the following type(s): {string.Join(", ", ValidCsvExtensions)}";
                    return RedirectToAction(nameof(Import));
                }
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Uploaded branch file for import: {Filename} of size {Filesize}",
                    viewModel.FileUpload.FileName,
                    viewModel.FileUpload.Length);

                var tempFile = _pathResolver.ResolvePrivateTempFilePath();

                using (var fileStream = new FileStream(tempFile, FileMode.Create))
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
    }
}
