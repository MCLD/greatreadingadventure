using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<int> CountAsync(EventFilter filter);

        Task DetachRelatedChallenge(int userId, int challengeId);

        Task DetachRelatedChallengeGroup(int userId, int challengeGroupId);

        Task DetachRelatedTrigger(int triggerId);

        Task<List<Event>> GetByChallengeGroupIdAsync(int challengeGroupId);

        Task<List<Event>> GetByChallengeIdAsync(int challengeId);

        Task<ICollection<DataWithCount<Event>>> GetCommunityExperienceAttendanceAsync(
            ReportCriterion criterion);

        Task<ICollection<Event>> GetEventListAsync(EventFilter filter);

        Task<ICollection<Event>> GetRelatedEventsForTriggerAsync(int triggerId);

        Task<string> GetSecretCodeForStreamingEventAsync(int eventId);

        Task<IEnumerable<int>> GetUserFavoriteEvents(int userId,
            IEnumerable<int> eventIds = null);

        Task<bool> LocationInUse(int siteId, int locationId);

        Task<ICollection<Event>> PageAsync(EventFilter filter);

        Task<int> RemoveFavoritesAsync(int eventId);

        Task UpdateUserFavoritesAsync(int authUserId, int userId,
                    IEnumerable<int> favoritesToAdd,
                    IEnumerable<int> favoritesToRemove);

        Task<IEnumerable<int>> ValidateEventIdsAsync(int siteId,
            IEnumerable<int> eventIds);
    }
}
