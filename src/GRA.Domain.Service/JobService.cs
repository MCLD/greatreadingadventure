using System;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class JobService : Abstract.BaseUserService<JobService>
    {
        private readonly IJobRepository _jobRepository;
        private readonly AvatarService _avatarService;
        private readonly EmailBulkService _emailBulkService;
        private readonly ReportService _reportService;
        private readonly UserService _userService;
        private readonly VendorCodeService _vendorCodeService;

        public JobService(ILogger<JobService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IJobRepository jobRepository,
            AvatarService avatarService,
            EmailBulkService emailBulkService,
            ReportService reportService,
            UserService userService,
            VendorCodeService vendorCodeService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _avatarService = avatarService
                ?? throw new ArgumentNullException(nameof(avatarService));
            _emailBulkService = emailBulkService
                ?? throw new ArgumentNullException(nameof(emailBulkService));
            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));
            _userService = userService
                ?? throw new ArgumentNullException(nameof(userService));
            _vendorCodeService = vendorCodeService
                ?? throw new ArgumentNullException(nameof(vendorCodeService));
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
                        progress.Report(new JobStatus
                        {
                            Status = "Loading job..."
                        });

                        await _jobRepository.UpdateStartAsync(jobInfo.Id);

                        JobStatus status;
                        try
                        {
                            switch (jobInfo.JobType)
                            {
                                case JobType.AvatarImport:
                                    status = await _avatarService.ImportAvatarsAsync(jobInfo.Id,
                                        token,
                                        progress);
                                    break;
                                case JobType.HouseholdImport:
                                    status = await _userService.ImportHouseholdMembersAsync(jobInfo.Id,
                                        token,
                                        progress);
                                    break;
                                case JobType.RunReport:
                                    status = await _reportService.RunReportJobAsync(jobInfo.Id,
                                        token,
                                        progress);
                                    break;
                                case JobType.UpdateVendorStatus:
                                    status = await _vendorCodeService.UpdateStatusFromExcelAsync(
                                        jobInfo.Id,
                                        token,
                                        progress);
                                    break;
                                default: // case JobType.SendBulkEmails:
                                    status = await _emailBulkService.RunJobAsync(userId,
                                        jobInfo.Id,
                                        token,
                                        progress);
                                    break;
                            }

                            await _jobRepository.UpdateFinishAsync(jobInfo.Id,
                                token.IsCancellationRequested);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "Error executing job: {Message}",
                                ex.Message);
                            status = new JobStatus
                            {
                                Status = $"A software error occurred running the job: {ex.Message}",
                                Complete = true,
                                Error = true,
                                SuccessRedirect = false
                            };
                            progress.Report(status);
                        }
                        return status;
                    }
                    else
                    {
                        _logger.LogError("User {RequestingUser} specified a job token with no associated job id: {JobToken}.",
                            userId,
                            jobTokenString);
                        return ErrorStatus("Job not found.");
                    }
                }
                else
                {
                    _logger.LogError("User {RequestingUser} specified an invalid job token: {JobToken}.",
                        userId,
                        jobTokenString);
                    return ErrorStatus("Invalid job.");
                }
            }
            else
            {
                _logger.LogError("User {RequestingUser} doesn't have permission to run jobs.",
                    userId);
                return ErrorStatus("Permission denied");
            }
        }

        private JobStatus ErrorStatus(string description)
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
            job.SiteId = GetCurrentSiteId();
            job.CreatedAt = _dateTimeProvider.Now;
            job.CreatedBy = GetActiveUserId();
            job.JobToken = Guid.NewGuid();

            var insertedJob = await _jobRepository.AddSaveNoAuditAsync(job);

            return insertedJob.JobToken;
        }
    }
}
