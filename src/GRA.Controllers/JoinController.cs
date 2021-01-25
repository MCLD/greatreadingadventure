﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
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
        private const string TempStep1 = "TempStep1";
        private const string TempStep2 = "TempStep2";

        private const string AuthCodeAttempts = "AuthCodeAttempts";
        private const string EnteredAuthCode = "EnteredAuthCode";

        private readonly ILogger<JoinController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly AuthenticationService _authenticationService;
        private readonly AuthorizationCodeService _authorizationCodeService;
        private readonly MailService _mailService;
        private readonly PointTranslationService _pointTranslationService;
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly UserService _userService;

        private readonly ICodeSanitizer _codeSanitizer;

        public static string Name { get { return "Join"; } }

        public JoinController(ILogger<JoinController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            AuthorizationCodeService authorizationCodeService,
            MailService mailService,
            PointTranslationService pointTranslationService,
            SchoolService schoolService,
            SiteService siteService,
            QuestionnaireService questionnaireService,
            UserService userService,
            ICodeSanitizer codeSanitizer)
                : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = context.Mapper;
            _authenticationService = authenticationService
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _authorizationCodeService = authorizationCodeService
                ?? throw new ArgumentNullException(nameof(authorizationCodeService));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _pointTranslationService = pointTranslationService
                ?? throw new ArgumentNullException(nameof(pointTranslationService));
            _schoolService = schoolService
                ?? throw new ArgumentNullException(nameof(schoolService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
            _questionnaireService = questionnaireService
                ?? throw new ArgumentNullException(nameof(questionnaireService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _codeSanitizer = codeSanitizer
                ?? throw new ArgumentNullException(nameof(codeSanitizer));
            PageTitle = _sharedLocalizer[Annotations.Title.Join];
        }

        public async Task<IActionResult> Index()
        {
            string authCode = null;
            var useAuthCode = TempData.ContainsKey(EnteredAuthCode);
            if (useAuthCode)
            {
                authCode = (string)TempData[EnteredAuthCode];
            }

            var site = await GetCurrentSiteAsync();
            if (!site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Step1));
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
                            site.RegistrationOpens.Value.ToString("d")];
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

            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
            var programViewObject = _mapper.Map<List<ProgramSettingsViewModel>>(programList);

            var viewModel = new SinglePageViewModel
            {
                RequirePostalCode = site.RequirePostalCode,
                ProgramJson = JsonConvert.SerializeObject(programViewObject),
                SystemList = NameIdSelectList(systemList.ToList()),
                ProgramList = NameIdSelectList(programList.ToList()),
                SchoolList = NameIdSelectList(await _schoolService.GetSchoolsAsync())
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
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
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
                viewModel.BranchList = NameIdSelectList(branchList.ToList());
                viewModel.SystemId = systemId;
            }

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
        public async Task<IActionResult> Index(SinglePageViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (!site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Step1));
            }
            if (site.RequirePostalCode && string.IsNullOrWhiteSpace(model.PostalCode))
            {
                ModelState.AddModelError(nameof(model.PostalCode),
                    _sharedLocalizer[ErrorMessages.Field,
                        _sharedLocalizer[DisplayNames.ZipCode]]);
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
                var subscriptionRequested = model.EmailSubscriptionRequested.Equals(
                        DropDownTrueValue, StringComparison.OrdinalIgnoreCase);
                if (subscriptionRequested && string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.AddModelError(nameof(model.Email), " ");
                    ModelState.AddModelError(nameof(model.EmailSubscriptionRequested),
                        _sharedLocalizer[Annotations.Required.EmailForSubscription]);
                }
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
                    user.IsEmailSubscribed = model.EmailSubscriptionRequested.Equals(
                        DropDownTrueValue, StringComparison.OrdinalIgnoreCase);
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
                    if (!string.IsNullOrWhiteSpace(model.AuthorizationCode))
                    {
                        sanitized = _codeSanitizer.Sanitize(model.AuthorizationCode, 255);
                        useAuthCode = await _authorizationCodeService
                            .ValidateAuthorizationCode(sanitized);
                        if (!useAuthCode)
                        {
                            _logger.LogError($"Invalid auth code used on join: {model.AuthorizationCode}");
                        }
                    }
                    await _userService.RegisterUserAsync(user, model.Password,
                        allowDuringCloseProgram: useAuthCode);
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

                    if (!TempData.ContainsKey(TempDataKey.UserJoined))
                    {
                        TempData.Add(TempDataKey.UserJoined, true);
                    }

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
                var branchList = await _siteService.GetBranches(model.SystemId.Value);
                if (model.BranchId < 1)
                {
                    branchList = branchList.Prepend(new Branch() { Id = -1 });
                }
                model.BranchList = NameIdSelectList(branchList.ToList());
            }
            var systemList = await _siteService.GetSystemList();
            var programList = await _siteService.GetProgramList();
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
                model.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(model);
        }

        public async Task<IActionResult> Step1()
        {
            string authCode = null;
            var useAuthCode = TempData.ContainsKey(EnteredAuthCode);
            if (useAuthCode)
            {
                authCode = (string)TempData[EnteredAuthCode];
            }

            var site = await GetCurrentSiteAsync();
            if (site.SinglePageSignUp)
            {
                return RedirectToAction(nameof(Index));
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
                            site.RegistrationOpens.Value.ToString("d")];
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

            var systemList = await _siteService.GetSystemList();
            var viewModel = new Step1ViewModel
            {
                RequirePostalCode = site.RequirePostalCode,
                SystemList = NameIdSelectList(systemList.ToList())
            };

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
                viewModel.BranchList = NameIdSelectList(branchList.ToList());
                viewModel.SystemId = systemId;
            }
            else
            {
                viewModel.BranchList = NameIdSelectList(await _siteService.GetAllBranches(true));
            }

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
                viewModel.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                viewModel.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            return View(viewModel);
        }

        [HttpPost]
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
                var subscriptionRequested = model.EmailSubscriptionRequested.Equals(
                        DropDownTrueValue, StringComparison.OrdinalIgnoreCase);
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
                    user.IsEmailSubscribed = model.EmailSubscriptionRequested.Equals(
                        DropDownTrueValue, StringComparison.OrdinalIgnoreCase);
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
                        sanitized = _codeSanitizer.Sanitize(step1.AuthorizationCode, 255);
                        useAuthCode = await _authorizationCodeService
                            .ValidateAuthorizationCode(sanitized);
                        if (!useAuthCode)
                        {
                            _logger.LogError($"Invalid auth code used on join: {step1.AuthorizationCode}");
                        }
                    }
                    await _userService.RegisterUserAsync(user, model.Password,
                        allowDuringCloseProgram: useAuthCode);
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

                    if (!TempData.ContainsKey(TempDataKey.UserJoined))
                    {
                        TempData.Add(TempDataKey.UserJoined, true);
                    }

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
                model.TranslationDescriptionPastTense =
                    pointTranslation.TranslationDescriptionPastTense.Replace("{0}", "").Trim();
                model.ActivityDescriptionPlural = pointTranslation.ActivityDescriptionPlural;
            }

            PageTitle = _sharedLocalizer[Annotations.Title.JoinNow, site.Name];

            return View(model);
        }

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
        public async Task<IActionResult> AuthorizationCode(AuthorizationCodeViewModel model)
        {
            var site = await GetCurrentSiteAsync();
            if (!TempData.ContainsKey(AuthCodeAttempts) || (int)TempData.Peek(AuthCodeAttempts) < 5)
            {
                var sanitized = _codeSanitizer.Sanitize(model.AuthorizationCode, 255);
                if (await _authorizationCodeService.ValidateAuthorizationCode(sanitized))
                {
                    TempData.Remove(AuthCodeAttempts);
                    TempData[EnteredAuthCode] = model.AuthorizationCode;
                    ShowAlertInfo("Authorization code accepted.");

                    if (site.SinglePageSignUp)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return RedirectToAction(nameof(Step1));
                    }
                }
                if (TempData.ContainsKey(AuthCodeAttempts))
                {
                    TempData[AuthCodeAttempts] = (int)TempData[AuthCodeAttempts] + 1;
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
    }
}
