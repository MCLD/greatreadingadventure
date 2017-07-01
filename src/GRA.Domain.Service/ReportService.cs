using GRA.Domain.Model;
using GRA.Domain.Report;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using GRA.Domain.Report.Abstract;

namespace GRA.Domain.Service
{
    public class ReportService : Abstract.BaseUserService<ReportService>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IBranchRepository _branchRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IReportCriterionRepository _reportCriterionRepository;
        private readonly IReportRequestRepository _reportRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserLogRepository _userLogRepository;
        private readonly ISystemRepository _systemRepository;
        public ReportService(ILogger<ReportService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            IBranchRepository branchRepository,
            IReportCriterionRepository reportCriterionRepository,
            IReportRequestRepository reportRequestRepository,
            IUserRepository userRepository,
            IUserLogRepository userLogRepository,
            ISystemRepository systemRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _serviceProvider = Require.IsNotNull(serviceProvider, nameof(serviceProvider));
            _memoryCache = Require.IsNotNull(memoryCache, nameof(memoryCache));
            _branchRepository = Require.IsNotNull(branchRepository, nameof(branchRepository));
            _reportCriterionRepository = Require.IsNotNull(reportCriterionRepository,
                nameof(reportCriterionRepository));
            _reportRequestRepository = Require.IsNotNull(reportRequestRepository,
                nameof(reportRequestRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _userLogRepository = Require.IsNotNull(userLogRepository, nameof(userLogRepository));
            _systemRepository = Require.IsNotNull(systemRepository, nameof(systemRepository));
        }

        public async Task<StatusSummary> GetCurrentStatsAsync(ReportCriterion request)
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
                summary = new StatusSummary
                {
                    RegisteredUsers = await _userRepository.GetCountAsync(request),
                    Achievers = await _userRepository.GetAchieverCountAsync(request),
                    PointsEarned = await _userLogRepository.PointsEarnedTotalAsync(request),
                    ActivityEarnings = await _userLogRepository.ActivityEarningsTotalAsync(request),
                    CompletedChallenges = await _userLogRepository
                        .CompletedChallengeCountAsync(request),
                    BadgesEarned = await _userLogRepository.EarnedBadgeCountAsync(request),
                    DaysUntilEnd = await GetDaysUntilEnd()
                };
                _memoryCache.Set(cacheKey,
                    summary,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return summary;
        }

        public async Task<int?> GetDaysUntilEnd()
        {
            var site = await _userContextProvider.GetCurrentSiteAsync();
            if (site.ProgramEnds == null)
            {
                return null;
            }
            if (site.ProgramEnds <= _dateTimeProvider.Now)
            {
                return 0;
            }
            return ((DateTime)site.ProgramEnds - _dateTimeProvider.Now).Days;
        }

        public async Task<int> RequestReport(ReportCriterion criterion, int reportId)
        {
            // returns report request id
            if (HasPermission(Permission.ViewAllReporting))
            {
                if (criterion.SiteId == null || criterion.SiteId != GetCurrentSiteId())
                {
                    criterion.SiteId = GetCurrentSiteId();
                }

                criterion.CreatedAt = _dateTimeProvider.Now;
                criterion.CreatedBy = GetActiveUserId();

                criterion = await _reportCriterionRepository.AddSaveNoAuditAsync(criterion);

                var request = await _reportRequestRepository.AddSaveNoAuditAsync(new ReportRequest
                {
                    CreatedAt = criterion.CreatedAt,
                    CreatedBy = criterion.CreatedBy,
                    ReportCriteriaId = criterion.Id,
                    ReportId = reportId,
                    Name = GetReportList().Where(_ => _.Id == reportId).SingleOrDefault()?.Name,
                    SiteId = criterion.SiteId,
                });

                return request.Id;
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {requestingUser} doesn't have permission to view all reporting.");
                throw new GraException("Permission denied.");
            }
        }

        public IEnumerable<ReportDetails> GetReportList()
        {
            return new Catalog().Get().Where(_ => _.Id >= 0);
        }

        public async Task<OperationStatus> RunReport(string reportRequestIdString,
        CancellationToken token,
        IProgress<OperationStatus> progress = null)
        {
            if (HasPermission(Permission.ViewAllReporting))
            {
                BaseReport report = null;
                ReportRequest _request = null;

                int reportRequestId = 0;
                if (!int.TryParse(reportRequestIdString, out reportRequestId))
                {
                    _logger.LogError($"Couldn't covert report request id {reportRequestIdString} to a number.");
                    return new OperationStatus
                    {
                        PercentComplete = 100,
                        Status = $"Could not find report request {reportRequestIdString}.",
                        Error = true,
                        Complete = false
                    };
                }

                token.Register(() =>
                {
                    string duration = "";
                    if (report != null && report.Elapsed != null)
                    {
                        duration = $" after {((TimeSpan)report.Elapsed).TotalSeconds.ToString("N2")} seconds";
                    }
                    if (_request != null)
                    {
                        _logger.LogWarning($"Report {reportRequestId} for user {_request.CreatedBy} was cancelled{duration}.");
                    }
                    else
                    {
                        _logger.LogWarning($"Report {reportRequestId} was cancelled{duration}.");
                    }
                });

                try
                {
                    _request = await _reportRequestRepository.GetByIdAsync(reportRequestId)
                        ?? throw new GraException($"Cannot find report request id {reportRequestId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Could not find report request {reportRequestId}: {ex.Message}");
                    return new OperationStatus
                    {
                        PercentComplete = 0,
                        Status = "Could not find the report request.",
                        Error = true
                    };
                }

                var reportDetails = new Catalog().Get()
                    .Where(_ => _.Id == _request.ReportId)
                    .SingleOrDefault();

                if (reportDetails == null)
                {
                    _logger.LogError($"Cannot find report id {_request.ReportId} requested by request {reportRequestId}");
                    return new OperationStatus
                    {
                        PercentComplete = 0,
                        Status = "Could not find the requested report.",
                        Error = true
                    };
                }

                try
                {
                    report = _serviceProvider.GetService(reportDetails.ReportType) as BaseReport;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Couldn't instantiate report: {ex.Message}");
                    return new OperationStatus
                    {
                        PercentComplete = 100,
                        Status = "Unable to run report.",
                        Error = true
                    };
                }

                try
                {
                    await report.ExecuteAsync(_request, token, progress);
                }
                catch (Exception ex)
                {
                    return new OperationStatus
                    {
                        PercentComplete = 100,
                        Status = $"A software error occurred: {ex.Message}.",
                        Error = true
                    };
                }

                if (!token.IsCancellationRequested)
                {
                    return new OperationStatus
                    {
                        PercentComplete = 100,
                        Status = "Report processing complete.",
                    };
                }
                else
                {
                    return new OperationStatus
                    {
                        PercentComplete = 100,
                    };
                }
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {requestingUser} doesn't have permission to view all reporting.");
                return new OperationStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true
                };
            }
        }

        public async Task<(ReportRequest request, ReportCriterion criterion)> GetReportResultsAsync(int reportRequestId)
        {
            if (HasPermission(Permission.ViewAllReporting))
            {
                var reportRequest = await _reportRequestRepository.GetByIdAsync(reportRequestId);
                if (reportRequest == null)
                {
                    var requestingUser = GetClaimId(ClaimType.UserId);
                    _logger.LogError($"User {requestingUser} requested non-existant report results id: {reportRequestId}.");
                    throw new GraException("Report results not found.");
                }
                var reportCriteria = await _reportCriterionRepository
                    .GetByIdAsync(reportRequest.ReportCriteriaId);

                return (reportRequest, reportCriteria);
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError($"User {requestingUser} doesn't have permission to view all reporting.");
                throw new GraException("Permission denied.");
            }
        }
    }
}