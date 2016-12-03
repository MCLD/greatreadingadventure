using GRA.Controllers.ViewModel.Profile;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    [Authorize]
    public class ProfileController : Base.Controller
    {
        private readonly ILogger<ChallengesController> _logger;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public ProfileController(ILogger<ChallengesController> logger,
            ServiceFacade.Controller context,
            SiteService siteService,
            UserService userService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "My Profile";
        }

        public async Task<IActionResult> Index()
        {
            User user = await _userService.GetDetails((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));

            var getHouseholdCount = _userService.FamilyMemberCountAsync((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));
            var branchList = _siteService.GetBranches(user.SystemId);
            var programList = _siteService.GetProgramList();
            var systemList = _siteService.GetSystemList();
            await Task.WhenAll(getHouseholdCount, branchList, programList, systemList);

            ProfileDetailViewModel viewModel = new ProfileDetailViewModel()
            {
                User = user,
                HouseholdCount = getHouseholdCount.Result,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                BranchList = new SelectList(branchList.Result.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.Result.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.Result.ToList(), "Id", "Name")

            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.Update(model.User);
                AlertSuccess = "Updated profile";
                return RedirectToAction("Index");
            }
            else
            {
                var branchList = _siteService.GetBranches(model.User.SystemId);
                var programList = _siteService.GetProgramList();
                var systemList = _siteService.GetSystemList();
                await Task.WhenAll(branchList, programList, systemList);
                model.BranchList = new SelectList(branchList.Result.ToList(), "Id", "Name");
                model.ProgramList = new SelectList(programList.Result.ToList(), "Id", "Name");
                model.SystemList = new SelectList(systemList.Result.ToList(), "Id", "Name");

                return View(model);
            }
        }

        public async Task<IActionResult> Household(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var user = await _userService.GetDetails((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));

            User headOfHousehold = new User();
            bool isHead = false;
            if (user.HouseholdHeadUserId.HasValue)
            {
                headOfHousehold = await _userService
                    .GetDetails(user.HouseholdHeadUserId.Value);
            }
            else
            {
                headOfHousehold = user;
                isHead = true;
            }

            var household = await _userService
                .GetPaginatedFamilyListAsync(headOfHousehold.Id, skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = household.Count,
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

            HouseholdListViewModel viewModel = new HouseholdListViewModel()
            {
                Users = household.Data,
                PaginateModel = paginateModel,
                HouseholdCount = household.Count,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                Head = headOfHousehold,
                IsHead = isHead
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult LoginAs(int id)
        {
            HttpContext.Session.SetInt32(SessionKey.ActiveUserId, id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Books(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var books = await _userService.GetPaginatedUserBookListAsync((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId), skip, take);


            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = books.Count,
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

            var getUser = _userService.GetDetails((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));
            var getHouseholdCount = _userService.FamilyMemberCountAsync((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));
            await Task.WhenAll(getUser, getHouseholdCount);

            BookListViewModel viewModel = new BookListViewModel()
            {
                Books = books.Data,
                PaginateModel = paginateModel,
                HouseholdCount = getHouseholdCount.Result,
                HasAccount = !string.IsNullOrWhiteSpace(getUser.Result.Username)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> History(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var history = await _userService
                .GetPaginatedUserHistoryAsync((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId), skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = history.Count,
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

            var getUser = _userService.GetDetails((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));
            var getHouseholdCount = _userService.FamilyMemberCountAsync((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));
            await Task.WhenAll(getUser, getHouseholdCount);

            HistoryListViewModel viewModel = new HistoryListViewModel()
            {
                Historys = history.Data,
                PaginateModel = paginateModel,
                HouseholdCount = getHouseholdCount.Result,
                HasAccount = !string.IsNullOrWhiteSpace(getUser.Result.Username)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ChangePassword()
        {
            User user = await _userService.GetDetails((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction("Index");
            }

            ChangePasswordViewModel viewModel = new ChangePasswordViewModel()
            {
                HouseholdCount = await _userService.FamilyMemberCountAsync((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId)),
                HasAccount = true
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userService.GetDetails((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId));
                var loginAttempt = await _userService.AuthenticateUserAsync(user.Username, model.OldPassword);
                if (loginAttempt.PasswordIsValid)
                {
                    await _userService.ResetPassword((int)HttpContext.Session.GetInt32(SessionKey.ActiveUserId), model.NewPassword);
                    AlertSuccess = "Password changed";
                    return RedirectToAction("ChangePassword");
                }
                model.ErrorMessage = "The username and password entered do not match";
            }

            return View(model);
        }
    }
}
