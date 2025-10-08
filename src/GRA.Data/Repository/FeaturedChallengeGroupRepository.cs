using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Repository.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GRA.Data.Repository
{
    public class FeaturedChallengeGroupRepository
        : AuditingRepository<Model.FeaturedChallengeGroup, FeaturedChallengeGroup>,
        IFeaturedChallengeGroupRepository
    {
        public FeaturedChallengeGroupRepository(ServiceFacade.Repository repositoryFacade,
            ILogger<FeaturedChallengeGroupRepository> logger) : base(repositoryFacade, logger)
        {
        }

        public async Task<ICollection<DisplayChallengeGroup>> GetActiveAsync(int siteId,
            int languageId,
            int defaultLanguageId)
        {
            var now = _dateTimeProvider.Now;

            var activeFeatureGroups = await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId
                    && (!_.StartDate.HasValue || _.StartDate <= now)
                    && (!_.EndDate.HasValue || _.EndDate >= now))
                .OrderBy(_ => _.SortOrder)
                .ToListAsync();

            var challengeGroupIds = activeFeatureGroups.ConvertAll(_ => _.ChallengeGroupId);

            var groups = await _context.ChallengeGroups
                .AsNoTracking()
                .Where(_ => challengeGroupIds.Contains(_.Id))
                .ToListAsync();

            var displayChallengeGroups = new List<DisplayChallengeGroup>();

            foreach (var featureGroup in activeFeatureGroups)
            {
                var text
                    = await GetTextByFeaturedGroupAndLanguageAsync(featureGroup.Id, languageId);
                if (text == null)
                {
                    text = await GetTextByFeaturedGroupAndLanguageAsync(featureGroup.Id,
                        defaultLanguageId);
                }
                if (text != null)
                {
                    displayChallengeGroups.Add(new DisplayChallengeGroup
                    {
                        AltText = text.AltText,
                        ImageFile = text.Filename,
                        Name = featureGroup.Name,
                        SortOrder = featureGroup.SortOrder,
                        Stub = groups.Where(_ => _.Id == featureGroup.ChallengeGroupId)
                            .Select(_ => _.Stub)
                            .SingleOrDefault()
                    });
                }
            }

            return displayChallengeGroups;
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
                .ProjectToType<FeaturedChallengeGroup>()
                .ToListAsync();
        }

        public async Task<int?> GetMaxSortOrderAsync(int siteId)
        {
            return await DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId)
                .MaxAsync(_ => (int?)_.SortOrder);
        }

        public async Task<DateTime?> GetNextActiveTimestampAsync(int siteId)
        {
            var now = _dateTimeProvider.Now;

            var nextStart = await DbSet.AsNoTracking()
                .Where(_ => _.StartDate > now)
                .Select(_ => _.StartDate)
                .MinAsync() ?? DateTime.MaxValue;

            var nextEnd = await DbSet.AsNoTracking()
                .Where(_ => _.EndDate > now)
                .Select(_ => _.EndDate)
                .MinAsync() ?? DateTime.MinValue;

            var min = Math.Min(nextStart.Ticks, nextEnd.Ticks);

            _logger.LogDebug("Next active featured challenge transition times - start: {NextStart} end: {NextEnd}",
                nextStart,
                nextEnd);

            return min == DateTime.MaxValue.Ticks
                ? null
                : new DateTime(min);
        }

        public async Task<FeaturedChallengeGroup> GetNextInOrderAsync(int siteId,
            int sortOrder,
            bool increase)
        {
            var nextFeatuedGroup = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == siteId);

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
                .ProjectToType<FeaturedChallengeGroup>()
                .FirstOrDefaultAsync();
        }

        public async Task<ICollectionWithCount<FeaturedChallengeGroup>> PageAsync(
                                    BaseFilter filter)
        {
            var featuredGroups = DbSet
                .AsNoTracking()
                .Where(_ => _.SiteId == filter.SiteId);

            if (filter?.IsActive == true)
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
                .ProjectToType<FeaturedChallengeGroup>()
                .ToListAsync();

            return new ICollectionWithCount<FeaturedChallengeGroup>
            {
                Data = data,
                Count = count
            };
        }

        public override async Task RemoveSaveAsync(int userId, int id)
        {
            var sortOrder = await DbSet.AsNoTracking()
                .Where(_ => _.Id == id)
                .Select(_ => _.SortOrder)
                .SingleOrDefaultAsync();

            await base.RemoveSaveAsync(userId, id);

            var itemIds = await DbSet.AsNoTracking()
                .Where(_ => _.SortOrder > sortOrder)
                .Select(_ => _.Id)
                .ToListAsync();

            foreach (var itemId in itemIds)
            {
                var item = await GetByIdAsync(itemId);
                item.SortOrder--;
                await UpdateAsync(userId, item);
            }
            await _context.SaveChangesAsync();
        }

        #region Featured Challenge Group Text methods

        public async Task AddTextAsync(FeaturedChallengeGroupText text,
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
                .ProjectToType<FeaturedChallengeGroupText>()
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<FeaturedChallengeGroupText>> GetTextsForFeaturedGroupAsync(
            int featuredGroupId)
        {
            return await _context.FeaturedChallengeGroupTexts
                .AsNoTracking()
                .Where(_ => _.FeaturedChallengeGroupId == featuredGroupId)
                .ProjectToType<FeaturedChallengeGroupText>()
                .ToListAsync();
        }

        public void RemoveFeaturedGroupTexts(int featuredGroupId, int? languageId)
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

        #endregion Featured Challenge Group Text methods
    }
}
