using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Model.Utility;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Serilog.Context;
using Stubble.Core.Builders;

namespace GRA.Domain.Service
{
    public class EmailService : BaseService<EmailService>
    {
        private readonly IConfiguration _config;
        private readonly IDirectEmailHistoryRepository _directEmailHistoryRepository;
        private readonly IDirectEmailTemplateRepository _directEmailTemplateRepository;
        private readonly IEmailBaseRepository _emailBaseRepository;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly LanguageService _languageService;
        private readonly ISiteRepository _siteRepository;
        private readonly ISiteSettingRepository _siteSettingRepository;
        private readonly IUserRepository _userRepository;

        public EmailService(ILogger<EmailService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IConfiguration config,
            IDirectEmailHistoryRepository directEmailHistoryRepository,
            IDirectEmailTemplateRepository directEmailTemplateRepository,
            IEmailBaseRepository emailBaseRepository,
            IEmailTemplateRepository emailTemplateRepository,
            ISiteRepository siteRepository,
            ISiteSettingRepository siteSettingRepository,
            IUserRepository userRepository,
            LanguageService languageService) : base(logger, dateTimeProvider)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _directEmailHistoryRepository = directEmailHistoryRepository
                ?? throw new ArgumentNullException(nameof(directEmailHistoryRepository));
            _directEmailTemplateRepository = directEmailTemplateRepository
                ?? throw new ArgumentNullException(nameof(directEmailTemplateRepository));
            _emailBaseRepository = emailBaseRepository
                ?? throw new ArgumentNullException(nameof(emailBaseRepository));
            _emailTemplateRepository = emailTemplateRepository
                ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _siteSettingRepository = siteSettingRepository
                ?? throw new ArgumentNullException(nameof(siteSettingRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
        }

        public async Task<EmailTemplate> GetEmailTemplate(int emailTemplateId)
        {
            return await _emailTemplateRepository.GetByIdAsync(emailTemplateId);
        }

        public async Task SendBulkAsync(User user, int emailTemplateId)
        {
            if (user == null)
            {
                _logger.LogError("Unable to send email empty user for template id {TemplateId}.",
                    emailTemplateId);
                return;
            }

            var site = await _siteRepository.GetByIdAsync(user.SiteId);
            var template = await _emailTemplateRepository.GetByIdAsync(emailTemplateId);

            if (user?.SiteId != template.SiteId)
            {
                _logger.LogError("Site ID mismatch: user {UserId} is in site {UserSiteId} and template {TemplateId} is in site {TemplateSiteId}",
                    user.Id,
                    user.SiteId,
                    emailTemplateId,
                    template.SiteId);
                return;
            }

            var bodyText = template.BodyText.Replace("{{UnsubToken}}",
                    user.UnsubscribeToken, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{{Email}}", user.Email, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{{Name}}", user.FullName, StringComparison.InvariantCultureIgnoreCase);
            var bodyHtml = template.BodyHtml.Replace("{{UnsubToken}}",
                    user.UnsubscribeToken, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{{Email}}", user.Email, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{{Name}}", user.FullName, StringComparison.InvariantCultureIgnoreCase);

            if (!string.IsNullOrEmpty(user.Email))
            {
                await SendEmailAsync(site,
                    user.Email,
                    template.Subject,
                    bodyText,
                    bodyHtml,
                    emailName: user.FullName,
                    providedFromName: template.FromName,
                    providedFromEmail: template.FromAddress);
            }
            else
            {
                _logger.LogError("Unable to send email to user {UserId} for template id {TemplateId}: no email address configured.",
                    user.Id,
                    emailTemplateId);
            }
        }

        public async Task SendBulkAsync(EmailReminder emailReminder,
            int emailTemplateId,
            int siteId)
        {
            if (emailReminder == null)
            {
                _logger.LogError("Unable to send email due to blank configuration for template id {TemplateId}: no email address configured.",
                    emailTemplateId);
                return;
            }

            var site = await _siteRepository.GetByIdAsync(siteId);

            var template = await _emailTemplateRepository.GetByIdAsync(emailTemplateId);

            var bodyText = template.BodyText.Replace("{{Email}}",
                emailReminder.Email,
                StringComparison.InvariantCultureIgnoreCase);
            var bodyHtml = template.BodyHtml.Replace("{{Email}}",
                emailReminder.Email,
                StringComparison.InvariantCultureIgnoreCase);

            if (!string.IsNullOrEmpty(emailReminder.Email))
            {
                await SendEmailAsync(site,
                    emailReminder.Email,
                    template.Subject,
                    bodyText,
                    bodyHtml,
                    providedFromName: template.FromName,
                    providedFromEmail: template.FromAddress);
            }
            else
            {
                _logger.LogError("Unable to send email to address {EmailAddress} for template id {TemplateId}: no email address configured.",
                    emailReminder.Email,
                    emailTemplateId);
            }
        }

        public Task SendBulkTestAsync(string emailTo, int emailTemplateId)
        {
            if (string.IsNullOrEmpty(emailTo))
            {
                throw new ArgumentNullException(nameof(emailTo));
            }

            return SendBulkTestInternalAsync(emailTo, emailTemplateId);
        }

        public async Task<DirectEmailHistory> SendDirectAsync(DirectEmailDetails directEmailDetails)
        {
            if (directEmailDetails == null)
            {
                throw new ArgumentNullException(nameof(directEmailDetails));
            }

            var user = await _userRepository.GetByIdAsync(directEmailDetails.ToUserId
                ?? directEmailDetails.SendingUserId);
            var site = await _siteRepository.GetByIdAsync(user.SiteId);

            var history = new DirectEmailHistory
            {
                CreatedBy = directEmailDetails.SendingUserId,
                FromEmailAddress = site.FromEmailAddress,
                FromName = site.FromEmailName,
                IsBulk = directEmailDetails.IsBulk,
                LanguageId = string.IsNullOrEmpty(user.Culture)
                    ? await _languageService.GetDefaultLanguageIdAsync()
                    : await _languageService.GetLanguageIdAsync(user.Culture),
                OverrideToEmailAddress = string
                    .IsNullOrWhiteSpace(_config[ConfigurationKey.EmailOverride])
                    ? null
                    : _config[ConfigurationKey.EmailOverride],
                ToEmailAddress = user.Email,
                ToName = user.FullName
            };

            if (directEmailDetails.ToUserId.HasValue)
            {
                history.UserId = directEmailDetails.ToUserId.Value;
            }

            var directEmailTemplate = !string.IsNullOrEmpty(directEmailDetails.DirectEmailSystemId)
                ? await _directEmailTemplateRepository
                    .GetWithTextBySystemId(directEmailDetails.DirectEmailSystemId,
                        history.LanguageId)
                : await _directEmailTemplateRepository
                    .GetWithTextByIdAsync(directEmailDetails.DirectEmailTemplateId,
                    history.LanguageId);

            history.EmailBaseId = directEmailTemplate.EmailBaseId;

            var stubble = new StubbleBuilder().Build();

            history.Subject = await stubble
                .RenderAsync(directEmailTemplate.DirectEmailTemplateText.Subject,
                    directEmailDetails.Tags);
            history.BodyText = await stubble
                .RenderAsync(directEmailTemplate.DirectEmailTemplateText.BodyCommonMark,
                    directEmailDetails.Tags);
            history.BodyHtml = CommonMark.CommonMarkConverter.Convert(history.BodyText);

            string preview = await stubble
                .RenderAsync(directEmailTemplate.DirectEmailTemplateText.Preview,
                    directEmailDetails.Tags);
            string title = await stubble
                .RenderAsync(directEmailTemplate.DirectEmailTemplateText.Title,
                    directEmailDetails.Tags);
            string footer = CommonMark.CommonMarkConverter.Convert(await stubble
                .RenderAsync(directEmailTemplate.DirectEmailTemplateText.Footer,
                    directEmailDetails.Tags));

            history = await SendDirectAsync(site,
                history,
                new Dictionary<string, string>
            {
                { "Footer", footer },
                { "Preview", preview },
                { "Title", title },
                { "BodyHtml", history.BodyHtml },
                { "BodyText", history.BodyText }
            });

            if (directEmailDetails.IsBulk)
            {
                if (directEmailDetails.ToUserId.HasValue)
                {
                    history.BodyHtml = null;
                    history.BodyText = null;
                    await _directEmailHistoryRepository.AddSaveNoAuditAsync(history);
                }
                if (!directEmailTemplate.SentBulk)
                {
                    await _directEmailTemplateRepository
                        .UpdateSentBulkAsync(directEmailTemplate.Id);
                }
            }
            else
            {
                await _directEmailHistoryRepository.AddSaveNoAuditAsync(history);
            }

            return history;
        }

        public async Task SendEmailToAddressAsync(int siteId,
        string emailAddress,
        string subject,
        string body,
        string htmlBody = null)
        {
            var site = await _siteRepository.GetByIdAsync(siteId);

            await SendEmailAsync(site,
                emailAddress,
                subject,
                body,
                htmlBody);
        }

        public async Task UpdateSentCount(int emailTemplateId, int additionalMailsSent)
        {
            var template = await _emailTemplateRepository.GetByIdAsync(emailTemplateId);
            template.EmailsSent += additionalMailsSent;
            await _emailTemplateRepository.UpdateSaveNoAuditAsync(template);
        }

        private static bool SiteCanSendMail(Site site)
        {
            return !string.IsNullOrEmpty(site.FromEmailAddress)
                && !string.IsNullOrEmpty(site.FromEmailName)
                && !string.IsNullOrEmpty(site.OutgoingMailHost);
        }

        private async Task SendBulkTestInternalAsync(string emailTo, int emailTemplateId)
        {
            var template = await _emailTemplateRepository.GetByIdAsync(emailTemplateId);

            var bodyText = template.BodyText.Replace("{{Email}}",
                emailTo,
                StringComparison.InvariantCultureIgnoreCase);
            var bodyHtml = template.BodyHtml.Replace("{{Email}}",
                emailTo,
                StringComparison.InvariantCultureIgnoreCase);

            var site = await _siteRepository.GetByIdAsync(template.SiteId);

            await SendEmailAsync(site,
                emailTo,
                template.Subject,
                bodyText,
                bodyHtml,
                providedFromName: template.FromName,
                providedFromEmail: template.FromAddress);
        }

        private async Task<DirectEmailHistory> SendDirectAsync(Site site,
            DirectEmailHistory history,
            IDictionary<string, string> tags)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            var emailBase = await _emailBaseRepository.GetWithTextByIdAsync(history.EmailBaseId,
                history.LanguageId);

            var stubble = new StubbleBuilder().Build();

            using var message = new MimeMessage
            {
                Subject = history.Subject,
                Body = new BodyBuilder
                {
                    TextBody = await stubble
                        .RenderAsync(emailBase.EmailBaseText.TemplateText, tags),
                    HtmlBody = await stubble
                        .RenderAsync(emailBase.EmailBaseText.TemplateHtml,
                            tags,
                            new Stubble.Core.Settings.RenderSettings { SkipHtmlEncoding = true })
                }.ToMessageBody()
            };

            message.From.Add(new MailboxAddress(history.FromName, history.FromEmailAddress));

            if (string.IsNullOrEmpty(history.OverrideToEmailAddress))
            {
                message.To.Add(MailboxAddress.Parse(history.OverrideToEmailAddress));
            }
            else
            {
                message.To.Add(new MailboxAddress(history.ToName, history.ToEmailAddress));
            }

            using var client = new SmtpClient
            {
                // accept any STARTTLS certificate
                ServerCertificateValidationCallback = (_, __, ___, ____) => true
            };

            client.MessageSent += (sender, e) =>
            {
                history.SentResponse = e.Response?.Length > 255
                    ? e.Response[..255]
                    : e.Response;

                history.CreatedAt = _dateTimeProvider.Now;
            };

            client.Timeout = 10 * 1000;  // 10 seconds

            var sendTimer = Stopwatch.StartNew();

            await client.ConnectAsync(site.OutgoingMailHost,
                site.OutgoingMailPort ?? 25,
                false);

            client.AuthenticationMechanisms.Remove("XOAUTH2");

            if (!string.IsNullOrEmpty(site.OutgoingMailLogin)
                && !string.IsNullOrEmpty(site.OutgoingMailPassword))
            {
                client.Authenticate(site.OutgoingMailLogin, site.OutgoingMailPassword);
            }

            using (LogContext.PushProperty("EmailFromAddress", history.FromEmailAddress))
            using (LogContext.PushProperty("EmailFromName", history.FromName))
            using (LogContext.PushProperty("EmailLanguageId", history.LanguageId))
            using (LogContext.PushProperty("EmailSentAt", history.CreatedAt))
            using (LogContext.PushProperty("EmailServer", site.OutgoingMailHost))
            using (LogContext.PushProperty("EmailServerPort", site.OutgoingMailPort))
            using (LogContext.PushProperty("EmailSubject", history.Subject))
            using (LogContext.PushProperty("EmailToAddress", history.ToEmailAddress))
            using (LogContext.PushProperty("EmailToAddressOverride",
                history.OverrideToEmailAddress))
            using (LogContext.PushProperty("EmailToName", history.ToName))
            using (LogContext.PushProperty("EmailToUserId", history.UserId))
            using (LogContext.PushProperty("EmailTriggeredById", history.CreatedBy))
            {
                try
                {
                    await client.SendAsync(message);
                    using (LogContext.PushProperty("EmailServerResponse", history.SentResponse))
                    {
                        _logger.LogInformation("Email sent to {EmailToAddress} with subject {EmailSubject} in {Elapsed} ms",
                        history.OverrideToEmailAddress ?? history.ToEmailAddress,
                        history.Subject,
                        sendTimer.ElapsedMilliseconds);
                    }
                    history.Successful = true;
                }
                catch (MailKit.CommandException ex)
                {
                    using (LogContext.PushProperty("EmailServerResponse", history.SentResponse))
                    {
                        _logger.LogError(ex,
                        "Error sending email to {EmailToAddress} with subject {EmailSubject}: {ErrorMessage} in {Elapsed} ms",
                        history.OverrideToEmailAddress ?? history.ToEmailAddress,
                        history.Subject,
                        sendTimer.ElapsedMilliseconds,
                        ex.Message);
                    }
                }
                finally
                {
                    if (client.IsConnected)
                    {
                        await client.DisconnectAsync(true);
                    }
                }
            }

            if (string.IsNullOrEmpty(history.SentResponse))
            {
                history.SentResponse = "Server provided no response.";
            }

            return history;
        }

        private async Task SendEmailAsync(Site site,
            string emailAddress,
            string subject,
            string body,
            string htmlBody = null,
            string emailName = null,
            string providedFromName = null,
            string providedFromEmail = null)
        {
            if (!SiteCanSendMail(site))
            {
                throw new GraException("Sending email is not configured.");
            }

            using var message = new MimeMessage();

            string fromName = providedFromName ?? site.FromEmailName;
            string fromEmail = providedFromEmail ?? site.FromEmailAddress;

            message.From.Add(new MailboxAddress(fromName, fromEmail));

            if (!string.IsNullOrWhiteSpace(_config[ConfigurationKey.EmailOverride]))
            {
                message.To.Add(MailboxAddress.Parse(_config[ConfigurationKey.EmailOverride]));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(emailName))
                {
                    message.To.Add(new MailboxAddress(emailName, emailAddress));
                }
                else
                {
                    message.To.Add(MailboxAddress.Parse(emailAddress));
                }
            }
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = body
            };

            if (!string.IsNullOrWhiteSpace(htmlBody))
            {
                builder.HtmlBody = htmlBody;
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient
            {
                // accept any STARTTLS certificate
                ServerCertificateValidationCallback = (_, __, ___, ____) => true
            };

            string response = null;

            client.MessageSent += (sender, e) =>
            {
                response = e.Response;
            };

            await client.ConnectAsync(site.OutgoingMailHost,
                site.OutgoingMailPort ?? 25,
                false);

            client.AuthenticationMechanisms.Remove("XOAUTH2");

            if (!string.IsNullOrEmpty(site.OutgoingMailLogin)
                && !string.IsNullOrEmpty(site.OutgoingMailPassword))
            {
                client.Authenticate(site.OutgoingMailLogin, site.OutgoingMailPassword);
            }

            try
            {
                await client.SendAsync(message);
            }
            catch (MailKit.CommandException ex)
            {
                using (LogContext.PushProperty("EmailFromAddress", fromEmail))
                using (LogContext.PushProperty("EmailFromName", fromName))
                using (LogContext.PushProperty("EmailServer", site.OutgoingMailHost))
                using (LogContext.PushProperty("EmailServerPort", site.OutgoingMailPort))
                using (LogContext.PushProperty("EmailSubject", subject))
                using (LogContext.PushProperty("EmailToAddress", emailAddress))
                using (LogContext.PushProperty("EmailToAddressOverride",
                    _config[ConfigurationKey.EmailOverride]))
                using (LogContext.PushProperty("EmailToName", emailName))
                using (LogContext.PushProperty("EmailServerResponse", response))
                {
                    _logger.LogError(ex,
                    "Unable to send email to {EmailAddress} with subject {Subject}: {ErrorMessage}",
                    emailAddress,
                    subject,
                    ex.Message);
                }
                throw new GraException("Unable to send email.", ex);
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
