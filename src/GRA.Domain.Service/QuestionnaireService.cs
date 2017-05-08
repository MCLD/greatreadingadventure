using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class QuestionnaireService : BaseUserService<QuestionnaireService>
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IBadgeRepository _badgeRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionnaireRepository _questionnaireRepository;
        private readonly IRequiredQuestionnaireRepository _requiredQuestionnaireRepository;
        private readonly IUserLogRepository _userLogRepository;
        public QuestionnaireService(ILogger<QuestionnaireService> logger,
            IUserContextProvider userContextProvider,
            IAnswerRepository answerRepository,
            IBadgeRepository badgeRepository,
            INotificationRepository notificationRepository,
            IQuestionRepository questionRepository,
            IQuestionnaireRepository questionnaireRepository,
            IRequiredQuestionnaireRepository requiredQuestionnaireRepository,
            IUserLogRepository userLogRepository)
            : base(logger, userContextProvider)
        {
            SetManagementPermission(Permission.ManageQuestionnaires);
            _answerRepository = Require.IsNotNull(answerRepository, nameof(answerRepository));
            _badgeRepository = Require.IsNotNull(badgeRepository, nameof(badgeRepository));
            _notificationRepository = Require.IsNotNull(notificationRepository,
                nameof(notificationRepository));
            _questionRepository = Require.IsNotNull(questionRepository,
                nameof(questionRepository));
            _questionnaireRepository = Require.IsNotNull(questionnaireRepository,
                nameof(questionnaireRepository));
            _requiredQuestionnaireRepository = Require.IsNotNull(requiredQuestionnaireRepository,
                nameof(requiredQuestionnaireRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
        }

        public async Task<DataWithCount<ICollection<Questionnaire>>> GetPaginatedListAsync(
            BaseFilter filter)
        {
            VerifyManagementPermission();
            filter.SiteId = GetCurrentSiteId();
            return new DataWithCount<ICollection<Questionnaire>>
            {
                Data = await _questionnaireRepository.PageAsync(filter),
                Count = await _questionnaireRepository.CountAsync(filter)
            };
        }

        // add questionnaire
        public async Task<Questionnaire> AddAsync(Questionnaire questionnaire)
        {
            VerifyManagementPermission();

            questionnaire.SiteId = GetCurrentSiteId();
            questionnaire.RelatedBranchId = GetClaimId(ClaimType.BranchId);
            questionnaire.RelatedSystemId = GetClaimId(ClaimType.SystemId);

            var addedQuestionnaire = await _questionnaireRepository.AddSaveAsync(
                GetClaimId(ClaimType.UserId),
                questionnaire);

            if (questionnaire.Questions != null && questionnaire.Questions.Count > 0)
            {
                return await AddQuestionsAsync(addedQuestionnaire.Id, questionnaire.Questions);
            }
            else
            {
                return addedQuestionnaire;
            }
        }

        public async Task<Questionnaire> UpdateAsync(Questionnaire questionnaire)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            var currentQuestionnaire = await _questionnaireRepository.GetByIdAsync(questionnaire.Id);
            if (currentQuestionnaire.IsLocked)
            {
                _logger.LogError($"User {authId} cannot update locked questionnaire {currentQuestionnaire.Id}.");
                throw new GraException("Questionnaire is locked and cannot be edited.");
            }

            currentQuestionnaire.Name = questionnaire.Name;
            currentQuestionnaire.IsLocked = questionnaire.IsLocked;
            return await _questionnaireRepository.UpdateSaveAsync(authId, currentQuestionnaire);
        }

        public async Task UpdateQuestionListAsync(int questionnaireId, List<int> questionOrderList)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId, false);
            if (questionnaire.IsLocked)
            {
                _logger.LogError($"User {authId} cannot update locked questionnaire {questionnaire.Id}.");
                throw new GraException("Questionnaire is locked and cannot be edited.");
            }

            var questions = questionnaire.Questions;
            var questionsIdList = questions.Select(_ => _.Id);
            var invalidQuestions = questionOrderList.Except(questionsIdList);
            if (invalidQuestions.Any())
            {
                _logger.LogError($"User {authId} cannot update question {invalidQuestions.First()} for questionnaire {questionnaireId}.");
                throw new GraException("Invalid question selection.");
            }

            var questionUpdateList = questions.Where(_ => questionOrderList.Contains(_.Id));
            foreach (var question in questionUpdateList)
            {
                question.SortOrder = questionOrderList.IndexOf(question.Id);
                await _questionRepository.UpdateSaveAsync(authId, question);
            }

            var questionDeleteList = questions.Except(questionUpdateList);
            foreach (var question in questionDeleteList)
            {
                await _questionRepository.RemoveSaveAsync(authId, question.Id);
            }
        }

        // add question and answers to questionnaire
        public async Task<Questionnaire> AddQuestionsAsync(int questionnaireId,
            IEnumerable<Question> questions)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            foreach (var question in questions)
            {
                question.QuestionnaireId = questionnaireId;
                var addedQuestion = await _questionRepository.AddSaveAsync(authId, question);
                foreach (var answer in question.Answers)
                {
                    answer.QuestionId = addedQuestion.Id;
                    await _answerRepository.AddAsync(authId, answer);
                }
                await _answerRepository.SaveAsync();
            }

            return await _questionnaireRepository.GetByIdAsync(questionnaireId);
        }

        public async Task RemoveAsync(int id)
        {
            VerifyManagementPermission();
            await _questionnaireRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), id);
        }

        public async Task<Questionnaire> GetByIdAsync(int questionnaireId, bool includeAnswers)
        {
            var questionnaire = await _questionnaireRepository
                .GetByIdAsync(questionnaireId, includeAnswers);

            if (questionnaire == null)
            {
                throw new GraException("The requested questionnaire could not be accessed or does not exist.");
            }

            return questionnaire;
        }

        public async Task<ICollection<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _answerRepository.GetByQuestionIdAsync(questionId);
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            return await _questionRepository.GetByIdAsync(questionId);
        }

        public async Task<IList<Question>> GetQuestionsByQuestionnaireIdAsync(int questionnaireId,
            bool includeAnswers)
        {
            return await _questionRepository.GetByQuestionnaireIdAsync(questionnaireId, includeAnswers);
        }

        public async Task<Question> AddQuestionAsync(Question question)
        {
            VerifyManagementPermission();
            return await _questionRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), question);
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            VerifyManagementPermission();
            int authId = GetClaimId(ClaimType.UserId);

            return await _questionRepository.UpdateSaveAsync(authId, question);
        }

        public async Task<Answer> AddAnswerAsync(Answer answer)
        {
            VerifyManagementPermission();
            return await _answerRepository.AddSaveAsync(GetClaimId(ClaimType.UserId), answer);
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            VerifyManagementPermission();
            await _answerRepository.UpdateSaveAsync(GetClaimId(ClaimType.UserId), answer);
        }

        public async Task RemoveAnswerAsync(int answerId)
        {
            VerifyManagementPermission();
            await _answerRepository.RemoveSaveAsync(GetClaimId(ClaimType.UserId), answerId);
        }

        public async Task<int?> GetRequiredQuestionnaire(int userId, int? userAge)
        {
            var questionnaires = await _requiredQuestionnaireRepository
                .GetForUser(GetCurrentSiteId(), userId, userAge);
            if (questionnaires.Count > 0)
            {
                return questionnaires.First();
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> HasRequiredQuestionnaire(int userId, int? userAge,
            int questionnaireId)
        {
            return await _requiredQuestionnaireRepository
                .UserHasRequiredQuestionnaire(GetCurrentSiteId(), userId, userAge, questionnaireId);
        }

        public async Task SubmitQuestionnaire(int questionnaireId, int userId, int? userAge,
            IList<Question> questions)
        {
            var requiredQuestionnaires = await _requiredQuestionnaireRepository.
                GetForUser(GetCurrentSiteId(), userId, userAge);
            if (!requiredQuestionnaires.Contains(questionnaireId))
            {
                _logger.LogError($"User {userId} is not eligible to answer questionnaire {questionnaireId}.");
                throw new GraException("Not eligible to answer that questionnaire.");
            }

            var questionnaire = await _questionnaireRepository.GetByIdAsync(questionnaireId, true);
            var questionnaireQuestions = questionnaire.Questions.Where(_ => _.Answers.Count > 0);

            if (questions.Select(_ => _.Id).Except(questionnaireQuestions.Select(_ => _.Id)).Any()
                || questionnaireQuestions.Count() != questions.Count)
            {
                _logger.LogError($"User {userId} submitted invalid questions for questionnaire {questionnaireId}.");
                throw new GraException("Invalid questions answered.");
            }

            foreach (var question in questionnaireQuestions)
            {
                if (!question.Answers.Select(_ => _.Id).Contains(questions
                    .Where(_ => _.Id == question.Id)
                    .Select(_ => _.ParticipantAnswer)
                    .SingleOrDefault()))
                {
                    _logger.LogError($"User {userId} submitted invalid answers for question {question.Id}.");
                    throw new GraException("Invalid answer selected.");
                }
            }

            await _requiredQuestionnaireRepository.SubmitQuestionnaire(questionnaireId, userId,
                questions);

            if (questionnaire.BadgeId.HasValue)
            {
                await QuestionnaireNotificationBadge(questionnaire, userId);
                var badge = await _badgeRepository.GetByIdAsync(questionnaire.BadgeId.Value);
                await _badgeRepository.AddUserBadge(userId, questionnaire.BadgeId.Value);
            }
        }

        private async Task QuestionnaireNotificationBadge(Questionnaire questionnaire, int userId)
        {
            var badge = await _badgeRepository.GetByIdAsync(questionnaire.BadgeId.Value);
            await _badgeRepository.AddUserBadge(userId, badge.Id);
            await _userLogRepository.AddAsync(userId, new UserLog
            {
                UserId = userId,
                PointsEarned = 0,
                IsDeleted = false,
                BadgeId = badge.Id,
                Description = $"Completed Questionnaire {questionnaire.Name}!"
            });
            var notification = new Notification
            {
                PointsEarned = 0,
                Text = questionnaire.BadgeNotificiationMessage,
                UserId = userId,
                BadgeId = badge.Id,
                BadgeFilename = badge.Filename
            };

            await _notificationRepository.AddSaveAsync(userId, notification);
        }
    }
}
