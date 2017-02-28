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

        public async Task<IActionResult> Index(string FilterBy, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            bool archived = String.Equals(FilterBy, "Archived", StringComparison.OrdinalIgnoreCase);
            if (archived)
            {
                PageTitle = "Archived Drawings";
            }

            var drawingList = await _drawingService.GetPaginatedDrawingListAsync(skip, take, archived);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = drawingList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            DrawingListViewModel viewModel = new DrawingListViewModel()
            {
                Drawings = drawingList.Data,
                PaginateModel = paginateModel,
                Archived = archived
            };

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
            await _prizeWinnerService.RedeemPrizeAsync(prizeWinnerId);
            return RedirectToAction("Detail", new { id = drawingId, page = page });
        }

        [HttpPost]
        public async Task<IActionResult> UndoRedemption(int prizeWinnerId, int drawingId, int page = 1)
        {
            await _prizeWinnerService.UndoRedemptionAsync(prizeWinnerId);
            return RedirectToAction("Detail", new { id = drawingId, page = page });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveWinner(int prizeWinnerId, int drawingId, int page = 1)
        {
            try
            {
                await _prizeWinnerService.RedeemPrizeAsync(prizeWinnerId);
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

                return View(drawing);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view new drawing: ", gex);
                return RedirectToAction("Criteria");
            }
        }

        [HttpPost]
        public async Task<IActionResult> New(Drawing model)
        {
            if (string.IsNullOrWhiteSpace(model.NotificationSubject)
                && !string.IsNullOrWhiteSpace(model.NotificationMessage))
            {
                ModelState.AddModelError("NotificationSubject",
                    "A subject is required to accompany the message");
            }

            if (!string.IsNullOrWhiteSpace(model.NotificationSubject)
                && string.IsNullOrWhiteSpace(model.NotificationMessage))
            {
                ModelState.AddModelError("NotificationMessage",
                    "A message is required to accompany the subject");
            }

            if (model.WinnerCount > model.DrawingCriterion.EligibleCount)
            {
                ModelState.AddModelError("WinnerCount", "Cannot have more Winners than Eligible Participants");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var drawing = await _drawingService.PerformDrawingAsync(model);
                    return RedirectToAction("Detail", new { id = drawing.Id });
                }
                catch (GraException gex)
                {
                    AlertInfo = gex.Message;
                    ModelState["DrawingCriterion.EligibleCount"].RawValue =
                        await _drawingService.GetEligibleCountAsync(model.DrawingCriterionId);

                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public async Task<IActionResult> Criteria(int page = 1)
        {
            PageTitle = "Drawing Criteria";

            int take = 15;
            int skip = take * (page - 1);

            var criterionList = await _drawingService.GetPaginatedCriterionListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = criterionList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            CriterionListViewModel viewModel = new CriterionListViewModel()
            {
                Criteria = criterionList.Data,
                PaginateModel = paginateModel
            };

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
                var site = await GetCurrentSiteAsync();
                CriterionDetailViewModel viewModel = new CriterionDetailViewModel()
                {
                    Criterion = criterion,
                    SystemList = new SelectList((await _siteService.GetSystemList()), "Id", "Name"),
                    ProgramList = new SelectList((await _siteService.GetProgramList()), "Id", "Name"),
                    ReadABook = criterion.ActivityAmount.HasValue,
                    EligibleCount = await _drawingService.GetEligibleCountAsync(id)
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
