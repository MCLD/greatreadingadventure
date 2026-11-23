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
    public class JoinQuestionRepository(ServiceFacade.Repository repositoryFacade,
        ILogger<JoinQuestionRepository> logger)
        : AuditingRepository<Model.JoinQuestion, JoinQuestion>(repositoryFacade, logger),
        IJoinQuestionRepository
    {
        public async Task<IEnumerable<JoinQuestion>> GetQuestionsAsync()
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsActive)
                .ProjectToType<JoinQuestion>()
                .ToListAsync();
        }
    }
}
