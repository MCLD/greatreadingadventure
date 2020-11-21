using System;
using System.Collections.Generic;
using System.Globalization;
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

        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IJobRepository _jobRepository;
        private readonly EmailReminderService _emailReminderService;
        private readonly EmailService _emailService;
        private readonly UserService _userService;

        public EmailBulkService(ILogger<EmailBulkService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            IEmailTemplateRepository emailTemplateRepository,
            IJobRepository jobRepository,
            EmailReminderService emailReminderService,
            EmailService emailService,
            UserService userService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _emailTemplateRepository = emailTemplateRepository
                ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _emailReminderService = emailReminderService
                ?? throw new ArgumentNullException(nameof(emailReminderService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userService = userService
                ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<JobStatus> RunJobAsync(int userId,
            int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress)
        {
            if (HasPermission(Permission.SendBulkEmails))
            {
                var job = await _jobRepository.GetByIdAsync(jobId);
                var jobDetails
                    = JsonConvert
                        .DeserializeObject<JobDetailsSendBulkEmails>(job.SerializedParameters);

                if (string.IsNullOrEmpty(jobDetails.To))
                {
                    // send for real
                    if (string.IsNullOrEmpty(jobDetails.MailingList))
                    {
                        return await SendBulkParticipantAsync(userId,
                            jobId,
                            token,
                            progress,
                            jobDetails);
                    }
                    else
                    {
                        return await SendBulkListAsync(userId, jobId, token, progress, jobDetails);
                    }
                }
                else
                {
                    return await SendTestAsync(jobId, token, progress, jobDetails);
                }
            }
            else
            {
                _logger.LogError("User {UserId} attempted to send bulk emails in job {JobId} without permission.",
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

        private async Task<JobStatus> SendTestAsync(int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress,
            JobDetailsSendBulkEmails jobDetails)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                string[] toArray = jobDetails.To.Contains(',')
                    ? jobDetails.To.Split(',')
                    : new[] { jobDetails.To };

                progress.Report(new JobStatus
                {
                    PercentComplete = 0,
                    Status = $"Preparing to send {toArray.Length} test emails...",
                });

                int sent = 0;
                bool cancelled = false;

                foreach (string toAddress in toArray)
                {
                    if (sent > 1)
                    {
                        Thread.Sleep(1000);
                    }

                    if (token.IsCancellationRequested)
                    {
                        cancelled = true;
                        break;
                    }

                    await _emailService
                        .SendBulkTestAsync(toAddress, jobDetails.EmailTemplateId);
                    sent++;

                    if (sent % 5 == 0 || sent == 1)
                    {
                        progress.Report(new JobStatus
                        {
                            PercentComplete = sent * 100 / toArray.Length,
                            Status = $"Sent {sent} test emails of {toArray.Length}",
                        });
                    }
                }

                sw.Stop();

                string outcome = cancelled ? "cancelled" : "completed";

                _logger.LogInformation("Email job {JobId}: " + outcome + ", {EmailsSent} test emails sent in {Elapsed} ms",
                    jobId,
                    toArray.Length,
                    sw.Elapsed.TotalMilliseconds);

                string status = cancelled
                    ? $"Sent {toArray.Length} test emails in {sw.Elapsed.ToString(SpanFormat, CultureInfo.InvariantCulture)} s."
                    : $"Cancelled after sending {toArray.Length} test emails in {sw.Elapsed.ToString(SpanFormat, CultureInfo.InvariantCulture)} s.";

                await _jobRepository.UpdateStatusAsync(jobId,
                    status.Substring(0, Math.Min(status.Length, 255)));

                return new JobStatus
                {
                    PercentComplete = sent * 100 / toArray.Length,
                    Status = status,
                    Complete = !cancelled
                };
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                _logger.LogInformation("Email job {JobId}: test send to {To} failed: {ErrorMessage}",
                    jobId,
                    jobDetails.To,
                    ex.Message);

                await _jobRepository.UpdateStatusAsync(jobId,
                    $"Error sending test: {ex.Message}");

                return new JobStatus
                {
                    Status = $"Error sending test: {ex.Message}",
                    Error = true
                };
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
        private async Task<JobStatus> SendBulkParticipantAsync(int userId,
            int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress,
            JobDetailsSendBulkEmails jobDetails)
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
                Take = 30
            };

            var subscribedUsers = await _userService.GetPaginatedUserListAsync(filter);
            _logger.LogInformation("Email job {JobId}: found {Count} subscribed users, processing first batch of {BatchCount}",
                jobId,
                subscribedUsers.Count,
                subscribedUsers.Data.Count());

            token.Register(() =>
            {
                _logger.LogWarning("Email job {JobId} for user {UserId} was cancelled after {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedUsersCount} in {TimeElapsed}.",
                    jobId,
                    userId,
                    emailsSent,
                    emailsSkipped,
                    subscribedUsers?.Count,
                    sw.Elapsed.ToString(SpanFormat, CultureInfo.InvariantCulture));
            });

            if (subscribedUsers.Count > 0)
            {
                var elapsedStatus = sw.Elapsed;
                var elapsedUpdateDbStatus = sw.Elapsed;
                var elapsedLogInfoStatus = sw.Elapsed;
                var elapsedLogInfoPercent = 0;

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
                    Thread.Sleep(1000);
                    foreach (var user in subscribedUsers.Data)
                    {
                        // check email has not be sent to user
                        var alreadyGotIt = await _userService
                            .HasReceivedBulkEmailAsync(jobDetails.EmailTemplateId, user.Email);

                        if (alreadyGotIt)
                        {
                            // send email
                            _logger.LogTrace("Email job {JobId}: skipping email {Count}/{Total} to user {User} at {Email} with template {EmailTemplate}",
                                jobId,
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
                            _logger.LogTrace("Email job {JobId}: sending email {Count}/{Total} to user {User} at {Email} with template {EmailTemplate}",
                                jobId,
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
                                await _userService.SentBulkEmailAsync(user.Id,
                                    jobDetails.EmailTemplateId,
                                    user.Email);

                                addSentCounter++;
                                emailsSent++;
                            }
#pragma warning disable CA1031 // Do not catch general exception types
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Email job {JobId}: Send failed to {UserId} at {Email}: {ErrorMessage}",
                                    jobId,
                                    user.Id,
                                    user.Email,
                                    ex.Message);

                                if (!problemUsers.Contains(user.Id))
                                {
                                    problemUsers.Add(user.Id);
                                }
                            }
#pragma warning restore CA1031 // Do not catch general exception types
                        }

                        userCounter++;

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (sw.Elapsed.TotalSeconds - elapsedStatus.TotalSeconds > 5
                            || userCounter == 1)
                        {
                            elapsedStatus = sw.Elapsed;

                            var remaining = TimeSpan
                                .FromMilliseconds(elapsedStatus.TotalMilliseconds / userCounter
                                    * (subscribedUsers.Count - userCounter))
                                .ToString(SpanFormat, System.Globalization.CultureInfo.InvariantCulture);

                            var status = new JobStatus
                            {
                                PercentComplete = userCounter * 100 / subscribedUsers.Count,
                                Status = $"Sent {emailsSent}, skipped {emailsSkipped} of {subscribedUsers.Count}; {elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture)}, remaining: {remaining}, problems: {problemUsers.Count}",
                                Error = false
                            };

                            progress.Report(status);

                            if (sw.Elapsed.TotalSeconds - elapsedUpdateDbStatus.TotalSeconds > 60
                                || userCounter == 1)
                            {
                                elapsedUpdateDbStatus = sw.Elapsed;

                                if (addSentCounter > 0)
                                {
                                    await _emailService.UpdateSentCount(template.Id, addSentCounter);
                                    addSentCounter = 0;
                                }

                                var dbStatusText = string.Format(CultureInfo.InvariantCulture,
                                    "{0}%: {1}",
                                    status.PercentComplete,
                                    status.Status);

                                await _jobRepository.UpdateStatusAsync(jobId,
                                    dbStatusText.Substring(0, Math.Min(dbStatusText.Length, 255)));
                            }

                            if (sw.Elapsed.TotalSeconds - elapsedLogInfoStatus.TotalSeconds > 500
                                || userCounter == 1
                                || status.PercentComplete - elapsedLogInfoPercent >= 20)
                            {
                                elapsedLogInfoStatus = sw.Elapsed;
                                elapsedLogInfoPercent = status.PercentComplete ?? 0;

                                _logger.LogInformation("Email job {JobId}: {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}, remaining: {EmailsRemaining}, problems: {EmailProblems}",
                                    jobId,
                                    emailsSent,
                                    emailsSkipped,
                                    subscribedUsers.Count,
                                    elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture),
                                    remaining,
                                    problemUsers.Count);
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

                template.EmailsSent = emailsSent;
                await _emailTemplateRepository.UpdateSaveNoAuditAsync(template);

                string taskStatus = token.IsCancellationRequested
                    ? "Cancelled after"
                    : "Task completed with";

                var finalStatus = new JobStatus
                {
                    PercentComplete = userCounter * 100 / subscribedUsers.Count,
                    Status = $"{taskStatus} {emailsSent} sent, {emailsSkipped} skipped of {subscribedUsers.Count} in {elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture)}."
                };

                var statusText = string.Format(CultureInfo.InvariantCulture,
                    "{0}%: {1}",
                    finalStatus.PercentComplete,
                    finalStatus.Status);

                await _jobRepository.UpdateStatusAsync(jobId,
                    statusText.Substring(0, Math.Min(statusText.Length, 255)));

                _logger.LogInformation("Email job {JobId}: " + taskStatus + " {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}",
                    jobId,
                    emailsSent,
                    emailsSkipped,
                    subscribedUsers.Count,
                    elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture));

                return finalStatus;
            }
            else
            {
                _logger.LogWarning("User {UserId} attempted to send bulk emails in job {JobId} with no subscribed participants.",
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

        private async Task<JobStatus> SendBulkListAsync(int userId,
            int jobId,
            CancellationToken token,
            IProgress<JobStatus> progress,
            JobDetailsSendBulkEmails jobDetails)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int emailsSent = 0;
            int emailsSkipped = 0;
            int userCounter = 0;

            int addSentCounter = 0;

            var problemEmails = new List<string>();

            var filter = new EmailReminderFilter
            {
                MailingList = jobDetails.MailingList,
                Take = 30
            };

            var subscribers = await _emailReminderService.GetSubscribersWithCountAsync(filter);

            var subscribedCount = subscribers.Count;

            var subscribed = subscribers.Data;

            _logger.LogInformation("Email job {JobId}: found {Count} subscribed users, processing first batch of {BatchCount}",
                jobId,
                subscribedCount,
                subscribed.Count);

            token.Register(() =>
            {
                _logger.LogWarning("Email job {JobId} for user {UserId} was cancelled after {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedUsersCount} in {TimeElapsed}.",
                    jobId,
                    userId,
                    emailsSent,
                    emailsSkipped,
                    subscribedCount,
                    sw.Elapsed.ToString(SpanFormat, CultureInfo.InvariantCulture));
            });

            if (subscribed.Count > 0)
            {
                var elapsedStatus = sw.Elapsed;
                var elapsedUpdateDbStatus = sw.Elapsed;
                var elapsedLogInfoStatus = sw.Elapsed;
                var elapsedLogInfoPercent = 0;

                var template
                    = await _emailService.GetEmailTemplate(jobDetails.EmailTemplateId);

                progress.Report(new JobStatus
                {
                    PercentComplete = 0,
                    Title = $"Sending email: {template.Description}",
                    Status = $"Preparing to email {subscribed.Count} participants...",
                    Error = false
                });

                var siteId = GetCurrentSiteId();

                while (subscribed.Count > 0)
                {
                    Thread.Sleep(1000);

                    foreach (var emailReminder in subscribed)
                    {
                        if (problemEmails.Contains(emailReminder.Email))
                        {
                            emailsSkipped++;
                            continue;
                        }

                        bool clearToSend = true;

                        if(!jobDetails.SendToParticipantsToo)
                        {
                            var isParticipant = await _userService
                                .IsEmailSubscribedAsync(emailReminder.Email);

                            if(isParticipant)
                            {
                                clearToSend = false;
                            }
                        }

                        if (emailReminder.SentAt != null || !clearToSend)
                        {
                            // send email
                            _logger.LogTrace("Email job {JobId}: skipping email {Count}/{Total} to {Email}: {Message}",
                                jobId,
                                userCounter + 1,
                                subscribedCount,
                                emailReminder.Email,
                                emailReminder.SentAt != null
                                    ? " already sent at " + emailReminder.SentAt
                                    : " is a subscribed participant");

                            emailsSkipped++;
                        }
                        else
                        {
                            // send email
                            _logger.LogTrace("Email job {JobId}: sending email {Count}/{Total} to {Email} with template {EmailTemplate}",
                                jobId,
                                userCounter + 1,
                                subscribedCount,
                                emailReminder.Email,
                                jobDetails.EmailTemplateId);

                            // send email to user
                            try
                            {
                                await _emailService
                                    .SendBulkAsync(emailReminder,
                                        jobDetails.EmailTemplateId,
                                        siteId);

                                await _emailReminderService.UpdateSentDateAsync(emailReminder.Id);

                                addSentCounter++;
                                emailsSent++;
                            }
#pragma warning disable CA1031 // Do not catch general exception types
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Email job {JobId}: Send failed to {UserId} at {Email}: {ErrorMessage}",
                                    jobId,
                                    emailReminder.Id,
                                    emailReminder.Email,
                                    ex.Message);

                                if (!problemEmails.Contains(emailReminder.Email))
                                {
                                    problemEmails.Add(emailReminder.Email);
                                }
                            }
#pragma warning restore CA1031 // Do not catch general exception types
                        }

                        userCounter++;

                        if (token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (sw.Elapsed.TotalSeconds - elapsedStatus.TotalSeconds > 5
                            || userCounter == 1)
                        {
                            elapsedStatus = sw.Elapsed;

                            var remaining = TimeSpan
                                .FromMilliseconds(elapsedStatus.TotalMilliseconds / userCounter
                                    * (subscribedCount - userCounter))
                                .ToString(SpanFormat, CultureInfo.InvariantCulture);

                            var status = new JobStatus
                            {
                                PercentComplete = userCounter * 100 / subscribedCount,
                                Status = $"Sent {emailsSent}, skipped {emailsSkipped} of {subscribedCount}; {elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture)}, remaining: {remaining}, problems: {problemEmails.Count}",
                                Error = false
                            };

                            progress.Report(status);

                            if (sw.Elapsed.TotalSeconds - elapsedUpdateDbStatus.TotalSeconds > 60
                                || userCounter == 1)
                            {
                                elapsedUpdateDbStatus = sw.Elapsed;

                                if (addSentCounter > 0)
                                {
                                    await _emailService.UpdateSentCount(template.Id, addSentCounter);
                                    addSentCounter = 0;
                                }

                                var dbStatusText = string.Format(CultureInfo.InvariantCulture,
                                    "{0}%: {1}",
                                    status.PercentComplete,
                                    status.Status);

                                await _jobRepository.UpdateStatusAsync(jobId,
                                    dbStatusText.Substring(0, Math.Min(dbStatusText.Length, 255)));
                            }

                            if (sw.Elapsed.TotalSeconds - elapsedLogInfoStatus.TotalSeconds > 500
                                || userCounter == 1
                                || status.PercentComplete - elapsedLogInfoPercent >= 20)
                            {
                                elapsedLogInfoStatus = sw.Elapsed;
                                elapsedLogInfoPercent = status.PercentComplete ?? 0;

                                _logger.LogInformation("Email job {JobId}: {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}, remaining: {EmailsRemaining}, problems: {EmailProblems}",
                                    jobId,
                                    emailsSent,
                                    emailsSkipped,
                                    subscribedCount,
                                    elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture),
                                    remaining,
                                    problemEmails.Count);
                            }
                        }
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    filter.Skip = userCounter;
                    subscribed = await _emailReminderService.GetSubscribersAsync(filter);
                }

                template.EmailsSent = emailsSent;
                await _emailTemplateRepository.UpdateSaveNoAuditAsync(template);

                string taskStatus = token.IsCancellationRequested
                    ? "Cancelled after"
                    : "Task completed with";

                var finalStatus = new JobStatus
                {
                    PercentComplete = userCounter * 100 / subscribedCount,
                    Status = $"{taskStatus} {emailsSent} sent, {emailsSkipped} skipped of {subscribedCount} in {elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture)}."
                };

                var statusText = string.Format(CultureInfo.InvariantCulture,
                    "{0}%: {1}",
                    finalStatus.PercentComplete,
                    finalStatus.Status);

                await _jobRepository.UpdateStatusAsync(jobId,
                    statusText.Substring(0, Math.Min(statusText.Length, 255)));

                _logger.LogInformation("Email job {JobId}: " + taskStatus + " {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}",
                    jobId,
                    emailsSent,
                    emailsSkipped,
                    subscribedCount,
                    elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture));

                return finalStatus;
            }
            else
            {
                _logger.LogWarning("User {UserId} attempted to send bulk emails in job {JobId} with no subscribed participants.",
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
    }
}
