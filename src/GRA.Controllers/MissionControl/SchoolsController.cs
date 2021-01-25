using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Schools;
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
    [Authorize(Policy = Policy.ManageSchools)]
    public class SchoolsController : Base.MCController
    {
        private readonly ILogger<SchoolsController> _logger;
        private readonly SchoolImportService _schoolImportService;
        private readonly SchoolService _schoolService;

        public SchoolsController(ILogger<SchoolsController> logger,
            ServiceFacade.Controller context,
            SchoolImportService schoolImportService,
            SchoolService schoolService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _schoolImportService = schoolImportService 
                ?? throw new ArgumentNullException(nameof(schoolImportService));
            _schoolService = schoolService 
                ?? throw new ArgumentNullException(nameof(schoolService));
            PageTitle = "School management";
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var schoolList = await _schoolService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = schoolList.Count,
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

            var viewModel = new SchoolsListViewModel
            {
                Schools = schoolList.Data.ToList(),
                PaginateModel = paginateModel,
                DistrictList = await _schoolService.GetDistrictsAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSchool(SchoolsListViewModel model)
        {
            try
            {
                await _schoolService.AddSchool(model.School.Name?.Trim(),
                    model.School.SchoolDistrictId);

                ShowAlertSuccess($"Added School '{model.School.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add School: ", gex);
            }
            return RedirectToAction("Index", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> EditSchool(SchoolsListViewModel model)
        {
            try
            {
                model.School.Name = model.School.Name?.Trim();
                await _schoolService.UpdateSchoolAsync(model.School);
                ShowAlertSuccess($"School District '{model.School.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit School: ", gex);
            }
            return RedirectToAction("Index", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSchool(int id, string search)
        {
            try
            {
                await _schoolService.RemoveSchool(id);
                AlertSuccess = "School removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete School: ", gex);
            }
            return RedirectToAction("Index", new { search });
        }

        public async Task<IActionResult> Districts(string search, int page = 1)
        {
            PageTitle = "School Districts";

            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var districtList = await _schoolService.GetPaginatedDistrictListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = districtList.Count,
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

            var viewModel = new DistrictListViewModel
            {
                SchoolDistricts = districtList.Data.ToList(),
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDistrict(DistrictListViewModel model)
        {
            try
            {
                model.District.Name = model.District.Name?.Trim();
                await _schoolService.AddDistrict(model.District);
                ShowAlertSuccess($"Added School District '{model.District.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add School District: ", gex);
            }
            return RedirectToAction("Districts", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> EditDistrict(DistrictListViewModel model)
        {
            try
            {
                model.District.Name = model.District.Name?.Trim();
                await _schoolService.UpdateDistrictAsync(model.District);
                ShowAlertSuccess($"School District '{model.District.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit School District: ", gex);
            }
            return RedirectToAction("Districts", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDistrict(int id, string search)
        {
            try
            {
                await _schoolService.RemoveDistrict(id);
                AlertSuccess = "School District removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete School District: ", gex);
            }
            return RedirectToAction("Districts", new { search });
        }

        [HttpGet]
        public IActionResult Import()
        {
            PageTitle = "Import Schools";
            return View("Import");
        }

        [HttpPost]
        public async Task<IActionResult> Import(Microsoft.AspNetCore.Http.IFormFile schoolFileCsv)
        {
            PageTitle = "Import Schools";
            if (schoolFileCsv == null
                || !string.Equals(Path.GetExtension(schoolFileCsv.FileName), ".csv",
                    System.StringComparison.OrdinalIgnoreCase))
            {
                AlertDanger = "You must select a .csv file.";
                ModelState.AddModelError("schoolFileCsv", "You must select a .csv file.");
            }

            if (ModelState.ErrorCount == 0)
            {
                using (var streamReader = new StreamReader(schoolFileCsv.OpenReadStream()))
                {
                    (ImportStatus status, string message)
                        = await _schoolImportService.FromCsvAsync(streamReader);

                    switch (status)
                    {
                        case ImportStatus.Success:
                            AlertSuccess = message;
                            break;
                        default:
                            AlertInfo = message;
                            break;
                        case ImportStatus.Warning:
                            AlertWarning = message;
                            break;
                        case ImportStatus.Danger:
                            AlertDanger = message;
                            break;
                    }
                }
            }
            return RedirectToAction("Import");
        }
    }
}
