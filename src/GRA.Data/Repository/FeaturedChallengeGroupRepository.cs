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
    public class FeaturedChallengeGroupRepository
        : AuditingRepository<Model.FeaturedChallengeGroup, Domain.Model.FeaturedChallengeGroup>,
        IFeaturedChallengeGroupRepository
    {
        public FeaturedChallengeGroupRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<FeaturedChallengeGroupRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<DataWithCount<IEnumerable<FeaturedChallengeGroup>>> PageAsync(
            BaseFilter filter)
        {
            var featuredGroups = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter.IsActive == true)
            {
                var now = _dateTimeProvider.Now;

                featuredGroups = featuredGroups
                    .Where(_ => (!_.StartDate.HasValue || _.StartDate <= now)
                        && (!_.EndDate.HasValue || _.EndDate >= now));
            }

            var count = await featuredGroups.CountAsync();

            var data = await featuredGroups
                .OrderBy(_ => _.SortOrder)
                .ApplyPagination(filter)
                .ProjectTo<FeaturedChallengeGroup>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new DataWithCount<IEnumerable<FeaturedChallengeGroup>>
            {
                Data = data,
                Count = count
            };
        }

        public async Task<ICollection<FeaturedChallengeGroup>> GetBetweenSortOrdersAsync(int siteId,
            int firstSortOrder,
            int secondSortOrder)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId
                    && ((_.SortOrder > firstSortOrder && _.SortOrder < secondSortOrder)
                        || (_.SortOrder < firstSortOrder && _.SortOrder > secondSortOrder)))
                .ProjectTo<FeaturedChallengeGroup>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int?> GetMaxSortOrderAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .MaxAsync(_ => (int?)_.SortOrder);
        }

        public async Task<FeaturedChallengeGroup> GetNextInOrderAsync(int siteId,
            int sortOrder,
            bool increase,
            bool active)
        {
            var nextFeatuedGroup = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

            if (active)
            {
                var now = _dateTimeProvider.Now;

                nextFeatuedGroup = nextFeatuedGroup
                    .Where(_ => (!_.StartDate.HasValue || _.StartDate <= now)
                        && (!_.EndDate.HasValue || _.EndDate >= now));
            }

            if (increase)
            {
                nextFeatuedGroup = nextFeatuedGroup
                    .Where(_ => _.SortOrder > sortOrder)
                    .OrderBy(_ => _.SortOrder);
            }
            else
            {
                nextFeatuedGroup = nextFeatuedGroup
                    .Where(_ => _.SortOrder < sortOrder)
                    .OrderByDescending(_ => _.SortOrder);
            }

            return await nextFeatuedGroup
                .ProjectTo<FeaturedChallengeGroup>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        #region Featured Challenge Group Text methods
        public async Task AddTextAsnyc(FeaturedChallengeGroupText text,
            int featuredGroupId,
            int languageId)
        {
            var textEntity = _mapper
                .Map<FeaturedChallengeGroupText, Model.FeaturedChallengeGroupText>(text);

            textEntity.FeaturedChallengeGroupId = featuredGroupId;
            textEntity.LanguageId = languageId;

            await _context.FeaturedChallengeGroupTexts.AddAsync(textEntity);
        }

        public async Task<FeaturedChallengeGroupText> GetTextByFeaturedGroupAndLanguageAsync(
            int featuredGroupId,
            int languageId)
        {
            return await _context.FeaturedChallengeGroupTexts
                .AsNoTracking()
                .Where(_ => _.FeaturedChallengeGroupId == featuredGroupId
                    && _.LanguageId == languageId)
                .ProjectTo<FeaturedChallengeGroupText>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<FeaturedChallengeGroupText>> GetTextsForFeaturedGroupAsync(
            int featuredGroupId)
        {
            return await _context.FeaturedChallengeGroupTexts
                .AsNoTracking()
                .Where(_ => _.FeaturedChallengeGroupId == featuredGroupId)
                .ProjectTo<FeaturedChallengeGroupText>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public void RemoveFeaturedGroupTexts(int featuredGroupId, int? languageId = null)
        {
            var textEntities = _context.FeaturedChallengeGroupTexts
                .AsNoTracking()
                .Where(_ => _.FeaturedChallengeGroupId == featuredGroupId);

            if (languageId.HasValue)
            {
                textEntities.Where(_ => _.LanguageId == languageId.Value);
            }

            _context.FeaturedChallengeGroupTexts.RemoveRange(textEntities);
        }

        public async Task UpdateTextAsync(FeaturedChallengeGroupText text,
            int featuredGroupId,
            int languageId)
        {
            var textEntity = await _context.FeaturedChallengeGroupTexts
                .AsNoTracking()
                .Where(_ => _.FeaturedChallengeGroupId == featuredGroupId
                    && _.LanguageId == languageId)
                .SingleOrDefaultAsync();

            textEntity.AltText = text.AltText;
            textEntity.Filename = text.Filename;

            _context.FeaturedChallengeGroupTexts.Update(textEntity);
        }
        #endregion
    }
}
