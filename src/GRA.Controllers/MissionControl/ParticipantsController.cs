using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewParticipantList)]
    public class ParticipantsController : Base.MCController
    {
        private readonly ILogger<ParticipantsController> _logger;
        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly DrawingService _drawingService;
        private readonly MailService _mailService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        public ParticipantsController(ILogger<ParticipantsController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            AuthenticationService authenticationService,
            DrawingService drawingService,
            MailService mailService,
            SiteService siteService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _drawingService = Require.IsNotNull(drawingService, nameof(drawingService));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Participants";
        }

        #region Index
        public async Task<IActionResult> Index(string search, string sort, int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            DataWithCount<IEnumerable<User>> participantsList = new DataWithCount<IEnumerable<User>>();

            if (!string.IsNullOrWhiteSpace(sort) && Enum.IsDefined(typeof(SortUsersBy), sort))
            {
                SortUsersBy userSort = (SortUsersBy)Enum.Parse(typeof(SortUsersBy), sort);
                participantsList = await _userService
                .GetPaginatedUserListAsync(skip, take, search, userSort);
            }
            else
            {
                participantsList = await _userService
                .GetPaginatedUserListAsync(skip, take, search);
            }

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
                CanViewDetails = UserHasPermission(Permission.ViewParticipantDetails),
                SortUsers = Enum.GetValues(typeof(SortUsersBy))
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
            try
            {
                var user = await _userService.GetDetails(id);
                SetPageTitle(user);
                var branchList = await _siteService.GetBranches(user.SystemId);
                var programList = await _siteService.GetProgramList();
                var systemList = await _siteService.GetSystemList();

                ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
                {
                    User = user,
                    Id = user.Id,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name")
                };
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant: ", gex);
                return RedirectToAction("Index");
            }
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

                var branchList = await _siteService.GetBranches(model.User.SystemId);
                var programList = await _siteService.GetProgramList();
                var systemList = await _siteService.GetSystemList();
                model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
                model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
                model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");

                return View(model);
            }
        }
        #endregion

        #region Household
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Household(int id, int page = 1)
        {
            try
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
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    Head = headOfHousehold
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's household: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> AddHouseholdMember(int id)
        {
            try
            {
                var headOfHousehold = await _userService.GetDetails(id);
                if (headOfHousehold.HouseholdHeadUserId != null)
                {
                    headOfHousehold = await _userService
                        .GetDetails((int)headOfHousehold.HouseholdHeadUserId);
                }

                SetPageTitle(headOfHousehold, "Add Household Member");

                var userBase = new User()
                {
                    LastName = headOfHousehold.LastName,
                    Email = headOfHousehold.Email,
                    PhoneNumber = headOfHousehold.PhoneNumber,
                    BranchId = headOfHousehold.BranchId,
                    ProgramId = headOfHousehold.ProgramId,
                    SystemId = headOfHousehold.SystemId
                };

                var branchList = await _siteService.GetBranches(headOfHousehold.SystemId);
                var programList = await _siteService.GetProgramList();
                var systemList = await _siteService.GetSystemList();

                HouseholdAddViewModel viewModel = new HouseholdAddViewModel()
                {
                    User = userBase,
                    Id = id,
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name")
                };

                return View("HouseholdAdd", viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's household: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> AddHouseholdMember(HouseholdAddViewModel model)
        {
            var headOfHousehold = await _userService.GetDetails(model.Id);
            if (headOfHousehold.HouseholdHeadUserId != null)
            {
                headOfHousehold = await _userService
                    .GetDetails((int)headOfHousehold.HouseholdHeadUserId);
            }

            if (ModelState.IsValid)
            {
                await _userService.AddHouseholdMemberAsync(headOfHousehold.Id, model.User);
                AlertSuccess = "Added household member";
                return RedirectToAction("Household", new { id = model.Id });
            }
            else
            {
                SetPageTitle(headOfHousehold, "Add Household Member");

                var branchList = await _siteService.GetBranches(model.User.SystemId);
                var programList = await _siteService.GetProgramList();
                var systemList = await _siteService.GetSystemList();
                model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
                model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
                model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");

                return View("HouseholdAdd", model);
            }
        }

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> RegisterHouseholdMember(int id)
        {
            try
            {
                var user = await _userService.GetDetails(id);
                if (!string.IsNullOrWhiteSpace(user.Username))
                {
                    return RedirectToAction("Household", new { id = id });
                }
                SetPageTitle(user, "Register Household Memeber");

                HouseholdRegisterViewModel viewModel = new HouseholdRegisterViewModel()
                {
                    Id = id
                };

                return View("HouseholdRegister", viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's registration: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> RegisterHouseholdMember(HouseholdRegisterViewModel model)
        {
            var user = await _userService.GetDetails(model.Id);
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction("Household", new { id = model.Id });
            }
            if (ModelState.IsValid)
            {
                user.Username = model.Username;
                try
                {
                    await _userService.RegisterHouseholdMemberAsync(user, model.Password);
                    AlertSuccess = "Household member registered!";
                    return RedirectToAction("Household", new { id = model.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to register household member:", gex);
                }
            }
            SetPageTitle(user, "Register Household Memeber");
            return View("HouseholdRegister", model);
        }

        #endregion

        #region Books
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Books(int id, int page = 1)
        {
            try
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
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's books: ", gex);
                return RedirectToAction("Index");
            }
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
                await _activityService.UpdateBookAsync(model.Id, model.Books[listId]);
                AlertSuccess = $"'{model.Books[listId].Title}' updated";
            }
            else
            {
                ShowAlertDanger("Missing required fields");
            }
            return RedirectToAction("Books", new { id = model.Id });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> AddBook(int id)
        {
            try
            {
                var user = await _userService.GetDetails(id);
                SetPageTitle(user, "Add Book");

                BookAddViewModel viewModel = new BookAddViewModel()
                {
                    Id = id
                };
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's books: ", gex);
                return RedirectToAction("Index");
            }
        }


        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> AddBook(BookAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _activityService.AddBookAsync(model.Id, model.Book);
                AlertSuccess = $"Added book '{model.Book.Title}'";
                return RedirectToAction("Books", new { id = model.Id });
            }
            else
            {
                var user = await _userService.GetDetails(model.Id);
                SetPageTitle(user, "Add Book");

                return View(model);
            }
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int id, int userId)
        {
            await _activityService.RemoveBookAsync(userId, id);
            AlertSuccess = "Book deleted";
            return RedirectToAction("Books", new { id = userId });
        }
        #endregion

        #region History
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> History(int id, int page = 1)
        {
            try
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
                    Historys = new List<HistoryItemViewModel>(),
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanRemoveHistory = UserHasPermission(Permission.LogActivityForAny),
                    TotalPoints = user.PointsEarned
                };

                bool editChallenges = UserHasPermission(Permission.EditChallenges);

                foreach (var item in history.Data)
                {
                    if (item.ChallengeId != null)
                    {
                        string url = "";
                        if (editChallenges)
                        {
                            url = Url.Action("Edit", "Challenges", new { id = item.ChallengeId });
                        }
                        else
                        {
                            url = Url.Action("Detail", "Challenges",
                            new { area = "", id = item.ChallengeId });
                        }
                        item.Description = $"<a target='_blank' href='{url}'>{item.Description}</a>";
                    }
                    HistoryItemViewModel itemModel = new HistoryItemViewModel()
                    {
                        Id = item.Id,
                        CreatedAt = item.CreatedAt.ToString("d"),
                        Description = item.Description,
                        PointsEarned = item.PointsEarned,
                    };
                    if (!string.IsNullOrWhiteSpace(item.BadgeFilename))
                    {
                        itemModel.BadgeFilename = _pathResolver.ResolveContentPath(item.BadgeFilename);
                    }
                    viewModel.Historys.Add(itemModel);
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's history: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> DeleteHistory(int id, int userId)
        {
            await _activityService.RemoveActivityAsync(userId, id);
            return RedirectToAction("History", new { id = userId });
        }
        #endregion

        #region Drawings
        [Authorize(Policy = Policy.ViewUserDrawings)]
        public async Task<IActionResult> Drawings(int id, int page = 1)
        {
            try
            {
                int take = 15;
                int skip = take * (page - 1);

                var drawingList = await _userService.GetPaginatedUserDrawingListAsync(id, skip, take);

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

                var user = await _userService.GetDetails(id);
                SetPageTitle(user);

                DrawingListViewModel viewModel = new DrawingListViewModel()
                {
                    DrawingWinners = drawingList.Data,
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username)
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's drawings: ", gex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Policy = Policy.ViewUserDrawings)]
        public async Task<IActionResult> RedeemWinner(int drawingId, int userId, int page = 1)
        {
            await _drawingService.RedeemWinnerAsync(drawingId, userId);
            return RedirectToAction("Drawings", new { id = userId, page = page });
        }

        [HttpPost]
        [Authorize(Policy = Policy.ViewUserDrawings)]
        public async Task<IActionResult> UndoRedemption(int drawingId, int userId, int page = 1)
        {
            await _drawingService.UndoRedemptionAsnyc(drawingId, userId);
            return RedirectToAction("Drawings", new { id = userId, page = page });
        }
        #endregion

        #region Mail
        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> Mail(int id, int page = 1)
        {
            try
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
                    CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail),
                    CanSendMail = UserHasPermission(Permission.MailParticipants)
                };
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's mail: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> MailDetail(int id)
        {
            try
            {
                var mail = await _mailService.GetDetails(id);
                var userId = mail.ToUserId ?? mail.FromUserId;

                var user = await _userService.GetDetails(userId);
                SetPageTitle(user, (mail.ToUserId.HasValue ? "To" : "From"));

                MailDetailViewModel viewModel = new MailDetailViewModel
                {
                    Mail = mail,
                    Id = userId,
                    CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail)
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view mail: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.DeleteAnyMail)]
        [HttpPost]
        public async Task<IActionResult> DeleteMail(int id, int userId)
        {
            await _mailService.RemoveAsync(id);
            AlertSuccess = "Mail deleted";
            return RedirectToAction("Mail", new { id = userId });
        }

        [Authorize(Policy = Policy.MailParticipants)]
        public async Task<IActionResult> MailSend(int id)
        {
            try
            {
                var user = await _userService.GetDetails(id);
                SetPageTitle(user, "Send Mail");

                MailSendViewModel viewModel = new MailSendViewModel()
                {
                    Id = user.Id
                };
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's mail: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.MailParticipants)]
        [HttpPost]
        public async Task<IActionResult> MailSend(MailSendViewModel model)
        {
            if (ModelState.IsValid)
            {
                Mail mail = new Mail()
                {
                    ToUserId = model.Id,
                    Subject = model.Subject,
                    Body = model.Body,
                };
                await _mailService.MCSendAsync(mail);
                AlertSuccess = "Mail sent to participant";
                return RedirectToAction("Mail", new { id = model.Id });
            }
            else
            {
                var user = await _userService.GetDetails(model.Id);
                SetPageTitle(user, "Send Mail");
                return View();
            }
        }
        #endregion

        #region PasswordReset
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> PasswordReset(int id)
        {
            try
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
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's password reset: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel model)
        {
            var user = await _userService.GetDetails(model.Id);
            if (ModelState.IsValid)
            {
                try
                {
                    await _authenticationService.ResetPassword(model.Id, model.NewPassword);
                    AlertSuccess = $"Password reset for <strong>{user.FullName} ('{user.Username}')</strong>.";
                    return RedirectToAction("PasswordReset", new { id = model.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to change password:", gex);
                }
            }

            SetPageTitle(user);
            return View(model);
        }
        #endregion

        private void SetPageTitle(User user, string title = "Participant")
        {
            var name = user.FullName;
            if (!string.IsNullOrEmpty(user.Username))
            {
                name += $"({user.Username})";
            }
            PageTitle = $"{title} - {name}";
        }
    }
}
