using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ViewParticipantList)]
    public class ParticipantsController : Base.MCController
    {
        private const string MinutesReadMessage = "MinutesReadMessage";
        private const string SecretCodeMessage = "SecretCodeMessage";

        private readonly ILogger<ParticipantsController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly MailService _mailService;
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
            DynamicAvatarService dynamicAvatarService,
            MailService mailService,
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
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService,
                nameof(dynamicAvatarService));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
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
            var districtList = await _schoolService.GetDistrictsAsync();

            ParticipantsAddViewModel viewModel = new ParticipantsAddViewModel()
            {
                RequirePostalCode = site.RequirePostalCode,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name")
            };

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
                if (program.SchoolRequired)
                {
                    if (!model.NewEnteredSchool && !model.SchoolId.HasValue)
                    {
                        ModelState.AddModelError("SchoolId", "The School field is required.");
                    }
                    else if (model.NewEnteredSchool
                        && string.IsNullOrWhiteSpace(model.EnteredSchoolName))
                    {
                        ModelState.AddModelError("EnteredSchoolName", "The School Name field is required.");
                    }
                }
                if (model.NewEnteredSchool && !model.SchoolDistrictId.HasValue
                    && ((program.AskSchool && !string.IsNullOrWhiteSpace(model.EnteredSchoolName))
                        || program.SchoolRequired))
                {
                    ModelState.AddModelError("SchoolDistrictId", "The School District field is required.");
                }
            }

            if (ModelState.IsValid)
            {
                if (!askAge)
                {
                    model.Age = null;
                }
                if (askSchool)
                {
                    if (model.NewEnteredSchool)
                    {
                        model.SchoolId = null;
                    }
                    else
                    {
                        model.EnteredSchoolName = null;
                    }
                }
                else
                {
                    model.SchoolId = null;
                    model.EnteredSchoolName = null;
                }

                User user = _mapper.Map<User>(model);
                user.SiteId = site.Id;
                try
                {
                    var newUser = await _userService.RegisterUserAsync(user, model.Password,
                        model.SchoolDistrictId, true);
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

            var districtList = await _schoolService.GetDistrictsAsync();
            if (model.SchoolId.HasValue)
            {
                var schoolDetails = await _schoolService.GetSchoolDetailsAsync(model.SchoolId.Value);
                var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDisctrictId);
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name",
                    schoolDetails.SchoolDisctrictId);
                model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                    schoolDetails.SchoolTypeId);
                model.SchoolList = new SelectList(schoolDetails.Schools.ToList(), "Id", "Name");
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

                var vendorCode = await _vendorCodeService.GetUserVendorCodeAsync(id);
                if (vendorCode != null)
                {
                    user.VendorCode = vendorCode.Code;
                    if (vendorCode.ShipDate.HasValue)
                    {
                        user.VendorCodeMessage = $"Shipped: {vendorCode.ShipDate.Value.ToString("d")}";
                    }
                    else if (vendorCode.OrderDate.HasValue)
                    {
                        user.VendorCodeMessage = $"Ordered: {vendorCode.OrderDate.Value.ToString("d")}";
                    }
                }

                ParticipantsDetailViewModel viewModel = new ParticipantsDetailViewModel()
                {
                    User = user,
                    Id = user.Id,
                    Username = user.Username,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                    ShowAge = userProgram.AskAge,
                    ShowSchool = userProgram.AskSchool,
                    HasSchoolId = user.SchoolId.HasValue,
                    ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name")
                };

                var districtList = await _schoolService.GetDistrictsAsync();
                if (user.SchoolId.HasValue)
                {
                    var schoolDetails = await _schoolService.GetSchoolDetailsAsync(user.SchoolId.Value);
                    var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDisctrictId);
                    viewModel.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name",
                        schoolDetails.SchoolDisctrictId);
                    viewModel.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                        schoolDetails.SchoolTypeId);
                    viewModel.SchoolList = new SelectList(schoolDetails.Schools.ToList(), "Id", "Name");
                }
                else
                {
                    viewModel.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name");
                }

                if (UserHasPermission(Permission.EditParticipantUsernames)
                    && !string.IsNullOrWhiteSpace(user.Username))
                {
                    viewModel.CanEditUsername = true;
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
            if (program.SchoolRequired && !model.User.EnteredSchoolId.HasValue)
            {
                if (!model.NewEnteredSchool && !model.User.SchoolId.HasValue)
                {
                    ModelState.AddModelError("User.SchoolId", "The School field is required.");
                }
                else if (model.NewEnteredSchool
                    && string.IsNullOrWhiteSpace(model.User.EnteredSchoolName))
                {
                    ModelState.AddModelError("User.EnteredSchoolName", "The School Name field is required.");
                }
            }
            if (model.NewEnteredSchool && !model.SchoolDistrictId.HasValue
                && ((program.AskSchool && !string.IsNullOrWhiteSpace(model.User.EnteredSchoolName))
                    || program.SchoolRequired))
            {
                ModelState.AddModelError("SchoolDistrictId", "The School District field is required.");
            }
            if (model.CanEditUsername && string.IsNullOrWhiteSpace(model.User.Username))
            {
                ModelState.AddModelError("User.Username", "The Username field is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bool hasSchool = false;
                    if (!program.AskAge)
                    {
                        model.User.Age = null;
                    }
                    if (program.AskSchool)
                    {
                        hasSchool = true;
                        if (model.NewEnteredSchool || model.User.EnteredSchoolId.HasValue)
                        {
                            model.User.SchoolId = null;
                        }
                        else
                        {
                            model.User.EnteredSchoolId = null;
                            model.User.EnteredSchoolName = null;
                        }
                    }

                    await _userService.MCUpdate(model.User, hasSchool, model.SchoolDistrictId);
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

            var districtList = await _schoolService.GetDistrictsAsync();
            if (model.User.SchoolId.HasValue)
            {
                var schoolDetails = await _schoolService.GetSchoolDetailsAsync(model.User.SchoolId.Value);
                var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDisctrictId);
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name",
                    schoolDetails.SchoolDisctrictId);
                model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                    schoolDetails.SchoolTypeId);
                model.SchoolList = new SelectList(schoolDetails.Schools.ToList(), "Id", "Name");
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

            return View(model);
        }
        #endregion

        #region Log Activity
        [Authorize(Policy = Policy.LogActivityForAny)]
        public async Task<IActionResult> LogActivity(int id)
        {
            var user = await _userService.GetDetails(id);
            SetPageTitle(user);

            LogActivityViewModel viewModel = new LogActivityViewModel()
            {
                Id = id,
                HasPendingQuestionnaire = (await _questionnaireService
                    .GetRequiredQuestionnaire(user.Id, user.Age)).HasValue,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username)
            };

            return View(viewModel);
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> LogActivity(LogActivityViewModel model, bool isSecretCode)
        {
            var user = await _userService.GetDetails(model.Id);
            SetPageTitle(user);

            if (!model.IsSecretCode)
            {
                if (!model.MinutesRead.HasValue || model.MinutesRead.Value < 1)
                {
                    ModelState.AddModelError("MinutesRead", "Enter a number greater than 0.");
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        await _activityService.LogActivityAsync(model.Id, model.MinutesRead.Value);
                        ShowAlertSuccess("Minutes applied!");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to apply minutes: ", gex.Message);
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
            return View(model);
        }
        #endregion

        #region Household
        [Authorize(Policy = Policy.ViewParticipantDetails)]
        public async Task<IActionResult> Household(int id)
        {
            try
            {
                var user = await _userService.GetDetails(id);
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
                var headVendorCode = await _vendorCodeService.GetUserVendorCodeAsync(head.Id);
                if (headVendorCode != null)
                {
                    head.VendorCode = headVendorCode.Code;
                    if (headVendorCode.ShipDate.HasValue)
                    {
                        head.VendorCodeMessage = $"Shipped: {headVendorCode.ShipDate.Value.ToString("d")}";
                    }
                    else if (headVendorCode.OrderDate.HasValue)
                    {
                        head.VendorCodeMessage = $"Ordered: {headVendorCode.OrderDate.Value.ToString("d")}";
                    }
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

                var household = await _userService.GetHouseholdAsync(head.Id, true, true, ReadAllMail,
                    ViewUserPrizes);

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
                    PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanEditDetails = UserHasPermission(Permission.EditParticipants),
                    CanLogActivity = UserHasPermission(Permission.LogActivityForAny),
                    CanReadMail = ReadAllMail,
                    CanViewPrizes = ViewUserPrizes,
                    Head = head,
                    SystemId = systemId,
                    BranchList = branchList,
                    SystemList = systemList
                };

                if (TempData.ContainsKey(MinutesReadMessage))
                {
                    viewModel.MinutesReadMessage = (string)TempData[MinutesReadMessage];
                }
                if (TempData.ContainsKey(SecretCodeMessage))
                {
                    viewModel.SecretCodeMessage = (string)TempData[SecretCodeMessage];
                }

                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view participant's household: ", gex);
                return RedirectToAction("Index");
            }
        }

        [Authorize(Policy = Policy.LogActivityForAny)]
        [HttpPost]
        public async Task<IActionResult> HouseholdApplyMinutesRead(HouseholdListViewModel model)
        {
            if (model.MinutesRead < 1)
            {
                TempData[MinutesReadMessage] = "You must enter how many minutes!";
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
                    await _activityService.LogHouseholdMinutesAsync(userSelection, model.MinutesRead);
                    ShowAlertSuccess("Minutes applied!");
                }
                catch (GraException gex)
                {
                    TempData[MinutesReadMessage] = gex.Message;
                }
            }
            else
            {
                TempData[MinutesReadMessage] = "No household members selected.";
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
                TempData[SecretCodeMessage] = "No household members selected.";
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
                ShowAlertDanger("Could not promote to head of household: ", gex.Message);
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
                ShowAlertSuccess("Participant removed from household.");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Could not remove from household: ", gex.Message);
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

                SetPageTitle(headOfHousehold, "Add Household Member");

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
                var districtList = await _schoolService.GetDistrictsAsync();

                HouseholdAddViewModel viewModel = new HouseholdAddViewModel()
                {
                    User = userBase,
                    Id = id,
                    RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                    ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                    BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                    ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                    SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                    SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name")
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
                if (program.SchoolRequired)
                {
                    if (!model.NewEnteredSchool && !model.User.SchoolId.HasValue)
                    {
                        ModelState.AddModelError("User.SchoolId", "The School field is required.");
                    }
                    else if (model.NewEnteredSchool
                        && string.IsNullOrWhiteSpace(model.User.EnteredSchoolName))
                    {
                        ModelState.AddModelError("User.EnteredSchoolName", "The School Name field is required.");
                    }
                }
                if (model.NewEnteredSchool && !model.SchoolDistrictId.HasValue
                    && ((program.AskSchool && !string.IsNullOrWhiteSpace(model.User.EnteredSchoolName))
                        || program.SchoolRequired))
                {
                    ModelState.AddModelError("SchoolDistrictId", "The School District field is required.");
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
                    if (askSchool)
                    {
                        if (model.NewEnteredSchool)
                        {
                            model.User.SchoolId = null;
                        }
                        else
                        {
                            model.User.EnteredSchoolName = null;
                        }
                    }
                    else
                    {
                        model.User.SchoolId = null;
                        model.User.EnteredSchoolName = null;
                    }

                    var newMember = await _userService.AddHouseholdMemberAsync(headOfHousehold.Id,
                        model.User, model.SchoolDistrictId);
                    await _mailService.SendUserBroadcastsAsync(newMember.Id, false, true);
                    AlertSuccess = "Added household member";
                    return RedirectToAction("Household", new { id = model.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to add household member: ", gex);
                }
            }
            SetPageTitle(headOfHousehold, "Add Household Member");

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

            var districtList = await _schoolService.GetDistrictsAsync();
            if (model.User.SchoolId.HasValue)
            {
                var schoolDetails = await _schoolService.GetSchoolDetailsAsync(model.User.SchoolId.Value);
                var typeList = await _schoolService.GetTypesAsync(schoolDetails.SchoolDisctrictId);
                model.SchoolDistrictList = new SelectList(districtList.ToList(), "Id", "Name",
                    schoolDetails.SchoolDisctrictId);
                model.SchoolTypeList = new SelectList(typeList.ToList(), "Id", "Name",
                    schoolDetails.SchoolTypeId);
                model.SchoolList = new SelectList(schoolDetails.Schools.ToList(), "Id", "Name");
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
                    ShowAlertDanger("Unable to register household member: ", gex);
                }
            }
            SetPageTitle(user, "Register Household Memeber");
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
                ShowAlertSuccess("Participant has been added to household!");
            }
            catch (GraException gex)
            {
                ShowAlertDanger("Unable to add participant to household: ", gex);
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
                    HasPendingQuestionnaire = (await _questionnaireService
                        .GetRequiredQuestionnaire(user.Id, user.Age)).HasValue,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanEditBooks = UserHasPermission(Permission.LogActivityForAny)
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

                HistoryListViewModel viewModel = new HistoryListViewModel()
                {
                    Historys = new List<HistoryItemViewModel>(),
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
                    HeadOfHouseholdId = user.HouseholdHeadUserId,
                    HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                    CanRemoveHistory = UserHasPermission(Permission.LogActivityForAny),
                    TotalPoints = user.PointsEarned
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
        public async Task<IActionResult> DeleteHistory(int id, int userId)
        {
            try
            {
                await _activityService.RemoveActivityAsync(userId, id);
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

                PrizeListViewModel viewModel = new PrizeListViewModel()
                {
                    PrizeWinners = prizeList.Data,
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
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

                MailListViewModel viewModel = new MailListViewModel()
                {
                    Mails = mail.Data,
                    PaginateModel = paginateModel,
                    Id = id,
                    HouseholdCount = await _userService.FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
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

                PasswordResetViewModel viewModel = new PasswordResetViewModel()
                {
                    Id = id,
                    HouseholdCount = await _userService
                        .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? id),
                    PrizeCount = await _prizeWinnerService.GetUserWinCount(id, false),
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
                    ShowAlertDanger("Unable to change password: ", gex);
                }
            }

            SetPageTitle(user);
            return View(model);
        }
        #endregion

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
