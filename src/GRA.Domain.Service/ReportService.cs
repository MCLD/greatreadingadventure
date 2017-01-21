using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Domain.Service
{
    public class ReportService : Abstract.BaseUserService<ReportService>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly ISystemRepository _systemRepository;
        public ReportService(ILogger<ReportService> logger,
            IUserContextProvider userContextProvider,
            IMemoryCache memoryCache,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository,
            ISystemRepository systemRepository) : base(logger, userContextProvider)
        {
            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
        }

        public async Task<StatusSummary> GetCurrentStatsAsync(StatusSummary request)
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
                summary.BadgesEarned = await _userLogRepository.EarnedBadgeCountAsync(request);
                summary.DaysUntilEnd = await GetDaysUntilEnd();
                _memoryCache.Set(cacheKey,
                    summary,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return summary;
        }

        public async Task<IEnumerable<StatusSummary>> GetAllByBranchAsync(StatusSummary request)
        {
            if (HasPermission(Permission.ViewAllReporting))
            {
                if (request.SiteId == null || request.SiteId != GetCurrentSiteId())
                {
                    request.SiteId = GetCurrentSiteId();
                }

                ICollection<int> systemIds = null;
                if (request.SystemId == null)
                {
                    var systems = await _systemRepository.GetAllAsync((int)request.SiteId);
                    systemIds = systems.Select(_ => _.Id).ToList();
                }
                else
                {
                    systemIds = new List<int>();
                    systemIds.Add((int)request.SystemId);
                }

                var result = new List<StatusSummary>();
                foreach (var systemId in systemIds)
                {
                    var branches = await _branchRepository.GetBySystemAsync(systemId);
                    foreach (var branch in branches)
                    {
                        request.SystemId = systemId;
                        request.BranchId = branch.Id;

                        result.Add(new StatusSummary
                        {
                            BranchId = branch.Id,
                            BranchName = branch.Name,
                            StartDate = request.StartDate,
                            EndDate = request.EndDate,
                            SystemId = systemId,
                            SiteId = request.SiteId,

                            RegisteredUsers = await _userRepository.GetCountAsync(request),
                            PointsEarned = await _userLogRepository.PointsEarnedTotalAsync(request),
                            ActivityEarnings = await _userLogRepository
                                .ActivityEarningsTotalAsync(request),
                            CompletedChallenges = await _userLogRepository
                                .CompletedChallengeCountAsync(request),
                            BadgesEarned = await _userLogRepository.EarnedBadgeCountAsync(request),
                            DaysUntilEnd = await GetDaysUntilEnd()
                        });
                    }
                }
                return result;
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {requestingUser} doesn't have permission to view all reporting.");
                throw new Exception("Permission denied.");
            }
        }

        public async Task<int?> GetDaysUntilEnd()
        {
            var site = await _userContextProvider.GetCurrentSiteAsync();
            if(site.ProgramEnds == null)
            {
                return null;
            }
            if(site.ProgramEnds <= DateTime.Now)
            {
                return 0;
            }
            return ((DateTime)site.ProgramEnds - DateTime.Now).Days;
        }
    }
}