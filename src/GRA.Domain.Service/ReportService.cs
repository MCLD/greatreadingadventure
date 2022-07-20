using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Report;
using GRA.Domain.Report.Abstract;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class ReportService : Abstract.BaseUserService<ReportService>
    {
        private readonly IGraCache _cache;
        private readonly IJobRepository _jobRepository;
        private readonly IReportCriterionRepository _reportCriterionRepository;
        private readonly IReportRequestRepository _reportRequestRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserLogRepository _userLogRepository;
        private readonly IUserRepository _userRepository;

        public ReportService(ILogger<ReportService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IGraCache cache,
            IJobRepository jobRepository,
            IReportCriterionRepository reportCriterionRepository,
            IReportRequestRepository reportRequestRepository,
            IServiceProvider serviceProvider,
            IUserLogRepository userLogRepository,
            IUserRepository userRepository)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _reportCriterionRepository = reportCriterionRepository
                ?? throw new ArgumentNullException(nameof(reportCriterionRepository));
            _reportRequestRepository = reportRequestRepository
                ?? throw new ArgumentNullException(nameof(reportRequestRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _userLogRepository = userLogRepository
                ?? throw new ArgumentNullException(nameof(userLogRepository));
        }

        public async Task<StatusSummary> GetCurrentStatsAsync(ReportCriterion request)
        {
            if (request.SiteId == null
                || request.SiteId != GetCurrentSiteId())
            {
                request.SiteId = GetCurrentSiteId();
            }
            string cacheKey = $"s{request.SiteId}.p{request.ProgramId}.sys{request.SystemId}.b{request.BranchId}.{CacheKey.CurrentStats}";
            var summaryJson = await _cache.GetStringFromCache(cacheKey);
            if (string.IsNullOrEmpty(summaryJson))
            {
                var summary = new StatusSummary
                {
                    RegisteredUsers = await _userRepository.GetCountAsync(request),
                    Achievers = await _userRepository.GetAchieverCountAsync(request),
                    PointsEarned = await _userLogRepository.PointsEarnedTotalAsync(request),
                    CompletedChallenges = await _userLogRepository
                        .CompletedChallengeCountAsync(request),
                    BadgesEarned = await _userLogRepository.EarnedBadgeCountAsync(request),
                    DaysUntilEnd = await GetDaysUntilEnd(),
                    AsOf = _dateTimeProvider.Now
                };
                await _cache.SaveToCacheAsync(cacheKey,
                    JsonConvert.SerializeObject(summary),
                    ExpireInTimeSpan());
                return summary;
            }
            else
            {
                return JsonConvert.DeserializeObject<StatusSummary>(summaryJson);
            }
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

        public IEnumerable<ReportDetails> GetReportList()
        {
            return new Catalog().Get().Where(_ => _.Id >= 0);
        }

        public async Task<(ReportRequest request, ReportCriterion criterion)> GetReportResultsAsync(int reportRequestId)
        {
            if (HasPermission(Permission.ViewAllReporting))
            {
                var reportRequest = await _reportRequestRepository.GetByIdAsync(reportRequestId);
                if (reportRequest == null)
                {
                    _logger.LogError("User {UserId} requested non-existant report results id: {ReportRequestId}.",
                        GetClaimId(ClaimType.UserId),
                        reportRequestId);
                    throw new GraException("Report results not found.");
                }
                var reportCriteria = await _reportCriterionRepository
                    .GetByIdAsync(reportRequest.ReportCriteriaId);

                return (reportRequest, reportCriteria);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to view all reporting.",
                    GetClaimId(ClaimType.UserId));
                throw new GraException("Permission denied.");
            }
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

                var reportRequest = await _reportRequestRepository
                    .AddSaveNoAuditAsync(new ReportRequest
                    {
                        CreatedAt = criterion.CreatedAt,
                        CreatedBy = criterion.CreatedBy,
                        ReportCriteriaId = criterion.Id,
                        ReportId = reportId,
                        Name = GetReportList().SingleOrDefault(_ => _.Id == reportId)?.Name,
                        SiteId = criterion.SiteId,
                    });

                return reportRequest.Id;
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError("User {UserId} doesn't have permission to view all reporting.",
                    requestingUser);
                throw new GraException("Permission denied.");
            }
        }

        public async Task<JobStatus> RunReportJobAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var job = await _jobRepository.GetByIdAsync(jobId);
            var jobDetails
                = JsonConvert
                    .DeserializeObject<JobDetailsRunReport>(job.SerializedParameters);

            int reportRequestId = jobDetails.ReportRequestId;

            if (HasPermission(Permission.ViewAllReporting))
            {
                BaseReport report = null;
                ReportRequest _request = null;

                token.Register(() =>
                {
                    string duration = "immediately";
                    if (report?.Elapsed != null)
                    {
                        duration = $"after {((TimeSpan)report.Elapsed).TotalSeconds.ToString("N2")} seconds";
                    }
                    if (_request != null)
                    {
                        _logger.LogWarning("Report {ReportRequestId} for user {UserId} was cancelled {Duration}.",
                            reportRequestId,
                            _request.CreatedBy,
                            duration);
                    }
                    else
                    {
                        _logger.LogWarning("Report {ReportRequestId} was cancelled {Duration}.",
                            reportRequestId,
                            duration);
                    }
                });

                try
                {
                    _request = await _reportRequestRepository.GetByIdAsync(reportRequestId)
                        ?? throw new GraException($"Cannot find report request id {reportRequestId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Could not find report request {ReportRequestId}: {Message}",
                        reportRequestId,
                        ex.Message);
                    return new JobStatus
                    {
                        Status = "Could not find the report request.",
                        Error = true,
                        Complete = true
                    };
                }

                var reportDetails = new Catalog().Get()
                    .SingleOrDefault(_ => _.Id == _request.ReportId);

                using (Serilog.Context.LogContext.PushProperty(LoggingEnrichment.ReportName,
                            reportDetails.Name))
                {
                    if (reportDetails == null)
                    {
                        _logger.LogError("Cannot find report id {ReportId} requested by request {ReportRequestId}",
                            _request.ReportId,
                            reportRequestId);

                        return new JobStatus
                        {
                            Status = "Could not find the requested report.",
                            Error = true,
                            Complete = true
                        };
                    }

                    try
                    {
                        report = _serviceProvider.GetService(reportDetails.ReportType) as BaseReport;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex,
                            "Couldn't instantiate report: {Message}",
                            ex.Message);
                        return new JobStatus
                        {
                            Status = "Unable to run report.",
                            Error = true,
                            Complete = true
                        };
                    }

                    try
                    {
                        await report.ExecuteAsync(_request, token, progress);
                    }
                    catch (Exception ex)
                    {
                        await _jobRepository.UpdateStatusAsync(jobId, $"An error occurred: {ex.Message}");
                        return new JobStatus
                        {
                            Status = $"A software error occurred: {ex.Message}.",
                            Complete = true,
                            Error = true
                        };
                    }

                    if (token.IsCancellationRequested)
                    {
                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Report request id {reportRequestId} cancelled.");

                        return new JobStatus
                        {
                            Status = "Report processing cancelled.",
                            Complete = true,
                            Error = true
                        };
                    }
                    else
                    {
                        await _jobRepository.UpdateStatusAsync(jobId,
                            $"Report request id {reportRequestId} completed.");

                        return new JobStatus
                        {
                            PercentComplete = 100,
                            Complete = true,
                            Status = "Report processing complete.",
                        };
                    }
                }
            }
            else
            {
                var requestingUser = GetClaimId(ClaimType.UserId);
                _logger.LogError("User {UserId} doesn't have permission to view all reporting.",
                    requestingUser);
                return new JobStatus
                {
                    Status = "Permission denied.",
                    Complete = true,
                    Error = true
                };
            }
        }
    }
}
