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
        private readonly SiteLookupService _siteLookupService;
        private readonly IUserRepository _userRepository;

        public EmailService(ILogger<EmailService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IConfiguration config,
            IDirectEmailHistoryRepository directEmailHistoryRepository,
            IDirectEmailTemplateRepository directEmailTemplateRepository,
            IEmailBaseRepository emailBaseRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IUserRepository userRepository,
            LanguageService languageService,
            SiteLookupService siteLookupService) : base(logger, dateTimeProvider)
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
            _siteLookupService = siteLookupService
                ?? throw new ArgumentNullException(nameof(siteLookupService));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));
        }

        public Task IncrementSentCountAsync(int directEmailTemplateId)
        {
            return IncrementSentCountAsync(directEmailTemplateId, 1);
        }

        public async Task IncrementSentCountAsync(int directEmailTemplateId, int incrementBy)
        {
            if (incrementBy == 0)
            {
                return;
            }

            await _directEmailTemplateRepository
                .IncrementSentCountAsync(directEmailTemplateId, incrementBy);
        }

        public async Task<DirectEmailHistory> SendDirectAsync(DirectEmailDetails directEmailDetails)
        {
            if (directEmailDetails == null)
            {
                throw new ArgumentNullException(nameof(directEmailDetails));
            }

            string toAddress;
            string toName;
            int languageId;
            Site site;

            if (directEmailDetails.ToUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(directEmailDetails.ToUserId
                    ?? directEmailDetails.SendingUserId);
                site = await _siteLookupService.GetByIdAsync(user.SiteId);
                toAddress = user.Email;
                toName = user.FullName;
                languageId = string.IsNullOrEmpty(user.Culture)
                    ? await _languageService.GetDefaultLanguageIdAsync()
                    : await _languageService.GetLanguageIdAsync(user.Culture);
            }
            else
            {
                var user = await _userRepository.GetByIdAsync(directEmailDetails.SendingUserId);
                site = await _siteLookupService.GetByIdAsync(user.SiteId);
                toAddress = directEmailDetails.ToAddress;
                toName = directEmailDetails.ToName;
                languageId = directEmailDetails.LanguageId
                    ?? await _languageService.GetDefaultLanguageIdAsync();
            }

            var history = new DirectEmailHistory
            {
                CreatedBy = directEmailDetails.SendingUserId,
                FromEmailAddress = site.FromEmailAddress,
                FromName = site.FromEmailName,
                IsBulk = directEmailDetails.IsBulk,
                LanguageId = languageId,
                OverrideToEmailAddress = string
                    .IsNullOrWhiteSpace(_config[ConfigurationKey.EmailOverride])
                    ? null
                    : _config[ConfigurationKey.EmailOverride],
                ToEmailAddress = toAddress,
                ToName = toName
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

            if (directEmailDetails.IsBulk && !directEmailDetails.IsTest)
            {
                if (directEmailDetails.ToUserId.HasValue)
                {
                    history.BodyHtml = null;
                    history.BodyText = null;
                    await _directEmailHistoryRepository.AddSaveNoAuditAsync(history);
                    await IncrementSentCountAsync(directEmailTemplate.Id);
                }
                if (!directEmailTemplate.SentBulk)
                {
                    await _directEmailTemplateRepository
                        .UpdateSentBulkAsync(directEmailTemplate.Id);
                }
            }
            else
            {
                if (!directEmailDetails.IsTest)
                {
                    await _directEmailHistoryRepository.AddSaveNoAuditAsync(history);
                    await IncrementSentCountAsync(directEmailTemplate.Id);
                }
            }

            return history;
        }

        private static bool SiteCanSendMail(Site site)
        {
            return !string.IsNullOrEmpty(site.FromEmailAddress)
                && !string.IsNullOrEmpty(site.FromEmailName)
                && !string.IsNullOrEmpty(site.OutgoingMailHost);
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

            if (emailBase.EmailBaseText == null)
            {
                emailBase = await _emailBaseRepository.GetWithTextByIdAsync(history.EmailBaseId,
                    await _languageService.GetDefaultLanguageIdAsync());
            }

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

            if (!string.IsNullOrEmpty(history.OverrideToEmailAddress))
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
    }
}
