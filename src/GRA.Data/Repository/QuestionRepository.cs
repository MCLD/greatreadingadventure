﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class QuestionRepository
        : AuditingRepository<Model.Question, Question>, IQuestionRepository
    {
        public QuestionRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<QuestionRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IList<Question>> GetByQuestionnaireIdAsync(int questionnaireId, bool includeAnswer)
        {
            var questions = DbSet.AsNoTracking()
                .Where(_ => _.QuestionnaireId == questionnaireId && _.IsDeleted == false)
                .OrderBy(_ => _.SortOrder);

            if (includeAnswer)
            {
                return await questions
                .ProjectTo<Question>(_mapper.ConfigurationProvider, _ => _.Answers)
                .ToListAsync();
            }
            else
            {
                return await questions
                .ProjectTo<Question>(_mapper.ConfigurationProvider)
                .ToListAsync();
            }
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet
                .Where(_ => _.IsDeleted == false && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }
    }
}
