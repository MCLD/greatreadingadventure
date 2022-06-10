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
        Task<DataWithCount<IEnumerable<FeaturedChallengeGroup>>> PageAsync(BaseFilter filter);
    }
}
