using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<Domain.Model.Participant> signInManager;
        public RegisterController(ILogger<RegisterController> logger,
            ServiceFacade.Controller context,
            RoleManager<IdentityRole> roleManager,
            SignInManager<Domain.Model.Participant> signInManager)
            : base(context)
        {
            this.logger = logger;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            PageTitle = "Register";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Domain.MissionControl.Register registerResponse)
        {
            bool initialSetup = service.GetSitePaths().Count() == 0;

            var user = new Domain.Model.Participant
            {
                UserName = registerResponse.Username,
                Email = registerResponse.Email,
            };

            IdentityResult result = await userManager.CreateAsync(user, registerResponse.Password);

            if (result.Succeeded)
            {
                AlertSuccess = $"Account created: {user.UserName}";
                logger.LogInformation($"Account create: {user.UserName}");

                if (initialSetup)
                {
                    logger.LogInformation("Site list from database is empty, initial setup");
                    service.InitialSetup(user);

                    // todo move identity out of aspnet
                    // possibly following http://timschreiber.com/2015/01/14/persistence-ignorant-asp-net-identity-with-patterns-part-1/
                    // this process should happen in service.InitialSetup(user)

                    var adminRole = await roleManager.FindByNameAsync(RoleName.SystemAdministrator);
                    if (adminRole == null)
                    {
                        adminRole = new IdentityRole(RoleName.SystemAdministrator);
                        await roleManager.CreateAsync(adminRole);

                        await roleManager.AddClaimAsync(adminRole,
                            new Claim(ClaimType.Privilege, ClaimName.MissionControlUser));

                        await userManager.AddToRoleAsync(user, adminRole.Name);
                    }
                    AlertSuccess = $"Initial account created: {user.UserName}";
                }
                
                // sign in the registered user
                var signInResult = await signInManager.PasswordSignInAsync(user.UserName,
                    registerResponse.Password,
                    false,
                    false);

                return RedirectToRoute(new { controller = "Home" });
            }
            else
            {
                StringBuilder errors = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errors.Append($"<li>{error.Description}</li>");
                    logger.LogError($"Problem creating account: {error.Description}");
                }
                AlertDanger = $"Unable to register your account:{errors}";
                PageTitle = "Register";
                return View();
            }
        }
    }
}
