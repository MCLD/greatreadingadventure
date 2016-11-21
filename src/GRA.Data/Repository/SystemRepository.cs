using GRA.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class SystemRepository
        : AuditingRepository<Model.System, Domain.Model.System>, ISystemRepository
    {
        public SystemRepository(Context context,
            ILogger<SystemRepository> logger,
            AutoMapper.IMapper mapper,
            IConfigurationRoot config)
            : base(context, logger, mapper, config)
        {
        }
    }
}
