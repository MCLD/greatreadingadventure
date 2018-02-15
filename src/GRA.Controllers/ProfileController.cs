using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.Profile;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
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
        private readonly DailyLiteracyTipService _dailyLiteracyTipService;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly MailService _mailService;
        private readonly PointTranslationService _pointTranslationService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        private string HouseholdTitle;

        public ProfileController(ILogger<ProfileController> logger,
            ServiceFacade.Controller context,
            Abstract.IPasswordValidator passwordValidator,
            ActivityService activityService,
            AuthenticationService authenticationService,
            DailyLiteracyTipService dailyLiteracyTipService,
            DynamicAvatarService dynamicAvatarService,
            MailService mailService,
            PointTranslationService pointTranslationService,
            QuestionnaireService questionnaireService,
            SchoolService schoolService,
            SiteService siteService,
            UserService userService,
            VendorCodeService vendorCodeService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _dailyLiteracyTipService = Require.IsNotNull(dailyLiteracyTipService,
                nameof(dailyLiteracyTipService));
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService,
                nameof(dynamicAvatarService));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _pointTranslationService = Require.IsNotNull(pointTranslationService,
                nameof(pointTranslationService));
            _questionnaireService = Require.IsNotNull(questionnaireService,
                nameof(questionnaireService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            PageTitle = "My Profile";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            HouseholdTitle = HttpContext.Items[ItemKey.HouseholdTitle] as string ?? "Family";
        }

        public async Task<IActionResult> Index()
        {
            User user = await _userService.GetDetails(GetActiveUserId());

            int householdCount = await _userService
                .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id);
            var branchList = await _siteService.GetBranches(user.SystemId);
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var userProgram = programList.Where(_ => _.Id == user.ProgramId).SingleOrDefault();
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);

            await _vendorCodeService.PopulateVendorCodeStatusAsync(user);

            ProfileDetailViewModel viewModel = new ProfileDetailViewModel()
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
                RestrictChangingSystemBranch = (await GetSiteSettingBoolAsync(SiteSettingKey.Users.RestrictChangingSystemBranch)),
                CategorySelectionAction = nameof(SchoolCategory),
                ShowPrivateOption = await _schoolService.AnyPrivateSchoolsAsync(),
                ShowCharterOption = await _schoolService.AnyCharterSchoolsAsync()
            };

            if (viewModel.RestrictChangingSystemBranch)
            {
                viewModel.SystemName = systemList.Where(_ => _.Id == viewModel.User.SystemId)
                    .FirstOrDefault()?
                    .Name;
                viewModel.BranchName = branchList.Where(_ => _.Id == viewModel.User.BranchId)
                    .FirstOrDefault()?
                    .Name;
            }

            var districtList = await _schoolService.GetDistrictsAsync(true);
            if (user.SchoolId.HasValue)
            {
                viewModel.SchoolId = user.SchoolId;
                var schoolDetails = await _schoolService.GetSchoolDetailsAsync(user.SchoolId.Value);
                var schoolDistrict = await _schoolService.GetDistrictByIdAsync(
                    schoolDetails.SchoolDistrictId);
                if (schoolDistrict.IsPrivate || schoolDistrict.IsCharter)
                {
                    viewModel.SchoolDistrictList = new SelectList(
                        districtList.ToList(), "Id", "Name");
                    viewModel.SchoolList = new SelectList(
                        schoolDetails.Schools.ToList(), "Id", "Name");

                    if (schoolDistrict.IsPrivate)
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
        public async Task<IActionResult> SchoolCategory(ProfileDetailViewModel model)
        {
            ModelState.Clear();

            model.PublicSelected = false;
            model.PrivateSelected = false;
            model.CharterSelected = false;
            model.IsHomeschooled = false;
            if (model.SetPrivate)
            {
                model.PrivateSelected = true;
                model.SchoolList = new SelectList(
                    await _schoolService.GetPrivateSchoolListAsync(), "Id", "Name");
            }
            else if (model.SetCharter)
            {
                model.CharterSelected = true;
                model.SchoolList = new SelectList(
                    await _schoolService.GetCharterSchoolListAsync(), "Id", "Name");
            }
            else if (model.SetHomeschool)
            {
                model.IsHomeschooled = true;
            }
            else
            {
                model.PublicSelected = true;
            }

            model.CategorySelectionAction = nameof(SchoolCategory);
            model.SchoolDistrictList = new SelectList(
                await _schoolService.GetDistrictsAsync(true), "Id", "Name");
            model.SchoolId = null;
            model.SchoolDistrictId = null;
            model.SchoolNotListed = false;

            var branchList = await _siteService.GetBranches(model.User.SystemId);
            if (model.User.BranchId < 1)
            {
                branchList = branchList.Prepend(new Branch() { Id = -1 });
            }
            var site = await GetCurrentSiteAsync();
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            if (model.User.ProgramId >= 0)
            {
                var program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);
                model.ShowAge = program.AskAge;
                model.ShowSchool = program.AskSchool;
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
            if (askActivityGoal)
            {
                var pointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(model.User.ProgramId);
                model.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(nameof(Index), model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ProfileDetailViewModel model)
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

                    await _userService.Update(model.User);
                    AlertSuccess = "Updated profile";
                    return RedirectToAction("Index");
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
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            model.ShowAge = program.AskAge;
            model.ShowSchool = program.AskSchool;

            model.CategorySelectionAction = nameof(SchoolCategory);
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
                        model.SchoolList = new SelectList(schoolList.ToList(), "Id", "Name");
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
            HouseholdListViewModel viewModel = new HouseholdListViewModel()
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
                DisableSecretCode = await GetSiteSettingBoolAsync(SiteSettingKey.SecretCode.Disable),
                ShowVendorCodes = showVendorCodes,
                PointTranslation = await _pointTranslationService
                        .GetByProgramIdAsync(authUser.ProgramId, true)
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
                                var imagePath = _pathResolver.ResolveContentFilePath(
                                    Path.Combine($"site{site.Id}", "dailyimages",
                                    $"dailyliteracytip{program.DailyLiteracyTipId}",
                                    $"{image.Id}.{image.Extension}"));
                                if (System.IO.File.Exists(imagePath))
                                {
                                    var dailyImageViewModel = new DailyImageViewModel()
                                    {
                                        DailyImageMessage = program.DailyLiteracyTip.Message,
                                        DailyImagePath = imagePath
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
            if (model.ActivityAmount < 1 && model.PointTranslation.IsSingleEvent == false)
            {
                TempData[ActivityMessage] = "You must enter how an amount!";
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
                TempData[ActivityMessage] = "No members selected.";
            }

            return RedirectToAction("Household");
        }

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
                TempData[SecretCodeMessage] = "No members selected.";
            }

            return RedirectToAction("Household");
        }

        public async Task<IActionResult> AddHouseholdMember()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                // if the authUser has a household head then they are not the household head
                return RedirectToAction("Household");
            }

            var (useGroups, maximumHousehold) =
                await GetSiteSettingIntAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup);
            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);

            if (useGroups)
            {
                var groupTypes = await _userService.GetGroupTypeListAsync();

                if (groupTypes.Count() == 0)
                {
                    _logger.LogError($"User {authUser.Id} should be forced to make a group but no group types are configured");
                }
                else
                {
                    var household
                        = await _userService.GetHouseholdAsync(authUser.Id, false, false, false);
                    if (household.Count() > maximumHousehold)
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
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            var districtList = await _schoolService.GetDistrictsAsync(true);

            HouseholdAddViewModel viewModel = new HouseholdAddViewModel()
            {
                User = userBase,
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                CategorySelectionAction = nameof(HouseholdAddSchoolCategory),
                PublicSelected = true,
                ShowPrivateOption = await _schoolService.AnyPrivateSchoolsAsync(),
                ShowCharterOption = await _schoolService.AnyCharterSchoolsAsync(),
                SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name")
            };

            if (programList.Count() == 1)
            {
                var programId = programList.SingleOrDefault().Id;
                var program = await _siteService.GetProgramByIdAsync(programId);
                viewModel.User.ProgramId = programList.SingleOrDefault().Id;
                viewModel.ShowAge = program.AskAge;
                viewModel.ShowSchool = program.AskSchool;
            }

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

        [HttpPost]
        public async Task<IActionResult> HouseholdAddSchoolCategory(HouseholdAddViewModel model)
        {
            ModelState.Clear();

            model.PublicSelected = false;
            model.PrivateSelected = false;
            model.CharterSelected = false;
            model.IsHomeschooled = false;
            if (model.SetPrivate)
            {
                model.PrivateSelected = true;
                model.SchoolList = new SelectList(
                    await _schoolService.GetPrivateSchoolListAsync(), "Id", "Name");
            }
            else if (model.SetCharter)
            {
                model.CharterSelected = true;
                model.SchoolList = new SelectList(
                    await _schoolService.GetCharterSchoolListAsync(), "Id", "Name");
            }
            else if (model.SetHomeschool)
            {
                model.IsHomeschooled = true;
            }
            else
            {
                model.PublicSelected = true;
            }

            model.CategorySelectionAction = nameof(HouseholdAddSchoolCategory);
            model.SchoolDistrictList = new SelectList(
                await _schoolService.GetDistrictsAsync(true), "Id", "Name");
            model.SchoolId = null;
            model.SchoolDistrictId = null;
            model.SchoolNotListed = false;

            var branchList = await _siteService.GetBranches(model.User.SystemId);
            if (model.User.BranchId < 1)
            {
                branchList = branchList.Prepend(new Branch() { Id = -1 });
            }
            var site = await GetCurrentSiteAsync();
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramViewModel>>(programList);
            model.BranchList = new SelectList(branchList.ToList(), "Id", "Name");
            model.SystemList = new SelectList(systemList.ToList(), "Id", "Name");
            model.ProgramList = new SelectList(programList.ToList(), "Id", "Name");
            model.ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject);
            model.RequirePostalCode = site.RequirePostalCode;
            if (model.User.ProgramId >= 0)
            {
                var program = await _siteService.GetProgramByIdAsync(model.User.ProgramId);
                model.ShowAge = program.AskAge;
                model.ShowSchool = program.AskSchool;
            }

            var askIfFirstTime = await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (askIfFirstTime)
            {
                model.AskFirstTime = EmptyNoYes();
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);
            if (askActivityGoal)
            {
                var pointTranslation = programList.First().PointTranslation;
                model.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View("HouseholdAdd", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddHouseholdMember(HouseholdAddViewModel model)
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

            var site = await GetCurrentSiteAsync();
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
                if (program.SchoolRequired && !model.SchoolId.HasValue
                    && !model.SchoolNotListed && !model.IsHomeschooled)
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

                    var newMember = await _userService.AddHouseholdMemberAsync(authUser.Id,
                        model.User);
                    await _mailService.SendUserBroadcastsAsync(newMember.Id, false, true);
                    HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                    var group = await _userService.GetGroupFromHouseholdHeadAsync(authUser.Id);
                    if (group == null)
                    {
                        AlertSuccess = "Added family member";
                    }
                    else
                    {
                        AlertSuccess = "Added group member";
                    }
                    return RedirectToAction("Household");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add member: ", gex);
                }
            }
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

            model.CategorySelectionAction = nameof(HouseholdAddSchoolCategory);
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

        public async Task<IActionResult> AddExistingParticipant()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

            // check if we're going to trip group membership requirements
            var (useGroups, maximumHousehold) =
                await GetSiteSettingIntAsync(SiteSettingKey.Users.MaximumHouseholdSizeBeforeGroup);

            int? addUserId = null;

            if (useGroups)
            {
                var groupTypes = await _userService.GetGroupTypeListAsync();

                if (groupTypes.Count() == 0)
                {
                    _logger.LogError($"User {authUser.Id} should be forced to make a group but no group types are configured");
                }
                else
                {
                    var currentHousehold = await _userService.GetHouseholdAsync(authUser.Id,
                        false,
                        false,
                        false);

                    int totalAddCount = 1;

                    if (currentHousehold.Count() + totalAddCount > maximumHousehold)
                    {
                        var groupInfo
                            = await _userService.GetGroupFromHouseholdHeadAsync(authUser.Id);

                        if (groupInfo == null)
                        {
                            _logger.LogInformation($"Redirecting user {authUser.Id} to create a group when adding member {maximumHousehold + 1}, group will total {currentHousehold.Count() + totalAddCount}");
                            // add authenticated user id to session
                            if (addUserId != null)
                            {
                                HttpContext.Session
                                    .SetString(SessionKey.AbsorbUserId, addUserId.ToString());
                            }
                            return View("GroupUpgrade", new GroupUpgradeViewModel
                            {
                                MaximumHouseholdAllowed = maximumHousehold,
                                GroupTypes = new SelectList(groupTypes.ToList(), "Id", "Name"),
                                AddExisting = true
                            });
                        }
                    }
                }
            }
            // end checking about groups

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddExistingParticipant(HouseholdExistingViewModel model)
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string addedMembers = await _userService
                        .AddParticipantToHouseholdAsync(model.Username, model.Password);
                    HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                    ShowAlertSuccess($"{addedMembers} has been added to your {HouseholdTitle.ToLower()}");
                    return RedirectToAction("Household");
                }
                catch (GraException gex)
                {
                    HttpContext.Session.Remove(SessionKey.AbsorbUserId);
                    ShowAlertDanger($"Could not add participant as {HouseholdTitle.ToLower()} Member: ", gex);
                }
            }
            return View(model);
        }

        [HttpPost]
        private async Task<IActionResult> AddExistingPreAuth()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

            if (!int.TryParse(HttpContext.Session.GetString(SessionKey.AbsorbUserId),
                out int userId))
            {
                return RedirectToAction("Household");
            }

            try
            {
                string addedMembers = await _userService
                    .AddParticipantToHouseholdAlreadyAuthorizedAsync(userId);
                HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                HttpContext.Session.Remove(SessionKey.AbsorbUserId);
                ShowAlertSuccess($"{addedMembers} has been added to your {HouseholdTitle.ToLower()}");
            }
            catch (GraException gex)
            {
                HttpContext.Session.Remove(SessionKey.AbsorbUserId);
                ShowAlertDanger($"Could not add participant as {HouseholdTitle.ToLower()} Member: ", gex);
            }
            return RedirectToAction("Household");
        }

        public IActionResult RegisterHouseholdMember()
        {
            return RedirectToAction("Household");
        }

        [HttpPost]
        public IActionResult CancelGroupUpgrade(GroupUpgradeViewModel viewModel)
        {
            if (viewModel.AddExisting == true)
            {
                HttpContext.Session.Remove(SessionKey.AbsorbUserId);
            }
            return RedirectToAction("Household");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterHouseholdMember(HouseholdRegisterViewModel model)
        {
            var user = await _userService.GetDetails(model.RegisterId);
            var authUser = GetId(ClaimType.UserId);
            if (user.HouseholdHeadUserId != authUser || !string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction("Household");
            }

            if (model.Validate)
            {
                if (ModelState.IsValid)
                {
                    user.Username = model.Username;
                    try
                    {
                        await _userService.RegisterHouseholdMemberAsync(user, model.Password);
                        AlertSuccess = $"{HouseholdTitle} member registered!";
                        return RedirectToAction("Household");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger($"Unable to register {HouseholdTitle.ToLower()} member: ", gex);
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

            if ((user.Id == authUser || user.HouseholdHeadUserId == authUser) && activeUser != loginId)
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
                ShowAlertSuccess($"You are now signed in as {user.FullName}.", "user");
            }
            if (goToMail)
            {
                return RedirectToAction("Index", "Mail");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Books(string sort, string order, int page = 1)
        {
            var filter = new BookFilter(page);

            bool isDescending = String.Equals(order, "Descending", StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(sort) && Enum.IsDefined(typeof(SortBooksBy), sort))
            {
                filter.SortBy = (SortBooksBy)Enum.Parse(typeof(SortBooksBy), sort);
                filter.OrderDescending = isDescending;
            }

            var books = await _userService
                .GetPaginatedUserBookListAsync(GetActiveUserId(), filter);

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

            User user = await _userService.GetDetails(GetActiveUserId());

            BookListViewModel viewModel = new BookListViewModel()
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
        public async Task<IActionResult> EditBook(BookListViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _activityService.UpdateBookAsync(model.Book);
                    ShowAlertSuccess($"'{model.Book.Title}' updated!");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Could not edit book: ", gex.Message);
                }
            }
            else
            {
                ShowAlertDanger("Could not edit book: Missing required fields.");
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction("Books", new { page = page });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook(BookListViewModel model)
        {
            try
            {
                await _activityService.RemoveBookAsync(model.Book.Id);
                ShowAlertSuccess($"'{model.Book.Title}' removed!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Could not remove book: ", gex.Message);
            }

            int? page = null;
            if (model.PaginateModel.CurrentPage != 1)
            {
                page = model.PaginateModel.CurrentPage;
            }
            return RedirectToAction("Books", new { page = page });
        }

        public async Task<IActionResult> History(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);
            var history = await _userService
                .GetPaginatedUserHistoryAsync(GetActiveUserId(), skip, take);

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

            User user = await _userService.GetDetails(GetActiveUserId());

            HistoryListViewModel viewModel = new HistoryListViewModel()
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
                HistoryItemViewModel itemModel = new HistoryItemViewModel()
                {
                    CreatedAt = item.CreatedAt.ToString("d"),
                    Description = item.Description,
                    PointsEarned = item.PointsEarned,
                };
                if (!string.IsNullOrWhiteSpace(item.BadgeFilename))
                {
                    itemModel.BadgeFilename = _pathResolver.ResolveContentPath(item.BadgeFilename);
                }
                else if (item.AvatarBundleId.HasValue)
                {
                    var bundle = await _dynamicAvatarService
                        .GetBundleByIdAsync(item.AvatarBundleId.Value, true);
                    if (bundle.DynamicAvatarItems.Count > 0)
                    {
                        itemModel.BadgeFilename = _pathResolver.ResolveContentPath(
                            bundle.DynamicAvatarItems.FirstOrDefault().Thumbnail);
                        if (bundle.DynamicAvatarItems.Count > 1)
                        {
                            itemModel.Description += $" <strong><a class=\"bundle-link\" data-id=\"{item.AvatarBundleId.Value}\">Click here</a></strong> to see all the items you unlocked.";
                        }
                    }
                }
                viewModel.Historys.Add(itemModel);
            }
            return View(viewModel);
        }

        public async Task<IActionResult> ChangePassword()
        {
            User user = await _userService.GetDetails(GetActiveUserId());
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                return RedirectToAction("Index");
            }

            ChangePasswordViewModel viewModel = new ChangePasswordViewModel()
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
                        AlertSuccess = "Password changed";
                        return RedirectToAction("ChangePassword");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to change password: ", gex);
                    }
                }
                else
                {
                    model.ErrorMessage = "The username and password entered do not match";
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DonateCode(ProfileDetailViewModel viewModel)
        {
            await _vendorCodeService.ResolveDonationStatusAsync(viewModel.User.Id, true);
            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> RedeemCode(ProfileDetailViewModel viewModel)
        {
            await _vendorCodeService.ResolveDonationStatusAsync(viewModel.User.Id, false);
            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> HandleHouseholdDonation(HouseholdListViewModel viewModel,
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
                AlertDanger = "Could not register your choice, please mail the administrators.";
            }
            else
            {
                await _vendorCodeService.ResolveDonationStatusAsync(userId, donationStatus);
            }
            return RedirectToAction("Household", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> HouseholdRedeemCode(HouseholdListViewModel viewModel, string redeemButton)
        {
            int userId = int.Parse(redeemButton);
            await _vendorCodeService.ResolveDonationStatusAsync(userId, false);
            return RedirectToAction("Household", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupUpgradeViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.GroupInfo?.Name?.Trim()))
            {
                AlertDanger = "You must specify a group name.";
                return View("GroupUpgrade", viewModel);
            }

            try
            {
                viewModel.GroupInfo.UserId = GetActiveUserId();
                await _userService.CreateGroup(viewModel.GroupInfo.UserId, viewModel.GroupInfo);
            }
            catch (Exception ex)
            {
                AlertDanger = $"Couldn't create group: {ex.Message}";
                return View("GroupUpgrade", viewModel);
            }
            HttpContext.Session.SetString(SessionKey.CallItGroup, "True");

            if (viewModel.AddExisting == true)
            {
                return RedirectToAction(nameof(AddExistingParticipant));
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
                return RedirectToAction("Household");
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
            return RedirectToAction("Household");
        }
    }
}
