using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Stubble.Core.Builders;

namespace GRA.Domain.Service
{
    public class EmailService : BaseService<EmailService>
    {
        private readonly IConfiguration _config;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;

        public EmailService(ILogger<EmailService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            IConfiguration config,
            IEmailTemplateRepository emailTemplateRepository,
            ISiteRepository siteRepository,
            IUserRepository userRepository) : base(logger, dateTimeProvider)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _emailTemplateRepository = emailTemplateRepository
                ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
        }

        private static bool SiteCanSendMail(Site site)
        {
            return !string.IsNullOrEmpty(site.FromEmailAddress)
                && !string.IsNullOrEmpty(site.FromEmailName)
                && !string.IsNullOrEmpty(site.OutgoingMailHost);
        }

        public async Task<bool> Send(int userId,
            string subject,
            string body,
            string htmlBody = null,
            IDictionary<string, string> tags = null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var site = await _siteRepository.GetByIdAsync(user.SiteId);

            if (!string.IsNullOrEmpty(user.Email))
            {
                string processedSubject;
                string processedBody;
                string processedHtml;

                if (tags?.Count > 0)
                {
                    var stubble = new StubbleBuilder().Build();
                    processedSubject = await stubble.RenderAsync(subject, tags);
                    processedBody = await stubble.RenderAsync(body, tags);
                    processedHtml = await stubble.RenderAsync(htmlBody, tags);
                }
                else
                {
                    processedSubject = subject;
                    processedBody = body;
                    processedHtml = htmlBody;
                }

                await SendEmailAsync(site,
                    user.Email,
                    processedSubject,
                    processedBody,
                    processedHtml,
                    user.FullName);
                return true;
            }
            else
            {
                _logger.LogError("Unable to send email to user {UserId} with subject {Subject}: no email address configured.",
                    userId,
                    subject);
                return false;
            }
        }

        public async Task<EmailTemplate> GetEmailTemplate(int emailTemplateId)
        {
            return await _emailTemplateRepository.GetByIdAsync(emailTemplateId);
        }

        public async Task UpdateSentCount(int emailTemplateId, int additionalMailsSent)
        {
            var template = await _emailTemplateRepository.GetByIdAsync(emailTemplateId);
            template.EmailsSent += additionalMailsSent;
            await _emailTemplateRepository.UpdateSaveNoAuditAsync(template);
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

            var message = new MimeMessage();

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
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                ServerCertificateValidationCallback = (_, __, ___, ____) => true
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
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
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unable to send email to {EmailAddress} with subject {Subject}: {ErrorMessage}",
                    emailAddress,
                    subject,
                    ex.Message);
                throw new GraException("Unable to send email.", ex);
            }
            await client.DisconnectAsync(true);
        }
    }
}
