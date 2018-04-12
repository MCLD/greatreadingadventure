using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.AccessFlightController)]
    public class FlightController : Base.MCController
    {
        private readonly ILogger<FlightController> _logger;
        private readonly ActivityService _activityService;
        private readonly QuestionnaireService _questionnaireService;
        private readonly VendorCodeService _vendorCodeService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FlightController(ILogger<FlightController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            QuestionnaireService questionnaireService,
            VendorCodeService vendorCodeService,
            IHostingEnvironment hostingEnvironment)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _questionnaireService = Require.IsNotNull(questionnaireService,
                nameof(questionnaireService));
            _vendorCodeService = Require.IsNotNull(vendorCodeService, nameof(vendorCodeService));
            _hostingEnvironment = Require.IsNotNull(hostingEnvironment, nameof(hostingEnvironment));
            PageTitle = "Flight Director";
        }

        public IActionResult Index()
        {
            if (!AuthUser.Identity.IsAuthenticated)
            {
                // not logged in, redirect to login page
                return RedirectToRoute(new
                {
                    area = string.Empty,
                    controller = "SignIn",
                    ReturnUrl = "/MissionControl"
                });
            }

            if (!UserHasPermission(Permission.AccessFlightController))
            {
                // not authorized for Mission Control, redirect to authorization code
                return RedirectToRoute(new
                {
                    area = "MissionControl",
                    controller = "Home",
                    action = "AuthorizationCode"
                });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateVendorCodesAsync(int numberOfCodes)
        {
            var allCodes = await _vendorCodeService.GetTypeAllAsync();
            var code = allCodes.FirstOrDefault();
            if (code == null)
            {
                code = await _vendorCodeService.AddTypeAsync(new VendorCodeType
                {
                    Description = "Free Book Code",
                    DonationOptionSubject = "Choose whether to receive your free book or donate it to a child",
                    DonationOptionMail = "If you'd like to redeem your free book code, please visit <a href=\"/Profile/\">your profile</a> and select the redeem option. If you're not interested in redeeming it, you can select the option to donate it to a child.",
                    MailSubject = "Here's your Free Book Code!",
                    Mail = $"Congratulations, you've earned a free book! Your free book code is: {TemplateToken.VendorCodeToken}!",
                    DonationMessage = "Your free book has been donated.Thank you!!!",
                    DonationSubject = "Thank you for donating your free book!",
                    DonationMail = "Thanks so much for the donation of your book.",
                    Url = "http://freebook/?Code={Code}"
                });
            }
            var sw = new Stopwatch();
            sw.Start();
            var generatedCount = await _vendorCodeService.GenerateVendorCodesAsync(code.Id, numberOfCodes);
            sw.Stop();

            AlertSuccess = $"Generated {generatedCount} codes in {sw.Elapsed.TotalSeconds} seconds of type: {code.Description}";

            return View("Index");
        }

        public async Task<IActionResult> RedeemSecretCodeAsync()
        {
            var userContext = _userContextProvider.GetContext();
            try
            {
                await _activityService.LogSecretCodeAsync((int)userContext.ActiveUserId,
                    "secretcode");
            }
            catch (GraException gex)
            {
                AlertWarning = gex.Message;
            }
            return RedirectToRoute(new
            {
                area = string.Empty,
                controller = "Home",
                action = "Index"
            });
        }

        public async Task<IActionResult> AddQuestionnaireAsync()
        {
            var questionnaire = new Questionnaire
            {
                IsLocked = false,
                IsDeleted = false,
                Name = "Test questionnaire",
                Questions = new List<Question>()
            };

            var question = new Question
            {
                Name = "Name question",
                Text = "What is your name?",
                Answers = new List<Answer>()
            };

            question.Answers.Add(new Answer
            {
                Text = "Sir Lancelot"
            });
            question.Answers.Add(new Answer
            {
                Text = "Sir Robin"
            });
            question.Answers.Add(new Answer
            {
                Text = "King Arthur"
            });

            questionnaire.Questions.Add(question);

            await _questionnaireService.AddAsync(questionnaire);

            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReloadSiteCacheAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var sites = await _siteLookupService.ReloadSiteCacheAsync();
            sw.Stop();
            ShowAlertSuccess($"Sites flushed from cache, reloaded in {sw.ElapsedMilliseconds} ms.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }
    }
}
