using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Filter;
using GRA.Controllers.ViewModel.Join;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Controllers
{
    [UnauthenticatedFilter]
    public class JoinController : Base.UserController
    {
        private const string AuthCodeAssignedBranch = "AuthCodeAssignedBranch";
        private const string AuthCodeAssignedProgram = "AuthCodeAssignedProgram";
        private const string AuthCodeAttempts = "AuthCodeAttempts";
        private const string EnteredAuthCode = "EnteredAuthCode";
        private const string SinglePageSignUp = "SinglePageSignUp";
        private const string TempStep1 = "TempStep1";
        private const string TempStep2 = "TempStep2";
        private const string TwoStepSignUp = "TwoStepSignUp";
        private readonly AuthenticationService _authenticationService;
        private readonly AuthorizationCodeService _authorizationCodeService;
        private readonly JoinCodeService _joinCodeService;
        private readonly LanguageService _languageService;
        private readonly ILogger<JoinController> _logger;
        private readonly MailService _mailService;
        private readonly MapsterMapper.IMapper _mapper;
        private readonly PointTranslationService _pointTranslationService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly SchoolService _schoolService;
        private readonly SegmentService _segmentService;
        private readonly SiteService _siteService;
        private readonly UserService _userService;

        public JoinController(ILogger<JoinController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            AuthorizationCodeService authorizationCodeService,
            JoinCodeService joinCodeService,
            LanguageService languageService,
            MailService mailService,
            PointTranslationService pointTranslationService,
            QuestionnaireService questionnaireService,
            SchoolService schoolService,
            SegmentService segmentService,
            SiteService siteService,
            UserService userService) : base(context)
        {
            ArgumentNullException.ThrowIfNull(authenticationService);
            ArgumentNullException.ThrowIfNull(authorizationCodeService);
            ArgumentNullException.ThrowIfNull(joinCodeService);
            ArgumentNullException.ThrowIfNull(languageService);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(mailService);
            ArgumentNullException.ThrowIfNull(pointTranslationService);
            ArgumentNullException.ThrowIfNull(questionnaireService);
            ArgumentNullException.ThrowIfNull(schoolService);
            ArgumentNullException.ThrowIfNull(segmentService);
            ArgumentNullException.ThrowIfNull(siteService);
            ArgumentNullException.ThrowIfNull(userService);

            _authenticationService = authenticationService;
            _authorizationCodeService = authorizationCodeService;
            _joinCodeService = joinCodeService;
            _languageService = languageService;
            _logger = logger;
            _mailService = mailService;
            _mapper = context?.Mapper;
            _pointTranslationService = pointTranslationService;
            _questionnaireService = questionnaireService;
            _schoolService = schoolService;
            _segmentService = segmentService;
            _siteService = siteService;
            _userService = userService;

            PageTitle = _sharedLocalizer[Annotations.Title.Join];
        }

        public static string Name
        { get { return "Join"; } }

        public IActionResult AuthorizationCode()
        {
            if (TempData.ContainsKey(AuthCodeAttempts) && (int)TempData.Peek(AuthCodeAttempts) >= 5)
            {
                ShowAlertDanger("Too many failed authorization attempts.");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
            return View();
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize authorization codes to lowercase")]
        public async Task<IActionResult> AuthorizationCode(AuthorizationCodeViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var site = await GetCurrentSiteAsync();
            if (!TempData.ContainsKey(AuthCodeAttempts) || (int)TempData.Peek(AuthCodeAttempts) < 5)
            {
                var sanitized = model.AuthorizationCode.Trim().ToLowerInvariant();
                if (await _authorizationCodeService.ValidateAuthorizationCode(sanitized))
                {
                    TempData.Remove(AuthCodeAttempts);
                    TempData[EnteredAuthCode] = model.AuthorizationCode;
                    ShowAlertInfo("Authorization code accepted.");

                    var singlePageSignUp = site.SinglePageSignUp;

                    if (await _authorizationCodeService.SinglePageSignUpCode(sanitized))
                    {
                        TempData[SinglePageSignUp] = true;
                        singlePageSignUp = true;
                    }

                    var programId = await _authorizationCodeService
                        .AssignedProgramFromCode(sanitized);

                    if (programId != null)
                    {
                        TempData[AuthCodeAssignedProgram] = programId;
                    }

                    var branchId = await _authorizationCodeService
                        .AssignedBranchFromCode(sanitized);

                    if (branchId != null)
                    {
                        TempData[AuthCodeAssignedBranch] = branchId;
                    }

                    if (singlePageSignUp)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Step1));
                    }
                }
                if (TempData.TryGetValue(AuthCodeAttempts, out object authCodeAttempts))
                {
                    TempData[AuthCodeAttempts] = (int)authCodeAttempts + 1;
                }
                else
                {
                    TempData[AuthCodeAttempts] = 1;
                }
            }

            if (TempData.ContainsKey(AuthCodeAttempts) && (int)TempData.Peek(AuthCodeAttempts) >= 5)
            {
                ShowAlertDanger("Too many failed authorization attempts.");
                return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
            }
            ShowAlertDanger("Invalid authorization code.");

            return View();
        }

        public async Task<IActionResult> Index(string src)
        {
            string authCode = null;
            var useAuthCode = TempData.ContainsKey(EnteredAuthCode);
            if (useAuthCode)
            {
                authCode = (string)TempData[EnteredAuthCode];
            }

            var site = await GetCurrentSiteAsync();

            var singlePageSignUp = site.SinglePageSignUp
                || TempData.ContainsKey(SinglePageSignUp)
                || (await _authorizationCodeService.SinglePageSignUpCode(authCode));

            if (!singlePageSignUp)
            {
                return RedirectToAction(nameof(Step1), new { src });
            }
            var siteStage = GetSiteStage();

            if (!useAuthCode)
            {
                if (siteStage == SiteStage.BeforeRegistration)
                {
                    if (site.RegistrationOpens.HasValue)
                    {
                        AlertInfo = _sharedLocalizer[Annotations.Info.YouCanJoinOn,
                            site.Name,
                            site.RegistrationOpens.Value.ToString("d", CultureInfo.CurrentCulture)];
                    }
                    else
                    {
                        AlertInfo = _sharedLocalizer[Annotations.Info.RegistrationNotOpenYet,
                            site.Name];
                    }
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                else if (siteStage >= SiteStage.ProgramEnded)
                {
                    AlertInfo = _sharedLocalizer[Annotations.Info.ProgramEnded, site.Name];
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            var systemList = TempData.ContainsKey(AuthCodeAssignedBranch)
                ? Array.Empty<Domain.Model.System>()
                : await _siteService.GetSystemList();
            var programList = TempData
                .TryGetValue(AuthCodeAssignedProgram, out object tempProgramList)
                    ? new Program[] {
                        await _siteService.GetProgramByIdAsync((int)tempProgramList)
                    }
                    : await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

            var viewModel = new SinglePageViewModel
            {
                RequirePostalCode = site.RequirePostalCode,
                ProgramJson = JsonConvert.SerializeObject(programViewObject),
                SystemList = NameIdSelectList(systemList.ToList()),
                ProgramList = NameIdSelectList(programList.ToList()),
                SchoolList = NameIdSelectList(await _schoolService.GetSchoolsAsync()),
                WelcomeMessage = await GetWelcomeMessageAsync()
            };

            if (TempData.TryGetValue(AuthCodeAssignedProgram, out object tempAuthCodeProgram))
            {
                viewModel.ProgramId = (int)tempAuthCodeProgram;
                TempData.Keep(AuthCodeAssignedProgram);
            }

            var askIfFirstTime = !TempData.ContainsKey(SinglePageSignUp)
                && await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);
            if (askIfFirstTime)
            {
                viewModel.AskFirstTime = EmptyNoYes();
            }

            var (askEmailSubscription, askEmailSubscriptionText)
                = TempData.ContainsKey(SinglePageSignUp)
                    ? (false, "")
                    : await GetSiteSettingStringAsync(SiteSettingKey.Users.AskEmailSubPermission);

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
                viewModel.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            List<Branch> branchList = [];

            if (TempData.TryGetValue(AuthCodeAssignedBranch, out object tempAuthCodeBranch))
            {
                var branch = await _siteService
                    .GetBranchByIdAsync((int)tempAuthCodeBranch);
                viewModel.BranchId = branch.Id;
                viewModel.SystemId = branch.SystemId;
                viewModel.EmailSubscriptionRequested = "No";
                viewModel.IsFirstTime = "No";
                TempData.Keep(AuthCodeAssignedBranch);
            }
            else
            {
                JoinCode joinCode = null;
                if (!string.IsNullOrWhiteSpace(src))
                {
                    joinCode = await _joinCodeService
                        .GetByCodeAndIncrementAccessCountAsync(src.Trim());
                }

                if (joinCode != null)
                {
                    if (joinCode.BranchSystemId.HasValue)
                    {
                        branchList.AddRange(await _siteService
                            .GetBranches(joinCode.BranchSystemId.Value));
                    }
                    else
                    {
                        branchList.AddRange(await _siteService.GetAllBranches(true));
                    }

                    viewModel.BranchId = joinCode.BranchId;
                    viewModel.JoinCode = joinCode.Code;
                    viewModel.SystemId = joinCode.BranchSystemId;
                }
                else if (systemList.Count() == 1)
                {
                    var systemId = systemList.Single().Id;
                    branchList.AddRange(await _siteService.GetBranches(systemId));
                    viewModel.SystemId = systemId;
                }
                else
                {
                    branchList.AddRange(await _siteService.GetAllBranches(true));
                }
            }

            if (branchList.Count > 1)
            {
                branchList.Insert(0, new Branch { Id = -1 });
            }
            else
            {
                viewModel.BranchId = branchList.SingleOrDefault()?.Id;
            }
            viewModel.BranchList = NameIdSelectList(branchList.ToList());

            if (programList.Count() == 1)
            {
                var programId = programList.Single().Id;
                var program = await _siteService.GetProgramByIdAsync(programId);
                viewModel.ProgramId = programId;
                viewModel.ShowAge = program.AskAge;
                viewModel.ShowSchool = program.AskSchool;
            }

            if (useAuthCode)
            {
                viewModel.AuthorizationCode = authCode;
            }

            return View(viewModel);
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize authorization codes to lowercase")]
        public async Task<IActionResult> Index(SinglePageViewModel model)
        {
            bool askIfFirstTime;
            bool askAge = false;
            bool askSchool = false;
            var site = await GetCurrentSiteAsync();
            bool askEmailSubscription;
            string askEmailSubscriptionText;
            bool askActivityGoal;
            int defaultDailyGoal;

            var singlePageSignUp = site.SinglePageSignUp || TempData.ContainsKey(SinglePageSignUp);
            TempData.Keep(SinglePageSignUp);

            if (!singlePageSignUp)
            {
                return RedirectToAction(nameof(Step1));
            }
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.PostalCode))
            {
                ModelState.AddModelError(nameof(model.PostalCode),
                    _sharedLocalizer[ErrorMessages.Field,
                    _sharedLocalizer[DisplayNames.ZipCode]]);
            }
            askIfFirstTime = !TempData.ContainsKey(SinglePageSignUp)
                && await GetSiteSettingBoolAsync(SiteSettingKey.Users.AskIfFirstTime);

            if (!askIfFirstTime)
            {
                ModelState.Remove(nameof(model.IsFirstTime));
            }

            (askEmailSubscription, askEmailSubscriptionText)
                = TempData.ContainsKey(SinglePageSignUp)
                    ? (false, "")
                    : await GetSiteSettingStringAsync(SiteSettingKey.Users.AskEmailSubPermission);

            TempData.Keep(SinglePageSignUp);

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
                        _sharedLocalizer[Annotations.Required.EmailForSubscription]);
                }
            }

            (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);

            if (model.ProgramId.HasValue)
            {
                var program = await _siteService.GetProgramByIdAsync(model.ProgramId.Value);

                askAge = program.AskAge;
                askSchool = program.AskSchool;
                if (program.AgeRequired && !model.Age.HasValue)
                {
                    ModelState.AddModelError(DisplayNames.Age,
                        _sharedLocalizer[ErrorMessages.Field,
                            _sharedLocalizer[DisplayNames.Age]]);
                }
                if (program.SchoolRequired && !model.SchoolId.HasValue && !model.SchoolNotListed
                    && !model.IsHomeschooled)
                {
                    ModelState.AddModelError(nameof(model.SchoolId),
                        _sharedLocalizer[ErrorMessages.Field,
                            _sharedLocalizer[DisplayNames.School]]);
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

                if (askActivityGoal && user?.DailyPersonalGoal > 0)
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
                    bool useAuthCode = false;
                    string sanitized = null;
                    if (!string.IsNullOrWhiteSpace(model.AuthorizationCode))
                    {
                        sanitized = model.AuthorizationCode.Trim().ToLowerInvariant();
                        useAuthCode = await _authorizationCodeService
                            .ValidateAuthorizationCode(sanitized);
                        if (!useAuthCode)
                        {
                            _logger.LogError("Invalid authorization code used on join: {AuthorizationCode}",
                                model.AuthorizationCode);
                        }
                    }
                    await _userService.RegisterUserAsync(user, model.Password, false, useAuthCode);
                    var loginAttempt = await _authenticationService
                        .AuthenticateUserAsync(user.Username, model.Password, useAuthCode);
                    await LoginUserAsync(loginAttempt);

                    if (useAuthCode)
                    {
                        string role = await _userService.ActivateAuthorizationCode(sanitized,
                                loginAttempt.User.Id);

                        if (!string.IsNullOrEmpty(role))
                        {
                            var auth = await _authenticationService
                                .RevalidateUserAsync(loginAttempt.User.Id);
                            auth.Message = $"Code applied, you are a member of the role: {role}.";
                            await LoginUserAsync(auth);
                        }
                    }

                    await _mailService.SendUserBroadcastsAsync(loginAttempt.User.Id, false, true,
                        true);
                    var questionnaireId = await _questionnaireService
                            .GetRequiredQuestionnaire(loginAttempt.User.Id, loginAttempt.User.Age);
                    if (questionnaireId.HasValue)
                    {
                        HttpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                            questionnaireId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(model.JoinCode))
                    {
                        await _joinCodeService.IncrementJoinCountForCodeAsync(
                            model.JoinCode.Trim());
                    }

                    if (!TempData.ContainsKey(TempDataKey.UserJoined))
                    {
                        TempData.Add(TempDataKey.UserJoined, true);
                    }

                    TempData.Remove(SinglePageSignUp);
                    TempData.Remove(AuthCodeAssignedProgram);
                    TempData.Remove(AuthCodeAssignedBranch);

                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(_sharedLocalizer[Annotations.Validate.CouldNotCreate,
                       _sharedLocalizer[gex.Message]]);
                    if (gex.GetType() == typeof(GraPasswordValidationException))
                    {
                        ModelState.AddModelError(nameof(model.Password),
                            _sharedLocalizer[Annotations.Validate.PasswordIssue]);
                    }
                }
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            if (model.SystemId.HasValue)
            {
                var branchList = TempData
                    .TryGetValue(AuthCodeAssignedBranch, out object tempAuthCodeBranch)
                        ? new Branch[] {
                            await _siteService.GetBranchByIdAsync((int)tempAuthCodeBranch)
                        }
                        : await _siteService.GetBranches(model.SystemId.Value);
                if (model.BranchId < 1)
                {
                    branchList = branchList.Prepend(new Branch() { Id = -1 });
                }
                model.BranchList = NameIdSelectList(branchList.ToList());
            }
            var systemList = TempData.ContainsKey(AuthCodeAssignedBranch)
                ? Array.Empty<Domain.Model.System>()
                : await _siteService.GetSystemList();
            var programList = TempData
                .TryGetValue(AuthCodeAssignedProgram, out object tempAuthCodeProgram)
                    ? new Program[] {
                        await _siteService.GetProgramByIdAsync((int)tempAuthCodeProgram)
                    }
                    : await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);
            model.SystemList = NameIdSelectList(systemList.ToList());
            model.ProgramList = NameIdSelectList(programList.ToList());
            model.SchoolList = NameIdSelectList(await _schoolService.GetSchoolsAsync());
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
                model.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(model);
        }

        public async Task<IActionResult> Step1(string src)
        {
            string authCode = null;
            var useAuthCode = TempData.ContainsKey(EnteredAuthCode);
            if (useAuthCode)
            {
                authCode = (string)TempData[EnteredAuthCode];
            }

            var site = await GetCurrentSiteAsync();
            var singlePageSignUp = site.SinglePageSignUp
                || TempData.ContainsKey(SinglePageSignUp)
                || (await _authorizationCodeService.SinglePageSignUpCode(authCode));

            if (singlePageSignUp)
            {
                return RedirectToAction(nameof(Index), new { src });
            }

            if (!useAuthCode)
            {
                var siteStage = GetSiteStage();
                if (siteStage == SiteStage.BeforeRegistration)
                {
                    if (site.RegistrationOpens.HasValue)
                    {
                        AlertInfo = _sharedLocalizer[Annotations.Info.YouCanJoinOn,
                            site.Name,
                            site.RegistrationOpens.Value.ToString("d", CultureInfo.CurrentCulture)];
                    }
                    else
                    {
                        AlertInfo = _sharedLocalizer[Annotations.Info.RegistrationNotOpenYet,
                            site.Name];
                    }
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                else if (siteStage >= SiteStage.ProgramEnded)
                {
                    AlertInfo = _sharedLocalizer[Annotations.Info.ProgramEnded, site.Name];
                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            var systemList = TempData.ContainsKey(AuthCodeAssignedBranch)
                ? Array.Empty<Domain.Model.System>()
                : await _siteService.GetSystemList();
            var viewModel = new Step1ViewModel
            {
                RequirePostalCode = site.RequirePostalCode,
                SystemList = NameIdSelectList(systemList.ToList()),
                WelcomeMessage = await GetWelcomeMessageAsync()
            };

            if (TempData.TryGetValue(AuthCodeAssignedProgram, out object tempAuthCodeProgram))
            {
                var program = await _siteService
                    .GetProgramByIdAsync((int)tempAuthCodeProgram);
                TempData.Keep(AuthCodeAssignedProgram);

                if (program?.AskAge == false && !program.AskSchool)
                {
                    TempData[TwoStepSignUp] = true;
                }
            }

            List<Branch> branchList = [];

            if (TempData.TryGetValue(AuthCodeAssignedBranch, out object tempAuthCodeBranch))
            {
                var branch = await _siteService
                    .GetBranchByIdAsync((int)tempAuthCodeBranch);
                viewModel.BranchId = branch.Id;
                viewModel.SystemId = branch.SystemId;
                TempData.Keep(AuthCodeAssignedBranch);
            }
            else
            {
                JoinCode joinCode = null;
                if (!string.IsNullOrWhiteSpace(src))
                {
                    joinCode = await _joinCodeService
                        .GetByCodeAndIncrementAccessCountAsync(src.Trim());
                }

                if (joinCode != null)
                {
                    if (joinCode.BranchSystemId.HasValue)
                    {
                        branchList.AddRange(await _siteService
                        .GetBranches(joinCode.BranchSystemId.Value));
                    }
                    else
                    {
                        branchList.AddRange(await _siteService.GetAllBranches(true));
                    }

                    viewModel.BranchId = joinCode.BranchId;
                    viewModel.JoinCode = joinCode.Code;
                    viewModel.SystemId = joinCode.BranchSystemId;
                }
                else if (systemList.Count() == 1)
                {
                    var systemId = systemList.Single().Id;
                    branchList.AddRange(await _siteService.GetBranches(systemId));
                    viewModel.SystemId = systemId;
                }
                else
                {
                    branchList.AddRange(await _siteService.GetAllBranches(true));
                }
            }

            if (branchList.Count > 1)
            {
                branchList.Insert(0, new Branch { Id = -1 });
            }
            else
            {
                viewModel.BranchId = branchList.SingleOrDefault()?.Id;
            }
            viewModel.BranchList = NameIdSelectList(branchList.ToList());

            if (useAuthCode)
            {
                viewModel.AuthorizationCode = authCode;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Step1(Step1ViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Index));
            }
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.PostalCode))
            {
                ModelState.AddModelError(nameof(model.PostalCode),
                    _sharedLocalizer[ErrorMessages.Field,
                    _sharedLocalizer[DisplayNames.ZipCode]]);
            }
            if (model.SystemId.HasValue
                && model.BranchId.HasValue
                && !await _siteService.ValidateBranch(model.BranchId.Value, model.SystemId.Value))
            {
                ModelState.AddModelError(nameof(model.BranchId),
                    _sharedLocalizer[Annotations.Validate.Branch]);
            }

            if (ModelState.IsValid)
            {
                TempData[TempStep1] = JsonConvert.SerializeObject(model);

                if (TempData.TryGetValue(AuthCodeAssignedProgram, out object tempAuthCodeProgram))
                {
                    var program = await _siteService
                        .GetProgramByIdAsync((int)tempAuthCodeProgram);

                    if (!TempData.ContainsKey(TwoStepSignUp))
                    {
                        return RedirectToAction(nameof(Step2));
                    }

                    TempData[TempStep2] = JsonConvert.SerializeObject(new Step2ViewModel
                    {
                        ProgramId = program.Id,
                        SchoolId = null,
                        SchoolNotListed = false,
                        IsHomeschooled = false,
                        Age = null
                    });

                    TempData.Keep(AuthCodeAssignedProgram);

                    return RedirectToAction(nameof(Step3));
                }

                return RedirectToAction(nameof(Step2));
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];
            var systemList = await _siteService.GetSystemList();
            model.SystemList = NameIdSelectList(systemList.ToList());
            if (model.SystemId.HasValue)
            {
                var branchList = await _siteService.GetBranches(model.SystemId.Value);
                if (model.BranchId < 1)
                {
                    branchList = branchList.Prepend(new Branch() { Id = -1 });
                }
                model.BranchList = NameIdSelectList(branchList.ToList());
            }
            else if (systemList.Count() > 1)
            {
                model.BranchList = NameIdSelectList(await _siteService.GetAllBranches(true));
            }
            model.RequirePostalCode = site.RequirePostalCode;

            return View(model);
        }

        public async Task<IActionResult> Step2()
        {
            var site = await GetCurrentSiteAsync();
            if (site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Index));
            }
            if (!TempData.ContainsKey(TempStep1))
            {
                return RedirectToAction(nameof(Step1));
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

            var viewModel = new Step2ViewModel
            {
                ProgramJson = JsonConvert.SerializeObject(programViewObject),
                ProgramList = NameIdSelectList(programList.ToList()),
                SchoolList = NameIdSelectList(await _schoolService.GetSchoolsAsync())
            };

            if (TempData.TryGetValue(AuthCodeAssignedProgram, out object tempAuthCodeProgram))
            {
                var program = await _siteService
                    .GetProgramByIdAsync((int)tempAuthCodeProgram);

                if (program != null)
                {
                    viewModel.ProgramId = program.Id;
                    viewModel.ShowAge = program.AskAge;
                    viewModel.ShowSchool = program.AskSchool;
                }

                TempData.Keep(AuthCodeAssignedProgram);
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
        public async Task<IActionResult> Step2(Step2ViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Index));
            }
            if (!TempData.ContainsKey(TempStep1))
            {
                return RedirectToAction(nameof(Step1));
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
                    ModelState.AddModelError(DisplayNames.Age,
                        _sharedLocalizer[ErrorMessages.Field,
                            _sharedLocalizer[DisplayNames.Age]]);
                }
                if (program.SchoolRequired && !model.SchoolId.HasValue && !model.SchoolNotListed
                    && !model.IsHomeschooled)
                {
                    ModelState.AddModelError(nameof(model.SchoolId),
                        _sharedLocalizer[ErrorMessages.Field,
                        _sharedLocalizer[DisplayNames.School]]);
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

                TempData[TempStep2] = JsonConvert.SerializeObject(model);
                return RedirectToAction(nameof(Step3));
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);
            model.ProgramList = NameIdSelectList(programList.ToList());
            model.SchoolList = NameIdSelectList(await _schoolService.GetSchoolsAsync());
            model.ProgramJson = JsonConvert.SerializeObject(programViewObject);
            model.ShowAge = askAge;
            model.ShowSchool = askSchool;

            return View(model);
        }

        public async Task<IActionResult> Step3()
        {
            var site = await GetCurrentSiteAsync();
            if (site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Index));
            }
            if (!TempData.ContainsKey(TempStep1))
            {
                return RedirectToAction(nameof(Step1));
            }
            else if (!TempData.ContainsKey(TempStep2))
            {
                return RedirectToAction(nameof(Step2));
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            var viewModel = new Step3ViewModel();

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
                string step2Json = (string)TempData.Peek(TempStep2);
                var step2 = JsonConvert.DeserializeObject<Step2ViewModel>(step2Json);
                var pointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(step2.ProgramId.Value);
                viewModel.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(viewModel);
        }

        [HttpPost]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase",
            Justification = "Normalize authorization code to lowercase")]
        public async Task<IActionResult> Step3(Step3ViewModel model)
        {
            var site = await GetCurrentSiteAsync();
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
                    _sharedLocalizer[Annotations.Required.EmailForSubscription]);
                }
            }

            var (askActivityGoal, defaultDailyGoal) = await GetSiteSettingIntAsync(
                SiteSettingKey.Users.DefaultDailyPersonalGoal);

            if (site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Index));
            }
            if (!TempData.ContainsKey(TempStep1))
            {
                return RedirectToAction(nameof(Step1));
            }
            else if (!TempData.ContainsKey(TempStep2))
            {
                return RedirectToAction(nameof(Step2));
            }

            if (ModelState.IsValid)
            {
                string step1Json = (string)TempData.Peek(TempStep1);
                string step2Json = (string)TempData.Peek(TempStep2);

                var step1 = JsonConvert.DeserializeObject<Step1ViewModel>(step1Json);
                var step2 = JsonConvert.DeserializeObject<Step2ViewModel>(step2Json);

                var user = new User();
                _mapper.Map<Step1ViewModel, User>(step1, user);
                _mapper.Map<Step2ViewModel, User>(step2, user);
                _mapper.Map<Step3ViewModel, User>(model, user);
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

                try
                {
                    bool useAuthCode = false;
                    string sanitized = null;
                    if (!string.IsNullOrWhiteSpace(step1.AuthorizationCode))
                    {
                        sanitized = step1.AuthorizationCode.Trim().ToLowerInvariant();
                        useAuthCode = await _authorizationCodeService
                            .ValidateAuthorizationCode(sanitized);
                        if (!useAuthCode)
                        {
                            _logger.LogError("Invalid authorization code used on join: {AuthorizationCode}",
                                step1.AuthorizationCode);
                        }
                    }
                    await _userService.RegisterUserAsync(user, model.Password, false, useAuthCode);
                    TempData.Remove(TempStep1);
                    TempData.Remove(TempStep2);
                    var loginAttempt = await _authenticationService
                        .AuthenticateUserAsync(user.Username, model.Password, useAuthCode);
                    await LoginUserAsync(loginAttempt);

                    if (useAuthCode)
                    {
                        string role = await _userService.ActivateAuthorizationCode(sanitized,
                                loginAttempt.User.Id);

                        if (!string.IsNullOrEmpty(role))
                        {
                            var auth = await _authenticationService
                                .RevalidateUserAsync(loginAttempt.User.Id);
                            auth.Message = $"Code applied, you are a member of the role: {role}.";
                            await LoginUserAsync(auth);
                        }
                    }

                    await _mailService.SendUserBroadcastsAsync(loginAttempt.User.Id, false, true,
                        true);
                    var questionnaireId = await _questionnaireService
                            .GetRequiredQuestionnaire(loginAttempt.User.Id, loginAttempt.User.Age);
                    if (questionnaireId.HasValue)
                    {
                        HttpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                            questionnaireId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(step1.JoinCode))
                    {
                        await _joinCodeService.IncrementJoinCountForCodeAsync(
                            step1.JoinCode.Trim());
                    }

                    if (!TempData.ContainsKey(TempDataKey.UserJoined))
                    {
                        TempData.Add(TempDataKey.UserJoined, true);
                    }

                    TempData.Remove(TwoStepSignUp);
                    TempData.Remove(AuthCodeAssignedProgram);
                    TempData.Remove(AuthCodeAssignedBranch);

                    return RedirectToAction(nameof(HomeController.Index), HomeController.Name);
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(_sharedLocalizer[Annotations.Validate.CouldNotCreate,
                       _sharedLocalizer[gex.Message]]);
                    if (gex.GetType() == typeof(GraPasswordValidationException))
                    {
                        ModelState.AddModelError(nameof(model.Password),
                            _sharedLocalizer[Annotations.Validate.PasswordIssue]);
                    }
                }
            }

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
                string step2Json = (string)TempData.Peek(TempStep2);
                var step2 = JsonConvert.DeserializeObject<Step2ViewModel>(step2Json);
                var pointTranslation = await _pointTranslationService
                    .GetByProgramIdAsync(step2.ProgramId.Value);
                model.TranslationDescriptionPastTense = pointTranslation
                    .TranslationDescriptionPastTense
                    .Replace("{0}", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            return View(model);
        }

        private async Task<string> GetWelcomeMessageAsync()
        {
            var (welcomeSet, welcomeSegmentId) = await _siteLookupService
                .GetSiteSettingIntAsync(GetCurrentSiteId(), SiteSettingKey.Site.WelcomeMessage);

            if (welcomeSet)
            {
                var languageId = await _languageService
                    .GetLanguageIdAsync(_userContextProvider.GetCurrentCulture()?.Name);

                var message = await _segmentService.GetTextAsync(welcomeSegmentId, languageId);

                if (!string.IsNullOrWhiteSpace(message))
                {
                    return CommonMark.CommonMarkConverter.Convert(message);
                }
            }
            return null;
        }
    }
}
