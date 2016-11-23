using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
            bool initialSetup = configurationService.NeedsInitialSetup();

            if (initialSetup)
            {
                var user = new Domain.Model.User
                {
                    Username = registerResponse.Username,
                    Email = registerResponse.Email,
                    FirstName = registerResponse.Username
                };
                user = configurationService.InitialSetup(user, registerResponse.Password);

                AlertSuccess = $"Account created: {user.Username}";
                logger.LogInformation($"Initial account create: {user.Username}");
            }
            else
            {
                AlertDanger = "Can't register a regular user yet, sorry!";
            }

            LoginUser(userService.AuthenticateUser(registerResponse.Username,
                registerResponse.Password));

            return RedirectToRoute(new { controller = "Home" });
        }
    }
}
