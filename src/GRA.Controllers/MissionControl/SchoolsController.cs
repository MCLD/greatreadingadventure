using GRA.Controllers.ViewModel.MissionControl.Schools;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            _logger = Require.IsNotNull(logger, nameof(logger));
            _schoolImportService = Require.IsNotNull(schoolImportService,
                nameof(schoolImportService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            PageTitle = "School management";
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var schoolList = await _schoolService.GetPaginatedListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = schoolList.Count,
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

            SchoolsListViewModel viewModel = new SchoolsListViewModel()
            {
                Schools = schoolList.Data.ToList(),
                PaginateModel = paginateModel,
                DistrictList = await _schoolService.GetDistrictsAsync(),
                SchoolTypes = new SelectList(await _schoolService.GetTypesAsync(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSchool(SchoolsListViewModel model)
        {
            try
            {
                await _schoolService.AddSchool(model.School.Name,
                    model.School.SchoolDistrictId,
                    model.School.SchoolTypeId);

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
            return RedirectToAction("Index", new { search = search });
        }

        public async Task<IActionResult> Districts(string search, int page = 1)
        {
            PageTitle = "School Districts";

            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var districtList = await _schoolService.GetPaginatedDistrictListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = districtList.Count,
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

            DistrictListViewModel viewModel = new DistrictListViewModel()
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
                if (model.TypeSelection == 1)
                {
                    model.District.IsPrivate = true;
                    model.District.IsCharter = false;
                }
                else if (model.TypeSelection == 2)
                {
                    model.District.IsPrivate = false;
                    model.District.IsCharter = true;
                }
                else
                {
                    model.District.IsPrivate = false;
                    model.District.IsCharter = false;
                }
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
                if (model.TypeSelection == 1)
                {
                    model.District.IsPrivate = true;
                    model.District.IsCharter = false;
                }
                else if (model.TypeSelection == 2)
                {
                    model.District.IsPrivate = false;
                    model.District.IsCharter = true;
                }
                else
                {
                    model.District.IsPrivate = false;
                    model.District.IsCharter = false;
                }
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
            return RedirectToAction("Districts", new { search = search });
        }

        public async Task<IActionResult> Types(string search, int page = 1)
        {
            PageTitle = "School Types";

            var filter = new BaseFilter(page)
            {
                Search = search
            };

            var typeList = await _schoolService.GetPaginatedTypeListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = typeList.Count,
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

            TypeListViewModel viewModel = new TypeListViewModel()
            {
                SchoolTypes = typeList.Data.ToList(),
                PaginateModel = paginateModel,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddType(TypeListViewModel model)
        {
            try
            {
                await _schoolService.AddSchoolType(model.Type.Name);
                ShowAlertSuccess($"Added School Type '{model.Type.Name}'");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add School Type: ", gex);
            }
            return RedirectToAction("Types", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> EditType(TypeListViewModel model)
        {
            try
            {
                await _schoolService.UpdateTypeAsync(model.Type);
                ShowAlertSuccess($"School Type '{model.Type.Name}' updated");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to edit School Type: ", gex);
            }
            return RedirectToAction("Types", new { search = model.Search });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteType(int id, string search)
        {
            try
            {
                await _schoolService.RemoveType(id);
                AlertSuccess = "School Type removed";
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to delete School Type: ", gex);
            }
            return RedirectToAction("Types", new { search = search });
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
            PageTitle = "Import Events";
            if (schoolFileCsv == null
                || Path.GetExtension(schoolFileCsv.FileName).ToLower() != ".csv")
            {
                AlertDanger = "You must select a .csv file.";
                ModelState.AddModelError("eventFileCsv", "You must select a .csv file.");
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
                        case ImportStatus.Info:
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
