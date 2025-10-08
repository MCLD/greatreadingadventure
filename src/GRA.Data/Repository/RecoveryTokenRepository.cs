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
    public class RecoveryTokenRepository
        : AuditingRepository<Model.RecoveryToken, Domain.Model.RecoveryToken>,
        IRecoveryTokenRepository
    {
        public RecoveryTokenRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<RecoveryTokenRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<RecoveryToken>> GetByUserIdAsync(int userId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderByDescending(_ => _.CreatedAt)
                .ProjectToType<RecoveryToken>()
                .ToListAsync();
        }
    }
}
