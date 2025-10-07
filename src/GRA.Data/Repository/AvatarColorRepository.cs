using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class AvatarColorRepository : AuditingRepository<Model.AvatarColor, AvatarColor>,
        IAvatarColorRepository
    {
        public AvatarColorRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<AvatarColorRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task AddTextsAsync(IEnumerable<AvatarColorText> texts)
        {
            var addTexts = _mapper
                .Map<IEnumerable<AvatarColorText>, IEnumerable<Model.AvatarColorText>>(texts);

            await _context.AvatarColorTexts.AddRangeAsync(addTexts);
        }

        public async Task<ICollection<AvatarColor>> GetByLayerAsync(int layerId)
        {
            return await DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayerId == layerId)
                .OrderBy(_ => _.SortOrder)
                .ProjectTo<AvatarColor>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<IEnumerable<AvatarColorText>> GetTextsByColorIdsAsync(
            IEnumerable<int> colorIds)
        {
            return await _context.AvatarColorTexts
                .AsNoTracking()
                .Where(_ => colorIds.Contains(_.AvatarColorId))
                .ProjectTo<AvatarColorText>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<DataWithCount<ICollection<AvatarColor>>> PageAsync(AvatarFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var colors = ApplyFilters(filter);

            var count = await colors.CountAsync();

            var data = await colors
                .OrderBy(_ => _.SortOrder)
                .ApplyPagination(filter)
                .Include(_ => _.Texts)
                .ProjectTo<AvatarColor>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new DataWithCount<ICollection<AvatarColor>>
            {
                Data = data,
                Count = count
            };
        }

        public void RemoveTexts(IEnumerable<AvatarColorText> texts)
        {
            var removeTexts = _mapper
                .Map<IEnumerable<AvatarColorText>, IEnumerable<Model.AvatarColorText>>(texts);

            _context.AvatarColorTexts.RemoveRange(removeTexts);
        }

        public void UpdateTexts(IEnumerable<AvatarColorText> texts)
        {
            var updateTexts = _mapper
                .Map<IEnumerable<AvatarColorText>, IEnumerable<Data.Model.AvatarColorText>>(texts);
            _context.AvatarColorTexts.UpdateRange(updateTexts);
        }

        private IQueryable<Model.AvatarColor> ApplyFilters(AvatarFilter filter)
        {
            var colors = DbSet.AsNoTracking()
                .Where(_ => _.AvatarLayer.SiteId == filter.SiteId);

            if (filter.TextMissing == true)
            {
                var completeTexts = _context.AvatarColorTexts
                    .GroupBy(_ => _.AvatarColorId)
                    .Where(_ => _.Count() == _context.Languages.Count())
                    .Select(_ => new { _.Key })
                    .Select(_ => _.Key);

                colors = colors.Where(_ => !completeTexts.Contains(_.Id));
            }

            return colors;
        }
    }
}
