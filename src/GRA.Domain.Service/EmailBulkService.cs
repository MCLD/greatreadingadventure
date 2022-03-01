using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GRA.Abstract;
using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GRA.Domain.Service
{
    public class EmailBulkService : BaseUserService<EmailBulkService>
    {
        private const string SpanFormat = @"hh\:mm\:ss";

        private readonly IDirectEmailHistoryRepository _directEmailHistoryRepository;
        private readonly EmailReminderService _emailReminderService;
        private readonly EmailService _emailService;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IJobRepository _jobRepository;
        private readonly SiteLookupService _siteLookupService;
        private readonly UserService _userService;

        public EmailBulkService(ILogger<EmailBulkService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            EmailReminderService emailReminderService,
            EmailService emailService,
            IDirectEmailHistoryRepository directEmailHistoryRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IJobRepository jobRepository,
            UserService userService,
            SiteLookupService siteLookupService)
            : base(logger, dateTimeProvider, userContextProvider)
        {
            _directEmailHistoryRepository = directEmailHistoryRepository
                ?? throw new ArgumentNullException(nameof(directEmailHistoryRepository));
            _emailReminderService = emailReminderService
                ?? throw new ArgumentNullException(nameof(emailReminderService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _emailTemplateRepository = emailTemplateRepository
                ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
            _jobRepository = jobRepository
                ?? throw new ArgumentNullException(nameof(jobRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
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
                            progress,
                            jobDetails,
                            token);
                    }
                    else
                    {
                        return await SendBulkListAsync(userId, jobId, progress, jobDetails, token);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async Task<JobStatus> SendBulkListAsync(int userId,
            int jobId,
            IProgress<JobStatus> progress,
            JobDetailsSendBulkEmails jobDetails,
            CancellationToken token)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            int emailsSent = 0;
            int emailsSkipped = 0;
            int userCounter = 0;

            int addSentCounter = 0;

            var problemEmails = new HashSet<string>();

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
                var site = await _siteLookupService.GetByIdAsync(GetCurrentSiteId());

                var emailDetails = new DirectEmailDetails(site.Name)
                {
                    IsBulk = true,
                    SendingUserId = userId,
                    DirectEmailTemplateId = jobDetails.EmailTemplateId
                };

                var elapsedStatus = sw.Elapsed;
                var elapsedUpdateDbStatus = sw.Elapsed;
                var elapsedLogInfoStatus = sw.Elapsed;
                var elapsedLogInfoPercent = 0;

                progress.Report(new JobStatus
                {
                    PercentComplete = 0,
                    Title = $"Sending email...",
                    Status = $"Preparing to email {subscribed.Count} participants...",
                    Error = false
                });

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

                        if (!jobDetails.SendToParticipantsToo)
                        {
                            var isParticipant = await _userService
                                .IsEmailSubscribedAsync(emailReminder.Email);

                            if (isParticipant)
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
                                emailDetails.ToAddress = emailReminder.Email;
                                emailDetails.LanguageId = emailReminder.LanguageId;
                                emailDetails.ClearTags();
                                emailDetails.SetTag("Email", emailReminder.Email);

                                var result = await _emailService.SendDirectAsync(emailDetails);
                                await _emailReminderService.UpdateSentDateAsync(emailReminder.Id);

                                if (result.Successful)
                                {
                                    addSentCounter++;
                                    emailsSent++;
                                }
                                else
                                {
                                    problemEmails.Add(emailReminder.Email);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Email job {JobId}: Send failed to {UserId} at {Email}: {ErrorMessage}",
                                    jobId,
                                    emailReminder.Id,
                                    emailReminder.Email,
                                    ex.Message);

                                problemEmails.Add(emailReminder.Email);
                            }
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
                                    await _emailService.UpdateSentCount(jobDetails.EmailTemplateId,
                                        addSentCounter);
                                    addSentCounter = 0;
                                }

                                var dbStatusText = string.Format(CultureInfo.InvariantCulture,
                                    "{0}%: {1}",
                                    status.PercentComplete,
                                    status.Status);

                                await _jobRepository.UpdateStatusAsync(jobId,
                                    dbStatusText[..Math.Min(dbStatusText.Length, 255)]);
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

                await _emailService.UpdateSentCount(jobDetails.EmailTemplateId,
                    addSentCounter);
                addSentCounter = 0;

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
                    statusText[..Math.Min(statusText.Length, 255)]);

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Single email failure should not kill the entire job.")]
        private async Task<JobStatus> SendBulkParticipantAsync(int userId,
            int jobId,
            IProgress<JobStatus> progress,
            JobDetailsSendBulkEmails jobDetails,
            CancellationToken token)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            int emailsSent = 0;
            int emailsSkipped = 0;
            int userCounter = 0;

            int addSentCounter = 0;

            var problemUsers = new HashSet<int>();

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
                var site = await _siteLookupService.GetByIdAsync(GetCurrentSiteId());

                var emailDetails = new DirectEmailDetails(site.Name)
                {
                    IsBulk = true,
                    SendingUserId = userId,
                    DirectEmailTemplateId = jobDetails.EmailTemplateId
                };

                var elapsedStatus = sw.Elapsed;
                var elapsedUpdateDbStatus = sw.Elapsed;
                var elapsedLogInfoStatus = sw.Elapsed;
                var elapsedLogInfoPercent = 0;

                progress.Report(new JobStatus
                {
                    PercentComplete = 0,
                    Title = $"Sending email...",
                    Status = $"Preparing to email {subscribedUsers.Count} participants...",
                    Error = false
                });

                var alreadyReceived = await _directEmailHistoryRepository
                    .GetSentEmailByTemplateIdAsync(jobDetails.EmailTemplateId);

                while (subscribedUsers.Data.Any())
                {
                    Thread.Sleep(1000);
                    foreach (var user in subscribedUsers.Data)
                    {
                        if (alreadyReceived.Contains(user.Email))
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

                            emailDetails.ToUserId = user.Id;
                            emailDetails.ClearTags();
                            emailDetails.SetTag("Name", user.FullName);
                            emailDetails.SetTag("Email", user.Email);

                            // send email to user
                            try
                            {
                                var result = await _emailService.SendDirectAsync(emailDetails);
                                if (result.Successful)
                                {
                                    alreadyReceived.Add(user.Email);
                                    addSentCounter++;
                                    emailsSent++;
                                }
                                else
                                {
                                    problemUsers.Add(user.Id);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Email job {JobId}: Send failed to {UserId} at {Email}: {ErrorMessage}",
                                    jobId,
                                    user.Id,
                                    user.Email,
                                    ex.Message);

                                problemUsers.Add(user.Id);
                            }
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
                                    await _emailService.UpdateSentCount(jobDetails.EmailTemplateId,
                                        addSentCounter);
                                    addSentCounter = 0;
                                }

                                var dbStatusText = string.Format(CultureInfo.InvariantCulture,
                                    "{0}%: {1}",
                                    status.PercentComplete,
                                    status.Status);

                                await _jobRepository.UpdateStatusAsync(jobId,
                                    dbStatusText[..Math.Min(dbStatusText.Length, 255)]);
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

                await _emailService.UpdateSentCount(jobDetails.EmailTemplateId, addSentCounter);

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

        private async Task<JobStatus> SendTestAsync(int jobId,
                            CancellationToken token,
            IProgress<JobStatus> progress,
            JobDetailsSendBulkEmails jobDetails)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
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

                _logger.LogInformation("Email job {JobId}: " + outcome + ", {EmailsSent} test email(s) sent in {Elapsed} ms",
                    jobId,
                    toArray.Length,
                    sw.Elapsed.TotalMilliseconds);

                string status = cancelled
                    ? $"Cancelled after sending {toArray.Length} test email(s) in {sw.Elapsed.ToString(SpanFormat, CultureInfo.InvariantCulture)} s."
                    : $"Sent {toArray.Length} test email(s) in {sw.Elapsed.ToString(SpanFormat, CultureInfo.InvariantCulture)} s.";

                await _jobRepository.UpdateStatusAsync(jobId,
                    status.Substring(0, Math.Min(status.Length, 255)));

                return new JobStatus
                {
                    PercentComplete = sent * 100 / toArray.Length,
                    Status = status,
                    Complete = !cancelled
                };
            }
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
        }
    }
}
