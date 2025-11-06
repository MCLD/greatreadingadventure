using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class JoinCodeRepository : AuditingRepository<Model.JoinCode, JoinCode>,
        IJoinCodeRepository
    {
        public JoinCodeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<JoinCodeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.Code == code)
                .AnyAsync();
        }

        public async Task<JoinCode> GetByCodeAsync(string code)
        {
            return await DbSet
               .AsNoTracking()
               .Where(_ => _.Code == code)
               .ProjectToType<JoinCode>()
               .SingleOrDefaultAsync();
        }

        public async Task<JoinCode> GetByTypeAndBranchAsync(bool isQRCode, int? branchId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.IsQRCode == isQRCode && _.BranchId == branchId)
                .ProjectToType<JoinCode>()
                .SingleOrDefaultAsync();
        }

        public async Task IncrementAccessCountAsync(int id)
        {
            await _context
                    .Database
                    .ExecuteSqlInterpolatedAsync($"UPDATE [JoinCodes] SET [AccessCount] = [AccessCount] + 1 WHERE [Id] = {id}");
        }

        public async Task IncrementJoinCountAsync(int id)
        {
            await _context
                    .Database
                    .ExecuteSqlInterpolatedAsync($"UPDATE [JoinCodes] SET [JoinCount] = [JoinCount] + 1 WHERE [Id] = {id}");
        }

        public async Task<DataWithCount<IEnumerable<JoinCode>>> PageAsync(BaseFilter filter)
        {
            var joinCodes = DbSet.AsNoTracking().Where(_ => _.SiteId == filter.SiteId);

            var count = await joinCodes.CountAsync();
            var data = await joinCodes
                .OrderBy(_ => _.Branch.Name)
                .ThenByDescending(_ => _.IsQRCode)
                .ApplyPagination(filter)
                .ProjectToType<JoinCode>()
                .ToListAsync();

            return new DataWithCount<IEnumerable<JoinCode>>
            {
                Data = data,
                Count = count
            };
        }
    }
}
