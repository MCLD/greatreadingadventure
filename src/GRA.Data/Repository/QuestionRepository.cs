using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
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
                .Where(_ => _.QuestionnaireId == questionnaireId && !_.IsDeleted)
                .OrderBy(_ => _.SortOrder);

            if (includeAnswer)
            {
                var forkedConfig = _mapper.Config
                    .Fork(_ => _.NewConfig<Model.Question, Question>()
                        .Map(dest => dest.Answers, src => src.Answers.OrderBy(_ => _.SortOrder)));

                return await questions
                .ProjectToType<Question>(forkedConfig)
                .ToListAsync();
            }
            else
            {
                return await questions
                .ProjectToType<Question>()
                .ToListAsync();
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
