using GRA.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Data.ServiceFacade;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

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
                .ProjectTo<RecoveryToken>()
                .ToListAsync();
        }
    }
}
