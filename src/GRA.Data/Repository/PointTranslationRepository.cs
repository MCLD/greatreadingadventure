using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

        public async Task<IEnumerable<PointTranslation>> GetAllAsync(int siteId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .OrderBy(_ => _.TranslationName)
                .ProjectTo<PointTranslation>()
                .ToListAsync();
        }

        public async Task<int> CountAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .CountAsync();
        }

        public async Task<ICollection<PointTranslation>> PageAsync(BaseFilter filter)
        {
            return await ApplyFilters(filter)
                .OrderBy(_ => _.TranslationName)
                .ApplyPagination(filter)
                .ProjectTo<PointTranslation>()
                .ToListAsync();
        }

        private IQueryable<Model.PointTranslation> ApplyFilters(BaseFilter filter)
        {
            var pointTranslationList = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                pointTranslationList = pointTranslationList
                    .Where(_ => _.TranslationName.Contains(filter.Search));
            }

            return pointTranslationList;
        }

        public async Task<bool> IsInUseAsync(int pointTranslationId)
        {
            return await _context.Programs.AsNoTracking()
                .Where(_ => _.PointTranslationId == pointTranslationId)
                .Select(_ => _.Id)
                .Concat(
                    _context.ChallengeTaskTypes.AsNoTracking()
                    .Where(_ => _.PointTranslationId == pointTranslationId)
                    .Select(_ => _.Id)
                ).AnyAsync();
        }

        public async Task<PointTranslation> GetByProgramIdAsync(int programId)
        {
            var translation = await (from translations in DbSet.AsNoTracking()
                                     join programs in _context.Programs.AsNoTracking().Where(_ => _.Id == programId)
                                     on translations.Id equals programs.PointTranslationId
                                     select translations)
                               .SingleOrDefaultAsync();

            if (translation == null)
            {
                return null;
            }
            return _mapper.Map<PointTranslation>(translation);
        }
    }
}
