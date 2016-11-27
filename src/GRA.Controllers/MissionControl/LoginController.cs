using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class LoginController : Base.Controller
    {
        private readonly ILogger<LoginController> logger;
        private readonly UserService userService;
        private readonly ActivityService activityService;
        public LoginController(ILogger<LoginController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            UserService userService)
            : base(context)
        {
            this.logger = Require.IsNotNull(logger, nameof(logger));
            this.userService = Require.IsNotNull(userService, nameof(userService));
            this.activityService = Require.IsNotNull(activityService, nameof(activityService));
            PageTitle = "Sign in";
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Domain.Model.MissionControl.Login model)
        {
            await LoginUserAsync(
                await userService.AuthenticateUserAsync(model.Username, model.Password));
            if (CurrentUser.Identity.IsAuthenticated)
            {
                return View("Index");
            }
            else
            {
                return RedirectToRoute(new { controller = "Home" });
            }
        }
    }
}
