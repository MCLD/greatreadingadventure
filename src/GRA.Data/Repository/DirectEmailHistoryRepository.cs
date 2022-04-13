using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class DirectEmailHistoryRepository
        : AuditingRepository<Model.DirectEmailHistory, Domain.Model.DirectEmailHistory>,
        IDirectEmailHistoryRepository
    {
        public DirectEmailHistoryRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<IDirectEmailHistoryRepository> logger)
            : base(repositoryFacade, logger)
        {
        }

        public async Task<HashSet<string>> GetSentEmailByTemplateIdAsync(int directEmailTemplateId)
        {
            var result = await DbSet
                .AsNoTracking()
                .Where(_ => _.DirectEmailTemplateId == directEmailTemplateId)
                .Select(_ => _.ToEmailAddress)
                .Distinct()
                .ToListAsync();

            return new HashSet<string>(result);
        }
    }
}
