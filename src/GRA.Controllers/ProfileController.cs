using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailValidation;
using GRA.Controllers.ViewModel.Profile;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    [Authorize]
    public class ProfileController : Base.UserController
    {
        private const string ActivityMessage = "ActivityMessage";
        private const string SecretCodeMessage = "SecretCodeMessage";

        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly AvatarService _avatarService;
        private readonly BadgeService _badgeService;
        private readonly ChallengeService _challengeService;
        private readonly DailyLiteracyTipService _dailyLiteracyTipService;
        private readonly EventService _eventService;
        private readonly ILogger<ProfileController> _logger;
        private readonly MailService _mailService;
        private readonly MapsterMapper.IMapper _mapper;
        private readonly PointTranslationService _pointTranslationService;
        private readonly PrizeWinnerService _prizeWinnerService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        private string HouseholdTitle;

        public ProfileController(ILogger<ProfileController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            AuthenticationService authenticationService,
            AvatarService avatarService,
            BadgeService badgeService,
            ChallengeService challengeService,
            DailyLiteracyTipService dailyLiteracyTipService,
            EventService eventService,
            MailService mailService,
            PointTranslationService pointTranslationService,
            PrizeWinnerService prizeWinnerService,
            QuestionnaireService questionnaireService,
            SchoolService schoolService,
            SiteService siteService,
            UserService userService,
            VendorCodeService vendorCodeService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(activityService);
            ArgumentNullException.ThrowIfNull(authenticationService);
            ArgumentNullException.ThrowIfNull(avatarService);
            ArgumentNullException.ThrowIfNull(badgeService);
            ArgumentNullException.ThrowIfNull(challengeService);
            ArgumentNullException.ThrowIfNull(dailyLiteracyTipService);
            ArgumentNullException.ThrowIfNull(eventService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(mailService);
            ArgumentNullException.ThrowIfNull(pointTranslationService);
            ArgumentNullException.ThrowIfNull(prizeWinnerService);
            ArgumentNullException.ThrowIfNull(questionnaireService);
            ArgumentNullException.ThrowIfNull(schoolService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(userService);
            ArgumentNullException.ThrowIfNull(vendorCodeService);

            _activityService = activityService;
            _authenticationService = authenticationService;
            _avatarService = avatarService;
            _badgeService = badgeService;
            _challengeService = challengeService;
            _dailyLiteracyTipService = dailyLiteracyTipService;
            _eventService = eventService;
            _logger = logger;
            _mailService = mailService;
            _mapper = context?.Mapper;
            _pointTranslationService = pointTranslationService;
            _prizeWinnerService = prizeWinnerService;
            _questionnaireService = questionnaireService;
            _schoolService = schoolService;
            _siteService = siteService;
            _userService = userService;
            _vendorCodeService = vendorCodeService;

            PageTitle = "My Profile";
        }

        public static string Name
        { get { return "Profile"; } }

        [HttpPost]
        public async Task<IActionResult> AddBook(BookListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                try
                {
                    model.Book.Author = model.Book.Author?.Trim();
                    model.Book.Title = model.Book.Title.Trim();
                    var result = await _activityService.AddBookAsync(GetActiveUserId(), model.Book);
                    if (result.Status == ServiceResultStatus.Warning
                            && !string.IsNullOrWhiteSpace(result.Message))
                    {
                        ShowAlertWarning(result.Message);
                    }
                    else if (result.Status == ServiceResultStatus.Success)
                    {
                        ShowAlertSuccess(_sharedLocalizer[Annotations.Info.BookAdded,
                            model.Book.Title,
                            model.Book.Author]);
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotCreate,
                        model.Book.Title,
                        gex.Message]);
                }
            }
            else
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotCreate,
                    model.Book.Title,
                    _sharedLocalizer[Annotations.Required.Missing]]);
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction(nameof(Books), new { page });
        }

        public async Task<IActionResult> AddExistingParticipant()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction(nameof(Household));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExistingParticipant(HouseholdExistingViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction(nameof(Household));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if we're going to trip group membership requirements
                    var (useGroups, maximumHousehold) =
                        await GetSiteSettingIntAsync(SiteSettingKey
                            .Users
                            .MaximumHouseholdSizeBeforeGroup);

                    if (useGroups)
                    {
                        var groupTypes = await _userService.GetGroupTypeListAsync();

                        if (!groupTypes.Any())
                        {
                            _logger.LogError("User {ActiveUserId} should be forced to make a group but no group types are configured",
                                authUser.Id);
                        }
                        else
                        {
                            var currentHousehold
                                = await _userService.GetHouseholdAsync(authUser.Id,
                                false,
                                false,
                                false);

                            var (totalAddCount, addUserId) =
                                await _userService.CountParticipantsToAdd(model.Username,
                                    model.Password);

                            // +1 for household manager, counting the people we're adding so >
                            if (currentHousehold.Count() + 1 + totalAddCount > maximumHousehold)
                            {
                                var groupInfo
                                    = await _userService.GetGroupFromHouseholdHeadAsync(authUser.Id);

                                if (groupInfo == null)
                                {
                                    _logger.LogInformation("Redirecting user {ActiveUserId} to create a group when adding member {GroupMemberCount}, group will total {TotalGropuMemberCount}",
                                        authUser.Id,
                                        maximumHousehold + 1,
                                        currentHousehold.Count() + totalAddCount);

                                    // add authenticated user id to session
                                    HttpContext.Session.SetString(SessionKey.AbsorbUserId,
                                        addUserId.ToString(CultureInfo.InvariantCulture));
                                    return View("GroupUpgrade", new GroupUpgradeViewModel
                                    {
                                        MaximumHouseholdAllowed = maximumHousehold,
                                        GroupTypes
                                            = new SelectList(groupTypes.ToList(), "Id", "Name"),
                                        AddExisting = true
                                    });
                                }
                            }
                        }
                    }
                    // end checking about groups

                    string addedMembers = await _userService
                        .AddParticipantToHouseholdAsync(model.Username, model.Password);
                    HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                    ShowAlertSuccess(
                        _sharedLocalizer[Annotations.Interface.AddedParticipantGroupFamily,
                            addedMembers,
                            _sharedLocalizer[HouseholdTitle]]);
                    return RedirectToAction(nameof(Household));
                }
                catch (GraException gex)
                {
                    HttpContext.Session.Remove(SessionKey.AbsorbUserId);
                    ShowAlertDanger(_sharedLocalizer[Annotations.Validate.UnableToAdd,
                        gex.Message]);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> AddHouseholdMember()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                // if the authUser has a household head then they are not the household head
                return RedirectToAction(nameof(Household));
            }

            var (useGroups, maximumHousehold) =
                await GetSiteSettingIntAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup);

            if (useGroups)
            {
                var groupTypes = await _userService.GetGroupTypeListAsync();

                if (!groupTypes.Any())
                {
                    _logger.LogError("User {ActiveUserId} should be forced to make a group but no group types are configured",
                        authUser.Id);
                }
                else
                {
                    var household
                        = await _userService.GetHouseholdAsync(authUser.Id, false, false, false);
                    // not counting the person we're adding, so >=
                    if (household.Count() + 1 >= maximumHousehold)
                    {
                        var groupInfo
                            = await _userService.GetGroupFromHouseholdHeadAsync(authUser.Id);

                        if (groupInfo == null)
                        {
                            _logger.LogInformation("Redirecting user {ActiveUserId} to create a group when adding member {GroupMemberCount}",
                                authUser.Id,
                                maximumHousehold + 1);
                            return View("GroupUpgrade", new GroupUpgradeViewModel
                            {
                                GroupTypes = new SelectList(groupTypes.ToList(), "Id", "Name"),
                                MaximumHouseholdAllowed = maximumHousehold
                            });
                        }
                    }
                }
            }

            var userBase = new User
            {
                LastName = authUser.LastName,
                PostalCode = authUser.PostalCode,
                Email = authUser.Email,
                PhoneNumber = authUser.PhoneNumber,
                BranchId = authUser.BranchId,
                SystemId = authUser.SystemId
            };

            var systemList = await _siteService.GetSystemList();
            var branchList = await _siteService.GetBranches(authUser.SystemId);
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

            var viewModel = new HouseholdAddViewModel
            {
                User = userBase,
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name"),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
            };

            if (programList.Count() == 1)
            {
                var programId = programList.Single().Id;
                var program = await _siteService.GetProgramByIdAsync(programId);
                viewModel.User.ProgramId = programId;
                viewModel.ShowAge = program.AskAge;
                viewModel.ShowSchool = program.AskSchool;
            }

            var askIfFirstTime
                = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (askIfFirstTime)
            {
                viewModel.AskFirstTime = EmptyNoYes();
            }

            var (askEmailSubscription, askEmailSubscriptionText)
                = await GetSiteSettingStringAsync(SiteSettingKey.Users.AskEmailSubPermission);
            if (askEmailSubscription)
            {
                viewModel.AskEmailSubscription = EmptyNoYes();
                viewModel.AskEmailSubscriptionText = askEmailSubscriptionText;
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
            if (askActivityGoal)
            {
                viewModel.User.DailyPersonalGoal = defaultDailyGoal;
                var pointTranslation = programList.First().PointTranslation;
                viewModel.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            var askPersonalPointGoal = await GetSiteSettingBoolAsync(
                SiteSettingKey.Users.AskPersonalPointGoal);
            if (askPersonalPointGoal)
            {
                viewModel.AskPersonalPointGoal = true;
                viewModel.MinimumPersonalPointGoal = programList.First().AchieverPointAmount;

                var (maximumPointsSet, maximumPointsValue) = await GetSiteSettingIntAsync(
                            SiteSettingKey.Users.MaximumActivityPermitted);
                if (maximumPointsSet)
                {
                    viewModel.MaximumPersonalPointGoal = maximumPointsValue;
                }
            }

            return View("HouseholdAdd", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddHouseholdMember(HouseholdAddViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction(nameof(Household));
            }

            var site = await GetCurrentSiteAsync();

            if (!string.IsNullOrWhiteSpace(model.User.Email)
                && !EmailValidator.Validate(model.User.Email.Trim()))
            {
                ModelState.AddModelError("User.Email",
                    _sharedLocalizer[Annotations.Validate.Email,
                        _sharedLocalizer[DisplayNames.EmailAddress]]);
            }

            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode",
                    _sharedLocalizer[ErrorMessages.Field,
                        _sharedLocalizer[DisplayNames.ZipCode]]);
            }

            var askIfFirstTime
                = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (!askIfFirstTime)
            {
                ModelState.Remove(nameof(model.IsFirstTime));
            }

            var (askEmailSubscription, askEmailSubscriptionText)
                = await GetSiteSettingStringAsync(SiteSettingKey.Users.AskEmailSubPermission);
            if (!askEmailSubscription)
            {
                ModelState.Remove(nameof(model.EmailSubscriptionRequested));
            }
            else
            {
                var subscriptionRequested = DropDownTrueValue.Equals(
                        model.EmailSubscriptionRequested, StringComparison.OrdinalIgnoreCase);
                if (subscriptionRequested && string.IsNullOrWhiteSpace(model.User.Email))
                {
                    ModelState.AddModelError("User.Email", " ");
                    ModelState.AddModelError(nameof(model.EmailSubscriptionRequested),
                        _sharedLocalizer[Annotations.Validate.EmailSubscription]);
                }
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);

            var askPersonalPointGoal = await GetSiteSettingBoolAsync(
                SiteSettingKey.Users.AskPersonalPointGoal);

            bool askAge = false;
            bool askSchool = false;
            Program program = null;
            if (model.User.ProgramId >= 0)
            {
                program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);
                askAge = program.AskAge;
                askSchool = program.AskSchool;
                if (program.AgeRequired && !model.User.Age.HasValue)
                {
                    ModelState.AddModelError("User.Age",
                        _sharedLocalizer[ErrorMessages.Field,
                            _sharedLocalizer[DisplayNames.Age]]);
                }
                if (program.SchoolRequired && !model.SchoolId.HasValue
                    && !model.SchoolNotListed && !model.IsHomeschooled)
                {
                    ModelState.AddModelError("SchoolId",
                        _sharedLocalizer[ErrorMessages.Field,
                            _sharedLocalizer[DisplayNames.School]]);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (!askAge)
                    {
                        model.User.Age = null;
                    }
                    model.User.SchoolId = null;
                    model.User.SchoolNotListed = false;
                    model.User.IsHomeschooled = false;
                    if (askSchool)
                    {
                        if (model.IsHomeschooled)
                        {
                            model.User.IsHomeschooled = true;
                        }
                        else if (model.SchoolNotListed)
                        {
                            model.User.SchoolNotListed = true;
                        }
                        else
                        {
                            model.User.SchoolId = model.SchoolId;
                        }
                    }

                    if (askIfFirstTime)
                    {
                        model.User.IsFirstTime = model.IsFirstTime.Equals(DropDownTrueValue,
                           StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        model.User.IsFirstTime = false;
                    }

                    if (askEmailSubscription)
                    {
                        model.User.IsEmailSubscribed = DropDownTrueValue.Equals(
                            model.EmailSubscriptionRequested,
                            StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        model.User.IsEmailSubscribed = false;
                    }

                    if (askActivityGoal && model.User.DailyPersonalGoal > 0)
                    {
                        if (model.User.DailyPersonalGoal > Defaults.MaxDailyActivityGoal)
                        {
                            model.User.DailyPersonalGoal = Defaults.MaxDailyActivityGoal;
                        }
                    }
                    else
                    {
                        model.User.DailyPersonalGoal = null;
                    }

                    if (!askPersonalPointGoal
                        || model.User.PersonalPointGoal <= program.AchieverPointAmount)
                    {
                        model.User.PersonalPointGoal = null;
                    }
                    else
                    {
                        var (maximumPointsSet, maximumPointsValue) = await GetSiteSettingIntAsync(
                            SiteSettingKey.Users.MaximumActivityPermitted);
                        if (maximumPointsSet && model.User.PersonalPointGoal > maximumPointsValue)
                        {
                            model.User.PersonalPointGoal = maximumPointsValue;
                        }
                    }

                    var newMember = await _userService.AddHouseholdMemberAsync(authUser.Id,
                        model.User,
                        false,
                        false);
                    await _mailService.SendUserBroadcastsAsync(newMember.Id, false, true);
                    HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                    string groupTypeName
                        = await _userService.GetGroupFromHouseholdHeadAsync(authUser.Id) == null
                            ? Annotations.Interface.Family
                            : Annotations.Interface.Group;
                    ShowAlertSuccess(
                        _sharedLocalizer[Annotations.Interface.AddedParticipantGroupFamily,
                            model.User.FullName,
                            _sharedLocalizer[groupTypeName]]);
                    return RedirectToAction(nameof(Household));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(
                        _sharedLocalizer[Annotations.Validate.UnableToAdd, gex.Message]);
                }
            }
            var branchList = await _siteService.GetBranches(model.User.SystemId);
            if (model.User.BranchId < 1)
            {
                branchList = branchList.Prepend(new Branch { Id = -1 });
            }
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.SchoolList
                = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.ShowAge = askAge;
            model.ShowSchool = askSchool;

            if (askIfFirstTime)
            {
                model.AskFirstTime = EmptyNoYes();
            }

            if (askEmailSubscription)
            {
                model.AskEmailSubscription = EmptyNoYes();
                model.AskEmailSubscriptionText = askEmailSubscriptionText;
            }

            if (askActivityGoal)
            {
                var pointTranslation = programList.First().PointTranslation;
                model.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            if (askPersonalPointGoal)
            {
                model.AskPersonalPointGoal = true;

                var (maximumPointsSet, maximumPointsValue) = await GetSiteSettingIntAsync(
                    SiteSettingKey.Users.MaximumActivityPermitted);
                if (maximumPointsSet)
                {
                    model.MaximumPersonalPointGoal = maximumPointsValue;
                }

                program ??= programList.First();
                model.MinimumPersonalPointGoal = program.AchieverPointAmount;
            }

            return View("HouseholdAdd", model);
        }

        public async Task<IActionResult> Attachments(int page)
        {
            page = page == 0 ? 1 : page;
            User user = await _userService.GetDetails(GetActiveUserId());

            var filter = new UserLogFilter(page)
            {
                HasAttachment = true
            };

            var userLogs = await _userService.GetPaginatedUserHistoryAsync(user.Id, filter);

            var viewModel = new AttachmentListViewModel
            {
                Attachments = new List<AttachmentItemViewModel>(),
                UserLogs = userLogs.Data,
                ItemCount = userLogs.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username)
            };

            if (viewModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = viewModel.LastPage ?? 1
                    });
            }

            foreach (var userLog in userLogs.Data)
            {
                var item = new AttachmentItemViewModel
                {
                    AttachmentFilename
                        = _pathResolver.ResolveContentPath(userLog.AttachmentFilename),
                    Description = userLog.Description,
                    EarnedOn = userLog.CreatedAt.ToShortDateString(),
                    ShowCertificate = userLog.AttachmentIsCertificate
                };

                viewModel.Attachments.Add(item);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Badges(int page)
        {
            page = page == 0 ? 1 : page;

            User user = await _userService.GetDetails(GetActiveUserId());

            var filter = new UserLogFilter(page)
            {
                HasBadge = true
            };

            var userLogs = await _userService.GetPaginatedUserHistoryAsync(user.Id, filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = userLogs.Count,
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

            foreach (var userLog in userLogs.Data)
            {
                userLog.BadgeFilename = _pathResolver.ResolveContentPath(userLog.BadgeFilename);
            }

            var viewModel = new BadgeListViewModel
            {
                UserLogs = userLogs.Data,
                PaginateModel = paginateModel,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Books(string sort, string order, int page = 1)
        {
            var filter = new BookFilter(page);

            bool isDescending = string.Equals(order,
                "Descending",
                StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(sort) && Enum.IsDefined(typeof(SortBooksBy), sort))
            {
                filter.SortBy = (SortBooksBy)Enum.Parse(typeof(SortBooksBy), sort);
                filter.OrderDescending = isDescending;
            }

            var books = await _userService
                .GetPaginatedUserBookListAsync(GetActiveUserId(), filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = books.Count,
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

            User user = await _userService.GetDetails(GetActiveUserId());
            var viewModel = new BookListViewModel
            {
                Books = books.Data,
                PaginateModel = paginateModel,
                Sort = sort,
                IsDescending = isDescending,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                CanEditBooks = GetSiteStage() == SiteStage.ProgramOpen,
                SortBooks = Enum.GetValues(typeof(SortBooksBy))
            };

            return View(viewModel);
        }

        public IActionResult CancelGroupUpgrade()
        {
            if (HttpContext.Session.Keys.Contains(SessionKey.AbsorbUserId))
            {
                HttpContext.Session.Remove(SessionKey.AbsorbUserId);
            }
            return RedirectToAction(nameof(Household));
        }

        public async Task<IActionResult> ChangePassword()
        {
            User user = await _userService.GetDetails(GetActiveUserId());
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ChangePasswordViewModel
            {
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = true
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                User user = await _userService.GetDetails(GetActiveUserId());
                var loginAttempt = await _authenticationService
                    .AuthenticateUserAsync(user.Username, model.OldPassword);
                if (loginAttempt.PasswordIsValid)
                {
                    try
                    {
                        await _authenticationService.ResetPassword(GetActiveUserId(),
                            model.NewPassword);
                        ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.PasswordChanged]);
                        return RedirectToAction(nameof(ChangePassword));
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger(_sharedLocalizer[Annotations
                                .Validate
                                .UnableToChangePassword,
                            gex.Message]);
                    }
                }
                else
                {
                    model.ErrorMessage
                        = _sharedLocalizer[Annotations.Validate.Password];
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupUpgradeViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (string.IsNullOrEmpty(viewModel.GroupInfo?.Name?.Trim()))
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Required.GroupName]);
                return View("GroupUpgrade", viewModel);
            }

            try
            {
                viewModel.GroupInfo.UserId = GetId(ClaimType.UserId);
                await _userService.CreateGroup(viewModel.GroupInfo.UserId, viewModel.GroupInfo);
            }
            catch (GraException graex)
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotCreate,
                    _sharedLocalizer[Annotations.Interface.Group],
                    graex.Message]);
                return View("GroupUpgrade", viewModel);
            }
            HttpContext.Session.SetString(SessionKey.CallItGroup, "True");

            if (viewModel.AddExisting)
            {
                return await AddExistingPreAuth();
            }
            else
            {
                return RedirectToAction(nameof(AddHouseholdMember));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DonateCode(ProfileDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            await _vendorCodeService.ResolveCodeStatusAsync(viewModel.User.Id, true, false);
            return RedirectToAction(nameof(ProfileController.Index), ProfileController.Name);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(BookListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                try
                {
                    await _activityService.UpdateBookAsync(model.Book);
                    ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.UpdatedItem,
                        model.Book.Title]);
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotUpdate,
                        model.Book.Title,
                        gex.Message]);
                }
            }
            else
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotUpdate,
                    model.Book.Title,
                    _sharedLocalizer[Annotations.Required.Missing]]);
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction(nameof(Books), new { page });
        }

        public async Task<IActionResult> EmailAward(int? id)
        {
            try
            {
                User user = await _userService.GetDetails(id ?? GetActiveUserId());
                await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

                if (!user.NeedsToAnswerVendorCodeQuestion)
                {
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new EmailAwardViewModel
                {
                    Email = user.Email,
                    EmailAwardInstructions = user.EmailAwardInstructions,
                    Name = user.FullName,
                    UserId = id
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertDanger(gex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmailAward(EmailAwardViewModel emailAwardModel)
        {
            ArgumentNullException.ThrowIfNull(emailAwardModel);

            if (!string.IsNullOrWhiteSpace(emailAwardModel.Email)
                && !EmailValidator.Validate(emailAwardModel.Email.Trim()))
            {
                ModelState.AddModelError(nameof(emailAwardModel.Email),
                    _sharedLocalizer[Annotations.Validate.Email,
                        _sharedLocalizer[DisplayNames.EmailAddress]]);
            }

            if (!ModelState.IsValid)
            {
                User user = await _userService.GetDetails(emailAwardModel.UserId
                    ?? GetActiveUserId());
                await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

                emailAwardModel.Name = user.FullName;
                emailAwardModel.EmailAwardInstructions = user.EmailAwardInstructions;

                return View(emailAwardModel);
            }

            var userId = emailAwardModel.UserId ?? GetActiveUserId();

            await _vendorCodeService.ResolveCodeStatusAsync(userId,
                false,
                true,
                emailAwardModel.Email);

            if (emailAwardModel.UserId.HasValue)
            {
                return RedirectToAction(nameof(ProfileController.Household));
            }
            else
            {
                return RedirectToAction(nameof(ProfileController.Index));
            }
        }

        public async Task<IActionResult> GetUserBadgeInfo(int id)
        {
            var viewModel = new BadgeInfoViewModel();
            try
            {
                viewModel.UserLog = await _userService.GetUserLogByIdAsync(id);
                var badge = await _badgeService
                    .GetByIdAsync(viewModel.UserLog.BadgeId.Value);
                viewModel.UserLog.BadgeAltText = badge.AltText;

                if (viewModel.UserLog.ChallengeId.HasValue)
                {
                    var challenge = await _challengeService
                        .GetChallengeDetailsAsync(viewModel.UserLog.ChallengeId.Value);
                    var url = Url.Action(nameof(ChallengesController.Detail),
                        ChallengesController.Name,
                        new { id = challenge.Id });

                    viewModel.HtmlMessage = _sharedLocalizer[Annotations.Info.ChallengeBadgeEarned,
                        url,
                        challenge.Name];
                }
                else if (viewModel.UserLog.EventId.HasValue)
                {
                    var graEvent = await _eventService.GetDetails(viewModel.UserLog.EventId.Value);
                    var url = Url.Action(nameof(EventsController.Detail),
                        EventsController.Name,
                        new { id = graEvent.Id });

                    viewModel.HtmlMessage = _sharedLocalizer[Annotations.Info.EventBadgeEarned,
                        url,
                        graEvent.Name];
                }
                else
                {
                    viewModel.Message = viewModel.UserLog.Description;
                }

                var fileName = await _badgeService.GetBadgeFilenameAsync(
                    viewModel.UserLog.BadgeId.Value);
                viewModel.UserLog.BadgeFilename = _pathResolver.ResolveContentPath(fileName);
            }
            catch (GraException gex)
            {
                viewModel.Message = gex.Message;
            }

            return PartialView("_BadgeInfoPartial", viewModel);
        }

        public async Task<IActionResult> GroupDetails()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            var groupInfo = await _userService.GetGroupFromHouseholdHeadAsync(authUser.Id);

            if (groupInfo == null)
            {
                return RedirectToAction(nameof(Household));
            }
            else
            {
                return View("GroupDetails", new GroupInfo
                {
                    Id = groupInfo.Id,
                    Name = groupInfo.Name,
                    GroupTypeName = groupInfo.GroupTypeName
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GroupDetails(GroupInfo groupInfo)
        {
            ArgumentNullException.ThrowIfNull(groupInfo);

            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            groupInfo.UserId = authUser.Id;
            await _userService.UpdateGroupName(authUser.Id, groupInfo);
            return RedirectToAction(nameof(Household));
        }

        [HttpPost]
        public async Task<IActionResult> HandleHouseholdDonation(
            HouseholdListViewModel viewModel,
            string donateButton,
            string redeemButton)
        {
            int userId = 0;
            bool? donationStatus = null;
            if (!string.IsNullOrEmpty(donateButton))
            {
                donationStatus = true;
                userId = int.Parse(donateButton, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(redeemButton))
            {
                donationStatus = false;
                userId = int.Parse(redeemButton, CultureInfo.InvariantCulture);
            }
            if (userId == 0)
            {
                _logger.LogError("User {ActiveUserId} unsuccessfully attempted to change donation for user {UserId} to {DonationStatus}",
                    GetActiveUserId(),
                    userId,
                    donationStatus);
                ShowAlertDanger(_sharedLocalizer[Annotations.Validate.SomethingWentWrong]);
            }
            else
            {
                await _vendorCodeService.ResolveCodeStatusAsync(userId, donationStatus, false);
            }
            return RedirectToAction(nameof(ProfileController.Household), ProfileController.Name);
        }

        public async Task<IActionResult> History(int page)
        {
            page = page == 0 ? 1 : page;
            var filter = new UserLogFilter(page);

            var history = await _userService
                .GetPaginatedUserHistoryAsync(GetActiveUserId(), filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = history.Count,
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

            User user = await _userService.GetDetails(GetActiveUserId());

            var viewModel = new HistoryListViewModel
            {
                Historys = new List<HistoryItemViewModel>(),
                PaginateModel = paginateModel,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                TotalPoints = user.PointsEarned
            };

            foreach (var item in history.Data)
            {
                if (item.ChallengeId != null)
                {
                    var url = Url.Action("Detail", "Challenges", new { id = item.ChallengeId });
                    item.Description = $"<a target='_blank' href='{url}'>{item.Description}</a>";
                }
                var description = new StringBuilder(item.Description);
                var itemModel = new HistoryItemViewModel
                {
                    CreatedAt = item.CreatedAt.ToString("d", CultureInfo.InvariantCulture),
                    PointsEarned = item.PointsEarned,
                };
                if (!string.IsNullOrWhiteSpace(item.BadgeFilename))
                {
                    itemModel.BadgeFilename = _pathResolver.ResolveContentPath(item.BadgeFilename);
                    itemModel.BadgeAltText = item.BadgeAltText;
                }
                else if (item.AvatarBundleId.HasValue)
                {
                    var bundle = await _avatarService
                        .GetBundleByIdAsync(item.AvatarBundleId.Value, true);
                    if (bundle.AvatarItems.Count > 0)
                    {
                        itemModel.BadgeFilename = _pathResolver.ResolveContentPath(
                            bundle.AvatarItems.First().Thumbnail);
                        if (bundle.AvatarItems.Count > 1)
                        {
                            var bundleLink = Url.Action(nameof(AvatarController.Index),
                                AvatarController.Name,
                                new { bundle = item.AvatarBundleId.Value });
                            description.AppendFormat(
                                CultureInfo.InvariantCulture,
                                " <strong><a href=\"{0}\">{1}</a></strong>",
                                bundleLink,
                                _sharedLocalizer[Annotations.Interface.SeeItemsUnlocked]);
                            itemModel.BadgeAltText = _sharedLocalizer
                                [Annotations.Interface.AvatarBundleAltText, bundle.Name];
                        }
                    }
                }
                if (item.AttachmentId.HasValue
                    && !string.IsNullOrWhiteSpace(item.AttachmentFilename))
                {
                    itemModel.AttachmentId = item.AttachmentId.Value;
                    itemModel.ShowCertificate
                        = item.AttachmentIsCertificate && item.TriggerId.HasValue;
                    itemModel.AttachmentFilename
                        = _pathResolver.ResolveContentPath(item.AttachmentFilename);
                    itemModel.AttachmentDownload
                        = item.AttachmentFilename[item.AttachmentFilename.LastIndexOf('/')..]
                            .Trim('/');
                }
                itemModel.Description = description.ToString();
                viewModel.Historys.Add(itemModel);
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Household()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            var hasAccount = true;
            var activeUserId = GetActiveUserId();
            if (authUser.Id != activeUserId)
            {
                User activeUser = await _userService.GetDetails(activeUserId);
                if (string.IsNullOrWhiteSpace(activeUser.Username))
                {
                    hasAccount = false;
                }
            }

            User headUser = null;
            bool authUserIsHead = !authUser.HouseholdHeadUserId.HasValue;
            bool showVendorCodes = authUserIsHead && await _vendorCodeService.SiteHasCodesAsync();
            GroupInfo groupInfo = null;

            if (!authUserIsHead)
            {
                groupInfo = await _userService.GetGroupFromHouseholdHeadAsync(headUser?.Id
                    ?? (int)authUser.HouseholdHeadUserId);
                headUser = await _userService.GetDetails((int)authUser.HouseholdHeadUserId);
            }
            else
            {
                groupInfo = await _userService.GetGroupFromHouseholdHeadAsync(authUser.Id);
                authUser.HasNewMail = await _mailService.UserHasUnreadAsync(authUser.Id);
                if (showVendorCodes)
                {
                    await _vendorCodeService.PopulateVendorCodeStatusAsync(authUser);
                }
            }

            var household = await _userService
                .GetHouseholdAsync(authUser.HouseholdHeadUserId ?? authUser.Id, authUserIsHead,
                showVendorCodes, authUserIsHead);

            var siteStage = GetSiteStage();
            var viewModel = new HouseholdListViewModel
            {
                Users = household,
                HouseholdCount = household.Count(),
                HasAccount = hasAccount,
                Head = headUser ?? authUser,
                AuthUserIsHead = authUserIsHead,
                ActiveUser = activeUserId,
                CanLogActivity = siteStage == SiteStage.ProgramOpen,
                CanEditHousehold = siteStage == SiteStage.RegistrationOpen
                    || siteStage == SiteStage.ProgramOpen,
                DisableSecretCode
                    = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                ShowVendorCodes = showVendorCodes,
                PointTranslation = await _pointTranslationService
                        .GetByProgramIdAsync(authUser.ProgramId),
                LocalizedHouseholdTitle
                    = _sharedLocalizer[HttpContext.Items[ItemKey.HouseholdTitle].ToString()]
            };

            if (groupInfo != null)
            {
                viewModel.GroupName = groupInfo.Name;
                viewModel.GroupLeader = authUserIsHead && authUser.Id == activeUserId;
            }

            if (authUserIsHead)
            {
                var householdProgramIds = household.Select(_ => _.ProgramId).Distinct().ToList();
                if (!householdProgramIds.Contains(authUser.ProgramId))
                {
                    householdProgramIds.Add(authUser.ProgramId);
                }

                var site = await GetCurrentSiteAsync();
                var dailyImageDictionary = new Dictionary<int, DailyImageViewModel>();

                foreach (var programId in householdProgramIds)
                {
                    var program = await _siteService.GetProgramByIdAsync(programId);
                    if (program.DailyLiteracyTipId.HasValue)
                    {
                        var day = _siteLookupService.GetSiteDay(site);
                        if (day.HasValue)
                        {
                            var image = await _dailyLiteracyTipService.GetImageByDayAsync(
                                program.DailyLiteracyTipId.Value, day.Value);
                            if (image != null)
                            {
                                var imagePath = Path.Combine($"site{site.Id}", "dailyimages",
                                    $"dailyliteracytip{program.DailyLiteracyTipId}",
                                    $"{image.Name}{image.Extension}");
                                if (System.IO.File.Exists(_pathResolver
                                    .ResolveContentFilePath(imagePath)))
                                {
                                    var dailyLiteracyTip = await _dailyLiteracyTipService
                                        .GetByIdAsync(program.DailyLiteracyTipId.Value);
                                    var dailyImageViewModel = new DailyImageViewModel
                                    {
                                        DailyImageMessage = dailyLiteracyTip.Message,
                                        DailyImagePath
                                            = _pathResolver.ResolveContentPath(imagePath),
                                        IsLarge = dailyLiteracyTip.IsLarge,
                                        Day = day.Value
                                    };
                                    viewModel.DailyImageDictionary.Add(program.Id,
                                        dailyImageViewModel);
                                }
                            }
                        }
                    }
                }

                if (showVendorCodes)
                {
                    await _vendorCodeService.PopulateVendorCodeStatusAsync(viewModel.Head);
                }
            }

            if (TempData.TryGetValue(ActivityMessage, out object activityValue))
            {
                viewModel.ActivityMessage = (string)activityValue;
            }
            if (TempData.TryGetValue(SecretCodeMessage, out object secretCodeValue))
            {
                viewModel.SecretCodeMessage = (string)secretCodeValue;
            }

            if (string.IsNullOrWhiteSpace(viewModel.Head.EmailAwardInstructions))
            {
                viewModel.Head.EmailAwardInstructions = viewModel.Users
                    .Where(_ => !string.IsNullOrWhiteSpace(_.EmailAwardInstructions))
                    .Select(_ => _.EmailAwardInstructions)
                    .FirstOrDefault();
            }

            return View(viewModel);
        }

        public async Task<IActionResult> HouseholdApplyActivity(HouseholdListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userService.GetDetails(GetId(ClaimType.UserId));
            model.PointTranslation = await _pointTranslationService
                .GetByProgramIdAsync(user.ProgramId, true);
            if (model.ActivityAmount < 1 && !model.PointTranslation.IsSingleEvent)
            {
                TempData[ActivityMessage]
                    = _sharedLocalizer[Annotations.Required.MustEnterAmount].ToString();
            }
            else if (!string.IsNullOrWhiteSpace(model.UserSelection))
            {
                var userSelection = model.UserSelection
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .Distinct()
                    .ToList();
                try
                {
                    await _activityService.LogHouseholdActivityAsync(userSelection,
                        model.PointTranslation.IsSingleEvent
                            ? 1
                            : model.ActivityAmount);
                    ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.ActivityApplied]);
                }
                catch (GraException gex)
                {
                    TempData[ActivityMessage] = gex.Message;
                }
            }
            else
            {
                TempData[ActivityMessage]
                    = _sharedLocalizer[Annotations.Required.SelectFirst].ToString();
            }

            return RedirectToAction(nameof(Household));
        }

        public async Task<IActionResult> HouseholdApplySecretCode(HouseholdListViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.SecretCode))
            {
                TempData[SecretCodeMessage]
                    = _sharedLocalizer[Annotations.Required.SecretCode].ToString();
            }
            else if (!string.IsNullOrWhiteSpace(model?.UserSelection))
            {
                var userSelection = model.UserSelection
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .Distinct()
                    .ToList();
                try
                {
                    var codeApplied = await _activityService
                        .LogHouseholdSecretCodeAsync(userSelection, model.SecretCode);
                    if (codeApplied)
                    {
                        ShowAlertSuccess(_sharedLocalizer[Annotations
                            .Interface
                            .SecretCodeApplied]);
                    }
                    else
                    {
                        TempData[SecretCodeMessage]
                            = _sharedLocalizer[Annotations.Validate.CodeAlready].ToString();
                    }
                }
                catch (GraException gex)
                {
                    TempData[SecretCodeMessage] = gex.Message;
                }
            }
            else
            {
                TempData[SecretCodeMessage]
                    = _sharedLocalizer[Annotations.Required.SelectFirst].ToString();
            }

            return RedirectToAction(nameof(Household));
        }

        [HttpPost]
        public async Task<IActionResult> HouseholdRedeemCode(
            HouseholdListViewModel viewModel,
            string redeemButton)
        {
            int userId = int.Parse(redeemButton, CultureInfo.InvariantCulture);
            await _vendorCodeService.ResolveCodeStatusAsync(userId, false, false);
            return RedirectToAction(nameof(ProfileController.Household), ProfileController.Name);
        }

        public async Task<IActionResult> Index()
        {
            User user = await _userService.GetDetails(GetActiveUserId());

            int householdCount = await _userService
                .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id);
            var branchList = await _siteService.GetBranches(user.SystemId);
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var userProgram = programList.SingleOrDefault(_ => _.Id == user.ProgramId);
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

            await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

            var viewModel = new ProfileDetailViewModel
            {
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                HouseholdCount = householdCount,
                IsHomeschooled = user.IsHomeschooled,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                RestrictChangingProgram = await GetSiteSettingBoolAsync(SiteSettingKey
                    .Users
                    .RestrictChangingProgram),
                RestrictChangingSystemBranch = await GetSiteSettingBoolAsync(SiteSettingKey
                    .Users
                    .RestrictChangingSystemBranch),
                SchoolId = user.SchoolId,
                SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name"),
                SchoolNotListed = user.SchoolNotListed,
                ShowAge = userProgram.AskAge,
                ShowSchool = userProgram.AskSchool,
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                User = user
            };

            if (viewModel.RestrictChangingProgram)
            {
                viewModel.ProgramName = programList
                    .FirstOrDefault(_ => _.Id == viewModel.User.ProgramId)?
                    .Name;
            }

            if (viewModel.RestrictChangingSystemBranch)
            {
                viewModel.SystemName = systemList
                    .FirstOrDefault(_ => _.Id == viewModel.User.SystemId)?
                    .Name;
                viewModel.BranchName = branchList
                    .FirstOrDefault(_ => _.Id == viewModel.User.BranchId)?
                    .Name;
            }

            var (askEmailSubscription, askEmailSubscriptionText)
                = await GetSiteSettingStringAsync(SiteSettingKey.Users.AskEmailSubPermission);
            if (askEmailSubscription)
            {
                viewModel.AskEmailSubscription = true;
                viewModel.AskEmailSubscriptionText = askEmailSubscriptionText;
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
            if (askActivityGoal)
            {
                var pointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(user.ProgramId);
                viewModel.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            var askPersonalPointGoal = await GetSiteSettingBoolAsync(
                SiteSettingKey.Users.AskPersonalPointGoal);
            if (askPersonalPointGoal)
            {
                viewModel.AskPersonalPointGoal = true;
                viewModel.MinimumPersonalPointGoal = userProgram.AchieverPointAmount;

                var (maximumPointsSet, maximumPointsValue) = await GetSiteSettingIntAsync(
                    SiteSettingKey.Users.MaximumActivityPermitted);
                if (maximumPointsSet)
                {
                    viewModel.MaximumPersonalPointGoal = maximumPointsValue;
                }
            }

            if (viewModel.User.CannotBeEmailed)
            {
                ShowAlertWarning(_sharedLocalizer[Annotations.Validate.EmailCannotBeEmailed,
                    viewModel.User.BranchName]);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileDetailViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await GetCurrentSiteAsync();
            var program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);

            if (!string.IsNullOrWhiteSpace(model.User.Email)
                && !EmailValidator.Validate(model.User.Email.Trim()))
            {
                ModelState.AddModelError("User.Email",
                    _sharedLocalizer[Annotations.Validate.Email, 
                        _sharedLocalizer[DisplayNames.EmailAddress]]);
            }
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode",
                    _sharedLocalizer[ErrorMessages.Field,
                        _sharedLocalizer[DisplayNames.ZipCode]]);
            }
            if (program.AgeRequired && !model.User.Age.HasValue)
            {
                ModelState.AddModelError("User.Age",
                    _sharedLocalizer[ErrorMessages.Field,
                        _sharedLocalizer[DisplayNames.Age]]);
            }
            if (program.SchoolRequired && !model.SchoolId.HasValue && !model.SchoolNotListed
                && !model.IsHomeschooled)
            {
                ModelState.AddModelError("SchoolId",
                    _sharedLocalizer[ErrorMessages.Field,
                        _sharedLocalizer[DisplayNames.School]]);
            }

            var (askEmailSubscription, askEmailSubscriptionText) = await GetSiteSettingStringAsync(
                SiteSettingKey.Users.AskEmailSubPermission);
            if (askEmailSubscription && model.User.IsEmailSubscribed
                && string.IsNullOrWhiteSpace(model.User.Email))
            {
                ModelState.AddModelError("User.Email", " ");
                ModelState.AddModelError("User.IsEmailSubscribed",
                    _sharedLocalizer[Annotations.Validate.EmailSubscription]);
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);

            var askPersonalPointGoal = await GetSiteSettingBoolAsync(
                SiteSettingKey.Users.AskPersonalPointGoal);

            if (ModelState.IsValid)
            {
                try
                {
                    if (!program.AskAge)
                    {
                        model.User.Age = null;
                    }
                    model.User.SchoolId = null;
                    model.User.SchoolNotListed = false;
                    model.User.IsHomeschooled = false;
                    if (program.AskSchool)
                    {
                        if (model.IsHomeschooled)
                        {
                            model.User.IsHomeschooled = true;
                        }
                        else if (model.SchoolNotListed)
                        {
                            model.User.SchoolNotListed = true;
                        }
                        else
                        {
                            model.User.SchoolId = model.SchoolId;
                        }
                    }

                    if (!askEmailSubscription)
                    {
                        model.User.IsEmailSubscribed = false;
                    }

                    if (askActivityGoal && model.User.DailyPersonalGoal > 0)
                    {
                        if (model.User.DailyPersonalGoal > Defaults.MaxDailyActivityGoal)
                        {
                            model.User.DailyPersonalGoal = Defaults.MaxDailyActivityGoal;
                        }
                    }
                    else
                    {
                        model.User.DailyPersonalGoal = null;
                    }

                    if (!askPersonalPointGoal
                        || model.User.PersonalPointGoal <= program.AchieverPointAmount)
                    {
                        model.User.PersonalPointGoal = null;
                    }
                    else
                    {
                        var (maximumPointsSet, maximumPointsValue) = await GetSiteSettingIntAsync(
                            SiteSettingKey.Users.MaximumActivityPermitted);
                        if (maximumPointsSet && model.User.PersonalPointGoal > maximumPointsValue)
                        {
                            model.User.PersonalPointGoal = maximumPointsValue;
                        }
                    }

                    await _userService.Update(model.User);
                    AlertSuccess = _sharedLocalizer[GRA.Annotations.Interface.ProfileUpdated];
                    return RedirectToAction(nameof(Index));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to update profile: ", gex);
                }
            }
            var branchList = await _siteService.GetBranches(model.User.SystemId);
            if (model.User.BranchId < 1)
            {
                branchList = branchList.Prepend(new Branch() { Id = -1 });
            }
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.SchoolList
                = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.RestrictChangingProgram = await GetSiteSettingBoolAsync(SiteSettingKey
                    .Users
                    .RestrictChangingProgram);
            model.RestrictChangingSystemBranch = await GetSiteSettingBoolAsync(SiteSettingKey
                    .Users
                    .RestrictChangingSystemBranch);
            model.ShowAge = program.AskAge;
            model.ShowSchool = program.AskSchool;

            await _vendorCodeService.PopulateVendorCodeStatusAsync(model.User);

            if (model.RestrictChangingProgram)
            {
                model.ProgramName = programList
                    .FirstOrDefault(_ => _.Id == model.User.ProgramId)?
                    .Name;
            }

            if (model.RestrictChangingSystemBranch)
            {
                model.SystemName = systemList
                    .FirstOrDefault(_ => _.Id == model.User.SystemId)?
                    .Name;
                model.BranchName = branchList
                    .FirstOrDefault(_ => _.Id == model.User.BranchId)?
                    .Name;
            }

            if (askEmailSubscription)
            {
                model.AskEmailSubscription = true;
                model.AskEmailSubscriptionText = askEmailSubscriptionText;
            }

            if (askActivityGoal)
            {
                var pointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(model.User.ProgramId);
                model.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            if (askPersonalPointGoal)
            {
                model.AskPersonalPointGoal = true;
                model.MinimumPersonalPointGoal = program.AchieverPointAmount;

                var (maximumPointsSet, maximumPointsValue) = await GetSiteSettingIntAsync(
                    SiteSettingKey.Users.MaximumActivityPermitted);
                if (maximumPointsSet)
                {
                    model.MaximumPersonalPointGoal = maximumPointsValue;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAs(int loginId, bool goToMail)
        {
            User user = null;
            try
            {
                user = await _userService.GetDetails(loginId);
            }
            catch (GraException)
            {
                AlertDanger = "Not able to log in as that user. Please log in as the manager of your family/group.";
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }

            var authUser = GetId(ClaimType.UserId);
            var activeUser = GetActiveUserId();

            if ((user.Id == authUser || user.HouseholdHeadUserId == authUser)
                && activeUser != loginId)
            {
                HttpContext.Session.SetInt32(SessionKey.ActiveUserId, loginId);
                var questionnaireId = await _questionnaireService
                    .GetRequiredQuestionnaire(user.Id, user.Age);
                if (questionnaireId.HasValue)
                {
                    HttpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                        questionnaireId.Value);
                }
                else
                {
                    HttpContext.Session.Remove(SessionKey.PendingQuestionnaire);
                }
                ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.SignedInAs, user.FullName],
                    "user");
            }

            if (goToMail)
            {
                return RedirectToAction(nameof(MailController.Index), MailController.Name);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            HouseholdTitle = HttpContext.Items[ItemKey.HouseholdTitle] as string
                ?? Annotations.Interface.Family;
        }

        public async Task<IActionResult> Prizes(int page)
        {
            page = page == 0 ? 1 : page;

            var id = GetActiveUserId();

            var user = await _userService.GetDetails(id);

            var filter = new PrizeFilter(page)
            {
                UserIds = (await _userService
                    .GetHouseholdUserIdsAsync(user.HouseholdHeadUserId ?? id))
                    .ToList()
            };

            var prizeList = await _prizeWinnerService.PageUserPrizes(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = prizeList.Count,
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

            return View(new PrizeListViewModel
            {
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                HeadOfHouseholdId = user.HouseholdHeadUserId,
                HouseholdCount
                    = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                PaginateModel = paginateModel,
                PrizeWinners = prizeList.Data
            });
        }

        [HttpPost]
        public async Task<IActionResult> RedeemCode(ProfileDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            await _vendorCodeService.ResolveCodeStatusAsync(viewModel.User.Id, false, false);
            return RedirectToAction(nameof(ProfileController.Index), ProfileController.Name);
        }

        public IActionResult RegisterHouseholdMember()
        {
            return RedirectToAction(nameof(Household));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterHouseholdMember(HouseholdRegisterViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userService.GetDetails(model.RegisterId);
            var authUser = GetId(ClaimType.UserId);
            if (user.HouseholdHeadUserId != authUser || !string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction(nameof(Household));
            }

            if (model.Validate)
            {
                if (ModelState.IsValid)
                {
                    user.Username = model.Username;
                    try
                    {
                        await _userService.RegisterHouseholdMemberAsync(user, model.Password, false);
                        ShowAlertSuccess(
                            _sharedLocalizer[Annotations.Interface.AddedParticipantGroupFamily,
                                user.FullName,
                                _sharedLocalizer[HouseholdTitle]]);
                        return RedirectToAction(nameof(Household));
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger(_sharedLocalizer[Annotations.Validate.UnableToAdd,
                            gex.Message]);
                    }
                }
                return View("HouseholdRegisterMember", model);
            }
            else
            {
                ModelState.Clear();
                return View("HouseholdRegisterMember", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook(BookListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                await _activityService.RemoveBookAsync(model.Book.Id);
                ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.RemovedItem,
                    model.Book.Title]);
            }
            catch (GraException gex)
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotRemove,
                    model.Book.Title,
                    gex.Message]);
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction(nameof(Books), new { page });
        }

        public async Task<IActionResult> RemoveHouseholdMember(int id)
        {
            User member = null;
            try
            {
                member = await _userService.GetDetails(id);

                var viewModel = new HouseholdRemoveViewModel
                {
                    MemberId = member.Id,
                    MemberName = member.FullName,
                    MemberUsername = member.Username,
                    HouseholdTitle = HouseholdTitle
                };

                return View("HouseholdRemove", viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotRemove,
                    member?.FullName ?? _sharedLocalizer[Annotations.Title.Participant],
                    gex.Message]);
                return RedirectToAction(nameof(Household));
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveHouseholdMember(HouseholdRemoveViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var member = await _userService.GetDetails(model.MemberId);
            var memberUsername = member.Username;
            var registerMember = string.IsNullOrWhiteSpace(memberUsername);

            if (!registerMember)
            {
                ModelState.Remove("Username");
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (registerMember)
                    {
                        member.Username = model.Username;
                        await _userService
                            .RegisterHouseholdMemberAsync(member, model.Password, false);
                    }

                    if (GetActiveUserId() == member.Id)
                    {
                        var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));

                        HttpContext.Session.SetInt32(SessionKey.ActiveUserId, authUser.Id);

                        var questionnaireId = await _questionnaireService
                            .GetRequiredQuestionnaire(authUser.Id, authUser.Age);
                        if (questionnaireId.HasValue)
                        {
                            HttpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                                questionnaireId.Value);
                        }
                        else
                        {
                            HttpContext.Session.Remove(SessionKey.PendingQuestionnaire);
                        }
                    }

                    await _userService.RemoveFromHouseholdAsync(member.Id);
                    ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.RemovedItem,
                        member.FullName]);
                    return RedirectToAction(nameof(Household));
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotRemove,
                        member.FullName,
                        gex.Message]);
                }
            }

            model.MemberId = member.Id;
            model.MemberName = member.FullName;
            model.MemberUsername = memberUsername;
            model.HouseholdTitle = HouseholdTitle;
            return View("HouseholdRemove", model);
        }

        private async Task<IActionResult> AddExistingPreAuth()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction(nameof(Household));
            }

            if (!int.TryParse(HttpContext.Session.GetString(SessionKey.AbsorbUserId),
                out int userId))
            {
                return RedirectToAction(nameof(Household));
            }

            try
            {
                string addedMembers = await _userService
                    .AddParticipantToHouseholdAlreadyAuthorizedAsync(userId);
                HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                HttpContext.Session.Remove(SessionKey.AbsorbUserId);
                ShowAlertSuccess(
                    _sharedLocalizer[Annotations.Interface.AddedParticipantGroupFamily,
                    addedMembers,
                    HouseholdTitle]);
            }
            catch (GraException gex)
            {
                HttpContext.Session.Remove(SessionKey.AbsorbUserId);
                ShowAlertDanger(_sharedLocalizer[Annotations.Validate.UnableToAdd,
                    gex.Message]);
            }
            return RedirectToAction(nameof(Household));
        }
    }
}
