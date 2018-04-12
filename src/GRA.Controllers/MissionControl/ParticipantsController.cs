using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewParticipantList)]
    public class ParticipantsController : Base.MCController
    {
        private const string ActivityMessage = "ActivityMessage";
        private const string SecretCodeMessage = "SecretCodeMessage";
        private const string VendorCodeMessage = "VendorCodeMessage";

        private readonly ILogger<ParticipantsController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly AvatarService _avatarService;
        private readonly GroupTypeService _groupTypeService;
        private readonly MailService _mailService;
        private readonly PointTranslationService _pointTranslationService;
        private readonly PrizeWinnerService _prizeWinnerService;
        private readonly QuestionnaireService _questionnaireService;
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
            GroupTypeService groupTypeService,
            MailService mailService,
            PointTranslationService pointTranslationService,
            PrizeWinnerService prizeWinnerService,
            QuestionnaireService questionnaireService,
            SchoolService schoolService,
            SiteService siteService,
            TriggerService triggerService,
            UserService userService,
            VendorCodeService vendorCodeService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _avatarService = Require.IsNotNull(avatarService,
                nameof(avatarService));
            _groupTypeService = groupTypeService
                ?? throw new ArgumentNullException(nameof(groupTypeService));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _pointTranslationService = Require.IsNotNull(pointTranslationService,
                nameof(pointTranslationService));
            _prizeWinnerService = Require.IsNotNull(prizeWinnerService, nameof(prizeWinnerService));
            _questionnaireService = Require.IsNotNull(questionnaireService,
                nameof(questionnaireService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _triggerService = Require.IsNotNull(triggerService, nameof(triggerService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            PageTitle = "Participants";
        }

        #region Index
        public async Task<IActionResult> Index(string search, string sort, string order,
            int? systemId, int? branchId, int? programId, int page = 1)
        {
            UserFilter filter = new UserFilter(page);

            if (!string.IsNullOrWhiteSpace(search))
            {
                filter.Search = search.Trim();
            }
            if (branchId.HasValue)
            {
                filter.BranchIds = new List<int>() { branchId.Value };
            }
            else if (systemId.HasValue)
            {
                filter.SystemIds = new List<int>() { systemId.Value };
            }
            if (programId.HasValue)
            {
                filter.ProgramIds = new List<int?>() { programId.Value };
            }

            bool isDescending = String.Equals(order, "Descending", StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(sort) && Enum.IsDefined(typeof(SortUsersBy), sort))
            {
                filter.SortBy = (SortUsersBy)Enum.Parse(typeof(SortUsersBy), sort);
                filter.OrderDescending = isDescending;
            }

            var participantsList = await _userService.GetPaginatedUserListAsync(filter);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = participantsList.Count,
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

            ParticipantsListViewModel viewModel = new ParticipantsListViewModel()
            {
                Users = participantsList.Data,
                PaginateModel = paginateModel,
                Search = search,
                Sort = sort,
                IsDescending = isDescending,
                SystemId = systemId,
                BranchId = branchId,
                ProgramId = programId,
                CanRemoveParticipant = UserHasPermission(Permission.DeleteParticipants),
                CanViewDetails = UserHasPermission(Permission.ViewParticipantDetails),
                SortUsers = Enum.GetValues(typeof(SortUsersBy)),
                SystemList = systemList,
                ProgramList = await _siteService.GetProgramList()
            };

            if (branchId.HasValue)
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
                viewModel.ProgramName =
                    (await _siteService.GetProgramByIdAsync(programId.Value)).Name;
            }

            var siteStage = GetSiteStage();
            if (siteStage == SiteStage.RegistrationOpen || siteStage == SiteStage.ProgramOpen)
            {
                viewModel.CanSignUpParticipants = true;
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Add()
        {
            PageTitle = "Add Participant";
            var site = await GetCurrentSiteAsync();
            var siteStage = GetSiteStage();
            if (siteStage <= SiteStage.BeforeRegistration)
            {
                ShowAlertInfo("Registration has not opened yet");
                return RedirectToAction("Index", "Participants");
            }
            else if (siteStage >= SiteStage.ProgramEnded)
            {
                ShowAlertInfo("The program has ended, participants cannot be added");
                return RedirectToAction("Index", "Participants");
            }

            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            var districtList = await _schoolService.GetDistrictsAsync(true);

            var viewModel = new ParticipantsAddViewModel()
            {
                RequirePostalCode = site.RequirePostalCode,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                PublicSelected = true,
                ShowPrivateOption = await _schoolService.AnyPrivateSchoolsAsync(),
                ShowCharterOption = await _schoolService.AnyCharterSchoolsAsync(),
                SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name"),
                AskEmailReminder = siteStage == SiteStage.RegistrationOpen
                    && await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskPreregistrationReminder)
            };

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (askIfFirstTime)
            {
                viewModel.AskFirstTime = EmptyNoYes();
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
            if (askActivityGoal)
            {
                viewModel.DailyPersonalGoal = defaultDailyGoal;
                var pointTranslation = programList.First().PointTranslation;
                viewModel.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            if (systemList.Count() == 1)
            {
                var systemId = systemList.SingleOrDefault().Id;
                var branchList = await _siteService.GetBranches(systemId);
                if (branchList.Count() > 1)
                {
                    branchList = branchList.Prepend(new Branch() { Id = -1 });
                }
                else
                {
                    viewModel.BranchId = branchList.SingleOrDefault().Id;
                }
                viewModel.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
                viewModel.SystemId = systemId;
            }

            if (programList.Count() == 1)
            {
                var programId = programList.SingleOrDefault().Id;
                var program = await _siteService.GetProgramByIdAsync(programId);
                viewModel.ProgramId = programList.SingleOrDefault().Id;
                viewModel.ShowAge = program.AskAge;
                viewModel.ShowSchool = program.AskSchool;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ParticipantsAddViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.PostalCode))
            {
                ModelState.AddModelError("PostalCode", "The Zip Code field is required.");
            }

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (!askIfFirstTime)
            {
                ModelState.Remove(nameof(model.IsFirstTime));
            }

            model.AskEmailReminder = GetSiteStage() == SiteStage.RegistrationOpen
                && await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskPreregistrationReminder);

            if (model.AskEmailReminder && model.PreregistrationReminderRequested
                && string.IsNullOrWhiteSpace(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email),
                    "Please enter an email address to send the reminder to.");
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);

            bool askAge = false;
            bool askSchool = false;
            if (model.ProgramId.HasValue)
            {
                var program = await _siteService.GetProgramByIdAsync(model.ProgramId.Value);
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

                if (!model.AskEmailReminder)
                {
                    user.PreregistrationReminderRequested = false;
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

                try
                {
                    var newUser = await _userService.RegisterUserAsync(user, model.Password, true);
                    await _mailService.SendUserBroadcastsAsync(newUser.Id, false, true);
                    if (UserHasPermission(Permission.EditParticipants))
                    {
                        return RedirectToAction("Detail", "Participants", new { id = newUser.Id });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Participants");
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Could not create participant account: ", gex);
                    if (gex.Message.Contains("password"))
                    {
                        ModelState.AddModelError("Password", "Please correct the issues with the password.");
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
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.ShowAge = askAge;
            model.ShowSchool = askSchool;

            var districtList = await _schoolService.GetDistrictsAsync(true);
            if (model.PrivateSelected)
            {
                model.PublicSelected = false;
                model.CharterSelected = false;
                model.IsHomeschooled = false;
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                model.SchoolList = new SelectList(
                    await _schoolService.GetPrivateSchoolListAsync(), "Id", "Name");
            }
            else if (model.CharterSelected)
            {
                model.PublicSelected = false;
                model.PrivateSelected = false;
                model.IsHomeschooled = false;
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                model.SchoolList = new SelectList(
                    await _schoolService.GetCharterSchoolListAsync(), "Id", "Name");
            }
            else if (model.IsHomeschooled)
            {
                model.PublicSelected = false;
                model.PrivateSelected = false;
                model.CharterSelected = false;
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
            }
            else
            {
                model.PublicSelected = true;
                model.PrivateSelected = false;
                model.CharterSelected = false;
                model.IsHomeschooled = false;

                if (model.SchoolId.HasValue)
                {
                    var schoolDetails = await _schoolService.GetSchoolDetailsAsync(model.SchoolId.Value);
                    var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDistrictId);
                    model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name",
                        schoolDetails.SchoolDistrictId);
                    model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                        schoolDetails.SchoolTypeId);
                    model.SchoolList = new SelectList(schoolDetails.Schools.ToList(), "Id", "Name");
                    ModelState.Remove(nameof(model.SchoolTypeId));
                }
                else
                {
                    model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                    if (model.SchoolDistrictId.HasValue)
                    {
                        var typeList = await _schoolService.GetTypesAsync(model.SchoolDistrictId);
                        model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                            model.SchoolTypeId);
                        var schoolList = await _schoolService.GetSchoolsAsync(model.SchoolDistrictId,
                            model.SchoolTypeId);
                        model.SchoolList = new SelectList(schoolList.ToList(), "Id", "Name");
                    }
                }
            }

            if (askIfFirstTime)
            {
                model.AskFirstTime = EmptyNoYes();
            }

            if (askActivityGoal)
            {
                var pointTranslation = programList.First().PointTranslation;
                model.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
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
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Could not delete participant: ", gex);
            }
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
                var systemList = await _siteService.GetSystemList();
                var programList = await _siteService.GetProgramList();
                var userProgram = programList.Where(_ => _.Id == user.ProgramId).SingleOrDefault();
                var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);

                await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId ?? id);

                ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
                {
                    User = user,
                    Id = user.Id,
                    Username = user.Username,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    IsGroup = groupInfo != null,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                    ShowAge = userProgram.AskAge,
                    ShowSchool = userProgram.AskSchool,
                    ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                    ShowPrivateOption = await _schoolService.AnyPrivateSchoolsAsync(),
                    ShowCharterOption = await _schoolService.AnyCharterSchoolsAsync(),
                    AskEmailReminder = GetSiteStage() == SiteStage.RegistrationOpen
                        && await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskPreregistrationReminder)
                };

                if (UserHasPermission(Permission.ViewUserPrizes))
                {
                    viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);
                }

                var districtList = await _schoolService.GetDistrictsAsync(true);
                if (user.SchoolId.HasValue)
                {
                    viewModel.SchoolId = user.SchoolId;
                    var schoolDetails = await _schoolService.GetSchoolDetailsAsync(user.SchoolId.Value);
                    viewModel.School = schoolDetails.School;
                    viewModel.School.SchoolDistrict = await _schoolService.GetDistrictByIdAsync(
                        viewModel.School.SchoolDistrictId);
                    if (viewModel.School.SchoolDistrict.IsPrivate
                        || viewModel.School.SchoolDistrict.IsCharter)
                    {
                        viewModel.SchoolDistrictList = new SelectList(
                            districtList.ToList(), "Id", "Name");
                        viewModel.SchoolList = new SelectList(
                            schoolDetails.Schools.ToList(), "Id", "Name");

                        if (viewModel.School.SchoolDistrict.IsPrivate)
                        {
                            viewModel.PrivateSelected = true;
                        }
                        else
                        {
                            viewModel.CharterSelected = true;
                        }
                    }
                    else
                    {
                        var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDistrictId);
                        viewModel.SchoolDistrictList = new SelectList(
                            districtList.ToList(), "Id", "Name", schoolDetails.SchoolDistrictId);
                        viewModel.SchoolTypeList = new SelectList(
                            typeList.ToList(), "Id", "Name", schoolDetails.SchoolTypeId);
                        viewModel.SchoolList = new SelectList(
                            schoolDetails.Schools.ToList(), "Id", "Name");

                        viewModel.PublicSelected = true;
                    }
                }
                else
                {
                    viewModel.SchoolDistrictList = new SelectList(
                        districtList.ToList(), "Id", "Name");
                    if (user.IsHomeschooled)
                    {
                        viewModel.IsHomeschooled = true;
                    }
                    else
                    {
                        viewModel.PublicSelected = true;
                        if (user.SchoolNotListed)
                        {
                            viewModel.SchoolNotListed = true;
                        }
                    }
                }

                if (UserHasPermission(Permission.EditParticipantUsernames)
                    && !string.IsNullOrWhiteSpace(user.Username))
                {
                    viewModel.CanEditUsername = true;
                }

                var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
                if (askActivityGoal)
                {
                    var pointTranslation = await _pointTranslationService
                        .GetByProgramIdAsync(user.ProgramId);
                    viewModel.TranslationDescriptionPastTense =
                        pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                    viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
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
            var site = await GetCurrentSiteAsync();
            var program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode", "The Zip Code field is required.");
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

            model.AskEmailReminder = GetSiteStage() == SiteStage.RegistrationOpen
                && await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskPreregistrationReminder);

            if (model.AskEmailReminder && model.User.PreregistrationReminderRequested
                && string.IsNullOrWhiteSpace(model.User.Email))
            {
                ModelState.AddModelError("User.Email",
                    "Please enter an email address to send the reminder to.");
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);

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
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.ShowAge = program.AskAge;
            model.ShowSchool = program.AskSchool;

            var districtList = await _schoolService.GetDistrictsAsync(true);
            if (model.PrivateSelected)
            {
                model.PublicSelected = false;
                model.CharterSelected = false;
                model.IsHomeschooled = false;
                model.SchoolDistrictList = new SelectList(
                    districtList.ToList(), "Id", "Name");
                model.SchoolList = new SelectList(
                    await _schoolService.GetPrivateSchoolListAsync(), "Id", "Name");
            }
            else if (model.CharterSelected)
            {
                model.PublicSelected = false;
                model.PrivateSelected = false;
                model.IsHomeschooled = false;
                model.SchoolDistrictList = new SelectList(
                    districtList.ToList(), "Id", "Name");
                model.SchoolList = new SelectList(
                    await _schoolService.GetCharterSchoolListAsync(), "Id", "Name");
            }
            else if (model.IsHomeschooled)
            {
                model.PublicSelected = false;
                model.PrivateSelected = false;
                model.CharterSelected = false;
                model.SchoolDistrictList = new SelectList(
                    districtList.ToList(), "Id", "Name");
            }
            else
            {
                model.PublicSelected = true;
                model.PrivateSelected = false;
                model.CharterSelected = false;
                model.IsHomeschooled = false;

                if (model.SchoolId.HasValue)
                {
                    var schoolDetails = await _schoolService.GetSchoolDetailsAsync(
                        model.SchoolId.Value);
                    var typeList = await _schoolService.GetTypesAsync(
                        schoolDetails.SchoolDistrictId);
                    model.SchoolDistrictList = new SelectList(
                        districtList.ToList(), "Id", "Name", schoolDetails.SchoolDistrictId);
                    model.SchoolTypeList = new SelectList(
                        typeList.ToList(), "Id", "Name", schoolDetails.SchoolTypeId);
                    model.SchoolList = new SelectList(
                        schoolDetails.Schools.ToList(), "Id", "Name");
                    ModelState.Remove(nameof(model.SchoolTypeId));
                }
                else
                {
                    model.SchoolDistrictList = new SelectList(
                        districtList.ToList(), "Id", "Name");
                    if (model.SchoolDistrictId.HasValue)
                    {
                        var typeList = await _schoolService.GetTypesAsync(model.SchoolDistrictId);
                        model.SchoolTypeList = new SelectList(
                            typeList.ToList(), "Id", "Name", model.SchoolTypeId);
                        var schoolList = await _schoolService.GetSchoolsAsync(
                            model.SchoolDistrictId, model.SchoolTypeId);
                        model.SchoolList = new SelectList( schoolList.ToList(), "Id", "Name");
                    }
                }
            }

            if (askActivityGoal)
            {
                var pointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(model.User.ProgramId);
                model.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(model);
        }
        #endregion

        #region Log Activity
        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> LogActivity(int id)
        {
            var user = await _userService.GetDetails(id);
            SetPageTitle(user);

            var groupInfo
                = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId ?? id);

            LogActivityViewModel viewModel = new LogActivityViewModel()
            {
                Id = id,
                HasPendingQuestionnaire = (await _questionnaireService
                    .GetRequiredQuestionnaire(user.Id, user.Age)).HasValue,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                IsGroup = groupInfo != null,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                DisableSecretCode = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                PointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(user.ProgramId, true)
            };

            if (UserHasPermission(Permission.ViewUserPrizes))
            {
                viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);
            }

            if (UserHasPermission(Permission.ManageVendorCodes))
            {
                viewModel.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");

                if (TempData.ContainsKey(VendorCodeMessage))
                {
                    ModelState.AddModelError("VendorCodeTypeId", (string)TempData[VendorCodeMessage]);
                }
            }

            return View(viewModel);
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> LogActivity(LogActivityViewModel model, bool isSecretCode)
        {
            var user = await _userService.GetDetails(model.Id);
            SetPageTitle(user);
            model.PointTranslation = await _pointTranslationService
                .GetByProgramIdAsync(user.ProgramId, true);

            if (!model.IsSecretCode)
            {
                if ((!model.ActivityAmount.HasValue || model.ActivityAmount.Value < 1)
                    && model.PointTranslation.IsSingleEvent == false)
                {
                    ModelState.AddModelError("ActivityAmount", "Enter a number greater than 0.");
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        var activityAmount = 1;
                        if (model.PointTranslation.IsSingleEvent == false)
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
            if (UserHasPermission(Permission.ManageVendorCodes))
            {
                model.VendorCodeTypeList = new SelectList(
                    (await _vendorCodeService.GetTypeAllAsync()), "Id", "Description");
            }
            return View(model);
        }

        [Authorize(Policy = Policy.ManageVendorCodes)]
        [HttpPost]
        public async Task<IActionResult> AwardVendorCode(LogActivityViewModel model)
        {
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
        #endregion

        #region Household
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Household(int id)
        {
            try
            {
                var user = await _userService.GetDetails(id);
                var showVendorCodes = await _vendorCodeService.SiteHasCodesAsync();
                SetPageTitle(user);

                User head = new User();

                if (user.HouseholdHeadUserId.HasValue)
                {
                    head = await _userService
                        .GetDetails(user.HouseholdHeadUserId.Value);
                }
                else
                {
                    head = user;
                }
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
                if (ViewUserPrizes && UserHasPermission(Permission.ViewUserPrizes))
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

                HouseholdListViewModel viewModel = new HouseholdListViewModel()
                {
                    Users = household,
                    Id = id,
                    HouseholdCount = household.Count(),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    CanLogActivity = UserHasPermission(Permission.LogActivityForAny),
                    CanReadMail = ReadAllMail,
                    CanViewPrizes = ViewUserPrizes,
                    Head = head,
                    SystemId = systemId,
                    BranchList = branchList,
                    SystemList = systemList,
                    DisableSecretCode = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                    ShowVendorCodes = showVendorCodes,
                    PointTranslation = await _pointTranslationService
                        .GetByProgramIdAsync(user.ProgramId, true)
                };

                if (UserHasPermission(Permission.ViewUserPrizes))
                {
                    viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);
                }

                if (TempData.ContainsKey(ActivityMessage))
                {
                    viewModel.ActivityMessage = (string)TempData[ActivityMessage];
                }
                if (TempData.ContainsKey(SecretCodeMessage))
                {
                    viewModel.SecretCodeMessage = (string)TempData[SecretCodeMessage];
                }

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId
                    ?? user.Id);

                if (groupInfo != null)
                {
                    viewModel.GroupName = groupInfo.Name;
                    viewModel.GroupType = groupInfo.GroupTypeName;
                    viewModel.IsGroup = true;
                }
                else
                {
                    var (useGroups, maximumHousehold) =
                        await GetSiteSettingIntAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup);
                    if (useGroups && household.Count() + 1 >= maximumHousehold)
                    {
                        viewModel.UpgradeToGroup = true;
                    }
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's family/group: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> HouseholdApplyActivity(HouseholdListViewModel model)
        {
            var user = await _userService.GetDetails(model.Id);
            model.PointTranslation = await _pointTranslationService
                .GetByProgramIdAsync(user.ProgramId, true);
            if (model.ActivityAmount < 1 && model.PointTranslation.IsSingleEvent == false)
            {
                TempData[ActivityMessage] = "You must enter an amonunt!";
            }

            else if (!string.IsNullOrWhiteSpace(model.UserSelection))
            {
                List<int> userSelection = model.UserSelection
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(Int32.Parse)
                    .Distinct()
                    .ToList();
                try
                {
                    var activityAmount = 1;
                    if (model.PointTranslation.IsSingleEvent == false)
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
            if (string.IsNullOrWhiteSpace(model.SecretCode))
            {
                TempData[SecretCodeMessage] = "You must enter a code!";
            }

            else if (!string.IsNullOrWhiteSpace(model.UserSelection))
            {
                List<int> userSelection = model.UserSelection
                    .Split(',')
                    .Where(_ => !string.IsNullOrWhiteSpace(_))
                    .Select(Int32.Parse)
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
                        TempData[SecretCodeMessage] = "All selected members have already entered that Secret Code.";
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
            return RedirectToAction("Household", new { id = id });
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
            return RedirectToAction("Household", new { id = id });
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

                var groupInfo 
                    = await _userService.GetGroupFromHouseholdHeadAsync(headOfHousehold.Id);

                string callIt = groupInfo == null ? "Family" : "Group";
                SetPageTitle(headOfHousehold, $"Add {callIt} Member");

                var userBase = new User()
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
                var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
                var districtList = await _schoolService.GetDistrictsAsync(true);

                HouseholdAddViewModel viewModel = new HouseholdAddViewModel()
                {
                    User = userBase,
                    Id = id,
                    RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                    ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                    PublicSelected = true,
                    ShowPrivateOption = await _schoolService.AnyPrivateSchoolsAsync(),
                    ShowCharterOption = await _schoolService.AnyCharterSchoolsAsync(),
                    SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name")
                };

                var askIfFirstTime
                    = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
                if (askIfFirstTime)
                {
                    viewModel.AskFirstTime = EmptyNoYes();
                }

                var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
                if (askActivityGoal)
                {
                    viewModel.User.DailyPersonalGoal = defaultDailyGoal;
                    var pointTranslation = programList.First().PointTranslation;
                    viewModel.TranslationDescriptionPastTense =
                        pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                    viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
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
            var site = await GetCurrentSiteAsync();
            var headOfHousehold = await _userService.GetDetails(model.Id);
            if (headOfHousehold.HouseholdHeadUserId != null)
            {
                headOfHousehold = await _userService
                    .GetDetails((int)headOfHousehold.HouseholdHeadUserId);
            }

            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.User.PostalCode))
            {
                ModelState.AddModelError("User.PostalCode", "The Zip Code field is required.");
            }

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (!askIfFirstTime)
            {
                ModelState.Remove(nameof(model.IsFirstTime));
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);

            bool askAge = false;
            bool askSchool = false;
            if (model.User.ProgramId >= 0)
            {
                var program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);
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

                    var newMember = await _userService.AddHouseholdMemberAsync(headOfHousehold.Id,
                        model.User);
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
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.ShowAge = askAge;
            model.ShowSchool = askSchool;

            var districtList = await _schoolService.GetDistrictsAsync(true);
            if (model.PrivateSelected)
            {
                model.PublicSelected = false;
                model.CharterSelected = false;
                model.IsHomeschooled = false;
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                model.SchoolList = new SelectList(
                    await _schoolService.GetPrivateSchoolListAsync(), "Id", "Name");
            }
            else if (model.CharterSelected)
            {
                model.PublicSelected = false;
                model.PrivateSelected = false;
                model.IsHomeschooled = false;
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                model.SchoolList = new SelectList(
                    await _schoolService.GetCharterSchoolListAsync(), "Id", "Name");
            }
            else if (model.IsHomeschooled)
            {
                model.PublicSelected = false;
                model.PrivateSelected = false;
                model.CharterSelected = false;
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
            }
            else
            {
                model.PublicSelected = true;
                model.PrivateSelected = false;
                model.CharterSelected = false;
                model.IsHomeschooled = false;

                if (model.SchoolId.HasValue)
                {
                    var schoolDetails = await _schoolService.GetSchoolDetailsAsync(model.SchoolId.Value);
                    var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDistrictId);
                    model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name",
                        schoolDetails.SchoolDistrictId);
                    model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                        schoolDetails.SchoolTypeId);
                    model.SchoolList = new SelectList(schoolDetails.Schools.ToList(), "Id", "Name");
                    ModelState.Remove(nameof(model.SchoolTypeId));
                }
                else
                {
                    model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                    if (model.SchoolDistrictId.HasValue)
                    {
                        var typeList = await _schoolService.GetTypesAsync(model.SchoolDistrictId);
                        model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                            model.SchoolTypeId);
                        var schoolList = await _schoolService.GetSchoolsAsync(model.SchoolDistrictId,
                            model.SchoolTypeId);
                        model.SchoolList = new SelectList(schoolList.ToList(), "Id", "Name");
                    }
                }
            }

            if (askIfFirstTime)
            {
                model.AskFirstTime = EmptyNoYes();
            }

            if (askActivityGoal)
            {
                var pointTranslation = programList.First().PointTranslation;
                model.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View("HouseholdAdd", model);
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
                SetPageTitle(user, "Register Family Member");

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

        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> HouseholdGetParticipantsList(int userId,
            int? systemId,
            int? branchId,
            string search,
            int page = 1)
        {
            UserFilter filter = new UserFilter(page)
            {
                UserIds = new List<int>() { userId },
                Search = search,
                CanAddToHousehold = true
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

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = participants.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };
            ParticipantsListViewModel viewModel = new ParticipantsListViewModel()
            {
                Users = participants.Data,
                PaginateModel = paginateModel
            };

            return PartialView("_ParticipantListPartial", viewModel);
        }
        #endregion

        #region Books
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Books(int id, string sort, string order, int page = 1)
        {
            try
            {
                var filter = new BookFilter(page);

                bool isDescending = String.Equals(order, "Descending", StringComparison.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(sort) && Enum.IsDefined(typeof(SortBooksBy), sort))
                {
                    filter.SortBy = (SortBooksBy)Enum.Parse(typeof(SortBooksBy), sort);
                    filter.OrderDescending = isDescending;
                }

                var books = await _userService
                    .GetPaginatedUserBookListAsync(id, filter);

                PaginateViewModel paginateModel = new PaginateViewModel()
                {
                    ItemCount = books.Count,
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

                var user = await _userService.GetDetails(id);
                SetPageTitle(user);

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId ?? id);

                BookListViewModel viewModel = new BookListViewModel()
                {
                    Books = books.Data.ToList(),
                    PaginateModel = paginateModel,
                    Sort = sort,
                    IsDescending = isDescending,
                    Id = id,
                    HasPendingQuestionnaire = (await _questionnaireService
                        .GetRequiredQuestionnaire(user.Id, user.Age)).HasValue,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    IsGroup = groupInfo != null,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanEditBooks = UserHasPermission(Permission.LogActivityForAny),
                    SortBooks = Enum.GetValues(typeof(SortBooksBy))
                };

                if (UserHasPermission(Permission.ViewUserPrizes))
                {
                    viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);
                }
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
        public async Task<IActionResult> AddBook(BookListViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _activityService.AddBookAsync(model.Id, model.Book);
                    ShowAlertSuccess($"Added book '{model.Book.Title}'");
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
            return RedirectToAction("Books", new { id = model.Id, page = page });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> EditBook(BookListViewModel model)
        {
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
            return RedirectToAction("Books", new { id = model.Id, page = page });
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> RemoveBook(BookListViewModel model)
        {
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
            return RedirectToAction("Books", new { id = model.Id, page = page });
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

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId ?? id);

                HistoryListViewModel viewModel = new HistoryListViewModel()
                {
                    Historys = new List<HistoryItemViewModel>(),
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    IsGroup = groupInfo != null,
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanRemoveHistory = UserHasPermission(Permission.LogActivityForAny),
                    TotalPoints = user.PointsEarned
                };

                if (UserHasPermission(Permission.ViewUserPrizes))
                {
                    viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);
                }

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
                    HistoryItemViewModel itemModel = new HistoryItemViewModel()
                    {
                        Id = item.Id,
                        CreatedAt = item.CreatedAt.ToString("d"),
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
                                bundle.AvatarItems.FirstOrDefault().Thumbnail);
                            if (bundle.AvatarItems.Count > 1)
                            {
                                itemModel.Description += $" <strong><a class=\"bundle-link\" data-id=\"{item.AvatarBundleId.Value}\">Click here</a></strong> to see all the items you unlocked.";
                            }
                        }
                    }

                    if (!item.AvatarBundleId.HasValue)
                    {
                        if (item.BadgeId.HasValue && !item.ChallengeId.HasValue)
                        {
                            var trigger = await _triggerService.GetByBadgeIdAsync(item.BadgeId.Value);
                            if (trigger != null && !trigger.AwardAvatarBundleId.HasValue
                                && !trigger.AwardVendorCodeTypeId.HasValue
                                && string.IsNullOrWhiteSpace(trigger.AwardMail))
                            {
                                var prize = await _prizeWinnerService.GetUserTriggerPrizeAsync(id,
                                    trigger.Id);
                                if (prize == null || !prize.RedeemedAt.HasValue)
                                {
                                    itemModel.IsDeletable = true;
                                }
                            }
                        }
                        else
                        {
                            itemModel.IsDeletable = true;
                        }
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
        public async Task<IActionResult> DeleteHistory(string ids, int userId)
        {
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
        #endregion

        #region Prizes
        [Authorize(Policy = Policy.ViewUserPrizes)]
        public async Task<IActionResult> Prizes(int id, int page = 1)
        {
            try
            {
                BaseFilter filter = new BaseFilter(page);
                var prizeList = await _prizeWinnerService.PageUserPrizes(id, filter);

                PaginateViewModel paginateModel = new PaginateViewModel()
                {
                    ItemCount = prizeList.Count,
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

                var user = await _userService.GetDetails(id);
                SetPageTitle(user);

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId ?? id);

                PrizeListViewModel viewModel = new PrizeListViewModel()
                {
                    PrizeWinners = prizeList.Data,
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    IsGroup = groupInfo != null,
                    PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username)
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
        public async Task<IActionResult> RedeemWinner(int prizeWinnerId, int userId, int page = 1)
        {
            try
            {
                await _prizeWinnerService.RedeemPrizeAsync(prizeWinnerId);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to redeem prize: ", gex);
            }
            return RedirectToAction("Prizes", new { id = userId, page = page });
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
            return RedirectToAction("Prizes", new { id = userId, page = page });
        }
        #endregion

        #region Mail
        [Authorize(Policy = Policy.ReadAllMail)]
        public async Task<IActionResult> Mail(int id, int page = 1)
        {
            try
            {
                await _mailService.SendUserBroadcastsAsync(id, false);

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

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId ?? id);

                MailListViewModel viewModel = new MailListViewModel()
                {
                    Mails = mail.Data,
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    IsGroup = groupInfo != null,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanRemoveMail = UserHasPermission(Permission.DeleteAnyMail),
                    CanSendMail = UserHasPermission(Permission.MailParticipants)
                };

                if (UserHasPermission(Permission.ViewUserPrizes))
                {
                    viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);
                }

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

                var groupInfo
                    = await _userService.GetGroupFromHouseholdHeadAsync(user.HouseholdHeadUserId ?? id);

                PasswordResetViewModel viewModel = new PasswordResetViewModel()
                {
                    Id = id,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    IsGroup = groupInfo != null
                };

                if (UserHasPermission(Permission.ViewUserPrizes))
                {
                    viewModel.PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false);
                }

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
                    ShowAlertDanger("Unable to change password: ", gex);
                }
            }

            SetPageTitle(user);
            return View(model);
        }
        #endregion

        #region Handle code/dontation selection
        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> DonateCode(ParticipantsDetailViewModel viewModel)
        {
            await _vendorCodeService.ResolveDonationStatusAsync(viewModel.User.Id, true);
            return RedirectToAction("Detail", "Participants", new { id = viewModel.User.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> RedeemCode(ParticipantsDetailViewModel viewModel)
        {
            await _vendorCodeService.ResolveDonationStatusAsync(viewModel.User.Id, false);
            return RedirectToAction("Detail", "Participants", new { id = viewModel.User.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.UnDonateVendorCode)]
        public async Task<IActionResult> UndonateCode(ParticipantsDetailViewModel viewModel)
        {
            await _vendorCodeService.ResolveDonationStatusAsync(viewModel.User.Id, null);
            return RedirectToAction("Detail", "Participants", new { id = viewModel.User.Id });
        }

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> HandleHouseholdDonation(HouseholdListViewModel viewModel,
            string donateButton,
            string redeemButton,
            string undonateButton)
        {
            int userId = 0;
            bool? donationStatus = null;
            if (!string.IsNullOrEmpty(donateButton))
            {
                donationStatus = true;
                userId = int.Parse(donateButton);
            }
            if (!string.IsNullOrEmpty(redeemButton))
            {
                donationStatus = false;
                userId = int.Parse(redeemButton);
            }
            if (!string.IsNullOrEmpty(undonateButton) && UserHasPermission(Permission.UnDonateVendorCode))
            {
                donationStatus = null;
                userId = int.Parse(undonateButton);
            }
            if (userId == 0)
            {
                _logger.LogError($"User {GetActiveUserId()} unsuccessfully attempted to change donation for user {userId} to {donationStatus}");
                AlertDanger = "Could not make requested change.";
            }
            else
            {
                await _vendorCodeService.ResolveDonationStatusAsync(userId, donationStatus);
            }
            return RedirectToAction("Household", "Participants", new { id = viewModel.Id });
        }
        #endregion Handle code/dontation selection

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

            if (groupTypes.Count() == 0)
            {
                _logger.LogError($"MC attempt to add family member, need to make a group, no group types configured.");
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

        [HttpPost]
        [Authorize(Policy = Policy.EditParticipants)]
        public async Task<IActionResult> CreateGroup(GroupUpgradeViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.GroupInfo?.Name?.Trim()))
            {
                AlertDanger = "You must specify a group name.";
                return View("UpgradeToGroup", viewModel);
            }

            try
            {
                viewModel.GroupInfo.UserId = viewModel.Id;
                await _userService.CreateGroup(GetActiveUserId(), viewModel.GroupInfo);
            }
            catch (Exception ex)
            {
                AlertDanger = $"Couldn't create group: {ex.Message}";
                return View("GroupUpgrade", viewModel);
            }

            AlertSuccess = "Group successfully created, now you may add additional members.";
            return RedirectToAction("Household", new { id = viewModel.Id });
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
    }
}
