﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class QuestionnaireRepository
        : AuditingRepository<Model.Questionnaire, Questionnaire>, IQuestionnaireRepository
    {
        public QuestionnaireRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<QuestionnaireRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<Questionnaire>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.Name)
                .ApplyPagination(filter)
                .ProjectTo<Questionnaire>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        private IQueryable<Model.Questionnaire> ApplyFilters(BaseFilter filter)
        {
            return DbSet
                .AsNoTracking()
                .Where(_ => !_.IsDeleted && _.SiteId == filter.SiteId);
        }

        public async Task<Questionnaire> GetByIdAsync(int id, bool includeAnswers)
        {
            var questionnaire = DbSet
                .AsNoTracking()
                .Where(_ => _.Id == id && !_.IsDeleted);

            if (includeAnswers)
            {
                return await questionnaire
                    .ProjectTo<Questionnaire>(
                        _mapper.ConfigurationProvider,
                        _ => _.Questions.Select(a => a.Answers)
                    )
                    .SingleOrDefaultAsync();
            }
            else
            {
                return await questionnaire
                    .ProjectTo<Questionnaire>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();
            }
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var entity = await DbSet
                .Where(_ => !_.IsDeleted && _.Id == id)
                .SingleAsync();
            entity.IsDeleted = true;
            await base.UpdateAsync(userId, entity, null);
            await base.SaveAsync();
        }
    }
}
