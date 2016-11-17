using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    public class LoginController : Base.Controller
    {
        private readonly ILogger<LoginController> logger;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<Domain.Model.User> signInManager;
        public LoginController(ILogger<LoginController> logger,
            ServiceFacade.Controller context,
            RoleManager<IdentityRole> roleManager,
            SignInManager<Domain.Model.User> signInManager)
            : base(context)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index()
        {
            PageTitle = "Sign in";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Domain.Model.MissionControl.Login model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username,
                model.Password,
                model.CookieUsername,
                false);

            if (result.Succeeded)
            {
                AlertInfo = $"You have logged in as {model.Username}";
                var participant = await userManager.FindByNameAsync(model.Username);

                return RedirectToRoute(new { controller = "Home" });
            }
            else
            {
                if (result.IsNotAllowed)
                {
                    AlertDanger = "You are not allowed to log in.";
                }
                else if (result.IsLockedOut)
                {
                    AlertDanger = "Your account is locked out.";
                }
                else
                {
                    AlertDanger = "Login or password incorrect.";
                }
                return View("Index", model);
            }
        }
    }
}
