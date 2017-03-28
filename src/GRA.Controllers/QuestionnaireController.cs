using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Controllers.Attributes;
using GRA.Controllers.ViewModel.Questionnaire;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers
{
    [PreventQuestionnaireRedirect]
    public class QuestionnaireController : Base.UserController
    {
        private readonly ILogger<QuestionnaireController> _logger;
        private readonly QuestionnaireService _questionnaireService;
        private readonly UserService _userService;
        public QuestionnaireController(ILogger<QuestionnaireController> logger,
            ServiceFacade.Controller context,
            QuestionnaireService questionnaireService,
            UserService userService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _questionnaireService = Require.IsNotNull(questionnaireService,
                nameof(questionnaireService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Questionnaire";
        }

        public async Task<IActionResult> Index(int id)
        {
            var user = await _userService.GetDetails(GetActiveUserId());
            if (await _questionnaireService.HasRequiredQuestionnaire(user.Id, user.Age, id))
            {
                var questionList = await _questionnaireService
                    .GetQuestionsByQuestionnaireIdAsync(id, true);
                QuestionnaireViewModel viewModel = new QuestionnaireViewModel()
                {
                    QuestionnaireId = id,
                    Questions = questionList
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(QuestionnaireViewModel model)
        {
            ModelState.Clear();
            for (int i = 0; i < model.Questions.Count; i++)
            {
                if (model.Questions[i].Id > 0 && model.Questions[i].ParticipantAnswer == 0)
                {
                    ModelState.AddModelError($"Questions[{i}]", "Please select an answer.");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var questionList = model.Questions.Where(_ => _.Id > 0).ToList();
                    var user = await _userService.GetDetails(GetActiveUserId());
                    await _questionnaireService.SubmitQuestionnaire(model.QuestionnaireId, user.Id,
                        user.Age, questionList);

                    ShowAlertSuccess("Questionnaire successfully submited!");
                    var requiredQuestionnaire = await _questionnaireService.GetRequiredQuestionnaire(user.Id,
                        user.Age);
                    if (requiredQuestionnaire.HasValue)
                    {
                        HttpContext.Session.SetInt32(SessionKey.PendingQuestionnaire,
                                    requiredQuestionnaire.Value);
                    }
                    else
                    {
                        HttpContext.Session.Remove(SessionKey.PendingQuestionnaire);
                    }
                    return RedirectToAction("Index", "Home");
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to submit questionnaire: ", gex.Message);
                }
            }
            var questions = await _questionnaireService.GetQuestionsByQuestionnaireIdAsync(model.QuestionnaireId, true);
            foreach (var question in questions)
            {
                var modelQuestion = model.Questions.Where(_ => _.Id == question.Id).SingleOrDefault();
                if (modelQuestion != null)
                {
                    question.ParticipantAnswer = modelQuestion.ParticipantAnswer;
                }
            }
            model.Questions = questions;
            return View(model);
        }
    }
}
