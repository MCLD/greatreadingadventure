using GRA.Controllers.ViewModel.SignIn;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GRA.Controllers
{
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
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _questionnaireService = Require.IsNotNull(questionnaireService,
                nameof(questionnaireService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Sign In";
        }

        public async Task<IActionResult> Index(string ReturnUrl = null)
        {
            var site = await GetCurrentSiteAsync();
            PageTitle = $"Sign In to {site.Name}";

            SignInViewModel viewModel = new SignInViewModel();
            viewModel.ReturnUrl = ReturnUrl;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loginAttempt = await _authenticationService
                        .AuthenticateUserAsync(model.Username, model.Password);

                    if (loginAttempt.PasswordIsValid)
                    {
                        await LoginUserAsync(loginAttempt);
                        await _activityService.AwardUserTriggersAsync(loginAttempt.User.Id, true);
                        var questionnaireId = await _questionnaireService
                            .GetRequiredQuestionnaire(loginAttempt.User.Id, loginAttempt.User.Age);
                        if (questionnaireId.HasValue)
                        {
                            HttpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                                questionnaireId.Value);
                        }
                        if (!string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch (GraException gex)
                {
                    AlertInfo = gex.Message;
                    return RedirectToAction("Index", "Home");
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
                    string recoveryUrl = Url.Action("PasswordRecovery", "SignIn", null, HttpContext.Request.Scheme);
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
                    ShowAlertWarning("Could not recover usernames: ", gex.Message);
                }
            }

            return View();
        }

        public IActionResult PasswordRecovery(string username, string token)
        {
            PasswordRecoveryViewModel viewModel = new PasswordRecoveryViewModel()
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
                    await _authenticationService.ResetPassword(model.Username, model.NewPassword, model.Token);
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
