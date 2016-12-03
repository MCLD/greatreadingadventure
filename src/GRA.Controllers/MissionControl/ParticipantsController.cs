using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewParticipantList)]
    public class ParticipantsController : Base.Controller
    {
        private readonly ILogger<ParticipantsController> _logger;
        private readonly ActivityService _activityService;
        private readonly MailService _mailService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        public ParticipantsController(ILogger<ParticipantsController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            MailService mailService,
            SiteService siteService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Participants";
        }

        #region Index
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var participantsList = await _userService
                .GetPaginatedUserListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = participantsList.Count,
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

            ParticipantsListViewModel viewModel = new ParticipantsListViewModel()
            {
                Users = participantsList.Data,
                PaginateModel = paginateModel,
                Search = search,
                CanRemoveParticipant = UserHasPermission(Permission.DeleteParticipants),
                CanViewDetails = UserHasPermission(Permission.ViewParticipantDetails)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.DeleteParticipants)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.Remove(id);
            AlertSuccess = "Participant deleted";
            return RedirectToAction("Index");
        }
        #endregion

        #region Detail
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Detail(int id)
        {
            var user = await _userService.GetDetails(id);
            SetPageTitle(user);
            var branchList = _siteService.GetBranches(user.SystemId);
            var programList = _siteService.GetProgramList();
            var systemList = _siteService.GetSystemList();
            await Task.WhenAll(branchList, programList, systemList);

            ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
            {
                User = user,
                Id = user.Id,
                HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                CanEditDetails = UserHasPermission(Permission.EditParticipants),
                BranchList = new SelectList(branchList.Result.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.Result.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.Result.ToList(), "Id", "Name")
            };
            return View(viewModel);
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> Detail(ParticipantsDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.MCUpdate(model.User);
                AlertSuccess = "Participant infomation updated";
                return RedirectToAction("Detail", new { id = model.User.Id });
            }
            else
            {
                SetPageTitle(model.User);

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
        #endregion

        #region Household
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Household(int id, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var user = await _userService.GetDetails(id);
            SetPageTitle(user);

            User headOfHousehold = new User();

            if (user.HouseholdHeadUserId.HasValue)
            {
                headOfHousehold = await _userService
                    .GetDetails(user.HouseholdHeadUserId.Value);
            }
            else
            {
                headOfHousehold = user;
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
                Id = id,
                HouseholdCount = household.Count,
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                Head = headOfHousehold
            };

            return View(viewModel);
        }
        #endregion

        #region Books
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Books(int id, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var books = await _userService.GetPaginatedUserBookListAsync(id, skip, take);

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

            var user = await _userService.GetDetails(id);
            SetPageTitle(user);

            BookListViewModel viewModel = new BookListViewModel()
            {
                Books = books.Data.ToList(),
                PaginateModel = paginateModel,
                Id = id,
                HouseholdCount = await _userService
                .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                CanModifyBooks = UserHasPermission(Permission.LogActivityForAny)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> EditBook(BookListViewModel model, int listId)
        {
            foreach (string key in ModelState.Keys
                .Where(m => !m.StartsWith($"Books[{listId}].")).ToList())
            {
                ModelState.Remove(key);
            }

            if (ModelState.IsValid)
            {
                await _activityService.UpdateBook(model.Id, model.Books[listId]);
                AlertSuccess = $"'{model.Books[listId].Title}' updated";
            }
            else
            {
                AlertDanger = "Missing required fields";
            }
            return RedirectToAction("Books", new { id = model.Id });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> AddBook(int id)
        {
            var user = await _userService.GetDetails(id);
            SetPageTitle(user, addBook: true);

            BookAddViewModel viewModel = new BookAddViewModel()
            {
                Id = id
            };
            return View(viewModel);
        }


        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> AddBook(BookAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _activityService.AddBook(model.Id, model.Book);
                AlertSuccess = $"Added book '{model.Book.Title}'";
                return RedirectToAction("Books", new { id = model.Id });
            }
            else
            {
                var user = await _userService.GetDetails(model.Id);
                SetPageTitle(user, addBook: true);

                return View(model);
            }
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id, int userId)
        {
            await _activityService.RemoveBook(userId, id);
            AlertSuccess = "Book deleted";
            return RedirectToAction("Books", new { id = userId });
        }
        #endregion

        #region History
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> History(int id, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var history = await _userService
                .GetPaginatedUserHistoryAsync(id, skip, take);

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

            var user = await _userService.GetDetails(id);
            SetPageTitle(user);

            HistoryListViewModel viewModel = new HistoryListViewModel()
            {
                Historys = history.Data,
                PaginateModel = paginateModel,
                Id = id,
                HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                CanRemoveHistory = UserHasPermission(Permission.LogActivityForAny)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> DeleteHistory(int id, int userId)
        {
            await _activityService.RemoveActivityAsync(userId, id);
            return RedirectToAction("History", new { id = userId });
        }
        #endregion

        #region Mail
        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> Mail(int id, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var mail = await _mailService.GetUserPaginatedAsync(id, skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = mail.Count,
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

            var user = await _userService.GetDetails(id);
            SetPageTitle(user);

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mails = mail.Data,
                PaginateModel = paginateModel,
                Id = id,
                HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail)
            };
            return View(viewModel);
        }

        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> MailDetail(int id)
        {
            var mail = await _mailService.GetDetails(id);
            var userId = mail.ToUserId ?? mail.FromUserId;

            var user = await _userService.GetDetails(userId);
            SetPageTitle(user, mailTo: mail.ToUserId.HasValue);

            MailDetailViewModel viewModel = new MailDetailViewModel
            {
                Mail = mail,
                Id = userId,
                CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.DeleteAnyMail)]
        [HttpPost]
        public async Task<IActionResult> DeleteMail(int id, int userId)
        {
            await _mailService.RemoveAsync(id);
            AlertSuccess = "Mail deleted";
            return RedirectToAction("Mail", new { id = userId });
        }
        #endregion

        #region PasswordReset
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> PasswordReset(int id)
        {
            var user = await _userService.GetDetails(id);
            SetPageTitle(user);

            PasswordResetViewModel viewModel = new PasswordResetViewModel()
            {
                Id = id,
                HouseholdCount = await _userService
                .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.ResetPassword(model.Id, model.NewPassword);
                AlertSuccess = "Password reset";
                return RedirectToAction("PasswordReset", new { id = model.Id });
            }
            else
            {
                var user = await _userService.GetDetails(model.Id);
                SetPageTitle(user);

                return View(model);
            }
        }
        #endregion

        private void SetPageTitle(User user, bool addBook = false, bool? mailTo = null)
        {
            var name = user.FirstName + " " + user.LastName;
            if (!string.IsNullOrEmpty(user.Username))
            {
                name += $"({user.Username})";
            }

            if (addBook == true)
            {
                PageTitle = $"Add Book - {name}";
            }
            else if (mailTo != null)
            {
                PageTitle = $"{(mailTo.Value ? "To" : "From")} - {name}";
            }
            else
            {
                PageTitle = $"Participant - {name}";
            }
        }
    }
}
