using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class RegisterController : Base.Controller
    {
        private readonly ILogger<RegisterController> logger;
        private readonly ConfigurationService configurationService;
        private readonly UserService userService;
        public RegisterController(ILogger<RegisterController> logger,
            ServiceFacade.Controller context,
            ConfigurationService configurationService,
            UserService userService)
            : base(context)
        {
            this.logger = Require.IsNotNull(logger, nameof(logger));
            this.configurationService = Require.IsNotNull(configurationService,
                nameof(configurationService));
            this.userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Register";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Domain.Model.MissionControl.Register registerResponse)
        {
            var user = new Domain.Model.User
            {
                Username = registerResponse.Username,
                Email = registerResponse.Email,
                FirstName = registerResponse.Username
            };


            if (await configurationService.NeedsInitialSetupAsync())
            {
                user = await configurationService.InitialSetupAsync(user, registerResponse.Password);
                logger.LogInformation($"Initial account create: {user.Username}");
            }
            else
            {
                try
                {
                    user = await userService.RegisterUserAsync(user, registerResponse.Password);
                }
                catch (Exception ex)
                {
                    logger.LogError(null, ex, $"Could not register user: {ex.Message}");
                    AlertDanger = $"Could not create account: {ex.Message}";
                    return View("Register");
                }
                logger.LogInformation($"Created account: {user.Username}");
            }

            AlertSuccess = $"Account created: {user.Username}";

            await LoginUserAsync(await userService.AuthenticateUserAsync(registerResponse.Username,
                registerResponse.Password));

            return RedirectToRoute(new { controller = "Home" });
        }
    }
}
