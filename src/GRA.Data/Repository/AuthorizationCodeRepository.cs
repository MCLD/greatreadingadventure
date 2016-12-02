using GRA.Domain.Model;
using GRA.Domain.Repository;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GRA.Data.Repository
{
    public class AuthorizationCodeRepository
        : AuditingRepository<Model.AuthorizationCode, AuthorizationCode>, IAuthorizationCodeRepository
    {
        public AuthorizationCodeRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AuthorizationCodeRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<AuthorizationCode> GetByCodeAsync(int siteId, string authorizationCode)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId
                    && _.Code == authorizationCode)
                .ProjectTo<AuthorizationCode>()
                .SingleOrDefaultAsync();
        }
    }
}
