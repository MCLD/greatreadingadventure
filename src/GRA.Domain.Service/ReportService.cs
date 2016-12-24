using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Model;

namespace GRA.Domain.Service
{
    public class ReportService : Abstract.BaseUserService<ReportService>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;
        public ReportService(ILogger<ReportService> logger,
            IUserContextProvider userContextProvider,
            IMemoryCache memoryCache,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository) : base(logger, userContextProvider)
        {
            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
        }

        public async Task<StatusSummary> GetCurrentStats(
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            int siteId = GetCurrentSiteId();
            string cacheKey = CacheKey.CurrentStats + siteId;
            var summary = _memoryCache.Get<StatusSummary>(cacheKey);
            if (summary == null)
            {
                summary = new StatusSummary
                {
                    SiteId = siteId,
                    StartDate = startDate,
                    EndDate = endDate,
                    RegisteredUsers = await _userRepository.GetCountAsync(siteId,
                        registrationStartDate: startDate,
                        registrationEndDate: endDate),
                    PointsEarned = await _userLogRepository.PointsEarnedTotalAsync(siteId,
                        startDate,
                        endDate),
                    ActivityEarnings = await _userLogRepository.ActivityEarningsTotalAsync(siteId,
                        startDate,
                        endDate),
                    CompletedChallenges = await _userLogRepository.CompletedChallengeCountAsync(siteId,
                        startDate,
                        endDate)
                };
                _memoryCache.Set(cacheKey,
                    summary,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return summary;
        }
    }
}