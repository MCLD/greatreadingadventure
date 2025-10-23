using System;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.JoinCodes;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area(nameof(MissionControl))]
    [Authorize(Policy = Policy.ViewJoinCodes)]
    public class JoinCodesController : Base.MCController
    {
        public static readonly string CodeReplacementKey = "CodeReplacementKey";

        private readonly JoinCodeService _joinCodeService;
        private readonly ILogger _logger;
        private readonly SiteService _siteService;

        public JoinCodesController(ServiceFacade.Controller context,
            ILogger<JoinCodesController> logger,
            JoinCodeService joinCodeService,
            SiteService siteService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(joinCodeService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(siteService);

            _joinCodeService = joinCodeService;
            _logger = logger;
            _siteService = siteService;

            PageTitle = "Join Codes";
        }

        public static string Name
        { get { return "JoinCodes"; } }


        [HttpPost]
        public async Task<IActionResult> Delete(JoinCodeListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                await _joinCodeService.RemoveAsync(model.DeleteId);
                ShowAlertSuccess("Join code deleted.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"Unable to delete join code: {gex.Message}");
            }

            return RedirectToAction(nameof(Index), new
            {
                page = model.CurrentPage
            });
        }

        public async Task<JsonResult> GetCode(bool isQRCode, int? branchId)
        {
            var joinCode = await _joinCodeService.GetByTypeAndBranch(isQRCode, branchId);

            joinCode.JoinUrl = Url.Action(nameof(Controllers.JoinController.Index),
                    Controllers.JoinController.Name,
                    new { area = "", src = joinCode.Code },
                    HttpContext.Request.Scheme);

            return Json(joinCode);
        }

        public async Task<IActionResult> Index(int page)
        {
            page = page < 1 ? 1 : page;

            var filter = new BaseFilter(page);

            var joinCodes = await _joinCodeService.GetPaginatedListAsync(filter);

            var viewModel = new JoinCodeListViewModel
            {
                BranchList = NameIdSelectList(await _siteService.GetAllBranches(true)),
                CanManageJoinCodes = UserHasPermission(Permission.ManageJoinCodes),
                CurrentPage = page,
                ItemCount = joinCodes.Count,
                ItemsPerPage = filter.Take.Value,
                IsQRCode = true,
                JoinCodes = joinCodes.Data,
                JoinUrl = Url.Action(nameof(Controllers.JoinController.Index),
                    Controllers.JoinController.Name,
                    new { area = "", src = CodeReplacementKey },
                    HttpContext.Request.Scheme)
            };
            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = viewModel.LastPage ?? 1
                    });
            }

            return View(viewModel);
        }
    }
}
