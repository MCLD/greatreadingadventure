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
    public class JoinQuestionOptionRepository(ServiceFacade.Repository repositoryFacade,
        ILogger<JoinQuestionOptionRepository> logger)
        : AuditingRepository<Model.JoinQuestionOption, JoinQuestionOption>(repositoryFacade, logger),
        IJoinQuestionOptionRepository
    {
        public async Task AddSaveUserJoinQuestionOptions(
            IEnumerable<UserJoinQuestionOption> userJoinQuestionOptions)
        {
            var items = userJoinQuestionOptions
                .AsQueryable()
                .ProjectToType<Model.UserJoinQuestionOption>();

            await _context.AddRangeAsync(items);
            await SaveAsync();
        }

        public async Task<IEnumerable<JoinQuestionOption>> GetByJoinQuestionIdAsync(
            int joinQuestionId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.JoinQuestionId == joinQuestionId)
                .ProjectToType<JoinQuestionOption>()
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetOptionsAsync(IEnumerable<int> joinQuestionIds)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => joinQuestionIds.Contains(_.JoinQuestionId))
                .Select(_ => _.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<int>> GetSelectionsByUserIdAsync(int userId)
        {
            return await _context
                .UserJoinQuestionOptions
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .Select(_ => _.JoinQuestionOptionId)
                .ToListAsync();
        }
    }
}
