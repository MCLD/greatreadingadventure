using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Drawing;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.PerformDrawing)]
    public class DrawingController : Base.MCController
    {
        private readonly DrawingService _drawingService;
        private readonly PrizeWinnerService _prizeWinnerService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public DrawingController(ServiceFacade.Controller context,
            DrawingService drawingService,
            PrizeWinnerService prizeWinnerService,
            SiteService siteService,
            UserService userService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(drawingService);
            ArgumentNullException.ThrowIfNull(prizeWinnerService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(userService);

            _drawingService = drawingService;
            _prizeWinnerService = prizeWinnerService;
            _siteService = siteService;
            _userService = userService;

            PageTitle = "Drawing";
        }

        public static string Name
        { get { return "Drawing"; } }

        public async Task<IActionResult> Criteria(string search,
            int? systemId,
            int? branchId,
            bool? mine,
            int? programId,
            int page)
        {
            PageTitle = "Drawing Criteria";

            page = page > 0 ? page : 1;

            var filter = new BaseFilter(page);

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter.Search = search;
            }

            if (mine == true)
            {
                filter.UserIds = new List<int> { GetId(ClaimType.UserId) };
            }
            else if (branchId.HasValue)
            {
                filter.BranchIds = new List<int> { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                filter.SystemIds = new List<int> { systemId.Value };
            }

            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    filter.ProgramIds = new List<int?> { programId.Value };
                }
                else
                {
                    filter.ProgramIds = new List<int?> { null };
                }
            }

            var criterionList = await _drawingService.GetPaginatedCriterionListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = criterionList.Count,
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

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            var viewModel = new CriterionListViewModel
            {
                Criteria = criterionList.Data,
                PaginateModel = paginateModel,
                Search = search,
                SystemId = systemId,
                BranchId = branchId,
                ProgramId = programId,
                Mine = mine,
                SystemList = systemList,
                ProgramList = await _siteService.GetProgramList()
            };
            if (mine == true)
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Mine";
            }
            else if (branchId.HasValue)
            {
                var branch = await _siteService.GetBranchByIdAsync(branchId.Value);
                viewModel.BranchName = branch.Name;
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == branch.SystemId).Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == systemId.Value).Name;
                viewModel.BranchList = (await _siteService.GetBranches(systemId.Value))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "System";
            }
            else
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "All";
            }
            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    viewModel.ProgramName =
                        (await _siteService.GetProgramByIdAsync(programId.Value)).Name;
                }
                else
                {
                    viewModel.ProgramName = "Not Limited";
                }
            }

            return View(viewModel);
        }

        public async Task<IActionResult> CriteriaCreate()
        {
            PageTitle = "Drawing Criteria";

            var viewModel = new CriterionDetailViewModel
            {
                SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name"),
                BranchList = new SelectList(await _siteService.GetAllBranches(), "Id", "Name"),
                ProgramList = new SelectList(await _siteService.GetProgramList(), "Id", "Name"),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CriteriaCreate(CriterionDetailViewModel model,
            string Drawing)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                if (model.Criterion.ProgramIds?.Count == 1)
                {
                    model.Criterion.ProgramId = model.Criterion.ProgramIds.First();
                    model.Criterion.ProgramIds = null;
                }
                else
                {
                    model.Criterion.ProgramId = null;
                }

                var criterion = await _drawingService.AddCriterionAsync(model.Criterion);
                AlertSuccess = ($"Criteria <strong>{criterion.Name}</strong> created");
                if (string.IsNullOrWhiteSpace(Drawing))
                {
                    return RedirectToAction("CriteriaDetail", new { id = criterion.Id });
                }
                else
                {
                    return RedirectToAction("New", new { id = criterion.Id });
                }
            }
            else
            {
                PageTitle = "Drawing Criteria";
                model.SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name");
                if (model.Criterion.SystemId.HasValue)
                {
                    model.BranchList = new SelectList(
                        await _siteService
                            .GetBranches(model.Criterion.SystemId.Value), "Id", "Name");
                }
                else
                {
                    model.BranchList = new SelectList(await _siteService
                        .GetAllBranches(), "Id", "Name");
                }
                model.ProgramList = new SelectList(await _siteService
                    .GetProgramList(), "Id", "Name");
                return View(model);
            }
        }

        public async Task<IActionResult> CriteriaDetail(int id)
        {
            try
            {
                PageTitle = "Drawing Criteria";
                var criterion = await _drawingService.GetCriterionDetailsAsync(id);
                if (criterion.ProgramId.HasValue)
                {
                    criterion.ProgramIds.Add(criterion.ProgramId.Value);
                }
                var site = await GetCurrentSiteAsync();
                var programs = await _siteService.GetProgramList();
                var viewModel = new CriterionDetailViewModel()
                {
                    Criterion = criterion,
                    CreatedByName = await _userService.GetUsersNameByIdAsync(criterion.CreatedBy),
                    CanViewParticipants = UserHasPermission(Permission.ViewParticipantDetails),
                    SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name"),
                    ProgramList = new SelectList(programs, "Id", "Name"),
                    EligibleCount = await _drawingService.GetEligibleCountAsync(id),
                    ProgramPlaceholder = string.Join(", ", programs
                        .Where(_ => criterion.ProgramIds.Contains(_.Id))
                        .Select(_ => _.Name))
                };

                if (viewModel.Criterion.SystemId.HasValue)
                {
                    viewModel.BranchList = new SelectList(
                        await _siteService
                            .GetBranches(viewModel.Criterion.SystemId.Value), "Id", "Name");
                }
                else
                {
                    viewModel.BranchList = new SelectList(await _siteService
                        .GetAllBranches(), "Id", "Name");
                }
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view criteria: ", gex);
                return RedirectToAction("Criteria");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CriteriaDetail(CriterionDetailViewModel model,
            string Drawing)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                if (model.Criterion.ProgramIds?.Count == 1)
                {
                    model.Criterion.ProgramId = model.Criterion.ProgramIds.First();
                    model.Criterion.ProgramIds = null;
                }
                else
                {
                    model.Criterion.ProgramId = null;
                }

                var criterion = await _drawingService.EditCriterionAsync(model.Criterion);
                AlertSuccess = ($"Criteria <strong>{criterion.Name}</strong> saved");
                if (string.IsNullOrWhiteSpace(Drawing))
                {
                    return RedirectToAction("CriteriaDetail", new { id = criterion.Id });
                }
                else
                {
                    return RedirectToAction("New", new { id = criterion.Id });
                }
            }
            else
            {
                PageTitle = "Drawing Criteria";
                model.SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name");
                if (model.Criterion.SystemId.HasValue)
                {
                    model.BranchList = new SelectList(
                        await _siteService
                            .GetBranches(model.Criterion.SystemId.Value), "Id", "Name");
                }
                else
                {
                    model.BranchList = new SelectList(await _siteService
                        .GetAllBranches(), "Id", "Name");
                }
                model.ProgramList = new SelectList(await _siteService
                    .GetProgramList(), "Id", "Name");
                return View(model);
            }
        }

        public async Task<IActionResult> Detail(int id, int page)
        {
            page = page > 0 ? page : 1;
            const int take = 15;
            int skip = take * (page - 1);

            try
            {
                var drawing = await _drawingService.GetDetailsAsync(id, skip, take);

                var paginateModel = new PaginateViewModel()
                {
                    ItemCount = drawing.Count,
                    CurrentPage = page,
                    ItemsPerPage = take
                };

                if (paginateModel.PastMaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            id,
                            page = paginateModel.LastPage ?? 1
                        });
                }

                var viewModel = new DrawingDetailViewModel
                {
                    Drawing = drawing.Data,
                    CreatedByName = await _userService
                        .GetUsersNameByIdAsync(drawing.Data.CreatedBy),
                    CanViewParticipants = UserHasPermission(Permission.ViewParticipantDetails),
                    PaginateModel = paginateModel
                };

                if (UserHasPermission(Permission.MailParticipants)
                    && !drawing.Data.NotificationSent
                    && drawing.Data.Winners.Any())
                {
                    viewModel.CanMailWinners = true;
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view drawing: ", gex);
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadWinners(int id)
        {
            Drawing drawing = null;
            try
            {
                drawing = await _drawingService.GetDetailsAsync(id)
                    ?? throw new GraException("Drawing not found.");
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view drawing: ", gex);
                return RedirectToAction(nameof(Index));
            }

            string drawingName = drawing.Name;

            var data = new List<IEnumerable<string>>();

            var ContactDetails = new Dictionary<int, User>();

            foreach (var winner in drawing.Winners)
            {
                User householdHead = null;

                if (winner.UserHouseholdHeadUserId.HasValue)
                {
                    if (ContactDetails.TryGetValue(winner.UserHouseholdHeadUserId.Value,
                        out User value))
                    {
                        householdHead = value;
                    }
                    else
                    {
                        householdHead = await _userService
                            .GetContactDetailsAsync(winner.UserHouseholdHeadUserId.Value);
                        if (householdHead != null)
                        {
                            ContactDetails.Add(winner.UserHouseholdHeadUserId.Value, householdHead);
                        }
                    }
                }

                var row = new[]
                {
                    winner.RedeemedAt.HasValue
                        ? winner.RedeemedAt.ToString()
                        : "Available to redeem",
                    winner.UserUsername,
                    winner.UserFirstName,
                    winner.UserLastName,
                    winner.UserEmail,
                    winner.UserPhoneNumber,
                    winner.UserHouseholdHeadUserId.HasValue
                        ? householdHead?.Username
                        : string.Empty,
                    winner.UserHouseholdHeadUserId.HasValue
                        ? householdHead?.FirstName
                        : string.Empty,
                    winner.UserHouseholdHeadUserId.HasValue
                        ? householdHead?.LastName
                        : string.Empty,
                    winner.UserHouseholdHeadUserId.HasValue
                        ? householdHead?.Email
                        : string.Empty,
                    winner.UserHouseholdHeadUserId.HasValue
                        ? householdHead?.PhoneNumber
                        : string.Empty
                };
                data.Add(row);
            }

            var ms = ExcelExport.GenerateWorkbook(new[] {
                new StoredReport(drawingName, _dateTimeProvider.Now)
                    {
                        Data = data,
                        HeaderRow = new List<string> {
                            "Redeemed?",
                            "Username",
                            "First Name",
                            "Last Name",
                            "Email",
                            "Phone Number",
                            "Household Head Username",
                            "Household Head First Name",
                            "Household Head Last Name",
                            "Household Head Email",
                            "Household Head Phone Number",
                        }
                    }
                },
                new Dictionary<string, object>
                {
                    { "As Of", _dateTimeProvider.Now.ToString(CultureInfo.CurrentCulture) },
                    { "Drawing", drawing.Name },
                },
                "As Of");

            return new FileStreamResult(ms, ExcelExport.ExcelMimeType)
            {
                FileDownloadName = FileUtility
                    .EnsureValidFilename($"Winners-{drawingName}.{ExcelExport.ExcelFileExtension}")
            };
        }

        public async Task<IActionResult> Index(string search,
                    int? systemId,
            int? branchId,
            bool? mine,
            int? programId,
            bool? archived,
            int page)
        {
            page = page > 0 ? page : 1;

            var filter = new DrawingFilter(page);

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter.Search = search;
            }

            if (archived == true)
            {
                filter.Archived = true;
                PageTitle = "Archived Drawings";
            }

            if (mine == true)
            {
                filter.UserIds = new List<int> { GetId(ClaimType.UserId) };
            }
            else if (branchId.HasValue)
            {
                filter.BranchIds = new List<int> { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                filter.SystemIds = new List<int> { systemId.Value };
            }

            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    filter.ProgramIds = new List<int?> { programId.Value };
                }
                else
                {
                    filter.ProgramIds = new List<int?> { null };
                }
            }

            var drawingList = await _drawingService.GetPaginatedDrawingListAsync(filter);

            var paginateModel = new PaginateViewModel()
            {
                ItemCount = drawingList.Count,
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

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            var viewModel = new DrawingListViewModel
            {
                Drawings = drawingList.Data,
                PaginateModel = paginateModel,
                Archived = archived,
                Search = search,
                SystemId = systemId,
                BranchId = branchId,
                ProgramId = programId,
                Mine = mine,
                SystemList = systemList,
                ProgramList = await _siteService.GetProgramList()
            };

            if (mine == true)
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Mine";
            }
            else if (branchId.HasValue)
            {
                var branch = await _siteService.GetBranchByIdAsync(branchId.Value);
                viewModel.BranchName = branch.Name;
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == branch.SystemId).Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == systemId.Value).Name;
                viewModel.BranchList = (await _siteService.GetBranches(systemId.Value))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "System";
            }
            else
            {
                viewModel.BranchList = (await _siteService.GetBranches(GetId(ClaimType.SystemId)))
                        .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                        .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "All";
            }
            if (programId.HasValue)
            {
                if (programId.Value > 0)
                {
                    viewModel.ProgramName =
                        (await _siteService.GetProgramByIdAsync(programId.Value)).Name;
                }
                else
                {
                    viewModel.ProgramName = "Not Limited";
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = Policy.MailParticipants)]
        [HttpPost]
        public async Task<IActionResult> MailWinners(DrawingDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrWhiteSpace(model.Drawing.NotificationSubject)
                || string.IsNullOrWhiteSpace(model.Drawing.NotificationMessage))
            {
                ShowAlertWarning("A subject and message are required to send mail to winners.");
            }
            else
            {
                try
                {
                    await _drawingService.SendWinnerMailAsync(model.Drawing);
                    ShowAlertSuccess("Mail sent to winners.");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to mail drawing winners: ", gex);
                }
            }
            return RedirectToAction(nameof(Detail), new { model.Drawing.Id });
        }

        public async Task<IActionResult> New(int id)
        {
            try
            {
                var drawing = new Drawing()
                {
                    DrawingCriterionId = id,
                    DrawingCriterion = await _drawingService.GetCriterionDetailsAsync(id),
                    WinnerCount = 1
                };
                drawing.DrawingCriterion.EligibleCount
                    = await _drawingService.GetEligibleCountAsync(id);

                var viewModel = new DrawingNewViewModel()
                {
                    Drawing = drawing
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view new drawing: ", gex);
                return RedirectToAction("Criteria");
            }
        }

        [HttpPost]
        public async Task<IActionResult> New(DrawingNewViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model.Drawing.WinnerCount > model.Drawing.DrawingCriterion.EligibleCount)
            {
                ModelState.AddModelError("Drawing.WinnerCount",
                    "Cannot have more Winners than Eligible Participants");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var drawing = await _drawingService.PerformDrawingAsync(model.Drawing);
                    return RedirectToAction("Detail", new { id = drawing.Id });
                }
                catch (GraException gex)
                {
                    AlertInfo = gex.Message;
                    ModelState["DrawingCriterion.EligibleCount"].RawValue = await _drawingService
                        .GetEligibleCountAsync(model.Drawing.DrawingCriterionId);

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RedeemWinner(int prizeWinnerId,
            int drawingId,
            int page)
        {
            page = page > 0 ? page : 1;

            try
            {
                await _prizeWinnerService.RedeemPrizeAsync(prizeWinnerId, null);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to redeem prize: ", gex);
            }
            return RedirectToAction("Detail", new { id = drawingId, page });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveWinner(int prizeWinnerId,
            int drawingId,
            int page)
        {
            page = page > 0 ? page : 1;

            try
            {
                await _prizeWinnerService.RemovePrizeAsync(prizeWinnerId);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete winner: ", gex);
            }
            return RedirectToAction("Detail", new { id = drawingId, page });
        }

        [HttpPost]
        public async Task<IActionResult> UndoRedemption(int prizeWinnerId,
            int drawingId,
            int page)
        {
            page = page > 0 ? page : 1;

            try
            {
                await _prizeWinnerService.UndoRedemptionAsync(prizeWinnerId);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to undo redemption: ", gex);
            }
            return RedirectToAction("Detail", new { id = drawingId, page });
        }
    }
}
