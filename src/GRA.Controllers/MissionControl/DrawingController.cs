using GRA.Controllers.ViewModel.MissionControl.Drawing;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.PerformDrawing)]
    public class DrawingController : Base.MCController
    {
        private readonly ILogger<DrawingController> _logger;
        private readonly DrawingService _drawingService;
        private readonly PrizeWinnerService _prizeWinnerService;
        private readonly SiteService _siteService;
        public DrawingController(ILogger<DrawingController> logger,
            ServiceFacade.Controller context,
            DrawingService drawingService,
            PrizeWinnerService prizeWinnerService,
            SiteService siteService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _drawingService = Require.IsNotNull(drawingService, nameof(drawingService));
            _prizeWinnerService = Require.IsNotNull(prizeWinnerService, nameof(prizeWinnerService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            PageTitle = "Drawing";
        }

        public async Task<IActionResult> Index(string search,
            int? systemId, int? branchId, bool? mine, int? programId, bool? archived, int page = 1)
        {
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

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = drawingList.Count,
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

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            DrawingListViewModel viewModel = new DrawingListViewModel()
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
                    .Where(_ => _.Id == branch.SystemId).SingleOrDefault().Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == systemId.Value).SingleOrDefault().Name;
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

        public async Task<IActionResult> Detail(int id, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            try
            {
                var drawing = await _drawingService.GetDetailsAsync(id, skip, take);

                PaginateViewModel paginateModel = new PaginateViewModel()
                {
                    ItemCount = drawing.Count,
                    CurrentPage = page,
                    ItemsPerPage = take
                };

                if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            id = id,
                            page = paginateModel.LastPage ?? 1
                        });
                }

                DrawingDetailViewModel viewModel = new DrawingDetailViewModel()
                {
                    Drawing = drawing.Data,
                    PaginateModel = paginateModel
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view drawing: ", gex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RedeemWinner(int prizeWinnerId, int drawingId, int page = 1)
        {
            try
            {
                await _prizeWinnerService.RedeemPrizeAsync(prizeWinnerId);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to redeem prize: ", gex);
            }
            return RedirectToAction("Detail", new { id = drawingId, page = page });
        }

        [HttpPost]
        public async Task<IActionResult> UndoRedemption(int prizeWinnerId, int drawingId, int page = 1)
        {
            try
            {
                await _prizeWinnerService.UndoRedemptionAsync(prizeWinnerId);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to undo redemption: ", gex);
            }
            return RedirectToAction("Detail", new { id = drawingId, page = page });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveWinner(int prizeWinnerId, int drawingId, int page = 1)
        {
            try
            {
                await _prizeWinnerService.RemovePrizeAsync(prizeWinnerId);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete winner: ", gex);
            }
            return RedirectToAction("Detail", new { id = drawingId, page = page });
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
                drawing.DrawingCriterion.EligibleCount = await _drawingService.GetEligibleCountAsync(id);

                DrawingNewViewModel viewModel = new DrawingNewViewModel()
                {
                    Drawing = drawing,
                    CanSendMail = UserHasPermission(Permission.MailParticipants)
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
            if (string.IsNullOrWhiteSpace(model.Drawing.NotificationSubject)
                && !string.IsNullOrWhiteSpace(model.Drawing.NotificationMessage))
            {
                ModelState.AddModelError("NotificationSubject",
                    "A subject is required to accompany the message");
            }

            if (!string.IsNullOrWhiteSpace(model.Drawing.NotificationSubject)
                && string.IsNullOrWhiteSpace(model.Drawing.NotificationMessage))
            {
                ModelState.AddModelError("NotificationMessage",
                    "A message is required to accompany the subject");
            }

            if (model.Drawing.WinnerCount > model.Drawing.DrawingCriterion.EligibleCount)
            {
                ModelState.AddModelError("WinnerCount", "Cannot have more Winners than Eligible Participants");
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
                    ModelState["DrawingCriterion.EligibleCount"].RawValue =
                        await _drawingService.GetEligibleCountAsync(model.Drawing.DrawingCriterionId);

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Criteria(string search,
            int? systemId, int? branchId, bool? mine, int? programId, int page = 1)
        {
            PageTitle = "Drawing Criteria";

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

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = criterionList.Count,
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

            var systemList = (await _siteService.GetSystemList())
                .OrderByDescending(_ => _.Id == GetId(ClaimType.SystemId)).ThenBy(_ => _.Name);

            CriterionListViewModel viewModel = new CriterionListViewModel()
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
                    .Where(_ => _.Id == branch.SystemId).SingleOrDefault().Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .Where(_ => _.Id == systemId.Value).SingleOrDefault().Name;
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

            CriterionDetailViewModel viewModel = new CriterionDetailViewModel()
            {
                SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name"),
                ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name"),
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CriteriaCreate(CriterionDetailViewModel model,
            string Drawing)
        {
            if (ModelState.IsValid)
            {
                if (model.Criterion.ProgramIds?.Count() == 1)
                {
                    model.Criterion.ProgramId = model.Criterion.ProgramIds.First();
                    model.Criterion.ProgramIds = null;
                }
                else
                {
                    model.Criterion.ProgramId = null;
                }
                if (model.ReadABook)
                {
                    model.Criterion.PointTranslationId = 1;
                    model.Criterion.ActivityAmount = 1;
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
                model.SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name");
                if (model.Criterion.SystemId.HasValue)
                {
                    model.BranchList = new SelectList(
                        (await _siteService.GetBranches(model.Criterion.SystemId.Value)), "Id", "Name");
                }
                else
                {
                    model.BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name");
                }
                model.ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name");
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
                CriterionDetailViewModel viewModel = new CriterionDetailViewModel()
                {
                    Criterion = criterion,
                    SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                    ProgramList = new SelectList(programs, "Id", "Name"),
                    ReadABook = criterion.ActivityAmount.HasValue,
                    EligibleCount = await _drawingService.GetEligibleCountAsync(id),
                    ProgramPlaceholder = string.Join(", ", programs
                        .Where(_ => criterion.ProgramIds.Contains(_.Id))
                        .Select(_ => _.Name)) 
                };

                if (viewModel.Criterion.SystemId.HasValue)
                {
                    viewModel.BranchList = new SelectList(
                        (await _siteService.GetBranches(viewModel.Criterion.SystemId.Value)), "Id", "Name");
                }
                else
                {
                    viewModel.BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name");
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
            if (ModelState.IsValid)
            {
                if (model.Criterion.ProgramIds?.Count() == 1)
                {
                    model.Criterion.ProgramId = model.Criterion.ProgramIds.First();
                    model.Criterion.ProgramIds = null;
                }
                else
                {
                    model.Criterion.ProgramId = null;
                }
                if (model.ReadABook)
                {
                    model.Criterion.PointTranslationId = 1;
                    model.Criterion.ActivityAmount = 1;
                }
                else
                {
                    model.Criterion.PointTranslationId = null;
                    model.Criterion.ActivityAmount = null;
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
                model.SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name");
                if (model.Criterion.SystemId.HasValue)
                {
                    model.BranchList = new SelectList(
                        (await _siteService.GetBranches(model.Criterion.SystemId.Value)), "Id", "Name");
                }
                else
                {
                    model.BranchList = new SelectList((await _siteService.GetAllBranches()), "Id", "Name");
                }
                model.ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name");
                return View(model);
            }
        }
    }
}
