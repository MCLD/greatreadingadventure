using System;
using System.Collections.Generic;
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
    public class EmailBulkService : BaseUserService<EmailBulkService>
    {
        private const string SpanFormat = @"hh\:mm\:ss";

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

        public async Task<JobStatus> RunJobAsync(int userId,
            int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress = null)
        {
            if (HasPermission(Permission.SendBulkEmails))
            {
                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                int emailsSent = 0;
                int emailsSkipped = 0;
                int userCounter = 0;

                int addSentCounter = 0;

                var problemUsers = new List<int>();

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
                        sw.Elapsed.ToString(SpanFormat));
                });

                if (subscribedUsers.Count > 0)
                {
                    var elapsedStatus = sw.Elapsed;
                    var elapsedUpdateDbStatus = sw.Elapsed;

                    var job = await _jobRepository.GetByIdAsync(jobId);
                    var jobDetails
                        = JsonConvert
                            .DeserializeObject<JobDetailsSendBulkEmails>(job.SerializedParameters);

                    var template
                        = await _emailService.GetEmailTemplate(jobDetails.EmailTemplateId);

                    progress.Report(new JobStatus
                    {
                        PercentComplete = 0,
                        Title = $"Sending email: {template.Description}",
                        Status = $"Preparing to email {subscribedUsers.Count} participants...",
                        Error = false
                    });

                    while (subscribedUsers.Data.Any())
                    {
                        Thread.Sleep(5000);
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
                                try
                                {
                                    await _emailService
                                        .SendBulkAsync(user, jobDetails.EmailTemplateId);
                                    addSentCounter++;
                                    await _userService.SentBulkEmailAsync(user.Id,
                                        jobDetails.EmailTemplateId,
                                        user.Email);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex,
                                        "Bulk email send failed to {UserId} at {Email}: {Message}",
                                        user.Id,
                                        user.Email,
                                        ex.Message);

                                    problemUsers.Add(user.Id);
                                }
                                emailsSent++;
                            }

                            userCounter++;

                            if (token.IsCancellationRequested)
                            {
                                break;
                            }

                            if (sw.Elapsed.TotalSeconds - elapsedStatus.TotalSeconds > 15
                                || userCounter == 1)
                            {
                                elapsedStatus = sw.Elapsed;

                                var remaining = TimeSpan
                                    .FromMilliseconds(elapsedStatus.TotalMilliseconds / userCounter
                                        * (subscribedUsers.Count - userCounter))
                                    .ToString(SpanFormat);

                                var status = new JobStatus
                                {
                                    PercentComplete = (emailsSent + emailsSkipped) * 100 / subscribedUsers.Count,
                                    Status = $"Sent {emailsSent}, skipped {emailsSkipped} of {subscribedUsers.Count}; {elapsedStatus.ToString(SpanFormat)} elapsed, est. {remaining} remaining",
                                    Error = false
                                };

                                progress.Report(status);

                                if (addSentCounter > 0)
                                {
                                    await _emailService.UpdateSentCount(template.Id, addSentCounter);
                                    addSentCounter = 0;
                                }

                                if (sw.Elapsed.TotalSeconds - elapsedUpdateDbStatus.TotalSeconds > 60
                                    || userCounter == 1)
                                {
                                    var dbStatusText = string.Format("{0}%: {1}",
                                        status.PercentComplete,
                                        status.Status);

                                    await _jobRepository.UpdateStatusAsync(jobId,
                                        dbStatusText.Substring(0, Math.Min(dbStatusText.Length, 255)));
                                }
                            }
                        }

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        filter.Skip = userCounter;
                        subscribedUsers = await _userService.GetPaginatedUserListAsync(filter);

                        subscribedUsers.Data = subscribedUsers.Data
                            .Where(_ => !problemUsers.Contains(_.Id));
                    }

                    string taskStatus = token.IsCancellationRequested
                        ? "Cancelled after"
                        : "Task completed with";

                    var finalStatus = new JobStatus
                    {
                        PercentComplete = (emailsSent + emailsSkipped) * 100 / subscribedUsers.Count,
                        Status = $"{taskStatus} {emailsSent} sent, {emailsSkipped} skipped of {subscribedUsers.Count} in {elapsedStatus.ToString(SpanFormat)}."
                    };

                    var statusText = string.Format("{0}%: {1}",
                        finalStatus.PercentComplete,
                        finalStatus.Status);

                    await _jobRepository.UpdateStatusAsync(jobId,
                        statusText.Substring(0, Math.Min(statusText.Length, 255)));

                    return finalStatus;
                }
                else
                {
                    _logger.LogWarning("User {RequestingUser} attempted to send bulk emails in job {JobId} with no subscribed participants.",
                        userId,
                        jobId);

                    await _jobRepository.UpdateStatusAsync(jobId,
                        "No participants were subscribed.");

                    return new JobStatus
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

                await _jobRepository.UpdateStatusAsync(jobId,
                    "Insufficient permission to run job.");

                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "Permission denied.",
                    Error = true
                };
            }
        }
    }
}
