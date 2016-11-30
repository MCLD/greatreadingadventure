using GRA.Controllers.ViewModel.Participants;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly UserService _userService;
        public ParticipantsController(ILogger<ParticipantsController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            MailService mailService,
            UserService userService)
            : base(context)
        {
            this._logger = Require.IsNotNull(logger, nameof(logger));
            this._activityService = Require.IsNotNull(activityService, nameof(activityService));
            this._mailService = Require.IsNotNull(mailService, nameof(mailService));
            this._userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Participants";
        }

        #region Index
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var participantsList = await _userService
                .GetPaginatedUserListAsync(CurrentUser, skip, take);

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
            await _userService.Remove(CurrentUser, id);
            return RedirectToAction("Index");
        }
        #endregion

        #region Detail
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Detail(int id)
        {
            var user = await _userService.GetDetails(CurrentUser, id);
            SetPageTitle(user);
            ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
            {
                User = user,
                Id = user.Id,
                HouseholdCount = await _userService.FamilyMemberCountAsync(CurrentUser, user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                CanEditDetails = UserHasPermission(Permission.EditParticipants)
            };
            return View(viewModel);
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> Detail(ParticipantsDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.Update(CurrentUser, model.User);
                AlertSuccess = "Participant infomation was updated";
                return RedirectToAction("Detail", new { id = model.User.Id });
            }
            else
            {
                SetPageTitle(model.User);
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

            var user = await _userService.GetDetails(CurrentUser, id);
            SetPageTitle(user);

            User headOfHousehold = new User();

            if (user.HouseholdHeadUserId.HasValue)
            {
                headOfHousehold = await _userService
                    .GetDetails(CurrentUser, user.HouseholdHeadUserId.Value);
            }
            else
            {
                headOfHousehold = user;
            }

            var household = await _userService
                .GetPaginatedFamilyListAsync(CurrentUser, headOfHousehold.Id, skip, take);

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

            var books = await _userService.GetPaginatedUserBookListAsync(CurrentUser, id, skip, take);

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

            var user = await _userService.GetDetails(CurrentUser, id);
            SetPageTitle(user);

            BookListViewModel viewModel = new BookListViewModel()
            {
                Books = books.Data,
                PaginateModel = paginateModel,
                Id = id,
                HouseholdCount = await _userService
                .FamilyMemberCountAsync(CurrentUser, user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                CanModifyBooks = UserHasPermission(Permission.LogActivityForAny)
            };

            return View(viewModel);
        }
        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> DeleteBook (int id, int userId)
        {
            await _activityService.RemoveBook(CurrentUser, userId, id);
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
                .GetPaginatedUserHistoryAsync(CurrentUser, id, skip, take);

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

            var user = await _userService.GetDetails(CurrentUser, id);
            SetPageTitle(user);

            HistoryListViewModel viewModel = new HistoryListViewModel()
            {
                Historys = history.Data,
                PaginateModel = paginateModel,
                Id = id,
                HouseholdCount = await _userService.FamilyMemberCountAsync(CurrentUser, user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                CanRemoveHistory = UserHasPermission(Permission.LogActivityForAny)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> DeleteHistory(int id, int userId)
        {
            await _activityService.RemoveActivityAsync(CurrentUser, userId, id);
            return RedirectToAction("History", new { id = userId });
        }
        #endregion

        #region Mail
        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> Mail(int id, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var mail = await _mailService.GetUserPaginatedAsync(CurrentUser, id, skip, take);

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

            var user = await _userService.GetDetails(CurrentUser, id);
            SetPageTitle(user);

            MailListViewModel viewModel = new MailListViewModel()
            {
                Mails = mail.Data,
                PaginateModel = paginateModel,
                Id = id,
                HouseholdCount = await _userService.FamilyMemberCountAsync(CurrentUser, user.HouseholdHeadUserId ?? id),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail)
            };
            return View(viewModel);
        }

        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> MailDetail(int id)
        {
            var mail = await _mailService.GetDetails(CurrentUser, id);
            var userId = mail.ToUserId ?? mail.FromUserId;

            var user = await _userService.GetDetails(CurrentUser, userId);
            SetPageTitle(user, mail.ToUserId.HasValue);

            MailDetailViewModel viewModel = new MailDetailViewModel
            {
                Mail = mail,
                Id = userId,
                CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.DeleteAnyMail)]
        public async Task<IActionResult> DeleteMail(int id, int userId)
        {
            await _mailService.RemoveAsync(CurrentUser, id);

            return RedirectToAction("Mail", new { id = userId });
        }
        #endregion

        #region PasswordReset
        #endregion

        private void SetPageTitle(User user, bool? mailTo = null)
        {
            var name = user.FirstName + " " + user.LastName;
            if (!string.IsNullOrEmpty(user.Username))
            {
                name += $"({user.Username})";
            }
            if (mailTo == null)
            {
                PageTitle = $"Participant - {name}";
            }
            else
            {
                PageTitle = $"{(mailTo.Value ? "To" : "From")} - {name}";
            }
        }
    }
}
