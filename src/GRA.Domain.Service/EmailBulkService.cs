using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class EmailBulkService : Abstract.BaseUserService<EmailBulkService>
    {
        private readonly IJobRepository _jobRepository;
        private readonly EmailService _emailService;
        private readonly UserService _userService;

        public EmailBulkService(ILogger<EmailBulkService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IJobRepository jobRepository,
            EmailService emailService,
            UserService userService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userService = userService
                ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<OperationStatus> RunJobAsync(int userId,
            int jobId,
            CancellationToken token,
            IProgress<OperationStatus> progress = null)
        {
            if (HasPermission(Permission.SendBulkEmails))
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                int emailsSent = 0;
                int emailsSkipped = 0;
                int userCounter = 0;

                var filter = new UserFilter
                {
                    IsSubscribed = true,
                    SortBy = SortUsersBy.RegistrationDate,
                };

                var subscribedUsers = await _userService.GetPaginatedUserListAsync(filter);
                _logger.LogInformation("Found {Count} subscribed users, processing first batch of {BatchCount}",
                    subscribedUsers.Count,
                    subscribedUsers.Data.Count());

                token.Register(() =>
                {
                    _logger.LogWarning("Bulk email job {JobId} for user {UserId} was cancelled after {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedUsersCount} in {Elapsed}.",
                        jobId,
                        userId,
                        emailsSent,
                        emailsSkipped,
                        subscribedUsers?.Count,
                        sw.Elapsed.ToString(@"mm\:ss"));
                });

                if (subscribedUsers.Count > 0)
                {
                    var elapsed = sw.Elapsed;

                    var job = await _jobRepository.GetByIdAsync(jobId);
                    var jobDetails
                        = JsonConvert.DeserializeObject<JobDetailsSendBulkEmails>(job.SerializedParameters);

                    progress.Report(new OperationStatus
                    {
                        PercentComplete = 0,
                        Status = $"Preparing to email {subscribedUsers.Count} participants...",
                        Error = false
                    });

                    while (subscribedUsers.Data.Any())
                    {
                        foreach (var user in subscribedUsers.Data)
                        {
                            // check email has not be sent to user
                            var alreadyGotIt = await _userService
                                .HasReceivedBulkEmailAsync(jobDetails.EmailTemplateId, user.Email);

                            if (alreadyGotIt)
                            {
                                // send email
                                _logger.LogTrace("Skipping email {Count}/{Total} to user {User} at {Email} with template {EmailTemplate}",
                                    userCounter + 1,
                                    subscribedUsers.Count,
                                    user.Id,
                                    user.Email,
                                    jobDetails.EmailTemplateId);

                                emailsSkipped++;
                            }
                            else
                            {
                                // send email
                                _logger.LogTrace("Sending email {Count}/{Total} to user {User} at {Email} with template {EmailTemplate}",
                                    userCounter + 1,
                                    subscribedUsers.Count,
                                    user.Id,
                                    user.Email,
                                    jobDetails.EmailTemplateId);

                                // send email to user
                                await _emailService.SendBulkAsync(user, jobDetails.EmailTemplateId);

                                await _userService.SentBulkEmailAsync(user.Id,
                                    jobDetails.EmailTemplateId,
                                    user.Email);

                                emailsSent++;
                            }

                            userCounter++;

                            if (token.IsCancellationRequested)
                            {
                                break;
                            }

                            if (sw.Elapsed.TotalSeconds - elapsed.TotalSeconds > 15
                                || userCounter == 1)
                            {
                                elapsed = sw.Elapsed;

                                var remaining = TimeSpan
                                    .FromMilliseconds(elapsed.TotalMilliseconds / userCounter
                                        * (subscribedUsers.Count - userCounter))
                                    .ToString(@"mm\:ss");

                                _logger.LogTrace("Elapsed: {Elapsed} / Est. remaining: {Remaining}",
                                    elapsed.ToString(@"mm\:ss"),
                                    remaining);

                                progress.Report(new OperationStatus
                                {
                                    PercentComplete = emailsSent * 100 / subscribedUsers.Count,
                                    Status = $"Sent {emailsSent}, skipped {emailsSkipped} of {subscribedUsers.Count}; {elapsed.ToString(@"mm\:ss")} elapsed, est. {remaining} remaining",
                                    Error = false
                                });
                            }
                        }

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        filter.Skip = userCounter;
                        subscribedUsers = await _userService.GetPaginatedUserListAsync(filter);
                    }

                    string taskStatus = token.IsCancellationRequested
                        ? "Cancelled after"
                        : "Task completed with";

                    return new OperationStatus
                    {
                        PercentComplete = emailsSent * 100 / subscribedUsers.Count,
                        Status = $"{taskStatus} {emailsSent} sent, {emailsSkipped} skipped of {subscribedUsers.Count} in {elapsed.ToString(@"mm\:ss")}."
                    };
                }
                else
                {
                    _logger.LogWarning("User {RequestingUser} attempted to send bulk emails in job {JobId} with no subscribed participants.",
                        userId,
                        jobId);
                    return new OperationStatus
                    {
                        PercentComplete = 0,
                        Status = "No participants are subscribed.",
                        Error = true
                    };
                }
            }
            else
            {
                _logger.LogError("User {RequestingUser} attempted to send bulk emails in job {JobId} without permission.",
                    userId,
                    jobId);
                return new OperationStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true
                };
            }
        }
    }
}
