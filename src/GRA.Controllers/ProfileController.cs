using GRA.Controllers.ViewModel.Profile;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace GRA.Controllers
{
    [Authorize]
    public class ProfileController : Base.UserController
    {
        private const string MinutesReadMessage = "MinutesReadMessage";
        private const string SecretCodeMessage = "SecretCodeMessage";

        private readonly ILogger<ProfileController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly DynamicAvatarService _dynamicAvatarService;
        private readonly MailService _mailService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public ProfileController(ILogger<ProfileController> logger,
            ServiceFacade.Controller context,
            Abstract.IPasswordValidator passwordValidator,
            ActivityService activityService,
            AuthenticationService authenticationService,
            DynamicAvatarService dynamicAvatarService,
            MailService mailService,
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
            _dynamicAvatarService = Require.IsNotNull(dynamicAvatarService,
                nameof(dynamicAvatarService));
            _mailService = Require.IsNotNull(mailService, nameof(mailService));
            _questionnaireService = Require.IsNotNull(questionnaireService,
                nameof(questionnaireService));
            _schoolService = Require.IsNotNull(schoolService, nameof(schoolService));
            _siteService = Require.IsNotNull(siteService, nameof(siteService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            PageTitle = "My Profile";
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

            var vendorCode = await _vendorCodeService.GetUserVendorCodeAsync(user.Id);
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

            ProfileDetailViewModel viewModel = new ProfileDetailViewModel()
            {
                User = user,
                HouseholdCount = householdCount,
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                ShowAge = userProgram.AskAge,
                ShowSchool = userProgram.AskSchool,
                HasSchoolId = user.SchoolId.HasValue,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name")
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
            return View(viewModel);
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

                    await _userService.Update(model.User, hasSchool, model.SchoolDistrictId);
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
            if (!authUserIsHead)
            {
                headUser = await _userService.GetDetails((int)authUser.HouseholdHeadUserId);
            }
            else
            {
                authUser.HasNewMail = await _mailService.UserHasUnreadAsync(authUser.Id);
                var vendorCode = await _vendorCodeService.GetUserVendorCodeAsync(authUser.Id);
                if (vendorCode != null)
                {
                    authUser.VendorCode = vendorCode.Code;
                }
            }

            var household = await _userService
                .GetHouseholdAsync(authUser.HouseholdHeadUserId ?? authUser.Id, authUserIsHead,
                authUserIsHead, authUserIsHead);

            HouseholdListViewModel viewModel = new HouseholdListViewModel()
            {
                Users = household,
                HouseholdCount = household.Count(),
                HasAccount = hasAccount,
                Head = headUser ?? authUser,
                AuthUserIsHead = authUserIsHead,
                ActiveUser = activeUserId
            };

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
                    if (!string.IsNullOrWhiteSpace(program.DailyImageMessage))
                    {
                        var day = _siteLookupService.GetSiteDay(site);
                        if (day.HasValue)
                        {
                            var dailyImage = _pathResolver.ResolveContentPath(
                                Path.Combine($"site{site.Id}", "dailyimages", $"program{program.Id}",
                                $"{day}.jpg"));
                            if (System.IO.File.Exists(dailyImage))
                            {
                                var dailyImageViewModel = new DailyImageViewModel()
                                {
                                    DailyImageMessage = program.DailyImageMessage,
                                    DailyImagePath = dailyImage
                                };
                                dailyImageDictionary.Add(program.Id, dailyImageViewModel);
                            }
                        }
                    }
                }
                viewModel.DailyImageDictionary = dailyImageDictionary;

                var headVendorCode = await _vendorCodeService.GetUserVendorCodeAsync(authUser.Id);
                if (headVendorCode != null)
                {
                    viewModel.Head.VendorCode = headVendorCode.Code;
                    if (headVendorCode.ShipDate.HasValue)
                    {
                        viewModel.Head.VendorCodeMessage = $"Shipped: {headVendorCode.ShipDate.Value.ToString("d")}";
                    }
                    else if (headVendorCode.OrderDate.HasValue)
                    {
                        viewModel.Head.VendorCodeMessage = $"Ordered: {headVendorCode.OrderDate.Value.ToString("d")}";
                    }
                }
            }

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
                TempData[SecretCodeMessage] = "No household members selected.";
            }

            return RedirectToAction("Household");
        }

        public async Task<IActionResult> AddHouseholdMember()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
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
            var districtList = await _schoolService.GetDistrictsAsync();

            HouseholdAddViewModel viewModel = new HouseholdAddViewModel()
            {
                User = userBase,
                RequirePostalCode = (await GetCurrentSiteAsync()).RequirePostalCode,
                ProgramJson = Newtonsoft.Json.JsonConvert.SerializeObject(programViewObject),
                BranchList = new SelectList(branchList.ToList(), "Id", "Name"),
                ProgramList = new SelectList(programList.ToList(), "Id", "Name"),
                SystemList = new SelectList(systemList.ToList(), "Id", "Name"),
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

            return View("HouseholdAdd", viewModel);
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

                    var newMember = await _userService.AddHouseholdMemberAsync(authUser.Id,
                        model.User, model.SchoolDistrictId);
                    await _mailService.SendUserBroadcastsAsync(newMember.Id, false, true);
                    HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                    AlertSuccess = "Added household member";
                    return RedirectToAction("Household");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to add household member: ", gex);
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

        public async Task<IActionResult> AddExistingParticipant()
        {
            var authUser = await _userService.GetDetails(GetId(ClaimType.UserId));
            if (authUser.HouseholdHeadUserId != null)
            {
                return RedirectToAction("Household");
            }

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
                    ShowAlertSuccess(addedMembers + " has been added to your household");
                    return RedirectToAction("Household");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Could not add participant to household: ", gex.Message);
                }
            }
            return View(model);
        }

        public IActionResult RegisterHouseholdMember()
        {
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
                        AlertSuccess = "Household member registered!";
                        return RedirectToAction("Household");
                    }
                    catch (GraException gex)
                    {
                        ShowAlertDanger("Unable to register household member: ", gex);
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

        public async Task<IActionResult> Books(int page = 1)
        {
            int take = 15;
            int skip = take * (page - 1);

            var books = await _userService
                .GetPaginatedUserBookListAsync(GetActiveUserId(), skip, take);

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

            User user = await _userService.GetDetails(GetActiveUserId());

            BookListViewModel viewModel = new BookListViewModel()
            {
                Books = books.Data,
                PaginateModel = paginateModel,
                HouseholdCount = await _userService
                    .FamilyMemberCountAsync(user.HouseholdHeadUserId ?? user.Id),
                HasAccount = !string.IsNullOrWhiteSpace(user.Username),
                CanEditBooks = GetSiteStage() == SiteStage.ProgramOpen,
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
    }
}
