using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Filter;
using GRA.Controllers.ViewModel.SignIn;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    [UnauthenticatedFilter]
    public class SignInController : Base.UserController
    {
        private readonly ILogger<SignInController> _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly UserService _userService;

        public static string Name { get { return "SignIn"; } }

        public SignInController(ILogger<SignInController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            QuestionnaireService questionnaireService,
            UserService userService)
                : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticationService = authenticationService
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _questionnaireService = questionnaireService
                ?? throw new ArgumentNullException(nameof(questionnaireService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = _sharedLocalizer[Annotations.Title.SignIn];
        }

        public async Task<IActionResult> Index(string ReturnUrl = null, int? statusCode = null)
        {
            var site = await GetCurrentSiteAsync();
            PageTitle = _sharedLocalizer[Annotations.Title.SignInTo, site.Name];

            string sendTo = ReturnUrl;
            if (string.IsNullOrEmpty(sendTo) && TempData.ContainsKey(TempDataKey.ReturnUrl))
            {
                sendTo = TempData[TempDataKey.ReturnUrl].ToString();
                TempData.Remove(TempDataKey.ReturnUrl);
            }
            if (statusCode != null)
            {
                Response.StatusCode = statusCode.Value;
            }
            return View(new SignInViewModel { ReturnUrl = sendTo });
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginTimer = new System.Diagnostics.Stopwatch();
                try
                {
                    loginTimer.Start();
                    _logger.LogTrace("Authenticating user {Username}", model.Username);
                    var loginAttempt = await _authenticationService
                        .AuthenticateUserAsync(model.Username, model.Password);

                    if (loginAttempt.PasswordIsValid)
                    {
                        _logger.LogTrace("Password valid for {Username} ({UserId}), logging in",
                            loginAttempt.User.Username,
                            loginAttempt.User.Id);
                        await LoginUserAsync(loginAttempt);

                        _logger.LogTrace("Awarding triggers for {Username} ({UserId})",
                            loginAttempt.User.Id,
                            loginAttempt.User.Username);

                        var triggerTimer = new System.Diagnostics.Stopwatch();
                        triggerTimer.Start();
                        await _userService.AwardUserBadgesAsync(loginAttempt.User.Id, true, true);
                        triggerTimer.Stop();
                        if (triggerTimer.Elapsed.TotalSeconds > 5)
                        {
                            _logger.LogWarning("Login for user {Username} ({UserId}) took {Elapsed} ms to award triggers.",
                                loginAttempt.User.Username,
                                loginAttempt.User.Id,
                                triggerTimer.ElapsedMilliseconds);
                        }

                        _logger.LogTrace("Checking household count for {Username} ({UserId})",
                            loginAttempt.User.Username,
                            loginAttempt.User.Id);

                        var householdCount = await _userService.FamilyMemberCountAsync(
                            loginAttempt.User.Id, true);
                        if (householdCount > 0)
                        {
                            HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                        }

                        int householdHeadId = loginAttempt.User.HouseholdHeadUserId
                            ?? loginAttempt.User.Id;

                        _logger.LogTrace("Looking up group for {Username} ({UserId})",
                            loginAttempt.User.Username,
                            loginAttempt.User.Id);

                        var group
                            = await _userService.GetGroupFromHouseholdHeadAsync(householdHeadId);
                        if (group != null)
                        {
                            HttpContext.Session.SetString(SessionKey.CallItGroup, "True");
                        }

                        _logger.LogTrace("Performing questionnaire lookup for {Username} ({UserId})",
                            loginAttempt.User.Username,
                            loginAttempt.User.Id);
                        var questionnaireId = await _questionnaireService
                            .GetRequiredQuestionnaire(loginAttempt.User.Id, loginAttempt.User.Age);
                        if (questionnaireId.HasValue)
                        {
                            HttpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                                questionnaireId.Value);
                        }

                        if (!TempData.ContainsKey(TempDataKey.UserSignedIn))
                        {
                            TempData.Add(TempDataKey.UserSignedIn, true);
                        }

                        string role = "Participant";

                        if (loginAttempt.PermissionNames.Contains(nameof(Permission.ManageSites)))
                        {
                            role = "Site manager";
                        }
                        else if (loginAttempt
                            .PermissionNames
                            .Contains(nameof(Permission.AccessMissionControl)))
                        {
                            role = "Staff member";
                        }
                        else if (loginAttempt
                            .PermissionNames
                            .Contains(nameof(Permission.AccessPerformerRegistration)))
                        {
                            role = "Performer";
                        }

                        loginTimer.Stop();
                        _logger.LogInformation("{Role} sign in {Username} ({UserId}) took {Elapsed} ms.",
                            role,
                            loginAttempt.User.Username,
                            loginAttempt.User.Id,
                            triggerTimer.ElapsedMilliseconds);

                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            // if the user has MC access and we aren't open, send to MC
                            if (loginAttempt
                                .PermissionNames
                                .Contains(nameof(Permission.AccessMissionControl))
                                && (GetSiteStage() == SiteStage.BeforeRegistration
                                    || GetSiteStage() == SiteStage.AccessClosed))
                            {
                                return RedirectToAction(
                                    nameof(MissionControl.HomeController.Index),
                                    HomeController.Name,
                                    new { Area = nameof(MissionControl) });
                            }
                            else
                            {
                                return RedirectToAction(nameof(Index), HomeController.Name);
                            }
                        }
                    }
                }
                catch (GraException gex)
                {
                    _logger.LogWarning(gex, "Sign in error for {Username}: {Message}",
                        model.Username,
                        gex.Message);
                    AlertInfo = gex.Message;
                    return RedirectToAction(nameof(Index), HomeController.Name);
                }
                finally
                {
                    if (loginTimer.IsRunning)
                    {
                        loginTimer.Stop();
                    }
                }
                model.ErrorMessage
                    = _sharedLocalizer[Annotations.Validate.UsernamePasswordMismatch];
            }
            return View(model);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("", _sharedLocalizer[ErrorMessages.Field,
                    _sharedLocalizer[DisplayNames.Username]]);
            }
            if (ModelState.IsValid)
            {
                string recoveryUrl = Url.Action(nameof(PasswordRecovery),
                    Name,
                    null,
                    HttpContext.Request.Scheme);
                var result = await _authenticationService
                    .GenerateTokenAndEmail(username, recoveryUrl);

                if (result.Status == Domain.Service.Models.ServiceResultStatus.Success)
                {
                    AlertSuccess = _sharedLocalizer[Annotations.Info.PasswordRecoverySent,
                        username];
                }
                else
                {
                    ShowAlertWarning(_sharedLocalizer[Annotations.Validate.UnableToReset,
                        _sharedLocalizer[result.Message, result.Arguments]]);
                }
            }
            return View();
        }

        public IActionResult ForgotUsername()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotUsername(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", _sharedLocalizer[ErrorMessages.Field,
                    _sharedLocalizer[DisplayNames.EmailAddress]]);
            }
            if (ModelState.IsValid)
            {
                var result = await _authenticationService.EmailAllUsernames(email);
                if (result.Status == Domain.Service.Models.ServiceResultStatus.Success)
                {
                    AlertSuccess = _sharedLocalizer[Annotations.Info.UsernameListSent, email];
                }
                else
                {
                    ShowAlertWarning(_sharedLocalizer[Annotations.Validate.CouldNotRecover,
                        _sharedLocalizer[result.Message, result.Arguments]]);
                }
            }

            return View();
        }

        public IActionResult PasswordRecovery(string username, string token)
        {
            var viewModel = new PasswordRecoveryViewModel
            {
                Username = username,
                Token = token
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PasswordRecovery(PasswordRecoveryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _authenticationService.ResetPassword(model.Username,
                        model.NewPassword,
                        model.Token);
                    if (result.Status == Domain.Service.Models.ServiceResultStatus.Success)
                    {
                        AlertSuccess = _sharedLocalizer[Annotations.Info.PasswordResetFor,
                            model.Username];
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ShowAlertWarning(_sharedLocalizer[Annotations.Validate.UnableToReset,
                            _sharedLocalizer[result.Message, result.Arguments]]);
                    }
                }
                catch (GraException gex)
                {
                    ShowAlertDanger(_sharedLocalizer[Annotations.Validate.UnableToReset,
                       _sharedLocalizer[gex.Message]]);
                    if (gex.GetType() == typeof(GraPasswordValidationException))
                    {
                        ModelState.AddModelError(nameof(model.NewPassword),
                            _sharedLocalizer[Annotations.Validate.PasswordIssue]);
                    }
                }
            }
            return View(model);
        }
    }
}
