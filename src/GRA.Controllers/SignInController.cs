using System;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.Filter;
using GRA.Controllers.ViewModel.SignIn;
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
        private readonly ActivityService _activityService;
        private readonly AuthenticationService _authenticationService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly UserService _userService;

        public SignInController(ILogger<SignInController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            AuthenticationService authenticationService,
            QuestionnaireService questionnaireService,
            UserService userService)
                : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
            _authenticationService = authenticationService
                ?? throw new ArgumentNullException(nameof(authenticationService));
            _questionnaireService = questionnaireService
                ?? throw new ArgumentNullException(nameof(questionnaireService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            PageTitle = "Sign In";
        }

        public async Task<IActionResult> Index(string ReturnUrl = null)
        {
            var site = await GetCurrentSiteAsync();
            PageTitle = $"Sign In to {site.Name}";

            var viewModel = new SignInViewModel
            {
                ReturnUrl = ReturnUrl
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogTrace("Authenticating user {Username}", model.Username);
                    var loginAttempt = await _authenticationService
                        .AuthenticateUserAsync(model.Username, model.Password);

                    if (loginAttempt.PasswordIsValid)
                    {
                        _logger.LogTrace("Password valid for {Username}, logging in",
                            model.Username);
                        await LoginUserAsync(loginAttempt);
                        _logger.LogTrace("Awarding triggers for {Username}", model.Username);
                        await _activityService.AwardUserTriggersAsync(loginAttempt.User.Id, true);

                        _logger.LogTrace("Checking household count for {Username}", model.Username);
                        var householdCount = await _userService.FamilyMemberCountAsync(
                            loginAttempt.User.Id, true);
                        if (householdCount > 0)
                        {
                            HttpContext.Session.SetString(SessionKey.HeadOfHousehold, "True");
                        }

                        int householdHeadId = loginAttempt.User.HouseholdHeadUserId
                            ?? loginAttempt.User.Id;

                        _logger.LogTrace("Looking up group for {Username}", model.Username);
                        var group
                            = await _userService.GetGroupFromHouseholdHeadAsync(householdHeadId);
                        if (group != null)
                        {
                            HttpContext.Session.SetString(SessionKey.CallItGroup, "True");
                        }

                        _logger.LogTrace("Performing questionnaire lookup for {Username}",
                            model.Username);
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

                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            // if the user has Mission Control access and we aren't open, send to MC
                            if (loginAttempt.PermissionNames.Contains(nameof(Domain.Model.Permission.AccessMissionControl))
                                && (GetSiteStage() == Domain.Model.SiteStage.BeforeRegistration
                                    || GetSiteStage() == Domain.Model.SiteStage.AccessClosed))
                            {
                                return RedirectToAction(nameof(MissionControl.HomeController.Index),
                                    "Home",
                                    new { Area = nameof(MissionControl) });
                            }
                            else
                            {
                                return RedirectToAction(nameof(Index), "Home");
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
                    return RedirectToAction(nameof(Index), "Home");
                }
                model.ErrorMessage = "The username and password entered do not match";
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
                ModelState.AddModelError("", "The Username field is required");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string recoveryUrl = Url.Action("PasswordRecovery",
                        "SignIn",
                        null,
                        HttpContext.Request.Scheme);
                    await _authenticationService.GenerateTokenAndEmail(username, recoveryUrl);
                    AlertSuccess = $"A password recovery email has been sent to the email of <strong>{username}</strong>.";
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not recover password: ", gex);
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
                ModelState.AddModelError("", "The Email field is required");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _authenticationService.EmailAllUsernames(email);
                    AlertSuccess = $"A list of usernames associated with <strong>{email}</strong> has been sent.";
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Could not recover username(s): ", gex.Message);
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
                    await _authenticationService.ResetPassword(model.Username,
                        model.NewPassword,
                        model.Token);
                    AlertSuccess = $"Password reset for {model.Username}";
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    ShowAlertWarning("Unable to reset password: ", gex);
                }
            }
            return View(model);
        }
    }
}
