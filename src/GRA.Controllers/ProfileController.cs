using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private readonly ILogger<ProfileController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly AvatarService _avatarService;
        private readonly DailyLiteracyTipService _dailyLiteracyTipService;
        private readonly MailService _mailService;
        private readonly PointTranslationService _pointTranslationService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        private string HouseholdTitle;

        public static string Name { get { return "Profile"; } }

        public ProfileController(ILogger<ProfileController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            AuthenticationService authenticationService,
            AvatarService avatarService,
            DailyLiteracyTipService dailyLiteracyTipService,
            MailService mailService,
            PointTranslationService pointTranslationService,
            QuestionnaireService questionnaireService,
            SchoolService schoolService,
            SiteService siteService,
            UserService userService,
            VendorCodeService vendorCodeService) : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = context.Mapper;
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
            _authenticationService = authenticationService
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _avatarService = avatarService
                ?? throw new ArgumentNullException(nameof(avatarService));
            _dailyLiteracyTipService = dailyLiteracyTipService
                ?? throw new ArgumentNullException(nameof(dailyLiteracyTipService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _pointTranslationService = pointTranslationService
                ?? throw new ArgumentNullException(nameof(pointTranslationService));
            _questionnaireService = questionnaireService
                ?? throw new ArgumentNullException(nameof(questionnaireService));
            _schoolService = schoolService
                ?? throw new ArgumentNullException(nameof(schoolService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
            PageTitle = "My Profile";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            HouseholdTitle = HttpContext.Items[ItemKey.HouseholdTitle] as string
                ?? Annotations.Interface.Family;
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
                User = user,
                HouseholdCount = householdCount,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                ShowAge = userProgram.AskAge,
                ShowSchool = userProgram.AskSchool,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SchoolList = new SelectList(await _schoolService.GetSchoolsAsync(), "Id", "Name"),
                SchoolId = user.SchoolId,
                IsHomeschooled = user.IsHomeschooled,
                SchoolNotListed = user.SchoolNotListed,
                RestrictChangingSystemBranch = await GetSiteSettingBoolAsync(SiteSettingKey
                    .Users
                    .RestrictChangingSystemBranch),
            };

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
                viewModel.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileDetailViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            var program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);

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
            model.ShowAge = program.AskAge;
            model.ShowSchool = program.AskSchool;

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
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(model);
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
                                    $"{image.Id}{image.Extension}");
                                if (System.IO.File.Exists(_pathResolver
                                    .ResolveContentFilePath(imagePath)))
                                {
                                    var dailyLiteracyTip = await _dailyLiteracyTipService
                                        .GetByIdAsync(program.DailyLiteracyTipId.Value);
                                    var dailyImageViewModel = new DailyImageViewModel()
                                    {
                                        DailyImageMessage = dailyLiteracyTip.Message,
                                        DailyImagePath
                                            = _pathResolver.ResolveContentPath(imagePath),
                                        IsLarge = dailyLiteracyTip.IsLarge
                                    };
                                    dailyImageDictionary.Add(program.Id, dailyImageViewModel);
                                }
                            }
                        }
                    }
                }
                viewModel.DailyImageDictionary = dailyImageDictionary;

                if (showVendorCodes)
                {
                    await _vendorCodeService.PopulateVendorCodeStatusAsync(viewModel.Head);
                }
            }

            if (TempData.ContainsKey(ActivityMessage))
            {
                viewModel.ActivityMessage = (string)TempData[ActivityMessage];
            }
            if (TempData.ContainsKey(SecretCodeMessage))
            {
                viewModel.SecretCodeMessage = (string)TempData[SecretCodeMessage];
            }

            return View(viewModel);
        }

        public async Task<IActionResult> HouseholdApplyActivity(HouseholdListViewModel model)
        {
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
            if (string.IsNullOrWhiteSpace(model.SecretCode))
            {
                TempData[SecretCodeMessage]
                    = _sharedLocalizer[Annotations.Required.SecretCode].ToString();
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
                TempData[ActivityMessage]
                    = _sharedLocalizer[Annotations.Required.SelectFirst].ToString();
            }

            return RedirectToAction(nameof(Household));
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
                    _logger.LogError($"User {authUser.Id} should be forced to make a group but no group types are configured");
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
                            _logger.LogInformation($"Redirecting user {authUser.Id} to create a group when adding member {maximumHousehold + 1}");
                            return View("GroupUpgrade", new GroupUpgradeViewModel
                            {
                                MaximumHouseholdAllowed = maximumHousehold,
                                GroupTypes = new SelectList(groupTypes.ToList(), "Id", "Name")
                            });
                        }
                    }
                }
            }

            var userBase = new User()
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
                viewModel.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View("HouseholdAdd", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddHouseholdMember(HouseholdAddViewModel model)
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction(nameof(Household));
            }

            var site = await GetCurrentSiteAsync();
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
                var subscriptionRequested = model.EmailSubscriptionRequested.Equals(
                        DropDownTrueValue, StringComparison.OrdinalIgnoreCase);
                if (subscriptionRequested && string.IsNullOrWhiteSpace(model.User.Email))
                {
                    ModelState.AddModelError("User.Email", " ");
                    ModelState.AddModelError(nameof(model.EmailSubscriptionRequested),
                        _sharedLocalizer[Annotations.Validate.EmailSubscription]);
                }
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
                        model.User.IsEmailSubscribed = model.EmailSubscriptionRequested.Equals(
                            DropDownTrueValue,
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

                    var newMember = await _userService.AddHouseholdMemberAsync(authUser.Id,
                        model.User);
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
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View("HouseholdAdd", model);
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
                            _logger.LogError($"User {authUser.Id} should be forced to make a group but no group types are configured");
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
                                    _logger.LogInformation($"Redirecting user {authUser.Id} to create a group when adding member {maximumHousehold + 1}, group will total {currentHousehold.Count() + totalAddCount}");
                                    // add authenticated user id to session
                                    HttpContext.Session.SetString(SessionKey.AbsorbUserId,
                                        addUserId.ToString());
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

        public IActionResult RegisterHouseholdMember()
        {
            return RedirectToAction(nameof(Household));
        }

        public IActionResult CancelGroupUpgrade()
        {
            if (HttpContext.Session.Keys.Contains(SessionKey.AbsorbUserId))
            {
                HttpContext.Session.Remove(SessionKey.AbsorbUserId);
            }
            return RedirectToAction(nameof(Household));
        }

        [HttpPost]
        public async Task<IActionResult> RegisterHouseholdMember(HouseholdRegisterViewModel model)
        {
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
                        await _userService.RegisterHouseholdMemberAsync(user, model.Password);
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
        public async Task<IActionResult> LoginAs(int loginId, bool goToMail = false)
        {
            var user = await _userService.GetDetails(loginId);
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
            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
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

        [HttpPost]
        public async Task<IActionResult> AddBook(BookListViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.Book.Author = model.Book.Author.Trim();
                    model.Book.Title = model.Book.Title.Trim();
                    var result = await _activityService.AddBookAsync(GetActiveUserId(), model.Book);
                    if (result.Status == ServiceResultStatus.Warning
                            && !string.IsNullOrWhiteSpace(result.Message))
                    {
                        ShowAlertWarning(result.Message);
                    }
                    else if (result.Status == ServiceResultStatus.Success)
                    {
                        ShowAlertSuccess(_sharedLocalizer[Annotations.Interface.UpdatedItem,
                            Annotations.Interface.BookList]);
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

        [HttpPost]
        public async Task<IActionResult> EditBook(BookListViewModel model)
        {
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

        [HttpPost]
        public async Task<IActionResult> RemoveBook(BookListViewModel model)
        {
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

        public async Task<IActionResult> History(int page = 1)
        {
            const int take = 15;
            int skip = take * (page - 1);
            var history = await _userService
                .GetPaginatedUserHistoryAsync(GetActiveUserId(), skip, take);

            var paginateModel = new PaginateViewModel
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
                    CreatedAt = item.CreatedAt.ToString("d"),
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
                            bundle.AvatarItems.First().Thumbnail);
                        if (bundle.AvatarItems.Count > 1)
                        {
                            var bundleLink = Url.Action(nameof(AvatarController.Index),
                                AvatarController.Name,
                                new { bundle = item.AvatarBundleId.Value });
                            description.AppendFormat(
                                " <strong><a href=\"{0}\">{1}</a></strong>",
                                bundleLink,
                                _sharedLocalizer[Annotations.Interface.SeeItemsUnlocked]);
                        }
                    }
                }
                itemModel.Description = description.ToString();
                viewModel.Historys.Add(itemModel);
            }
            return View(viewModel);
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
                        = _sharedLocalizer[Annotations.Validate.UsernamePasswordMismatch];
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DonateCode(ProfileDetailViewModel viewModel)
        {
            await _vendorCodeService.ResolveCodeStatusAsync(viewModel.User.Id, true, false);
            return RedirectToAction(nameof(ProfileController.Index), ProfileController.Name);
        }

        [HttpPost]
        public async Task<IActionResult> RedeemCode(ProfileDetailViewModel viewModel)
        {
            await _vendorCodeService.ResolveCodeStatusAsync(viewModel.User.Id, false, false);
            return RedirectToAction(nameof(ProfileController.Index), ProfileController.Name);
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
                    Household = id.HasValue,
                    UserId = user.Id,
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
            if (!ModelState.IsValid)
            {
                return View(emailAwardModel);
            }

            await _vendorCodeService.ResolveCodeStatusAsync(emailAwardModel.UserId,
                false,
                true,
                emailAwardModel.Email);

            if (emailAwardModel.Household)
            {
                return RedirectToAction(nameof(ProfileController.Household));
            }
            else
            {
                return RedirectToAction(nameof(ProfileController.Index));
            }
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
                userId = int.Parse(donateButton);
            }
            if (!string.IsNullOrEmpty(redeemButton))
            {
                donationStatus = false;
                userId = int.Parse(redeemButton);
            }
            if (userId == 0)
            {
                _logger.LogError($"User {GetActiveUserId()} unsuccessfully attempted to change donation for user {userId} to {donationStatus}");
                ShowAlertDanger(_sharedLocalizer[Annotations.Validate.SomethingWentWrong]);
            }
            else
            {
                await _vendorCodeService.ResolveCodeStatusAsync(userId, donationStatus, false);
            }
            return RedirectToAction(nameof(ProfileController.Household), ProfileController.Name);
        }

        [HttpPost]
        public async Task<IActionResult> HouseholdRedeemCode(
            HouseholdListViewModel viewModel,
            string redeemButton)
        {
            int userId = int.Parse(redeemButton);
            await _vendorCodeService.ResolveCodeStatusAsync(userId, false, false);
            return RedirectToAction(nameof(ProfileController.Household), ProfileController.Name);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupUpgradeViewModel viewModel)
        {
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
            catch (Exception ex)
            {
                ShowAlertDanger(_sharedLocalizer[Annotations.Interface.CouldNotCreate,
                    _sharedLocalizer[Annotations.Interface.Group],
                    ex.Message]);
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
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            groupInfo.UserId = authUser.Id;
            await _userService.UpdateGroupName(authUser.Id, groupInfo);
            return RedirectToAction(nameof(Household));
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
                        await _userService.RegisterHouseholdMemberAsync(member, model.Password);
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
    }
}
