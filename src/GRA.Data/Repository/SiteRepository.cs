using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace GRA.Data.Repository
{
    public class SiteRepository
        : AuditingRepository<Model.Site, Domain.Model.Site>, ISiteRepository
    {
        public SiteRepository(
            Context context,
            ILogger<SiteRepository> logger,
            AutoMapper.IMapper mapper,
            IConfigurationRoot config)
            : base(context, logger, mapper, config)
        {
        }
    }
}
