using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class UserLogRepository
        : AuditingRepository<Model.UserLog, Domain.Model.UserLog>, IUserLogRepository
    {
        public UserLogRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<UserLogRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<UserLog>> PageHistoryAsync(int userId, int skip, int take)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.UserId == userId)
                .OrderBy(_ => _.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ProjectTo<UserLog>()
                .ToListAsync();
        }
    }
}
