using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
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
        private readonly SampleDataService _sampleDataService;

        public FlightController(ILogger<FlightController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            QuestionnaireService questionnaireService,
            SampleDataService sampleDataService)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _activityService = activityService
                ?? throw new ArgumentNullException(nameof(activityService));
            _questionnaireService = questionnaireService
                ?? throw new ArgumentNullException(nameof(questionnaireService));
            _sampleDataService = sampleDataService
                ?? throw new ArgumentNullException(nameof(sampleDataService));
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
        public async Task<IActionResult> LoadSampleData()
        {
            await _sampleDataService.InsertSampleData(GetId(ClaimType.UserId));
            AlertSuccess = "Inserted sample data.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RedeemSecretCode()
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

        public async Task<IActionResult> AddQuestionnaire()
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
        public async Task<IActionResult> ReloadSiteCache()
        {
            var sw = Stopwatch.StartNew();
            await _siteLookupService.ReloadSiteCacheAsync();
            sw.Stop();
            ShowAlertSuccess($"Sites flushed from cache, reloaded in {sw.ElapsedMilliseconds} ms.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogWarning()
        {
            _logger.LogWarning("Logging warning message per Flight Controller request.");
            AlertWarning = "Warning message logged.";
            return View(nameof(Index));
        }

        [HttpPost]
        public IActionResult LogError()
        {
            _logger.LogError("Logging error message per Flight Controller request.");
            AlertDanger = "Error message logged.";
            return View(nameof(Index));
        }
    }
}
