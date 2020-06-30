﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;

namespace GRA.Domain.Repository
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<int> CountAsync(EventFilter filter);
        Task<ICollection<Event>> PageAsync(EventFilter filter);
        Task<List<Event>> GetByChallengeIdAsync(int challengeId);
        Task<List<Event>> GetByChallengeGroupIdAsync(int challengeGroupId);
        Task<bool> LocationInUse(int siteId, int locationId);
        Task DetachRelatedTrigger(int triggerId);
        Task<ICollection<Event>> GetRelatedEventsForTriggerAsync(int triggerId);
        Task DetachRelatedChallenge(int userId, int challengeId);
        Task DetachRelatedChallengeGroup(int userId, int challengeGroupId);
        Task<ICollection<DataWithCount<Event>>> GetCommunityExperienceAttendanceAsync(
            ReportCriterion criterion);
        Task<ICollection<Event>> GetEventListAsync(EventFilter filter);
    }
}
