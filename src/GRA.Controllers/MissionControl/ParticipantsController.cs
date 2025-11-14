using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EmailValidation;
using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using GRA.Domain.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewParticipantList)]
    public class ParticipantsController : Base.MCController
    {
        private const string ActivityMessage = "ActivityMessage";
        private const string DrawingPrizeKey = "d";
        private const string SecretCodeMessage = "SecretCodeMessage";
        private const string TriggerPrizeKey = "t";
        private const string VendorCodeMessage = "VendorCodeMessage";
        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly AvatarService _avatarService;
        private readonly DrawingService _drawingService;
        private readonly EmailManagementService _emailManagementService;
        private readonly JobService _jobService;
        private readonly ILogger<ParticipantsController> _logger;
        private readonly MailService _mailService;
        private readonly MapsterMapper.IMapper _mapper;
        private readonly PointTranslationService _pointTranslationService;
        private readonly PrizeWinnerService _prizeWinnerService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly RoleService _roleService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly TriggerService _triggerService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public ParticipantsController(ILogger<ParticipantsController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            AuthenticationService authenticationService,
            AvatarService avatarService,
            DrawingService drawingService,
            EmailManagementService emailManagementService,
            JobService jobService,
            MailService mailService,
            PointTranslationService pointTranslationService,
            PrizeWinnerService prizeWinnerService,
            QuestionnaireService questionnaireService,
            RoleService roleService,
            SchoolService schoolService,
            SiteService siteService,
            TriggerService triggerService,
            UserService userService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(activityService);
            ArgumentNullException.ThrowIfNull(authenticationService);
            ArgumentNullException.ThrowIfNull(avatarService);
            ArgumentNullException.ThrowIfNull(drawingService);
            ArgumentNullException.ThrowIfNull(emailManagementService);
            ArgumentNullException.ThrowIfNull(jobService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(mailService);
            ArgumentNullException.ThrowIfNull(pointTranslationService);
            ArgumentNullException.ThrowIfNull(prizeWinnerService);
            ArgumentNullException.ThrowIfNull(questionnaireService);
            ArgumentNullException.ThrowIfNull(roleService);
            ArgumentNullException.ThrowIfNull(schoolService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(triggerService);
            ArgumentNullException.ThrowIfNull(userService);
            ArgumentNullException.ThrowIfNull(vendorCodeService);

            _activityService = activityService;
            _authenticationService = authenticationService;
            _avatarService = avatarService;
            _drawingService = drawingService;
            _emailManagementService = emailManagementService;
            _jobService = jobService;
            _logger = logger;
            _mailService = mailService;
            _mapper = context?.Mapper;
            _pointTranslationService = pointTranslationService;
            _prizeWinnerService = prizeWinnerService;
            _questionnaireService = questionnaireService;
            _roleService = roleService;
            _schoolService = schoolService;
            _siteService = siteService;
            _triggerService = triggerService;
            _userService = userService;
            _vendorCodeService = vendorCodeService;

            PageTitle = "Participants";
        }

        public static string Name
        { get { return "Participants"; } }

        #region Index

        public async Task<IActionResult> Add()
        {
            PageTitle = "Add Participant";
            var site = await GetCurrentSiteAsync();
            var siteStage = GetSiteStage();
            if (siteStage <= SiteStage.BeforeRegistration)
            {
                ShowAlertInfo("Registratin has not opened yet");
                return RedirectToAction("Index", "Participants");
            }
            else if (siteStage >= SiteStage.ProgramEnded)
            {
                ShowAlertInfo("The program has ended, participants cannot be added");
                return RedirectToAction("Index", "Participants");
            }

            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

            var viewModel = new ParticipantsAddViewModel
            {
                RequirePostalCode = site.RequirePostalCode,
                ProgramJson = JsonConvert.SerializeObject(programViewObject),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name")
            };

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (askIfFirstTime)
            {
                viewModel.AskFirstTime = EmptyNoYes();
            }

            var (askEmailSubscription, askEmailSubscriptionText) = await GetSiteSettingStringAsync(
                SiteSettingKey.Users.AskEmailSubPermission);
            if (askEmailSubscription)
            {
                viewModel.AskEmailSubscription = EmptyNoYes();
                viewModel.AskEmailSubscriptionText = askEmailSubscriptionText;
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
            if (askActivityGoal)
            {
                viewModel.DailyPersonalGoal = defaultDailyGoal;
                var pointTranslation = programList.First().PointTranslation;
                viewModel.TranslationDescriptionPastTense =
                    pointTranslation
                        .TranslationDescriptionPastTense
                        .Replace("{0}", "", StringComparison.OrdinalIgnoreCase).Trim();
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

            if (systemList.Count() == 1)
            {
                var systemId = systemList.Single().Id;
                var branchList = await _siteService.GetBranches(systemId);
                if (branchList.Count() > 1)
                {
                    branchList = branchList.Prepend(new Branch() { Id = -1 });
                }
                else
                {
                    viewModel.BranchId = branchList.SingleOrDefault()?.Id;
                }
                viewModel.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
                viewModel.SystemId = systemId;
            }

            if (programList.Count() == 1)
            {
                var programId = programList.Single().Id;
                var program = await _siteService.GetProgramByIdAsync(programId);
                viewModel.ProgramId = programList.SingleOrDefault()?.Id;
                viewModel.ShowAge = program.AskAge;
                viewModel.ShowSchool = program.AskSchool;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ParticipantsAddViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await GetCurrentSiteAsync();

            if (!string.IsNullOrWhiteSpace(model.Email)
                && !EmailValidator.Validate(model.Email.Trim()))
            {
                ModelState.AddModelError(nameof(model.Email),
                    string.Format(CultureInfo.InvariantCulture,
                        Annotations.Validate.Email,
                        DisplayNames.EmailAddress));
            }

            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.PostalCode))
            {
                ModelState.AddModelError("PostalCode", "The ZIP Code field is required.");
            }

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (!askIfFirstTime)
            {
                ModelState.Remove(nameof(model.IsFirstTime));
            }

            var (askEmailSubscription, askEmailSubscriptionText) = await GetSiteSettingStringAsync(
               SiteSettingKey.Users.AskEmailSubPermission);
            if (!askEmailSubscription)
            {
                ModelState.Remove(nameof(model.EmailSubscriptionRequested));
            }
            else
            {
                var subscriptionRequested = DropDownTrueValue.Equals(
                        model.EmailSubscriptionRequested, StringComparison.OrdinalIgnoreCase);
                if (subscriptionRequested && string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.AddModelError(nameof(model.Email), " ");
                    ModelState.AddModelError(nameof(model.EmailSubscriptionRequested),
                    "To receive email updates please supply an email address to send them to.");
                }
            }

            var (askActivityGoal, _) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
            var askPersonalPointGoal = await GetSiteSettingBoolAsync(
                SiteSettingKey.Users.AskPersonalPointGoal);

            bool askAge = false;
            bool askSchool = false;
            Program program = null;
            if (model.ProgramId.HasValue)
            {
                program = await _siteService.GetProgramByIdAsync(model.ProgramId.Value);
                askAge = program.AskAge;
                askSchool = program.AskSchool;
                if (program.AgeRequired && !model.Age.HasValue)
                {
                    ModelState.AddModelError("Age", "The Age field is required.");
                }
                if (program.SchoolRequired && !model.SchoolId.HasValue && !model.SchoolNotListed
                    && !model.IsHomeschooled)
                {
                    ModelState.AddModelError("SchoolId", "The School field is required.");
                }
            }

            if (ModelState.IsValid)
            {
                if (!askAge)
                {
                    model.Age = null;
                }
                if (!askSchool)
                {
                    model.SchoolId = null;
                    model.SchoolNotListed = false;
                    model.IsHomeschooled = false;
                }
                else if (model.IsHomeschooled)
                {
                    model.SchoolId = null;
                    model.SchoolNotListed = false;
                }
                else if (model.SchoolNotListed)
                {
                    model.SchoolId = null;
                }

                User user = _mapper.Map<User>(model);
                user.SiteId = site.Id;

                if (askIfFirstTime)
                {
                    user.IsFirstTime = model.IsFirstTime.Equals(DropDownTrueValue,
                        StringComparison.OrdinalIgnoreCase);
                }

                if (askEmailSubscription)
                {
                    user.IsEmailSubscribed = DropDownTrueValue.Equals(
                        model.EmailSubscriptionRequested, StringComparison.OrdinalIgnoreCase);
                }

                if (askActivityGoal && user.DailyPersonalGoal > 0)
                {
                    if (user.DailyPersonalGoal > Defaults.MaxDailyActivityGoal)
                    {
                        user.DailyPersonalGoal = Defaults.MaxDailyActivityGoal;
                    }
                }
                else
                {
                    user.DailyPersonalGoal = null;
                }

                if (!askPersonalPointGoal || user.PersonalPointGoal <= program.AchieverPointAmount)
                {
                    user.PersonalPointGoal = null;
                }
                else
                {
                    var (maximumPointsSet, maximumPointsValue) = await GetSiteSettingIntAsync(
                        SiteSettingKey.Users.MaximumActivityPermitted);
                    if (maximumPointsSet && user.PersonalPointGoal > maximumPointsValue)
                    {
                        user.PersonalPointGoal = maximumPointsValue;
                    }
                }

                try
                {
                    var newUser = await _userService.RegisterUserAsync(user,
                        model.Password,
                        true,
                        false);
                    await _mailService.SendUserBroadcastsAsync(newUser.Id, false, true);
                    if (UserHasPermission(Permission.EditParticipants))
                    {
                        return RedirectToAction(nameof(Detail), Name, new { id = newUser.Id });
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index), Name);
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Could not create participant account: ", gex);
                    if (gex.Message.Contains("password", StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("Password",
                            "Please correct the issues with the password.");
                    }
                }
            }
            PageTitle = "Add Participant";

            if (model.SystemId.HasValue)
            {
                var branchList = await _siteService.GetBranches(model.SystemId.Value);
                if (model.BranchId < 1)
                {
                    branchList = branchList.Prepend(new Branch() { Id = -1 });
                }
                model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            }
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name");
            model.ProgramJson = JsonConvert.SerializeObject(programViewObject);
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
                model.TranslationDescriptionPastTense =
                    pointTranslation
                        .TranslationDescriptionPastTense
                        .Replace("{0}", "", StringComparison.OrdinalIgnoreCase).Trim();
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

            return View(model);
        }

        [Authorize(Policy = Policy.DeleteParticipants)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.RemoveAsync(id);
                AlertSuccess = "Participant deleted";
                if (id == GetId(ClaimType.UserId))
                {
                    await LogoutUser();
                    return RedirectToAction("Index", "Home", new { area = string.Empty });
                }
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Could not delete participant: ", gex);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index(string search,
            string sort,
            string order,
            bool cannotBeEmailed,
            bool hasMultiplePrimaryVendorCodes,
            int? systemId,
            int? branchId,
            int? programId,
            int page)
        {
            page = page == 0 ? 1 : page;

            var filter = new UserFilter(page) 
            {
                CannotBeEmailed = cannotBeEmailed,
                HasMultiplePrimaryVendorCodes = hasMultiplePrimaryVendorCodes
            };

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter.Search = search.Trim();
            }
            if (branchId.HasValue)
            {
                filter.BranchIds = new List<int> { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                filter.SystemIds = new List<int> { systemId.Value };
            }
            if (programId.HasValue)
            {
                filter.ProgramIds = new List<int?> { programId.Value };
            }

            bool isDescending = string.Equals(order,
                "Descending",
                StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(sort) && Enum.IsDefined(typeof(SortUsersBy), sort))
            {
                filter.SortBy = (SortUsersBy)Enum.Parse(typeof(SortUsersBy), sort);
                filter.OrderDescending = isDescending;
            }

            var participantsList = await _userService.GetPaginatedUserListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = participantsList.Count,
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

            var viewModel = new ParticipantsListViewModel
            {
                BranchId = branchId,
                CannotBeEmailed = filter.CannotBeEmailed ?? false,
                CanRemoveParticipant = UserHasPermission(Permission.DeleteParticipants),
                CanViewDetails = UserHasPermission(Permission.ViewParticipantDetails),
                HasMultiplePrimaryVendorCodes = filter.HasMultiplePrimaryVendorCodes ?? false,
                HasVendorCodeManagement = UserHasPermission(Permission.ManageVendorCodes),
                IsDescending = isDescending,
                PaginateModel = paginateModel,
                ProgramId = programId,
                ProgramList = await _siteService.GetProgramList(),
                Search = search,
                Sort = sort,
                SortUsers = Enum.GetValues(typeof(SortUsersBy)),
                SystemId = systemId,
                SystemList = systemList,
                Users = participantsList.Data
            };

            if (branchId.HasValue)
            {
                var branch = await _siteService.GetBranchByIdAsync(branchId.Value);
                viewModel.BranchName = branch.Name;
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == branch.SystemId)?.Name;
                viewModel.BranchList = (await _siteService.GetBranches(branch.SystemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId))
                    .ThenBy(_ => _.Name);
                viewModel.ActiveNav = "Branch";
            }
            else if (systemId.HasValue)
            {
                viewModel.SystemName = systemList
                    .SingleOrDefault(_ => _.Id == systemId.Value)?.Name;
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
                viewModel.ProgramName =
                    (await _siteService.GetProgramByIdAsync(programId.Value)).Name;
            }

            if (UserHasPermission(Permission.AddParticipant))
            {
                var siteStage = GetSiteStage();
                if (siteStage == SiteStage.RegistrationOpen || siteStage == SiteStage.ProgramOpen)
                {
                    viewModel.CanSignUpParticipants = true;
                }
            }

            if (UserHasPermission(Permission.ViewGroupList)
                && await IsSiteSettingSetAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup))
            {
                viewModel.ShowGroupsButton = true;
            }

            return View(viewModel);
        }

        #endregion Index

        #region Detail

        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);
                var branchList = await _siteService.GetBranches(user.SystemId);
                var systemList = await _siteService.GetSystemList();
                var programList = await _siteService.GetProgramList();
                var userProgram = programList.SingleOrDefault(_ => _.Id == user.ProgramId);
                var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

                await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

                var viewModel = new ParticipantsDetailViewModel(await GetPopulatedBaseViewModel(user))
                {
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    CanViewParticipants = UserHasPermission(Permission.ViewParticipantDetails),
                    CreatedByName = await _userService.GetUsersNameByIdAsync(user.CreatedBy),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    IsHomeschooled = user.IsHomeschooled,
                    ProgramJson = JsonConvert.SerializeObject(programViewObject),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                    SchoolId = user.SchoolId,
                    SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(),
                        "Id",
                        "Name"),
                    SchoolNotListed = user.SchoolNotListed,
                    ShowAge = userProgram.AskAge,
                    ShowSchool = userProgram.AskSchool,
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                    User = user,
                    Username = user.Username
                };

                if (viewModel.SchoolId.HasValue)
                {
                    viewModel.School = await _schoolService.GetByIdAsync(viewModel.SchoolId.Value);
                }

                if (UserHasPermission(Permission.EditParticipantUsernames)
                    && !string.IsNullOrWhiteSpace(user.Username))
                {
                    viewModel.CanEditUsername = true;
                }

                var (askEmailSubscription, askEmailSubscriptionText) = await GetSiteSettingStringAsync(
                SiteSettingKey.Users.AskEmailSubPermission);
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
                    viewModel.TranslationDescriptionPastTense =
                        pointTranslation
                            .TranslationDescriptionPastTense
                            .Replace("{0}", "", StringComparison.OrdinalIgnoreCase).Trim();
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
                    ShowAlertWarning("This participant has an invalid email address and won't be able to receive emails until it is changed.");
                }

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
            ArgumentNullException.ThrowIfNull(model);

            var site = await GetCurrentSiteAsync();
            var program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);

            if (!string.IsNullOrWhiteSpace(model.User.Email)
                && !EmailValidator.Validate(model.User.Email.Trim()))
            {
                ModelState.AddModelError("User.Email",
                    string.Format(CultureInfo.InvariantCulture,
                        Annotations.Validate.Email,
                        DisplayNames.EmailAddress));
            }
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode", "The ZIP Code field is required.");
            }
            if (program.AgeRequired && !model.User.Age.HasValue)
            {
                ModelState.AddModelError("User.Age", "The Age field is required.");
            }
            if (program.SchoolRequired && !model.SchoolId.HasValue && !model.SchoolNotListed
                && !model.IsHomeschooled)
            {
                ModelState.AddModelError("SchoolId", "The School field is required.");
            }
            if (model.CanEditUsername && string.IsNullOrWhiteSpace(model.User.Username))
            {
                ModelState.AddModelError("User.Username", "The Username field is required.");
            }
            var (askEmailSubscription, askEmailSubscriptionText) = await GetSiteSettingStringAsync(
                SiteSettingKey.Users.AskEmailSubPermission);
            if (askEmailSubscription && model.User.IsEmailSubscribed
                && string.IsNullOrWhiteSpace(model.User.Email))
            {
                ModelState.AddModelError("User.Email", " ");
                ModelState.AddModelError("User.IsEmailSubscribed",
                "To receive email updates please supply an email address to send them to.");
            }

            var (askActivityGoal, _) = await GetSiteSettingIntAsync(
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

                    await _userService.MCUpdate(model.User);

                    AlertSuccess = "Participant infomation updated";
                    return RedirectToAction("Detail", new { id = model.User.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to update participant: ", gex);
                }
            }
            SetPageTitle(model.User, username: model.Username);

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
            model.SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name");
            model.ProgramJson = JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.ShowAge = program.AskAge;
            model.ShowSchool = program.AskSchool;

            await _vendorCodeService.PopulateVendorCodeStatusAsync(model.User);

            if (askEmailSubscription)
            {
                model.AskEmailSubscription = true;
                model.AskEmailSubscriptionText = askEmailSubscriptionText;
            }

            if (askActivityGoal)
            {
                var pointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(model.User.ProgramId);
                model.TranslationDescriptionPastTense =
                    pointTranslation
                        .TranslationDescriptionPastTense
                        .Replace("{0}", "", StringComparison.OrdinalIgnoreCase).Trim();
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

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> MarkVendorItemAsReceived(int id, string vendorCode)
        {
            var result = await _vendorCodeService
                .UnmarkDamagedMissingAsync(GetActiveUserId(), vendorCode);

            switch (result)
            {
                case true:
                    AlertSuccess = "Code unmarked as damaged/missing, email sent.";
                    break;

                case false:
                    AlertWarning = "Code unmarked as damaged/missing, email failed to send.";
                    break;

                default:
                    AlertInfo = "Code unmarked as damaged/missing, email not sent.";
                    break;
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        #endregion Detail

        #region Log Activity

        [Authorize(Policy = Policy.ManageVendorCodes)]
        [HttpPost]
        public async Task<IActionResult> AwardVendorCode(LogActivityViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (!model.VendorCodeTypeId.HasValue)
            {
                TempData[VendorCodeMessage] = "Please select a code to award.";
            }
            else
            {
                try
                {
                    await _activityService.MCAwardVendorCodeAsync(model.Id, model.VendorCodeTypeId.Value);
                    ShowAlertSuccess("Vendor Code awarded!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to award vendor code: ", gex.Message);
                }
            }

            return RedirectToAction("LogActivity", new { id = model.Id });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> LogActivity(int id)
        {
            var user = await _userService.GetDetailsByPermission(id);
            SetPageTitle(user);

            var viewModel = new LogActivityViewModel(await GetPopulatedBaseViewModel(user))
            {
                DisableSecretCode = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                HasPendingQuestionnaire = (await _questionnaireService
                    .GetRequiredQuestionnaire(user.Id, user.Age)).HasValue,
                OpenToLog = _activityService.IsOpenToLog(),
                PointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(user.ProgramId, true),
            };

            if (UserHasPermission(Permission.ManageVendorCodes))
            {
                viewModel.VendorCodeTypeList = new SelectList(
                    await _vendorCodeService.GetTypeAllAsync(), "Id", "Description");

                if (TempData.TryGetValue(VendorCodeMessage, out object value))
                {
                    ModelState.AddModelError("VendorCodeTypeId", (string)value);
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> LogActivity(LogActivityViewModel model, bool isSecretCode)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userService.GetDetailsByPermission(model.Id);
            model.PointTranslation = await _pointTranslationService
                .GetByProgramIdAsync(user.ProgramId, true);

            if (!model.IsSecretCode)
            {
                if ((!model.ActivityAmount.HasValue || model.ActivityAmount.Value < 1)
                    && !model.PointTranslation.IsSingleEvent)
                {
                    ModelState.AddModelError("ActivityAmount", "Enter a number greater than 0.");
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        var activityAmount = 1;
                        if (!model.PointTranslation.IsSingleEvent)
                        {
                            activityAmount = model.ActivityAmount.Value;
                        }
                        await _activityService.LogActivityAsync(model.Id, activityAmount);
                        ShowAlertSuccess("Activity applied!");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to apply activity: ", gex.Message);
                    }
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.SecretCode))
                {
                    ModelState.AddModelError("SecretCode", "Enter a secret code.");
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _activityService.LogSecretCodeAsync(model.Id, model.SecretCode);
                        ShowAlertSuccess("Secret Code applied!");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to apply secret code: ", gex.Message);
                    }
                }
            }

            return RedirectToAction(nameof(LogActivity), new { id = model.Id });
        }

        #endregion Log Activity

        #region Household

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> AddHouseholdMember(int id)
        {
            try
            {
                var headOfHousehold = await _userService.GetDetailsByPermission(id);
                if (headOfHousehold.HouseholdHeadUserId != null)
                {
                    headOfHousehold = await _userService
                        .GetDetailsByPermission((int)headOfHousehold.HouseholdHeadUserId);
                }

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(headOfHousehold.Id);

                string callIt = groupInfo == null ? "Family" : "Group";
                SetPageTitle(headOfHousehold, $"Add {callIt} Member");

                var userBase = new User
                {
                    LastName = headOfHousehold.LastName,
                    PostalCode = headOfHousehold.PostalCode,
                    Email = headOfHousehold.Email,
                    PhoneNumber = headOfHousehold.PhoneNumber,
                    BranchId = headOfHousehold.BranchId,
                    SystemId = headOfHousehold.SystemId
                };

                var branchList = await _siteService.GetBranches(headOfHousehold.SystemId);
                var systemList = await _siteService.GetSystemList();
                var programList = await _siteService.GetProgramList();
                var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

                var viewModel = new HouseholdAddViewModel
                {
                    User = userBase,
                    Id = id,
                    RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                    ProgramJson = JsonConvert.SerializeObject(programViewObject),
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(),
                        "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name")
                };

                var askIfFirstTime
                    = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
                if (askIfFirstTime)
                {
                    viewModel.AskFirstTime = EmptyNoYes();
                }

                var (askEmailSubscription, askEmailSubscriptionText) =
                    await GetSiteSettingStringAsync(SiteSettingKey.Users.AskEmailSubPermission);
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
                    viewModel.TranslationDescriptionPastTense =
                        pointTranslation
                            .TranslationDescriptionPastTense
                            .Replace("{0}", "", StringComparison.OrdinalIgnoreCase).Trim();
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
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's family/group: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> AddHouseholdMember(HouseholdAddViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await GetCurrentSiteAsync();
            var headOfHousehold = await _userService.GetDetailsByPermission(model.Id);
            if (headOfHousehold.HouseholdHeadUserId != null)
            {
                headOfHousehold = await _userService
                    .GetDetailsByPermission((int)headOfHousehold.HouseholdHeadUserId);
            }

            if (!string.IsNullOrWhiteSpace(model.User.Email)
                && !EmailValidator.Validate(model.User.Email.Trim()))
            {
                ModelState.AddModelError("User.Email",
                    string.Format(CultureInfo.InvariantCulture,
                        Annotations.Validate.Email,
                        DisplayNames.EmailAddress));
            }

            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode", "The ZIP Code field is required.");
            }

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (!askIfFirstTime)
            {
                ModelState.Remove(nameof(model.IsFirstTime));
            }

            var (askEmailSubscription, askEmailSubscriptionText) = await GetSiteSettingStringAsync(
                SiteSettingKey.Users.AskEmailSubPermission);
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
                    "To receive email updates please supply an email address to send them to.");
                }
            }

            var (askActivityGoal, _) = await GetSiteSettingIntAsync(
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
                    ModelState.AddModelError("User.Age", "The Age field is required.");
                }
                if (program.SchoolRequired && !model.SchoolId.HasValue && !model.SchoolNotListed
                    && !model.IsHomeschooled)
                {
                    ModelState.AddModelError("SchoolId", "The School field is required.");
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
                            model.EmailSubscriptionRequested, StringComparison.OrdinalIgnoreCase);
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

                    var newMember = await _userService.AddHouseholdMemberAsync(headOfHousehold.Id,
                        model.User,
                        false,
                        true);
                    await _mailService.SendUserBroadcastsAsync(newMember.Id, false, true);
                    AlertSuccess = "Added family/group member";
                    return RedirectToAction("Household", new { id = model.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to add family/group member: ", gex);
                }
            }
            SetPageTitle(headOfHousehold, "Add Family/Group Member");

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
            model.SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name");
            model.ProgramJson = JsonConvert.SerializeObject(programViewObject);
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
                model.TranslationDescriptionPastTense =
                    pointTranslation
                        .TranslationDescriptionPastTense
                        .Replace("{0}", "", StringComparison.OrdinalIgnoreCase).Trim();
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

        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Household(int id)
        {
            try
            {
                var user = await _userService.GetDetailsByPermission(id);
                var showVendorCodes = await _vendorCodeService.SiteHasCodesAsync();
                SetPageTitle(user);

                var head = user.HouseholdHeadUserId.HasValue
                    ? await _userService.GetDetailsByPermission(user.HouseholdHeadUserId.Value)
                    : user;

                if (showVendorCodes)
                {
                    await _vendorCodeService.PopulateVendorCodeStatusAsync(head);
                }

                head.HasPendingQuestionnaire = (await _questionnaireService
                    .GetRequiredQuestionnaire(head.Id, head.Age)).HasValue;
                bool ReadAllMail = UserHasPermission(Permission.ReadAllMail);
                bool ViewUserPrizes = UserHasPermission(Permission.ViewUserPrizes);
                if (ReadAllMail)
                {
                    await _mailService.SendUserBroadcastsAsync(head.Id, true);
                    head.HasNewMail = await _mailService.UserHasUnreadAsync(head.Id);
                }
                if (ViewUserPrizes)
                {
                    head.HasUnclaimedPrize = (await _prizeWinnerService
                        .GetUserWinCount(head.Id, false)) > 0;
                }

                var household = await _userService.GetHouseholdAsync(head.Id, true, showVendorCodes,
                    ReadAllMail, ViewUserPrizes);

                var systemId = GetId(ClaimType.SystemId);
                var branchList = (await _siteService.GetBranches(systemId))
                    .OrderByDescending(_ => _.Id == GetId(ClaimType.BranchId));
                var systemList = (await _siteService.GetSystemList())
                    .OrderByDescending(_ => _.Id == systemId);

                var viewModel = new HouseholdListViewModel(await GetPopulatedBaseViewModel(user))
                {
                    BranchList = branchList,
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    CanImportNewMembers = UserHasPermission(Permission.ImportHouseholdMembers),
                    OpenToLog = _activityService.IsOpenToLog(),
                    CanLogActivity = UserHasPermission(Permission.LogActivityForAny),
                    CanReadMail = ReadAllMail,
                    CanRedeemBulkVendorCodes = UserHasPermission(Permission.RedeemBulkVendorCodes),
                    CanViewPrizes = ViewUserPrizes,
                    DisableSecretCode
                        = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                    Head = head,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    PointTranslation
                        = await _pointTranslationService.GetByProgramIdAsync(user.ProgramId, true),
                    ShowVendorCodes = showVendorCodes,
                    SystemId = systemId,
                    SystemList = systemList,
                    Users = household,
                };

                if (ViewUserPrizes)
                {
                    viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);

                    var prizeList = await _prizeWinnerService
                        .GetHouseholdUnredeemedPrizesAsync(head.Id);
                    var prizeSelectList = new List<SelectListItem>();
                    foreach (var prize in prizeList)
                    {
                        var prizeName = $"{prize.Name} ({prize.Count})";
                        string prizeId;
                        if (prize.DrawingId.HasValue)
                        {
                            prizeId = $"{DrawingPrizeKey}{prize.DrawingId}";
                        }
                        else
                        {
                            prizeId = $"{TriggerPrizeKey}{prize.TriggerId}";
                        }
                        prizeSelectList.Add(new SelectListItem(prizeName, prizeId));
                    }

                    viewModel.HouseholdPrizeList = prizeSelectList;
                }

                if (TempData.TryGetValue(ActivityMessage, out object activityMessageValue))
                {
                    viewModel.ActivityMessage = (string)activityMessageValue;
                }
                if (TempData.TryGetValue(SecretCodeMessage, out object secretCodeValue))
                {
                    viewModel.SecretCodeMessage = (string)secretCodeValue;
                }

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(head.Id);

                if (groupInfo != null)
                {
                    viewModel.GroupName = groupInfo.Name;
                    viewModel.GroupType = groupInfo.GroupTypeName;
                    viewModel.IsGroup = true;
                }
                else
                {
                    var (useGroups, maximumHousehold) = await GetSiteSettingIntAsync(SiteSettingKey
                        .Users
                        .MaximumHouseholdSizeBeforeGroup);
                    viewModel.UseGroups = useGroups;
                    if (useGroups && household.Count() + 1 >= maximumHousehold)
                    {
                        viewModel.UpgradeToGroup = true;
                    }
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
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's family/group: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> HouseholdAddExistingParticipant(int Id,
            int userToAddId)
        {
            try
            {
                await _userService.MCAddParticipantToHouseholdAsync(Id, userToAddId);
                ShowAlertSuccess("Participant has been added to family/group!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add participant to family/group: ", gex);
            }

            return RedirectToAction("Household", new { id = Id });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> HouseholdApplyActivity(HouseholdListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userService.GetDetailsByPermission(model.Id);
            model.PointTranslation = await _pointTranslationService
                .GetByProgramIdAsync(user.ProgramId, true);
            if (model.ActivityAmount < 1 && !model.PointTranslation.IsSingleEvent)
            {
                TempData[ActivityMessage] = "You must enter an amount!";
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
                    var activityAmount = 1;
                    if (!model.PointTranslation.IsSingleEvent)
                    {
                        activityAmount = model.ActivityAmount;
                    }
                    await _activityService.LogHouseholdActivityAsync(userSelection, activityAmount);
                    ShowAlertSuccess("Activity applied!");
                }
                catch (GraException gex)
                {
                    TempData[ActivityMessage] = gex.Message;
                }
            }
            else
            {
                TempData[ActivityMessage] = "No family/group members selected.";
            }

            return RedirectToAction("Household", new { id = model.Id });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> HouseholdApplySecretCode(HouseholdListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrWhiteSpace(model.SecretCode))
            {
                TempData[SecretCodeMessage] = Annotations.Required.SecretCode;
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
                    var codeApplied = await _activityService
                        .LogHouseholdSecretCodeAsync(userSelection, model.SecretCode);
                    if (codeApplied)
                    {
                        ShowAlertSuccess("Secret Code applied!");
                    }
                    else
                    {
                        TempData[SecretCodeMessage]
                            = "All selected members have already entered that Secret Code.";
                    }
                }
                catch (GraException gex)
                {
                    TempData[SecretCodeMessage] = gex.Message;
                }
            }
            else
            {
                TempData[SecretCodeMessage] = "No family/group members selected.";
            }

            return RedirectToAction("Household", new { id = model.Id });
        }

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> HouseholdGetParticipantsList(int userId,
            int? systemId,
            int? branchId,
            string search,
            int page = 1)
        {
            var filter = new UserFilter(page, 10)
            {
                UserIds = new List<int>() { userId },
                Search = search,
                CanAddToHousehold = true,
            };
            if (branchId.HasValue)
            {
                filter.BranchIds = new List<int>() { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                filter.SystemIds = new List<int>() { systemId.Value };
            }

            var participants = await _userService.GetPaginatedUserListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = participants.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            var viewModel = new ParticipantsListViewModel
            {
                Users = participants.Data,
                PaginateModel = paginateModel
            };

            return PartialView("_ParticipantListPartial", viewModel);
        }

        [Authorize(Policy = Policy.ImportHouseholdMembers)]
        public async Task<IActionResult> HouseholdImport(int id)
        {
            try
            {
                _userService.VerifyCanHouseholdAction();

                var headOfHousehold = await _userService.GetDetailsByPermission(id);
                if (headOfHousehold.HouseholdHeadUserId != null)
                {
                    return RedirectToAction(nameof(HouseholdImport),
                        new { id = headOfHousehold.Id });
                }

                var branchList = await _siteService.GetBranches(headOfHousehold.SystemId);
                var systemList = await _siteService.GetSystemList();
                var programList = await _siteService.GetProgramList();
                var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

                var viewModel = new HouseholdImportViewModel
                {
                    Id = id,
                    SystemId = headOfHousehold.SystemId,
                    BranchId = headOfHousehold.BranchId,
                    ProgramJson = JsonConvert.SerializeObject(programViewObject),
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(),
                            "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name")
                };

                var askIfFirstTime
                        = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
                if (askIfFirstTime)
                {
                    viewModel.AskFirstTime = EmptyNoYes();
                }

                var groupInfo = await _userService
                    .GetGroupFromHouseholdHeadAsync(headOfHousehold.Id);

                string callIt = groupInfo == null ? "Family" : "Group";
                SetPageTitle(headOfHousehold, $"Import {callIt} Members");

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to edit participant's family/group: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.ImportHouseholdMembers)]
        [HttpPost]
        public async Task<IActionResult> HouseholdImport(HouseholdImportViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                _userService.VerifyCanHouseholdAction();
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to edit participant's family/group: ", gex);
                return RedirectToAction("Index");
            }

            var headOfHousehold = await _userService.GetDetailsByPermission(model.Id);
            if (headOfHousehold.HouseholdHeadUserId != null)
            {
                headOfHousehold = await _userService
                    .GetDetailsByPermission(headOfHousehold.HouseholdHeadUserId.Value);
            }

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (!askIfFirstTime)
            {
                ModelState.Remove(nameof(model.IsFirstTime));
            }

            Program program = null;
            if (model.ProgramId.HasValue)
            {
                program = await _siteService.GetProgramByIdAsync(model.ProgramId.Value);
                if (program.SchoolRequired && !model.SchoolId.HasValue && !model.SchoolNotListed
                    && !model.IsHomeschooled)
                {
                    ModelState.AddModelError("SchoolId", "The School field is required.");
                }
            }

            if (model.UserExcelFile != null && !ValidFiles.ExcelExtensions
                    .Contains(Path.GetExtension(model.UserExcelFile.FileName),
                        StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("UserExcelFile",
                    $"File must be one of the following types: {string.Join(", ", ValidFiles.ExcelExtensions)}");
            }

            if (ModelState.IsValid)
            {
                var tempFile = _pathResolver.ResolvePrivateTempFilePath();

                await using (var fileStream = new FileStream(tempFile, FileMode.Create))
                {
                    await model.UserExcelFile.CopyToAsync(fileStream);
                }

                string file = WebUtility.UrlEncode(Path.GetFileName(tempFile));

                var jobModel = new JobDetailsHouseholdImport
                {
                    Filename = file,
                    HeadOfHouseholdId = headOfHousehold.Id,
                    ProgramId = program.Id,
                    FirstTimeParticipating = askIfFirstTime && model.IsFirstTime?
                        .Equals(DropDownTrueValue, StringComparison.OrdinalIgnoreCase) == true
                };

                if (program.AskSchool)
                {
                    if (model.IsHomeschooled)
                    {
                        jobModel.IsHomeSchooled = true;
                    }
                    else if (model.SchoolNotListed)
                    {
                        jobModel.SchoolNotListed = true;
                    }
                    else
                    {
                        jobModel.SchoolId = model.SchoolId;
                    }
                }

                var jobToken = await _jobService.CreateJobAsync(new Job
                {
                    JobType = JobType.HouseholdImport,
                    SerializedParameters = JsonConvert
                        .SerializeObject(jobModel)
                });

                return View("Job", new ViewModel.MissionControl.Shared.JobViewModel
                {
                    CancelUrl = Url.Action(nameof(HouseholdImport)),
                    JobToken = jobToken.ToString(),
                    PingSeconds = 5,
                    SuccessRedirectUrl = "",
                    SuccessUrl = Url.Action(nameof(HouseholdImportComplete),
                        new { id = headOfHousehold.Id }),
                    Title = "Loading import..."
                });
            }

            var groupInfo = await _userService.GetGroupFromHouseholdHeadAsync(headOfHousehold.Id);
            string callIt = groupInfo == null ? "Family" : "Group";

            model.BranchList = new SelectList(await _siteService
                    .GetBranches(headOfHousehold.SystemId), "Id", "Name");
            model.SystemList = new SelectList(await _siteService.GetSystemList(), "Id", "Name");

            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);
            model.ProgramList = new SelectList(programList, "Id", "Name");
            model.ProgramJson = JsonConvert.SerializeObject(programViewObject);

            if (askIfFirstTime)
            {
                model.AskFirstTime = EmptyNoYes();
            }

            SetPageTitle(headOfHousehold, $"Import {callIt} Members");

            return View(model);
        }

        public async Task<IActionResult> HouseholdImportComplete(int id)
        {
            var groupInfo = await _userService.GetGroupFromHouseholdHeadAsync(id);
            if (groupInfo == null)
            {
                return RedirectToAction(nameof(Household), new { id });
            }
            else
            {
                return RedirectToAction(nameof(UpdateGroup), new { id });
            }
        }

        [Authorize(Policy = Policy.ViewUserPrizes)]
        public async Task<IActionResult> HouseholdPrize(int id, string prize)
        {
            try
            {
                if (prize == null || prize.Length < 2)
                {
                    throw new GraException("No prize selected");
                }

                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);

                var head = user.HouseholdHeadUserId.HasValue
                    ? await _userService.GetDetailsByPermission(user.HouseholdHeadUserId.Value)
                    : user;

                var prizeKey = prize[..1];
                var prizeValue = prize[1..];

                int? drawingId = null;
                int? triggerId = null;
                var prizeName = string.Empty;

                if (int.TryParse(prizeValue, out int prizeId))
                {
                    if (string.Equals(prizeKey, DrawingPrizeKey,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var drawingName = await _drawingService.GetDrawingNameAsync(prizeId);
                        if (string.IsNullOrWhiteSpace(drawingName))
                        {
                            throw new GraException("Prize not found.");
                        }

                        drawingId = prizeId;
                        prizeName = drawingName;
                    }
                    else if (string.Equals(prizeKey, TriggerPrizeKey,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        var triggerName = await _triggerService.GetTriggerPrizeNameAsync(prizeId);
                        if (string.IsNullOrWhiteSpace(triggerName))
                        {
                            throw new GraException("Prize not found.");
                        }

                        triggerId = prizeId;
                        prizeName = triggerName;
                    }
                    else
                    {
                        throw new GraException("Invalid prize selected");
                    }
                }
                else
                {
                    throw new GraException("Invalid prize selected");
                }

                var users = await _userService.GetHouseholdUsersWithAvailablePrizeAsync(head.Id,
                    drawingId, triggerId);

                if (users.Count == 0)
                {
                    throw new GraException("No members with the specified prize.");
                }

                var viewModel = new HouseholdPrizeViewModel
                {
                    Id = id,
                    DrawingId = drawingId,
                    TriggerId = triggerId,
                    PrizeName = prizeName,
                    Users = users
                };

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(head.Id);

                if (groupInfo != null)
                {
                    viewModel.GroupName = groupInfo.Name;
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to redeem household prize: ", gex);
                return RedirectToAction(nameof(Household), new { id });
            }
        }

        [Authorize(Policy = Policy.ViewUserPrizes)]
        [HttpPost]
        public async Task<IActionResult> HouseholdPrize(HouseholdPrizeViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userService.GetDetailsByPermission(model.Id);
            SetPageTitle(user);

            var head = user.HouseholdHeadUserId.HasValue
                ? await _userService.GetDetailsByPermission(user.HouseholdHeadUserId.Value)
                : user;

            var selectedUsers = model.UserSelection
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(int.Parse)
                    .Distinct()
                    .ToList();

            var users = await _userService.GetHouseholdUsersWithAvailablePrizeAsync(head.Id,
                    model.DrawingId, model.TriggerId);

            var prizeUsers = users.Where(_ => selectedUsers.Contains(_.Id));
            if (prizeUsers.Any())
            {
                int redeemedCount = 0;
                int unredeemedCount = 0;
                foreach (var prizeUser in prizeUsers)
                {
                    PrizeWinner prize;
                    if (model.DrawingId.HasValue)
                    {
                        prize = await _prizeWinnerService
                            .GetUserDrawingPrizeAsync(prizeUser.Id, model.DrawingId.Value);
                    }
                    else
                    {
                        prize = await _prizeWinnerService
                            .GetUserTriggerPrizeAsync(prizeUser.Id, model.TriggerId.Value);
                    }

                    try
                    {
                        await _prizeWinnerService.RedeemPrizeAsync(prize.Id, prize.StaffNotes);
                        redeemedCount++;
                    }
                    catch (GraException)
                    {
                        unredeemedCount++;
                    }
                }

                if (redeemedCount == 0)
                {
                    ShowAlertDanger($"Prize <strong>{model.PrizeName}</strong> could not be redeemed for any of the selected members.");
                }
                else if (unredeemedCount == 0)
                {
                    ShowAlertSuccess($"Prize <strong>{model.PrizeName}</strong> redeemed for <strong>{redeemedCount}</strong> members.");
                }
                else
                {
                    ShowAlertWarning($"Prize <strong>{model.PrizeName}</strong> redeemed for <strong>{redeemedCount}</strong> of <strong>{prizeUsers.Count()}</strong> selected members.");
                }

                return RedirectToAction(nameof(Household), new { id = model.Id });
            }

            ShowAlertDanger("No members were selected");
            model.Users = users;

            var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(head.Id);

            if (groupInfo != null)
            {
                model.GroupName = groupInfo.Name;
            }

            return View(model);
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> HouseholdPromote(int id, int promoteId)
        {
            try
            {
                await _userService.PromoteToHeadOfHouseholdAsync(promoteId);
                ShowAlertSuccess("Participant promoted to head of household.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Could not promote to family/group manager: ", gex.Message);
            }
            return RedirectToAction("Household", new { id });
        }

        [Authorize(Policy = Policy.EditParticipants)]
        [HttpPost]
        public async Task<IActionResult> HouseholdRemove(int id, int removeId)
        {
            try
            {
                await _userService.RemoveFromHouseholdAsync(removeId);
                ShowAlertSuccess("Participant removed from family/group.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Could not remove from family/group: ", gex.Message);
            }
            return RedirectToAction("Household", new { id });
        }

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> RegisterHouseholdMember(int id)
        {
            try
            {
                var user = await _userService.GetDetailsByPermission(id);
                if (!string.IsNullOrWhiteSpace(user.Username))
                {
                    return RedirectToAction("Household", new { id });
                }
                SetPageTitle(user, "Register Family Member");

                var viewModel = new HouseholdRegisterViewModel
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
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userService.GetDetailsByPermission(model.Id);
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction("Household", new { id = model.Id });
            }
            if (ModelState.IsValid)
            {
                user.Username = model.Username;
                try
                {
                    await _userService.RegisterHouseholdMemberAsync(user, model.Password, true);
                    AlertSuccess = "Family/group member registered!";
                    return RedirectToAction("Household", new { id = model.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to register family/group member: ", gex);
                }
            }
            SetPageTitle(user, "Register Family/Group Member");
            return View("HouseholdRegister", model);
        }

        #endregion Household

        #region Books

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> AddBook(BookListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _activityService.AddBookAsync(model.Id, model.Book, true);
                    if (result.Status == ServiceResultStatus.Warning
                            && !string.IsNullOrWhiteSpace(result.Message))
                    {
                        ShowAlertWarning(result.Message);
                    }
                    else if (result.Status == ServiceResultStatus.Success)
                    {
                        ShowAlertSuccess($"Added book '{model.Book.Title}'");
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to add book for participant: ", gex);
                }
            }
            else
            {
                ShowAlertDanger("Unable to add book for participant: Missing required fields");
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction("Books", new { id = model.Id, page });
        }

        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Books(int id, string sort, string order, int page = 1)
        {
            try
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
                    .GetPaginatedUserBookListAsync(id, filter);

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

                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);

                var viewModel = new BookListViewModel(await GetPopulatedBaseViewModel(user))
                {
                    Books = books.Data.ToList(),
                    CanEditBooks = UserHasPermission(Permission.LogActivityForAny),
                    HasPendingQuestionnaire
                        = (await _questionnaireService.GetRequiredQuestionnaire(user.Id,
                            user.Age)).HasValue,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    IsDescending = isDescending,
                    OpenToLog = _activityService.IsOpenToLog(),
                    PaginateModel = paginateModel,
                    Sort = sort,
                    SortBooks = Enum.GetValues(typeof(SortBooksBy)),
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
        public async Task<IActionResult> EditBook(BookListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                try
                {
                    await _activityService.UpdateBookAsync(model.Book, model.Id);
                    ShowAlertSuccess($"'{model.Book.Title}' updated!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit book for participant: ", gex.Message);
                }
            }
            else
            {
                ShowAlertDanger("Unable to edit book for participant: Missing required fields");
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction("Books", new { id = model.Id, page });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> RemoveBook(BookListViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            try
            {
                await _activityService.RemoveBookAsync(model.Book.Id, model.Id);
                ShowAlertSuccess($"'{model.Book.Title}' removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to remove book for participant: ", gex.Message);
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction("Books", new { id = model.Id, page });
        }

        private async Task<ParticipantPartialViewModel> GetPopulatedBaseViewModel(User user)
        {
            var hasAccount = !string.IsNullOrWhiteSpace(user.Username);
            var headUser = user.HouseholdHeadUserId ?? user.Id;
            return new ParticipantPartialViewModel
            {
                Id = user.Id,
                HouseholdCount = await _userService.FamilyMemberCountAsync(headUser),
                HasAccount = hasAccount,
                IsGroup = await _userService.GetGroupFromHouseholdHeadAsync(headUser) != null,
                EmailSubscriptionEnabled = await IsSiteSettingSetAsync(
                        SiteSettingKey.Users.AskEmailSubPermission),
                PrizeCount = UserHasPermission(Permission.ViewUserPrizes)
                    ? await _prizeWinnerService.GetUserWinCount(user.Id, false)
                    : default,
                HasElevatedRole = UserHasPermission(Permission.EditParticipants) && user.IsAdmin,
                RoleCount = UserHasPermission(Permission.ManageRoles) && hasAccount
                    ? (await _userService.GetUserRolesAsync(user.Id)).Count
                    : default
            };
        }

        #endregion Books

        #region History

        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> DeleteHistory(string ids, int userId)
        {
            ArgumentNullException.ThrowIfNull(ids);

            try
            {
                foreach (int numericId in ids.Split(',').Select(int.Parse))
                {
                    await _activityService.RemoveActivityAsync(userId, numericId);
                }
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Cannot delete history item: ", gex);
            }
            return RedirectToAction("History", new { id = userId });
        }

        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> History(int id, int page = 1)
        {
            try
            {
                var filter = new UserLogFilter(page);
                var history = await _userService.GetPaginatedUserHistoryAsync(id, filter);

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

                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);

                var viewModel = new HistoryListViewModel(await GetPopulatedBaseViewModel(user))
                {
                    Historys = new List<HistoryItemViewModel>(),
                    PaginateModel = paginateModel,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    CanRemoveHistory = UserHasPermission(Permission.LogActivityForAny),
                    TotalPoints = user.PointsEarned,
                    EmailSubscriptionEnabled = await IsSiteSettingSetAsync(
                        SiteSettingKey.Users.AskEmailSubPermission)
                };

                bool editChallenges = UserHasPermission(Permission.EditChallenges);

                foreach (var item in history.Data)
                {
                    var itemName = item.Description;
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
                    var itemModel = new HistoryItemViewModel
                    {
                        Id = item.Id,
                        CreatedAt = item.CreatedAt.ToString("d", CultureInfo.CurrentCulture),
                        Description = item.Description,
                        ItemName = itemName,
                        PointsEarned = item.PointsEarned,
                    };
                    if (!string.IsNullOrWhiteSpace(item.BadgeFilename))
                    {
                        itemModel.BadgeFilename = _pathResolver.ResolveContentPath(item.BadgeFilename);
                    }
                    else if (item.AvatarBundleId.HasValue)
                    {
                        var bundle = await _avatarService
                            .GetBundleByIdAsync(item.AvatarBundleId.Value, true);
                        if (bundle.AvatarItems.Count > 0)
                        {
                            itemModel.BadgeFilename = _pathResolver.ResolveContentPath(
                                bundle.AvatarItems.FirstOrDefault()?.Thumbnail);
                            if (bundle.AvatarItems.Count > 1)
                            {
                                itemModel.Description += $" <strong><a class=\"bundle-link\" data-id=\"{item.AvatarBundleId.Value}\">Click here</a></strong> to see all the items you unlocked.";
                            }
                        }
                    }

                    var whyNotDeletable = new StringBuilder();

                    if (!item.AvatarBundleId.HasValue)
                    {
                        if (item.BadgeId.HasValue && !item.ChallengeId.HasValue)
                        {
                            var trigger
                                = await _triggerService.GetByBadgeIdAsync(item.BadgeId.Value);
                            if (trigger?.AwardAvatarBundleId.HasValue == false
                                && !trigger.AwardVendorCodeTypeId.HasValue
                                && string.IsNullOrWhiteSpace(trigger.AwardMail))
                            {
                                var prize = await _prizeWinnerService.GetUserTriggerPrizeAsync(id,
                                    trigger.Id);
                                if (prize?.RedeemedAt.HasValue != true)
                                {
                                    itemModel.IsDeletable = true;
                                }
                                else
                                {
                                    whyNotDeletable.Append("A prize has already been redeemed associated with this item. ");
                                }
                            }
                            else
                            {
                                if (trigger?.AwardAvatarBundleId.HasValue != false)
                                {
                                    whyNotDeletable.Append("This awarded an avatar bundle which the participant might be using. ");
                                }
                                if (trigger?.AwardVendorCodeTypeId.HasValue == true)
                                {
                                    whyNotDeletable.Append("This awarded a vendor code prize. ");
                                }
                                if (!string.IsNullOrWhiteSpace(trigger?.AwardMail))
                                {
                                    whyNotDeletable.Append("This sent the participant an in-application mail which they may have seen. ");
                                }
                            }
                        }
                        else
                        {
                            itemModel.IsDeletable = true;
                        }
                    }
                    else
                    {
                        whyNotDeletable.Append("This is an avatar bundle, participant may be using parts of it. ");
                    }

                    itemModel.WhyNotDeletable = whyNotDeletable.ToString()?.Trim();

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

        #endregion History

        #region Prizes

        [Authorize(Policy = Policy.ViewUserPrizes)]
        public async Task<IActionResult> Prizes(int id, int page = 1)
        {
            try
            {
                var filter = new PrizeFilter(page)
                {
                    UserIds = new[] { id },
                    IncludeDrawings = true
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

                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);

                await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

                var viewModel = new PrizeListViewModel(await GetPopulatedBaseViewModel(user))
                {
                    PrizeWinners = prizeList.Data,
                    PaginateModel = paginateModel,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    User = user,
                    CanEditDetails = UserHasPermission(Permission.EditParticipants)
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's prizes: ", gex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Policy = Policy.ViewUserPrizes)]
        public async Task<IActionResult> RedeemWinner(int prizeWinnerId, int userId, string staffNotes, int page = 1)
        {
            try
            {
                await _prizeWinnerService.RedeemPrizeAsync(prizeWinnerId, staffNotes);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to redeem prize: ", gex);
            }
            return RedirectToAction("Prizes", new { id = userId, page });
        }

        [HttpPost]
        [Authorize(Policy = Policy.ViewUserPrizes)]
        public async Task<IActionResult> UndoRedemption(int prizeWinnerId, int userId, int page = 1)
        {
            try
            {
                await _prizeWinnerService.UndoRedemptionAsync(prizeWinnerId);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to undo redemption: ", gex);
            }
            return RedirectToAction("Prizes", new { id = userId, page });
        }

        #endregion Prizes

        #region Email Subscription Log

        public async Task<IActionResult> EmailSubscriptionLog(int id)
        {
            try
            {
                var auditLog = await _emailManagementService.GetUserAuditLogAsync(id);

                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);

                var viewModel = new EmailSubscriptionLogViewModel(await GetPopulatedBaseViewModel(user))
                {
                    SubscritionAuditLogs = auditLog,
                };

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant email subscription log: ", gex);
                return RedirectToAction("Index");
            }
        }

        #endregion Email Subscription Log

        #region Mail

        [Authorize(Policy = Policy.DeleteAnyMail)]
        [HttpPost]
        public async Task<IActionResult> DeleteMail(int id, int userId)
        {
            await _mailService.RemoveAsync(id);
            AlertSuccess = "Mail deleted";
            return RedirectToAction("Mail", new { id = userId });
        }

        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> Mail(int id, int page = 1)
        {
            try
            {
                await _mailService.SendUserBroadcastsAsync(id, false);

                const int take = 15;
                int skip = take * (page - 1);

                var mail = await _mailService.GetUserPaginatedAsync(id, skip, take);

                var paginateModel = new PaginateViewModel
                {
                    ItemCount = mail.Count,
                    CurrentPage = page,
                    ItemsPerPage = take
                };
                if (paginateModel.PastMaxPage)
                {
                    return RedirectToRoute(
                        new
                        {
                            page = paginateModel.LastPage ?? 1
                        });
                }

                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);

                var viewModel = new MailListViewModel(await GetPopulatedBaseViewModel(user))
                {
                    Mails = mail.Data,
                    PaginateModel = paginateModel,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail),
                    CanSendMail = UserHasPermission(Permission.MailParticipants),
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
                if (mail.ToUserId.HasValue)
                {
                    mail.Body = CommonMark.CommonMarkConverter.Convert(mail.Body);
                }

                var user = await _userService.GetDetailsByPermission(userId);
                SetPageTitle(user, mail.ToUserId.HasValue ? "To" : "From");

                var viewModel = new MailDetailViewModel
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

        [Authorize(Policy = Policy.MailParticipants)]
        public async Task<IActionResult> MailSend(int id)
        {
            try
            {
                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user, "Send Mail");

                var viewModel = new MailSendViewModel
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
            ArgumentNullException.ThrowIfNull(model);

            if (ModelState.IsValid)
            {
                var mail = new Mail
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
                var user = await _userService.GetDetailsByPermission(model.Id);
                SetPageTitle(user, "Send Mail");
                return View();
            }
        }

        #endregion Mail

        #region PasswordReset

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> PasswordReset(int id)
        {
            try
            {
                var user = await _userService.GetDetailsByPermission(id);
                SetPageTitle(user);

                var viewModel = new PasswordResetViewModel(await GetPopulatedBaseViewModel(user))
                {
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
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
            ArgumentNullException.ThrowIfNull(model);

            var user = await _userService.GetDetailsByPermission(model.Id);
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
                    ShowAlertDanger("Unable to change password: ", gex);
                }
            }

            SetPageTitle(user);
            return View(model);
        }

        #endregion PasswordReset

        #region Roles

        [Authorize(Policy = Policy.ManageRoles)]
        public async Task<IActionResult> Roles(int id)
        {
            try
            {
                var user = await _userService.GetDetailsByPermission(id);
                if (string.IsNullOrWhiteSpace(user.Username))
                {
                    ShowAlertDanger("User doesn't have a username and can't be assigned roles.");
                    return RedirectToAction(nameof(Detail), new { id });
                }

                SetPageTitle(user);

                var viewModel = new RolesViewModel(await GetPopulatedBaseViewModel(user));

                var userRoles = await _userService.GetUserRolesAsync(id);
                var roles = await _roleService.GetAllAsync();
                viewModel.RoleCount = userRoles.Count;
                viewModel.SelectedRoles = roles.Where(_ => userRoles.Contains(_.Id));
                viewModel.UnselectedRoles = roles.Except(viewModel.SelectedRoles);

                if (UserHasPermission(Permission.EditParticipants))
                {
                    viewModel.HasElevatedRole = user.IsAdmin;
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's roles: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.ManageRoles)]
        [HttpPost]
        public async Task<IActionResult> Roles(RolesViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            try
            {
                var roleIds = model.Roles?.Split(',').Select(int.Parse) ?? Array.Empty<int>();
                await _userService.UpdateUserRolesAsync(model.Id, roleIds);
                ShowAlertSuccess("User roles saved.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to update participant's roles: ", gex);
            }
            return RedirectToAction(nameof(Roles), new { id = model.Id });
        }

        #endregion Roles

        #region Handle code/dontation selection

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> DonateCode(ParticipantsDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            await _vendorCodeService.ResolveCodeStatusAsync(viewModel.User.Id, true, false);
            return RedirectToAction(viewModel.Action, new { id = viewModel.User.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> EmailAward(EmailAwardViewModel emailAwardModel)
        {
            ArgumentNullException.ThrowIfNull(emailAwardModel);

            if (!string.IsNullOrWhiteSpace(emailAwardModel.Email)
                && !EmailValidator.Validate(emailAwardModel.Email.Trim()))
            {
                ShowAlertWarning(string.Format(CultureInfo.InvariantCulture,
                    Annotations.Validate.EmailAddressInvalid,
                    emailAwardModel.Email.Trim()));
            }
            else
            {
                await _vendorCodeService.ResolveCodeStatusAsync(emailAwardModel.UserId,
                    false,
                    true,
                    emailAwardModel.Email.Trim());
            }

            return RedirectToAction(emailAwardModel.Action, new { id = emailAwardModel.UserId });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> HandleHouseholdDonation(HouseholdListViewModel viewModel,
            string donateButton,
            string redeemButton,
            string undonateButton)
        {
            if (viewModel == null)
            {
                ShowAlertWarning("Could not find that participant.");
                return RedirectToAction(nameof(Index));
            }

            int userId = 0;
            bool? emailAward = null;
            bool? donationStatus = null;
            if (!string.IsNullOrEmpty(donateButton))
            {
                donationStatus = true;
                userId = int.Parse(donateButton, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(redeemButton))
            {
                donationStatus = false;
                emailAward = false;
                userId = int.Parse(redeemButton, CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(undonateButton) && UserHasPermission(Permission.UnDonateVendorCode))
            {
                donationStatus = null;
                userId = int.Parse(undonateButton, CultureInfo.InvariantCulture);
            }
            if (userId == 0)
            {
                _logger.LogError("User {ActiveUserId)} unsuccessfully attempted to change donation for user {TargetUserId} to {DonationStatus}",
                    GetActiveUserId(),
                    userId,
                    donationStatus);
                ShowAlertDanger("Could not make requested change.");
            }
            else
            {
                await _vendorCodeService.ResolveCodeStatusAsync(userId, donationStatus, emailAward);
            }

            return RedirectToAction("Household", "Participants", new { id = viewModel.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.RedeemBulkVendorCodes)]
        public async Task<IActionResult> HouseholdBulkRedeemCode(int id)
        {
            var user = await _userService.GetDetailsByPermission(id);

            var headOfHouseholdId = user.HouseholdHeadUserId ?? user.Id;

            var redeemed = await _vendorCodeService.RedeemHouseholdCodes(headOfHouseholdId);

            if (redeemed > 0)
            {
                ShowAlertSuccess($"Redeemed {redeemed} codes!");
            }
            else
            {
                ShowAlertInfo("All codes have already been redeemed or donated.");
            }

            return RedirectToAction(nameof(Household), ParticipantsController.Name, new { id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> RedeemCode(ParticipantsDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            await _vendorCodeService.ResolveCodeStatusAsync(viewModel.User.Id, false, false);
            return RedirectToAction(viewModel.Action, new { id = viewModel.User.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.UnDonateVendorCode)]
        public async Task<IActionResult> UndonateCode(ParticipantsDetailViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            try
            {
                await _vendorCodeService.ResolveCodeStatusAsync(viewModel.User.Id, null, null);
            }
            catch (GraException gex)
            {
                AlertWarning = gex.Message;
            }
            return RedirectToAction(viewModel.Action, new { id = viewModel.User.Id });
        }

        #endregion Handle code/dontation selection

        #region VendorCode

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> AssignNewCode(int id, string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                ShowAlertWarning("You must supply a reason for reassigning a code.");
                return RedirectToAction(nameof(VendorCodes), new { id });
            }
            if (reason.Length > 255)
            {
                ShowAlertWarning("Please enter a reason of 255 characters or less.");
                return RedirectToAction(nameof(VendorCodes), new { id });
            }
            try
            {
                var vendorCode = await _vendorCodeService.GetUserVendorCodeAsync(id)
                    ?? throw new GraException("Could not find a vendor code for that user.");
                var prizeWinner = await _prizeWinnerService
                    .GetPrizeForVendorCodeAsync(vendorCode.Id);

                if (prizeWinner != null)
                {
                    await _prizeWinnerService.RemovePrizeAsync(prizeWinner.Id);
                }

                await _vendorCodeService.AssociateAsync(vendorCode.Id, reason);
                await _activityService.MCAwardVendorCodeAsync(id, vendorCode.VendorCodeTypeId);
            }
            catch (GraException gex)
            {
                ShowAlertDanger($"An error occurred: {gex.Message}");
            }
            return RedirectToAction(nameof(VendorCodes), new { id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> AssignSpareCode(int id, int vendorCodeTypeId, string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                ShowAlertWarning("You must supply an explanation for assigning a spare code.");
                return RedirectToAction(nameof(VendorCodes), new { id });
            }
            if (reason.Length > 255)
            {
                ShowAlertWarning("Please enter an explanation of 255 characters or less.");
                return RedirectToAction(nameof(VendorCodes), new { id });
            }

            await _vendorCodeService
                .AssignSpareAsync(vendorCodeTypeId, id, reason, GetActiveUserId());

            return RedirectToAction(nameof(VendorCodes), new { id });
        }

        [HttpGet]
        [Authorize(Policy = Policy.ManageVendorCodes)]
        public async Task<IActionResult> VendorCodes(int id)
        {
            var user = await _userService.GetDetailsByPermission(id);
            var viewModel = new VendorCodeViewModel(await GetPopulatedBaseViewModel(user))
            {
                AssociatedCodes = await _vendorCodeService.GetAssociatedVendorCodeInfoAsync(id),
                CurrentCodes = await _vendorCodeService.GetUserVendorCodesInfoAsync(id),
                CurrentUser = id == GetActiveUserId(),
                VendorCodeTypeList = new SelectList(await _vendorCodeService.GetTypeAllAsync(),
                    "Id",
                    "Description")
            };

            if (viewModel.CurrentCodes != null)
            {
                foreach (var vendorCode in viewModel.CurrentCodes.Where(_ => _.VendorCode?.PackingSlip?.Length > 0))
                {
                    vendorCode.PackingSlipLink
                        = Url.Action(nameof(VendorCodesController.ViewPackingSlip),
                            VendorCodesController.Name,
                            new { id = vendorCode.VendorCode?.PackingSlip });
                }
                foreach (var vendorCode in viewModel.CurrentCodes.Where(_ => _.VendorCode?.ReshipmentPriorPackingSlip?.Length > 0))
                {
                    vendorCode.ReshipmentPackingSlipLink
                        = Url.Action(nameof(VendorCodesController.ViewPackingSlip),
                            VendorCodesController.Name,
                            new { id = vendorCode.VendorCode?.ReshipmentPriorPackingSlip });
                }
            }

            foreach (var code in viewModel.AssociatedCodes)
            {
                if (code.VendorCode.ReassignedByUserId.HasValue)
                {
                    code.ReassignedByUser = await _userService
                        .GetUsersNameByIdAsync(code.VendorCode.ReassignedByUserId.Value);
                    code.ReassignedByLink = Url.Action(nameof(ParticipantsController.Detail),
                        ParticipantsController.Name,
                        new { id = code.VendorCode.ReassignedByUserId.Value });
                }
                if (!string.IsNullOrEmpty(code.VendorCode?.PackingSlip))
                {
                    code.PackingSlipLink = Url.Action(nameof(VendorCodesController.ViewPackingSlip),
                        VendorCodesController.Name,
                        new { id = code.VendorCode.PackingSlip });
                }
                if (!string.IsNullOrEmpty(code.VendorCode?.ReshipmentPriorPackingSlip))
                {
                    code.ReshipmentPackingSlipLink
                        = Url.Action(nameof(VendorCodesController.ViewPackingSlip),
                            VendorCodesController.Name,
                            new { id = code.VendorCode.ReshipmentPriorPackingSlip });
                }
            }
            SetPageTitle(user, "Assigned Vendor Codes");
            return View(viewModel);
        }

        #endregion VendorCode

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> CreateGroup(GroupUpgradeViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            if (string.IsNullOrEmpty(viewModel.GroupInfo?.Name?.Trim()))
            {
                AlertDanger = "You must specify a group name.";
                return View("UpgradeToGroup", viewModel);
            }

            try
            {
                var user = await _userService.GetDetailsByPermission(viewModel.Id);
                viewModel.GroupInfo.UserId = user.HouseholdHeadUserId ?? user.Id;
                await _userService.CreateGroup(GetActiveUserId(), viewModel.GroupInfo);
            }
            catch (GraException gex)
            {
                AlertDanger = $"Couldn't create group: {gex.Message}";
                return View("UpgradeToGroup", viewModel);
            }

            AlertSuccess = "Group successfully created, now you may add additional members.";
            return RedirectToAction("Household", new { id = viewModel.Id });
        }

        public async Task<IActionResult> DemoteCode(int userId, int? demoteCodeId, string reason)
        {
            if (!demoteCodeId.HasValue || string.IsNullOrEmpty(reason))
            {
                ShowAlertDanger("Cannot demote code without a valid code and reason.");
            }
            else
            {
                await _vendorCodeService.AssociateAsync(demoteCodeId.Value, reason);
            }
            return RedirectToAction(nameof(VendorCodes), new { id = userId });
        }

        public async Task<IActionResult> Groups(string search, int? type, int page = 1)
        {
            if (!await IsSiteSettingSetAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup))
            {
                return RedirectToAction(nameof(Index));
            }

            var groupTypes = await _userService.GetGroupTypeListAsync();
            int? groupTypeId = null;

            if (type.HasValue && groupTypes.Select(_ => _.Id).Contains(type.Value))
            {
                groupTypeId = type;
            }

            var filter = new GroupFilter(page)
            {
                Search = search?.Trim()
            };

            if (groupTypeId.HasValue)
            {
                filter.GroupTypeIds = new List<int> { groupTypeId.Value };
            }

            var groupList = await _userService.GetPaginatedGroupListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = groupList.Count,
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

            var viewModel = new GroupListViewModel
            {
                Groups = groupList.Data,
                PaginateModel = paginateModel,
                Search = search?.Trim(),
                Type = groupTypeId,
                GroupType = groupTypes.SingleOrDefault(_ => _.Id == groupTypeId),
                GroupTypeList = groupTypes
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> UpdateGroup(int id)
        {
            var groupInfo = await _userService.GetGroupFromHouseholdHeadAsync(id);
            if (groupInfo == null)
            {
                AlertDanger = "Could not find group to update.";
                return RedirectToAction("Household", new { id });
            }

            var groupTypes = await _userService.GetGroupTypeListAsync();

            return View("UpdateGroup", new UpdateGroupViewModel
            {
                HouseholdHeadUserId = id,
                GroupInfo = groupInfo,
                GroupTypes = new SelectList(groupTypes.ToList(), "Id", "Name")
            });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> UpdateGroup(UpdateGroupViewModel viewModel)
        {
            ArgumentNullException.ThrowIfNull(viewModel);

            var groupInfo
                = await _userService.GetGroupFromHouseholdHeadAsync(viewModel.HouseholdHeadUserId);

            if (groupInfo == null)
            {
                AlertDanger = "Could not find group to update.";
                return RedirectToAction("Household", new { id = viewModel.HouseholdHeadUserId });
            }

            groupInfo.UserId = viewModel.HouseholdHeadUserId;
            groupInfo.Name = viewModel.GroupInfo.Name;
            groupInfo.GroupTypeId = viewModel.GroupInfo.GroupTypeId;

            await _userService.UpdateGroup(GetActiveUserId(), groupInfo);

            return RedirectToAction("Household", new { id = viewModel.HouseholdHeadUserId });
        }

        [HttpPost]
        [Authorize(Policy = Policy.ViewUserPrizes)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1054:URI-like parameters should not be strings",
            Justification = "String name is built-in to the framework")]
        public async Task<IActionResult> UpdatePrizes(int id, string returnUrl)
        {
            var user = await _userService.GetDetailsByPermission(id);

            var headId = user.HouseholdHeadUserId ?? user.Id;

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            await _userService.AwardUserBadgesAsync(headId, true, true);
            sw.Stop();
            if (sw.Elapsed.TotalSeconds > 5)
            {
                _logger.LogInformation("Prize checking for family/group for user id {UserId} took {Elapsed} to award triggers.",
                    headId,
                    sw.Elapsed.ToString("c"));
            }

            return View("UpdatePrizes", returnUrl);
        }

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> UpgradeToGroup(int id)
        {
            var (useGroups, maximumHousehold) =
                await GetSiteSettingIntAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup);

            if (!useGroups)
            {
                return RedirectToAction("Household", new { id });
            }

            var groupTypes = await _userService.GetGroupTypeListAsync();

            if (!groupTypes.Any())
            {
                _logger.LogError("MC attempt to add family member, need to make a group, no group types configured.");
                AlertDanger = "In order to add more members to this family it must be converted to a group, however there are no group types configured.";
                return View("Household", id);
            }

            return View("UpgradeToGroup", new GroupUpgradeViewModel
            {
                Id = id,
                MaximumHouseholdAllowed = maximumHousehold,
                GroupTypes = new SelectList(groupTypes.ToList(), "Id", "Name")
            });
        }

        [HttpGet]
        [Authorize(Policy = Policy.ViewChangeHistory)]
        public async Task<IActionResult> ViewChangeHistory(int id)
        {
            User user = null;
            try
            {
                user = await _userService.GetDetailsByPermission(id);
            }
            catch (GraException gex)
            {
                ShowAlertWarning($"Unable to find user id {id}: {gex.Message}");
                return RedirectToAction(nameof(Index));
            }
            if (user == null)
            {
                ShowAlertWarning($"Unable to find user id {id}");
                return RedirectToAction(nameof(Index));
            }

            SetPageTitle(user, "Change History");

            var viewModel = new ChangeHistoryViewModel(await GetPopulatedBaseViewModel(user))
            {
                ChangedItems = await _userService.GetChangeHistoryAsync(id),
            };

            return View(viewModel);
        }

        private void SetPageTitle(User user, string title = "Participant", string username = null)
        {
            var name = user.FullName;
            if (!string.IsNullOrWhiteSpace(username))
            {
                name += $" ({username})";
            }
            else if (!string.IsNullOrEmpty(user.Username))
            {
                name += $" ({user.Username})";
            }
            PageTitleHtml = WebUtility.HtmlEncode($"{title} - {name}");
        }
    }
}
