using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Repository
{
    public class DrawingCriterionRepository
        : AuditingRepository<Model.DrawingCriterion, Domain.Model.DrawingCriterion>,
        IDrawingCriterionRepository
    {
        public DrawingCriterionRepository(ServiceFacade.Repository repositoryFacade, 
            ILogger<DrawingCriterionRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<IEnumerable<DrawingCriterion>> PageAllAsync(int siteId, int skip, int take)
        {
            return await DbSet
                    .AsNoTracking()
                    .Include(_ => _.Branch)
                    .Where(_ => _.SiteId == siteId)
                    .OrderBy(_ => _.Name)
                    .ThenBy(_ => _.Id)
                    .Skip(skip)
                    .Take(take)
                    .ProjectTo<DrawingCriterion>()
                    .ToListAsync();
        }

        public async Task<int> GetCountAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .CountAsync();
        }
    }
}
