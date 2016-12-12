using GRA.Controllers.ViewModel.SignIn;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class SignInController : Base.Controller
    {
        private readonly ILogger<SignInController> _logger;
        private readonly AuthenticationService _authenticationService;
        private readonly UserService _userService;
        public SignInController(ILogger<SignInController> logger,
            ServiceFacade.Controller context,
            AuthenticationService authenticationService,
            UserService userService)
                : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _authenticationService = Require.IsNotNull(authenticationService,
                nameof(authenticationService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Sign In";
        }

        public async Task<IActionResult> Index(string sitePath = null, string ReturnUrl = null)
        {
            var site = await GetCurrentSite(sitePath);
            PageTitle = $"Sign In to {site.Name}";

            SignInViewModel viewModel = new SignInViewModel();
            viewModel.ReturnUrl = ReturnUrl;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel model, string sitePath = null)
        {
            if (ModelState.IsValid)
            {
                var loginAttempt = await _authenticationService
                    .AuthenticateUserAsync(model.Username, model.Password);
                if (loginAttempt.PasswordIsValid)
                {
                    await LoginUserAsync(loginAttempt);
                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
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
            try
            {
                string recoveryUrl = Url.Action("PasswordRecovery", "SignIn", null, HttpContext.Request.Scheme);
                await _authenticationService.GenerateTokenAndEmail(username, recoveryUrl);
                AlertSuccess = $"A password recovery email has been sent to the email of '{username}'";
            }
            catch(GraException gex)
            {
                AlertWarning = gex.Message;
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
            try
            {
                await _authenticationService.EmailAllUsernames(email);
                AlertSuccess = $"A list of usernames associated with {email} has been emailed to you";
            }
            catch (GraException gex)
            {
                AlertWarning = gex.Message;
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
            if(ModelState.IsValid)
            {
                try
                {
                    await _authenticationService.ResetPassword(model.Username, model.NewPassword, model.Token);
                    AlertSuccess = $"Password reset for {model.Username}";
                    return RedirectToAction("Index");
                }
                catch (GraException gex)
                {
                    AlertWarning = gex.Message;
                }
            }
            return View(model);
        }
    }
}
