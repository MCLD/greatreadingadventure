﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var time = _dateTimeProvider.Now;
            var takenQuestionnaires = _context.UserQuestionnaires
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.QuestionnaireId);

            return await DbSet.AsNoTracking()
                                    .Where(_ => _.SiteId == siteId
                                        && !takenQuestionnaires.Contains(_.QuestionnaireId)
                                        && (!_.AgeMinimum.HasValue || _.AgeMinimum <= userAge)
                                        && (!_.AgeMaximum.HasValue || _.AgeMaximum >= userAge)
                                        && (!_.StartDate.HasValue || _.StartDate <= time)
                                        && (!_.EndDate.HasValue || _.EndDate >= time))
                                    .Select(_ => _.QuestionnaireId)
                                    .ToListAsync();
        }

        public async Task<bool> UserHasRequiredQuestionnaire(int siteId, int userId, int? userAge,
            int questionnaireId)
        {
            var time = _dateTimeProvider.Now;
            var takenQuestionnaires = _context.UserQuestionnaires
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.QuestionnaireId);

            return await DbSet.AsNoTracking()
                                    .Where(_ => _.SiteId == siteId
                                        && _.QuestionnaireId == questionnaireId
                                        && !takenQuestionnaires.Contains(_.QuestionnaireId)
                                        && (!_.AgeMinimum.HasValue || _.AgeMinimum <= userAge)
                                        && (!_.AgeMaximum.HasValue || _.AgeMaximum >= userAge)
                                        && (!_.StartDate.HasValue || _.StartDate <= time)
                                        && (!_.EndDate.HasValue || _.EndDate >= time))
                                    .Select(_ => _.QuestionnaireId)
                                    .AnyAsync();
        }

        public async Task SubmitQuestionnaire(int questionnaireId, int userId,
            IList<Question> questions)
        {
            var time = _dateTimeProvider.Now;
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
