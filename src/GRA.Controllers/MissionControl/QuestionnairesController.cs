using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GRA.Controllers.ViewModel.MissionControl.Questionnaires;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GRA.Controllers.MissionControl
{
    [Area("MissionControl")]
    [Authorize(Policy = Policy.ManageQuestionnaires)]
    public class QuestionnairesController : Base.MCController
    {
        private readonly ILogger<QuestionnairesController> _logger;
        private readonly QuestionnaireService _questionnaireService;
        public QuestionnairesController(ILogger<QuestionnairesController> logger,
           ServiceFacade.Controller context,
           QuestionnaireService questionnaireService)
            : base(context)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(questionnaireService);

            _logger = logger;
            _questionnaireService = questionnaireService;

            PageTitle = "Questionnaire management";
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var filter = new BaseFilter(page);
            var questionnaireList = await _questionnaireService.GetPaginatedListAsync(filter);

            var paginateModel = new PaginateViewModel
            {
                ItemCount = questionnaireList.Count,
                CurrentPage = page,
                ItemsPerPage = filter.Take.Value
            };

            if (paginateModel.PastMaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }

            var viewModel = new QuestionnairesListViewModel
            {
                Questionnaires = questionnaireList.Data,
                PaginateModel = paginateModel
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            PageTitle = "Create Questionnaire";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Questionnaire model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var questionnaire = await _questionnaireService.AddAsync(model);
                    ShowAlertSuccess($"Questionnaire '{questionnaire.Name}' successfully created!");
                    return RedirectToAction("Edit", new { id = questionnaire.Id });
                }
                catch (GraException gex)
                {
                    _logger.LogError(gex,
                        "Unable to create questionnaire: {ErrorMessage}",
                        gex.Message);
                    ShowAlertDanger("Unable to create questionnaire: ", gex);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var viewModel = new QuestionnairesDetailViewModel
                {
                    Questionnaire = await _questionnaireService.GetByIdAsync(id, true)
                };
                return View(viewModel);
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to view questionnaire: ", gex);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionnairesDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var questionnaire = await _questionnaireService.UpdateAsync(model.Questionnaire);
                    if (!string.IsNullOrWhiteSpace(model.QuestionSortOrder))
                    {
                        var questionOrderList = model.QuestionSortOrder
                            .Replace("question[]=", "")
                            .Split('&')
                            .Where(_ => !string.IsNullOrWhiteSpace(_))
                            .Select(int.Parse)
                            .Distinct()
                            .ToList();

                        await _questionnaireService.UpdateQuestionListAsync(questionnaire.Id,
                            questionOrderList);
                    }
                    else
                    {
                        await _questionnaireService.UpdateQuestionListAsync(questionnaire.Id,
                            new List<int>());
                    }
                    ShowAlertSuccess($"Questionnaire '{questionnaire.Name}' successfully updated!");
                    return RedirectToAction("Edit", new { id = questionnaire.Id });
                }
                catch (GraException gex)
                {
                    ShowAlertDanger("Unable to edit questionnaire: ", gex);
                    return RedirectToAction("Edit", new { id = model.Questionnaire.Id });
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _questionnaireService.RemoveAsync(id);
                ShowAlertSuccess("Questionnaire deleted.");
            }
            catch (GraException gex)
            {
                ShowAlertWarning("Unable to delete questionnaire: ", gex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<JsonResult> SaveQuestion(string parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                try
                {
                    // Create dictionary from parameter string
                    var parameterDictionary =
                        Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(parameters);
                    var questionId = int.Parse(parameterDictionary["Question.Id"]);

                    // Ensure the questionnaire isn't locked
                    var questionnaireId = int.Parse(parameterDictionary["Questionnaire.Id"]);
                    var questionnaire = await _questionnaireService
                        .GetByIdAsync(questionnaireId, false);
                    if (questionnaire.IsLocked)
                    {
                        _logger.LogError("User {UserId} cannot edit question id {QuestionId} for locked questionnaire id {QuestionnaireId}.",
                            GetId(ClaimType.UserId),
                            questionId,
                            questionnaireId);
                        throw new GraException("Questionnaire is locked and cannot be edited.");
                    }

                    // Create lists and validate
                    var newAnswersList = parameterDictionary
                        .Where(_ => _.Key.StartsWith("new_"))
                        .ToDictionary(_ => _.Key.Replace("new_", ""), _ => _.Value);

                    var updateAnswersList = parameterDictionary
                        .Where(_ => _.Key.StartsWith("update_"))
                        .ToDictionary(_ => int.Parse(_.Key.Replace("update_", "")), _ => _.Value);

                    var answerOrderList = parameterDictionary["AnswerSortOrder"].ToString()
                               .Replace("answer[]=", "")
                               .Split('&')
                               .Where(_ => !string.IsNullOrWhiteSpace(_))
                               .Distinct()
                               .ToList();

                    if (answerOrderList.Count > 0
                        && answerOrderList.Count != newAnswersList.Count + updateAnswersList.Count)
                    {
                        _logger.LogError("User {UserId} requested an invalid sort for question {QuestionId}.",
                            GetId(ClaimType.UserId),
                            questionId);
                        throw new GraException("Invalid answer sort selection.");
                    }

                    // Validate correct answer
                    parameterDictionary.TryGetValue("CorrectAnswer", out var correctAnswerId);
                    if (!string.IsNullOrWhiteSpace(correctAnswerId)
                        && newAnswersList.Count + updateAnswersList.Count > 0
                        && !parameterDictionary.ContainsKey($"update_{correctAnswerId}")
                        && !parameterDictionary.ContainsKey($"new_{correctAnswerId}"))
                    {
                        _logger.LogError("User {UserId} selected an invalid correct answer for question {QuestionId}.",
                            GetId(ClaimType.UserId),
                            questionId);
                        throw new GraException("Invalid correct answer selected.");
                    }

                    // If the question exists get the question and answers, else create a new one
                    var question = new Question();
                    ICollection<Answer> answers = new Collection<Answer>();
                    if (questionId > 0)
                    {
                        question = await _questionnaireService.GetQuestionByIdAsync(questionId);
                        question.Name = parameterDictionary["Question.Name"];
                        question.Text = parameterDictionary["Question.Text"];
                        answers = await _questionnaireService.GetAnswersByQuestionIdAsync(questionId);
                    }
                    else
                    {
                        question.QuestionnaireId = questionnaire.Id;
                        question.SortOrder = questionnaire.Questions.Count;
                        question.Name = parameterDictionary["Question.Name"];
                        question.Text = parameterDictionary["Question.Text"];
                        question = await _questionnaireService.AddQuestionAsync(question);
                    }

                    // Add new answers to the question
                    foreach (var newAnswer in newAnswersList)
                    {
                        var answer = new Answer
                        {
                            QuestionId = question.Id,
                            Text = newAnswer.Value,
                            SortOrder = answerOrderList.IndexOf(newAnswer.Key)
                        };
                        answer = await _questionnaireService.AddAnswerAsync(answer);
                        if (correctAnswerId == newAnswer.Key)
                        {
                            question.CorrectAnswerId = answer.Id;
                        }
                    }

                    // Update the question
                    if (parameterDictionary.ContainsKey($"update_{correctAnswerId}"))
                    {
                        question.CorrectAnswerId = int.Parse(correctAnswerId);
                    }
                    question = await _questionnaireService.UpdateQuestionAsync(question);

                    // Update/remove answers if the question already existed
                    if (questionId > 0)
                    {
                        // Validate and update answers
                        var invalidAnswers = updateAnswersList.Keys.Except(answers.Select(_ => _.Id));
                        if (invalidAnswers.Any())
                        {
                            _logger.LogError("User {UserId} cannot update answer {InvalidAnswerName} for question {QuestionId}.",
                                GetId(ClaimType.UserId),
                                invalidAnswers.FirstOrDefault(),
                                question.Id);
                            throw new GraException("Invalid answer to update.");
                        }
                        foreach (var updateAnswer in updateAnswersList)
                        {
                            var answer = answers.SingleOrDefault(_ => _.Id == updateAnswer.Key);
                            answer.Text = updateAnswer.Value;
                            if (answerOrderList.Count > 0)
                            {
                                answer.SortOrder = answerOrderList.IndexOf(updateAnswer.Key.ToString());
                            }
                            await _questionnaireService.UpdateAnswerAsync(answer);
                        }

                        // Remove answers that were not returned
                        var deleteAnswersList = answers.Select(_ => _.Id).Except(updateAnswersList.Keys);
                        invalidAnswers = deleteAnswersList.Except(answers.Select(_ => _.Id));
                        if (invalidAnswers.Any())
                        {
                            _logger.LogError("User {UserId} cannot delete answer {InvalidAnswerName} for question {QuestionId}.",
                                GetId(ClaimType.UserId),
                                invalidAnswers.First(),
                                question.Id);
                            throw new GraException("Invalid answer to delete.");
                        }
                        foreach (var answerId in deleteAnswersList)
                        {
                            await _questionnaireService.RemoveAnswerAsync(answerId);
                        }
                    }

                    // Get the updated answer list and return question and answers
                    var answerList = await _questionnaireService.GetAnswersByQuestionIdAsync(question.Id);
                    return Json(new { success = true, question, answers = answerList });
                }
                catch (GraException gex)
                {
                    return Json(new { success = false, message = gex.Message });
                }
            }
            return Json(new { success = false, message = "No values submitted" });
        }

        public async Task<JsonResult> GetAnswerList(int questionId)
        {
            var answerList = await _questionnaireService.GetAnswersByQuestionIdAsync(questionId);
            return Json(answerList.OrderBy(_ => _.SortOrder));
        }

        public async Task<IActionResult> Preview(int id)
        {
            var questionList = await _questionnaireService
                    .GetQuestionsByQuestionnaireIdAsync(id, true);
            foreach (var question in questionList)
            {
                question.Text = CommonMark.CommonMarkConverter.Convert(question.Text);
            }
            PageTitle = "Questionnaire Preview";
            return View(questionList);
        }
    }
}
