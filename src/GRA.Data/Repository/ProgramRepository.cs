using GRA.Domain.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class ProgramRepository
        : AuditingRepository<Model.Program, Domain.Model.Program>, IProgramRepository
    {
        public ProgramRepository(
            Context context,
            ILogger<ProgramRepository> logger,
            AutoMapper.IMapper mapper,
            IConfigurationRoot config)
            : base(context, logger, mapper, config)
        {
        }
    }
}
