using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class RequiredQuestionnaireRepository
        : AuditingRepository<Model.RequiredQuestionnaire, RequiredQuestionnaire>, IRequiredQuestionnaireRepository
    {
        public RequiredQuestionnaireRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<RequiredQuestionnaireRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<int>> GetForUser(int siteId, int userId, int? userAge)
        {
            var time = DateTime.Now;
            return await DbSet.AsNoTracking()
                                    .Where(_ => _.SiteId == siteId
                                        && (_.AgeMinimum.HasValue == false || _.AgeMinimum <= userAge)
                                        && (_.AgeMaximum.HasValue == false || _.AgeMaximum >= userAge)
                                        && (_.StartDate.HasValue == false || _.StartDate <= time)
                                        && (_.EndDate.HasValue == false || _.EndDate >= time))
                                    .Select(_ => _.QuestionnaireId)
                                    .Except(_context.UserQuestionnaires
                                                .Where(_ => _.UserId == userId)
                                                .Select(_ => _.QuestionnaireId))
                                    .ToListAsync();
        }

        public async Task<bool> UserHasRequiredQuestionnaire(int siteId, int userId, int? userAge,
            int questionnaireId)
        {
            var time = DateTime.Now;
            return await DbSet.AsNoTracking()
                                    .Where(_ => _.SiteId == siteId
                                        && _.QuestionnaireId == questionnaireId
                                        && (_.AgeMinimum.HasValue == false || _.AgeMinimum <= userAge)
                                        && (_.AgeMaximum.HasValue == false || _.AgeMaximum >= userAge)
                                        && (_.StartDate.HasValue == false || _.StartDate <= time)
                                        && (_.EndDate.HasValue == false || _.EndDate >= time))
                                    .Select(_ => _.QuestionnaireId)
                                    .Except(_context.UserQuestionnaires
                                                .Where(_ => _.UserId == userId)
                                                .Select(_ => _.QuestionnaireId))
                                    .AnyAsync();
        }

        public async Task SubmitQuestionnaire(int questionnaireId, int userId,
            IList<Question> questions)
        {
            var time = DateTime.Now;
            foreach (var question in questions)
            {
                await _context.UserAnswers.AddAsync(
                    new Model.UserAnswer
                    {
                        AnswerId = question.ParticipantAnswer,
                        UserId = userId,
                        CreatedAt = time
                    });
            }
            await _context.UserQuestionnaires.AddAsync(new Model.UserQuestionnaire
            {
                QuestionnaireId = questionnaireId,
                UserId = userId,
                CreatedAt = time
            });
            await _context.SaveChangesAsync();
        }
    }
}
