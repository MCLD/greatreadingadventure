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
        private readonly EmailBulkService _emailBulkService;

        private OperationStatus PermissionDeniedStatus
        {
            get
            {
                return new OperationStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true
                };
            }
        }

        public JobService(ILogger<JobService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IJobRepository jobRepository,
            EmailBulkService emailBulkService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _emailBulkService = emailBulkService
                ?? throw new ArgumentNullException(nameof(emailBulkService));
        }

        public async Task<OperationStatus> RunJob(string jobTokenString,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            var userId = GetClaimId(ClaimType.UserId);

            if (HasPermission(Permission.AccessMissionControl))
            {
                if (Guid.TryParse(jobTokenString, out Guid jobToken)) {
                    var jobInfo = await _jobRepository.GetJobInfoFromTokenAsync(jobToken);
                    if (jobInfo != null)
                    {
                        switch (jobInfo.JobType)
                        {
                            default: //case JobType.SendBulkEmails:
                                return await _emailBulkService.RunJobAsync(userId,
                                    jobInfo.Id,
                                    token,
                                    progress);
                        }
                    }
                    else
                    {
                        _logger.LogError("User {RequestingUser} specified a job token with no associated job id: {JobToken}.",
                            userId,
                            jobTokenString);
                        return PermissionDeniedStatus;
                    }
                }
                else
                {
                    _logger.LogError("User {RequestingUser} specified an invalid job token: {JobToken}.",
                        userId,
                        jobTokenString);
                    return PermissionDeniedStatus;
                }
            }
            else
            {
                _logger.LogError("User {RequestingUser} doesn't have permission to run jobs.",
                    userId);
                return PermissionDeniedStatus;
            }
        }
    }
}
