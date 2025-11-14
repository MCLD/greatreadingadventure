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
        private readonly IJobRepository _jobRepository;
        private readonly SiteLookupService _siteLookupService;
        private readonly UserService _userService;

        public EmailBulkService(ILogger<EmailBulkService> logger,
            IDateTimeProvider dateTimeProvider,
            IUserContextProvider userContextProvider,
            EmailReminderService emailReminderService,
            EmailService emailService,
            IDirectEmailHistoryRepository directEmailHistoryRepository,
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
            ArgumentNullException.ThrowIfNull(progress);

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
                    return await SendTestAsync(userId, jobId, progress, jobDetails, token);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Unattended job should never completely fail.")]
        public async Task<bool> SendWelcomeScheduledTask()
        {
            int sentMails = 0;
            const int defaultMaximumSend = 20;
            int skip = 0;
            const int take = 1000;

            foreach (var site in await _siteLookupService.GetAllAsync())
            {
                var stage = _siteLookupService.GetSiteStage(site);
                if (stage == SiteStage.ProgramOpen)
                {
                    var (emailIdSet, welcomeEmailId) = await _siteLookupService
                        .GetSiteSettingIntAsync(site.Id, SiteSettingKey.Email.WelcomeTemplateId);

                    if (!emailIdSet || welcomeEmailId == 0)
                    {
                        // abort if no welcome email is set
                        return false;
                    }

                    var (unsubSet, unsubBase) = await _siteLookupService
                        .GetSiteSettingStringAsync(site.Id, SiteSettingKey.Email.UnsubscribeBase);

                    if (!unsubSet || string.IsNullOrEmpty(unsubBase))
                    {
                        _logger.LogWarning("Welcome email: can't send, UnsubscribeBase is not set.");
                        return false;
                    }

                    var emailDetails = new DirectEmailDetails(site.Name)
                    {
                        SendingUserId = await _siteLookupService.GetSystemUserId(),
                        DirectEmailTemplateId = welcomeEmailId
                    };

                    _logger.LogTrace("Welcome email: loading users - skip {Skip}, take {Take}",
                        skip,
                        take);
                    var users = await _userService.GetWelcomeRecipientsAsync(site.Id, skip, take);
                    skip = users.Count();

                    if (!users.Any())
                    {
                        return false;
                    }

                    var problemUsers = new HashSet<int>();

                    var alreadyReceived = await _directEmailHistoryRepository
                        .GetSentEmailByTemplateIdAsync(welcomeEmailId);

                    var (maximumSendSet, maximumSendSetting) = await _siteLookupService
                        .GetSiteSettingIntAsync(site.Id,
                            SiteSettingKey.Email.MaximumWelcomeEmailSendBlock);

                    var maximumSend = maximumSendSet
                        ? maximumSendSetting
                        : defaultMaximumSend;

                    while (sentMails <= maximumSend && users.Any())
                    {
                        foreach (var user in users)
                        {
                            if (!problemUsers.Contains(user.Id)
                                && !alreadyReceived.Contains(user.Email,
                                    StringComparer.OrdinalIgnoreCase))
                            {
                                // do the email send
                                emailDetails.ToUserId = user.Id;
                                emailDetails.ClearTags();
                                emailDetails.SetTag("Name", user.FullName);
                                emailDetails.SetTag("Email", user.Email);
                                emailDetails.SetTag("UnsubscribeLink",
                                    BuildUnsub(unsubBase, user.UnsubscribeToken));

                                try
                                {
                                    var result = await _emailService.SendDirectAsync(emailDetails);
                                    if (result.Successful)
                                    {
                                        alreadyReceived.Add(user.Email);
                                        sentMails++;
                                    }
                                    else
                                    {
                                        _logger.LogTrace("Welcome email: problem user {UserId} with address {EmailAddress}: {SentResponse}",
                                            user.Id,
                                            user.Email,
                                            result.SentResponse);
                                        problemUsers.Add(user.Id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex,
                                        "Welcome email: Send failed to {UserId} at {Email}: {ErrorMessage}",
                                        user.Id,
                                        user.Email,
                                        ex.Message);

                                    problemUsers.Add(user.Id);
                                }
                            }

                            if (sentMails >= maximumSend)
                            {
                                return sentMails > 0;
                            }
                        }

                        _logger.LogTrace("Welcome email: loading users - skip {Skip}, take {Take}",
                            skip,
                            take);
                        users = await _userService.GetWelcomeRecipientsAsync(site.Id, skip, take);
                        skip += users.Count();
                    }
                }
            }

            return sentMails > 0;
        }

        private static string BuildUnsub(string baseLink, string token)
        {
            if (string.IsNullOrEmpty(baseLink))
            {
                return null;
            }

            return baseLink.EndsWith('/')
                ? baseLink + token
                : $"{baseLink}/{token}";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Single email failure should not kill the entire job.")]
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
                _logger.LogWarning("Email job {JobId} for user {UserId} was cancelled after {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedUsersCount} total in {TimeElapsed}.",
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
                    Title = "Sending email...",
                    Status = $"Preparing to email {subscribed.Count} participants...",
                    Error = false
                });

                while (subscribed.Count > 0)
                {
                    foreach (var emailReminder in subscribed)
                    {
                        if (problemEmails.Contains(emailReminder.Email))
                        {
                            emailsSkipped++;
                            continue;
                        }

                        bool clearToSend = true;

                        var isParticipant = await _userService
                            .IsEmailSubscribedAsync(emailReminder.Email);

                        if (isParticipant)
                        {
                            clearToSend = false;
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

                                DirectEmailHistory result = null;
                                try
                                {
                                    result = await _emailService.SendDirectAsync(emailDetails);
                                    await _emailReminderService
                                        .UpdateSentDateAsync(emailReminder.Id);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogWarning("Unable to email {ToAddress}: {ErrorMessage}",
                                        emailDetails.ToAddress,
                                        ex.Message);
                                }

                                if (result?.Successful == true)
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
                                    await _emailService.IncrementSentCountAsync(
                                        jobDetails.EmailTemplateId,
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
                    if (subscribed.Any())
                    {
                        Thread.Sleep(1000);
                    }
                }

                await _emailService.IncrementSentCountAsync(jobDetails.EmailTemplateId,
                    addSentCounter);

                string taskStatus = token.IsCancellationRequested
                    ? "Cancelled after"
                    : "Task completed with";

                var finalStatus = new JobStatus
                {
                    PercentComplete = userCounter * 100 / subscribedCount,
                    Status = $"{taskStatus} {emailsSent} sent, {emailsSkipped} skipped of {subscribedCount} total in {elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture)}."
                };

                var statusText = string.Format(CultureInfo.InvariantCulture,
                    "{0}%: {1}",
                    finalStatus.PercentComplete,
                    finalStatus.Status);

                await _jobRepository.UpdateStatusAsync(jobId,
                    statusText[..Math.Min(statusText.Length, 255)]);

                _logger.LogInformation("Email job {JobId}: {TaskStatus} {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}",
                    taskStatus,
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
                _logger.LogWarning("Email job {JobId} for user {UserId} cancelled after {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedUsersCount} total in {TimeElapsed}.",
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
                    Title = "Sending email...",
                    Status = $"Preparing to email {subscribedUsers.Count} participants...",
                    Error = false
                });

                var alreadyReceived = await _directEmailHistoryRepository
                    .GetSentEmailByTemplateIdAsync(jobDetails.EmailTemplateId);

                while (subscribedUsers.Data.Any())
                {
                    foreach (var user in subscribedUsers.Data)
                    {
                        if (user.CannotBeEmailed || alreadyReceived.Contains(user.Email))
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
                            emailDetails.SetTag("UnsubscribeLink",
                                BuildUnsub(jobDetails.UnsubscribeBase, user.UnsubscribeToken));

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
                                .ToString(SpanFormat, CultureInfo.InvariantCulture);

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
                                    await _emailService.IncrementSentCountAsync(
                                        jobDetails.EmailTemplateId,
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

                    if (subscribedUsers.Data.Any())
                    {
                        Thread.Sleep(1000);
                    }
                }

                await _emailService.IncrementSentCountAsync(jobDetails.EmailTemplateId,
                    addSentCounter);

                string taskStatus = token.IsCancellationRequested
                ? "Cancelled after"
                : "Task completed with";

                var finalStatus = new JobStatus
                {
                    PercentComplete = userCounter * 100 / subscribedUsers.Count,
                    Status = $"{taskStatus} {emailsSent} sent, {emailsSkipped} skipped of {subscribedUsers.Count} total in {elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture)}."
                };

                var statusText = string.Format(CultureInfo.InvariantCulture,
                    "{0}%: {1}",
                    finalStatus.PercentComplete,
                    finalStatus.Status);

                await _jobRepository.UpdateStatusAsync(jobId,
                    statusText[..Math.Min(statusText.Length, 255)]);

                _logger.LogInformation("Email job {JobId}: {TaskStatus} {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}",
                    jobId,
                    taskStatus,
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design",
            "CA1031:Do not catch general exception types",
            Justification = "Test email failure should not kill the entire site.")]
        private async Task<JobStatus> SendTestAsync(int userId,
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

            var tos = new List<EmailReminder>();

            foreach (var address in jobDetails.To.Split(','))
            {
                tos.Add(new EmailReminder
                {
                    Email = address,
                    LanguageId = jobDetails.TestLanguageId,
                });
            }

            var subscribedCount = tos.Count;
            var subscribed = tos;

            _logger.LogInformation("Test email job {JobId}: found {Count} subscribed users, processing first batch of {BatchCount}",
                jobId,
                subscribedCount,
                subscribed.Count);

            token.Register(() =>
            {
                _logger.LogWarning("Test email job {JobId} for user {UserId} was cancelled after {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedUsersCount} in {TimeElapsed}.",
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
                    IsTest = true,
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
                    Title = "Sending test email...",
                    Status = $"Preparing to send a test email to {subscribed.Count} participants...",
                    Error = false
                });

                var sent = new List<EmailReminder>();

                while (subscribed.Count > 0)
                {
                    foreach (var emailReminder in subscribed)
                    {
                        if (problemEmails.Contains(emailReminder.Email))
                        {
                            emailsSkipped++;
                            continue;
                        }

                        if (emailReminder.SentAt != null)
                        {
                            // send email
                            _logger.LogTrace("Test email job {JobId}: skipping email {Count}/{Total} to {Email}: {Message}",
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
                            _logger.LogTrace("Test email job {JobId}: sending email {Count}/{Total} to {Email} with template {EmailTemplate}",
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
                                emailDetails.SetTag("UnsubscribeLink",
                                    BuildUnsub(jobDetails.UnsubscribeBase, "TESTEMAIL"));

                                sent.Add(emailReminder);
                                var result = await _emailService.SendDirectAsync(emailDetails);

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
                                    "Test email job {JobId}: Send failed to {UserId} at {Email}: {ErrorMessage}",
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

                                _logger.LogInformation("Test email job {JobId}: {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}, remaining: {EmailsRemaining}, problems: {EmailProblems}",
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

                    subscribed = subscribed.Except(sent).ToList();

                    if (subscribed.Any())
                    {
                        Thread.Sleep(1000);
                    }
                }

                string taskStatus = token.IsCancellationRequested
                    ? "Cancelled after"
                    : "Task completed with";

                var finalStatus = new JobStatus
                {
                    PercentComplete = userCounter * 100 / subscribedCount,
                    Status = $"{taskStatus} {emailsSent} tests sent, {emailsSkipped} skipped of {subscribedCount} in {elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture)}."
                };

                var statusText = string.Format(CultureInfo.InvariantCulture,
                    "{0}%: {1}",
                    finalStatus.PercentComplete,
                    finalStatus.Status);

                await _jobRepository.UpdateStatusAsync(jobId,
                    statusText[..Math.Min(statusText.Length, 255)]);

                _logger.LogInformation("Test email job {JobId}: {TaskStatus} {EmailsSent} sent, {EmailsSkipped} skipped of {SubscribedCount} total in {ElapsedTime}",
                    jobId,
                    taskStatus,
                    emailsSent,
                    emailsSkipped,
                    subscribedCount,
                    elapsedStatus.ToString(SpanFormat, CultureInfo.InvariantCulture));

                return finalStatus;
            }
            else
            {
                _logger.LogWarning("User {UserId} attempted to send test emails in job {JobId} with no provided addresses.",
                    userId,
                    jobId);

                await _jobRepository.UpdateStatusAsync(jobId,
                    "No addresses were provided.");

                return new JobStatus
                {
                    PercentComplete = 0,
                    Status = "No addresses were provided.",
                    Error = true
                };
            }
        }
    }
}
