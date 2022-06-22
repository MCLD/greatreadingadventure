using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IFeaturedChallengeGroupRepository : IRepository<FeaturedChallengeGroup>
    {
        Task AddTextAsync(FeaturedChallengeGroupText text, int featuredGroupId, int languageId);

        Task<ICollection<DisplayChallengeGroup>> GetActiveAsync(int siteId,
            int languageId,
            int defaultLanguageId);

        Task<int?> GetMaxSortOrderAsync(int siteId);

        Task<DateTime?> GetNextActiveTimestampAsync(int siteId);

        Task<FeaturedChallengeGroup> GetNextInOrderAsync(int siteId, int sortOrder, bool increase);

        Task<FeaturedChallengeGroupText> GetTextByFeaturedGroupAndLanguageAsync(int featuredGroupId,
            int languageId);

        Task<ICollection<FeaturedChallengeGroupText>>
            GetTextsForFeaturedGroupAsync(int featuredGroupId);

        Task<ICollectionWithCount<FeaturedChallengeGroup>> PageAsync(BaseFilter filter);

        void RemoveFeaturedGroupTexts(int featuredGroupId, int? languageId = null);

        Task UpdateTextAsync(FeaturedChallengeGroupText text, int featuredGroupId, int languageId);
    }
}
