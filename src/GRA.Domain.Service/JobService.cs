using System;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class JobService : Abstract.BaseUserService<JobService>
    {
        private readonly IJobRepository _jobRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobService(ILogger<JobService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IJobRepository jobRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<JobStatus> RunJob(string jobTokenString,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var userId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.AccessMissionControl))
            {
                if (Guid.TryParse(jobTokenString, out Guid jobToken))
                {
                    var jobInfo = await _jobRepository.GetJobInfoFromTokenAsync(jobToken);

                    if (jobInfo != null)
                    {
                        using (Serilog.Context.LogContext.PushProperty(LoggingEnrichment.JobType,
                            jobInfo.JobType))
                        using (Serilog.Context.LogContext.PushProperty(LoggingEnrichment.JobToken,
                            jobTokenString))
                        {
                            progress?.Report(new JobStatus
                            {
                                Status = "Loading job..."
                            });

                            await _jobRepository.UpdateStartAsync(jobInfo.Id);

                            JobStatus status;
                            try
                            {
                                switch (jobInfo.JobType)
                                {
                                    case JobType.SendBulkEmails:
                                        status = await SendBulkEmails(userId,
                                            jobInfo.Id,
                                            token,
                                            progress);
                                        break;
                                    case JobType.AvatarImport:
                                        status = await ImportAvatarsAsync(jobInfo.Id,
                                            token,
                                            progress);
                                        break;
                                    case JobType.HouseholdImport:
                                        status = await ImportHouseholdMembersAsync(jobInfo.Id,
                                            token,
                                            progress);
                                        break;
                                    case JobType.RunReport:
                                        status = await RunReportJobAsync(jobInfo.Id,
                                            token,
                                            progress);
                                        break;
                                    case JobType.UpdateVendorStatus:
                                        status = await UpdateStatusFromExcelAsync(
                                            jobInfo.Id,
                                            token,
                                            progress);
                                        break;
                                    case JobType.GenerateVendorCodes:
                                        status = await GenerateVendorCodesAsync(
                                            jobInfo.Id,
                                            token,
                                            progress);
                                        break;
                                    case JobType.UpdateEmailAwardStatus:
                                        status = await UpdateEmailAwardStatusFromExcelAsync(
                                                jobInfo.Id,
                                                token,
                                                progress);
                                        break;
                                    case JobType.BranchImport:
                                        status = await ImportBranches(
                                            jobInfo.Id,
                                            token,
                                            progress);
                                        break;
                                    default:
                                        throw new GraException($"Undefined job type: {jobInfo.JobType}");
                                }

                                await _jobRepository.UpdateFinishAsync(jobInfo.Id,
                                    token.IsCancellationRequested);
                            }
#pragma warning disable CA1031 // Do not catch general exception types
                            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                            {
                                _logger.LogError(ex,
                                    "Error executing job id {JobId} ({JobToken}): {Message}",
                                    jobInfo.Id,
                                    jobInfo.JobToken,
                                    ex.Message);
                                status = new JobStatus
                                {
                                    Status = $"A software error occurred running the job: {ex.Message}",
                                    Complete = true,
                                    Error = true,
                                    SuccessRedirect = false
                                };
                                progress?.Report(status);
                            }
                            return status;
                        }
                    }
                    else
                    {
                        _logger.LogError("User {UserId} specified a job token with no associated job id: {JobToken}.",
                            userId,
                            jobTokenString);
                        return ErrorStatus("Job not found.");
                    }
                }
                else
                {
                    _logger.LogError("User {UserId} specified an invalid job token: {JobToken}.",
                        userId,
                        jobTokenString);
                    return ErrorStatus("Invalid job.");
                }
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to run jobs.",
                    userId);
                return ErrorStatus("Permission denied");
            }
        }

        private static JobStatus ErrorStatus(string description)
        {
            return new JobStatus
            {
                PercentComplete = 0,
                Status = description,
                Title = "Error",
                Error = true
            };
        }

        public async Task<Guid> CreateJobAsync(Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }
            job.SiteId = GetCurrentSiteId();
            job.CreatedAt = _dateTimeProvider.Now;
            job.CreatedBy = GetActiveUserId();
            job.JobToken = Guid.NewGuid();

            var insertedJob = await _jobRepository.AddSaveNoAuditAsync(job);

            return insertedJob.JobToken;
        }

        private async Task<JobStatus> ImportAvatarsAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var avatarService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(AvatarService)) as AvatarService;
            return await avatarService.ImportAvatarsAsync(jobId, token, progress);
        }

        private async Task<JobStatus> ImportHouseholdMembersAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var userService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(UserService)) as UserService;
            return await userService.ImportHouseholdMembersAsync(jobId, token, progress);
        }

        private async Task<JobStatus> RunReportJobAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var reportService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(ReportService)) as ReportService;
            return await reportService.RunReportJobAsync(jobId, token, progress);
        }

        private async Task<JobStatus> UpdateStatusFromExcelAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var vendorCodeService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(VendorCodeService)) as VendorCodeService;
            return await vendorCodeService.UpdateStatusFromExcelAsync(jobId, token, progress);
        }

        private async Task<JobStatus> GenerateVendorCodesAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var vendorCodeService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(VendorCodeService)) as VendorCodeService;
            return await vendorCodeService.GenerateVendorCodesAsync(jobId, token, progress);
        }

        private async Task<JobStatus> UpdateEmailAwardStatusFromExcelAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var vendorCodeService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(VendorCodeService)) as VendorCodeService;
            return await vendorCodeService.UpdateEmailAwardStatusFromExcelAsync(
                jobId, token, progress);
        }

        private async Task<JobStatus> SendBulkEmails(int userId,
            int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var emailBulkService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(EmailBulkService)) as EmailBulkService;
            return await emailBulkService.RunJobAsync(userId, jobId, token, progress);
        }

        private async Task<JobStatus> ImportBranches(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            var branchImportExportService = _httpContextAccessor
                .HttpContext
                .RequestServices
                .GetService(typeof(BranchImportExportService)) as BranchImportExportService;
            return await branchImportExportService.RunImportJobAsync(jobId, token, progress);
        }
    }
}
