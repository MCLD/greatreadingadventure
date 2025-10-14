using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class PsProgramImageRepository
        : AuditingRepository<Model.PsProgramImage, Domain.Model.PsProgramImage>, IPsProgramImageRepository
    {
        public PsProgramImageRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<PsProgramImageRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<List<PsProgramImage>> GetByProgramIdAsync(int programId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.ProgramId == programId)
                .ProjectToType<PsProgramImage>()
                .ToListAsync();
        }
    }
}
