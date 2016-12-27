using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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

        public async Task<StatusSummary> GetCurrentStats(StatusSummary request)
        {
            if (request.SiteId == null
                || request.SiteId != GetCurrentSiteId())
            {
                request.SiteId = GetCurrentSiteId();
            }
            string cacheKey = $"{CacheKey.CurrentStats}s{request.SiteId}p{request.ProgramId}sys{request.SystemId}b{request.BranchId}";
            var summary = _memoryCache.Get<StatusSummary>(cacheKey);
            if (summary == null)
            {
                summary = request;
                summary.RegisteredUsers = await _userRepository.GetCountAsync(request);
                summary.PointsEarned = await _userLogRepository.PointsEarnedTotalAsync(request);
                summary.ActivityEarnings = await _userLogRepository
                    .ActivityEarningsTotalAsync(request);
                summary.CompletedChallenges = await _userLogRepository
                     .CompletedChallengeCountAsync(request);
                _memoryCache.Set(cacheKey,
                    summary,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return summary;
        }
    }
}