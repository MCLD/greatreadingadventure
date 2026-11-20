using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class JoinQuestionService(
        ILogger<JoinCodeService> logger,
        IDateTimeProvider dateTimeProvider,
        IUserContextProvider userContextProvider,
        IJoinQuestionOptionRepository joinQuestionOptionRepository,
        IJoinQuestionRepository joinQuestionRepository,
        SegmentService segmentService)
        : BaseUserService<JoinCodeService>(logger, dateTimeProvider, userContextProvider)
    {
        public async Task<IEnumerable<int>> GetSelectionsAsync(int userId)
        {
            return await joinQuestionOptionRepository.GetSelectionsByUserIdAsync(userId);
        }
        public async Task<IEnumerable<JoinQuestion>> GetQuestionsAsync()
        {
            return await joinQuestionRepository.GetQuestionsAsync();
        }

        public async Task<IEnumerable<int>> GetQuestionOptionsAsync(
            IEnumerable<int> joinQuestionIds)
        {
            return await joinQuestionOptionRepository.GetOptionsAsync(joinQuestionIds);
        }

        public async Task<IEnumerable<JoinQuestionView>> GetQuestionsViewAsync(int languageId)
        {
            var joinQuestionViewItems = new List<JoinQuestionView>();

            var joinQuestions = await joinQuestionRepository.GetQuestionsAsync();

            if (joinQuestions.Any())
            {
                foreach (var question in joinQuestions)
                {
                    var joinQuestionView = new JoinQuestionView
                    {
                        DisplayOrder = question.DisplayOrder,
                        IsRequired = question.IsRequired,
                        JoinQuestionId = question.Id,
                        JoinQuestionText = await segmentService.GetTextAsync(
                            question.SegmentId,
                            languageId),
                    };

                    var options = await GetOptionsAsync(question.Id);

                    joinQuestionView.JoinQuestionOptions.Add("", "");

                    foreach (var option in options.OrderBy(_ => _.DisplayOrder))
                    {
                        joinQuestionView.JoinQuestionOptions.Add(
                            option.Id.ToString(CultureInfo.InvariantCulture),
                            await segmentService.GetTextAsync(option.SegmentId, languageId));
                    }

                    if (joinQuestionView.JoinQuestionOptions.Count > 0)
                    {
                        joinQuestionViewItems.Add(joinQuestionView);
                    }
                }
            }

            return joinQuestionViewItems
                .OrderBy(_ => _.DisplayOrder)
                .ThenBy(_ => _.JoinQuestionText);
        }

        public async Task AddReponsesAsync(int userId, IEnumerable<int> optionIds)
        {
            await joinQuestionOptionRepository.AddSaveUserJoinQuestionOptions(
                optionIds.Select(_ => new UserJoinQuestionOption
                {
                    JoinQuestionOptionId = _,
                    UserId = userId
                }));
        }

        public async Task<IEnumerable<JoinQuestionOption>> GetOptionsAsync(int joinQuestionId)
        {
            return await joinQuestionOptionRepository.GetByJoinQuestionIdAsync(joinQuestionId);
        }
    }
}
