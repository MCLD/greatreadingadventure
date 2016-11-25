using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

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
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
            if (configurationService == null)
            {
                throw new ArgumentNullException(nameof(configurationService));
            }
            this.configurationService = configurationService;
            if (userService == null)
            {
                throw new ArgumentNullException(nameof(userService));
            }
            this.userService = userService;
        }

        public IActionResult Index()
        {
            PageTitle = "Register";
            return View();
        }

        [HttpPost]
        public IActionResult Register(Domain.Model.MissionControl.Register registerResponse)
        {
            var user = new Domain.Model.User
            {
                Username = registerResponse.Username,
                Email = registerResponse.Email,
                FirstName = registerResponse.Username
            };


            if (configurationService.NeedsInitialSetup())
            {
                user = configurationService.InitialSetup(user, registerResponse.Password);
                logger.LogInformation($"Initial account create: {user.Username}");
            }
            else
            {
                user = userService.RegisterUser(user, registerResponse.Password);
                logger.LogInformation($"Created account: {user.Username}");
            }

            AlertSuccess = $"Account created: {user.Username}";

            LoginUser(userService.AuthenticateUser(registerResponse.Username,
                registerResponse.Password));

            return RedirectToRoute(new { controller = "Home" });
        }
    }
}
