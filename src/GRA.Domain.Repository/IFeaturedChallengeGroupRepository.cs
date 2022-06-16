using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IFeaturedChallengeGroupRepository : IRepository<FeaturedChallengeGroup>
    {
        Task AddTextAsnyc(FeaturedChallengeGroupText text, int featuredGroupId, int languageId);
        Task<ICollection<FeaturedChallengeGroup>> GetBetweenSortOrdersAsync(int siteId, int firstSortOrder, int secondSortOrder);
        Task<int?> GetMaxSortOrderAsync(int siteId);
        Task<FeaturedChallengeGroup> GetNextInOrderAsync(int siteId, int sortOrder, bool increase, bool active);
        Task<FeaturedChallengeGroupText> GetTextByFeaturedGroupAndLanguageAsync(int featuredGroupId, int languageId);
        Task<ICollection<FeaturedChallengeGroupText>> GetTextsForFeaturedGroupAsync(int featuredGroupId);
        Task<DataWithCount<IEnumerable<FeaturedChallengeGroup>>> PageAsync(BaseFilter filter);
        void RemoveFeaturedGroupTexts(int featuredGroupId, int? languageId = null);
        Task UpdateTextAsync(FeaturedChallengeGroupText text, int featuredGroupId, int languageId);
    }
}
