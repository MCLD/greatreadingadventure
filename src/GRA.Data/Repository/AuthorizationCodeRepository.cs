using GRA.Domain.Model;
using GRA.Domain.Repository;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository.Extensions;

namespace GRA.Data.Repository
{
    public class AuthorizationCodeRepository
        : AuditingRepository<Model.AuthorizationCode, AuthorizationCode>, IAuthorizationCodeRepository
    {
        public AuthorizationCodeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AuthorizationCodeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<DataWithCount<IEnumerable<AuthorizationCode>>> PageAsync(BaseFilter filter)
        {
            var authorizationCodes = DbSet.AsNoTracking().Where(_ => _.SiteId == filter.SiteId);
            var count = await authorizationCodes.CountAsync();
            var data = await authorizationCodes
                .OrderBy(_ => _.Code)
                .ApplyPagination(filter)
                .ProjectTo<AuthorizationCode>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new DataWithCount<IEnumerable<AuthorizationCode>>
            {
                Data = data,
                Count = count
            };
        }

        public async Task<AuthorizationCode> GetByCodeAsync(int siteId, string authorizationCode)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId
                    && _.Code == authorizationCode)
                .ProjectTo<AuthorizationCode>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }
    }
}
