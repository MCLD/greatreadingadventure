using System;
using GRA.Data.ServiceFacade;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class PointTranslationRepository
        : AuditingRepository<Model.PointTranslation, PointTranslation>,
        IPointTranslationRepository
    {
        public PointTranslationRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PointTranslationRepository> logger) : base(repositoryFacade, logger)
        { }

        public async Task<PointTranslation> GetByProgramIdAsync(int programId)
        {
            var translation = await DbSet
                .AsNoTracking()
                .Where(_ => _.ProgramId == programId)
                .SingleOrDefaultAsync();

            if (translation == null)
            {
                return null;
            }
            return _mapper.Map<PointTranslation>(translation);
        }
    }
}
