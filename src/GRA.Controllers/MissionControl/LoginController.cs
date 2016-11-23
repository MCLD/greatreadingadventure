using GRA.Domain.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class LoginController : Base.Controller
    {
        private readonly ILogger<LoginController> logger;
        private readonly UserService userService;
        public LoginController(ILogger<LoginController> logger,
            ServiceFacade.Controller context,
            UserService userService)
            : base(context)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            this.logger = logger;
            if (userService == null)
            {
                throw new ArgumentNullException(nameof(userService));
            }
            this.userService = userService;
        }

        public IActionResult Index()
        {
            PageTitle = "Sign in";
            return View();
        }

        [HttpPost]
        public IActionResult Login(Domain.Model.MissionControl.Login model)
        {
            LoginUser(userService.AuthenticateUser(model.Username, model.Password));
            if (CurrentUser != null && CurrentUser.Authenticated)
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
