using GRA.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class BranchRepository
        : AuditingRepository<Model.Branch, Domain.Model.Branch>, IBranchRepository
    {
        public BranchRepository(Context context,
            ILogger<BranchRepository> logger,
            AutoMapper.IMapper mapper,
            IConfigurationRoot config) : base(context, logger, mapper, config)
        {
        }
    }
}
