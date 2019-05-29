using System;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

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
            _config = Require.IsNotNull(config, nameof(config));
            _emailTemplateRepository = emailTemplateRepository
                ?? throw new ArgumentNullException(nameof(emailTemplateRepository));
            _siteRepository = siteRepository
                ?? throw new ArgumentNullException(nameof(siteRepository));
            _userRepository = userRepository
                ?? throw new ArgumentNullException(nameof(userRepository));
        }

        private bool CanSendMailTo(Site site)
        {
            return !string.IsNullOrEmpty(site.FromEmailAddress)
                && !string.IsNullOrEmpty(site.FromEmailName)
                && !string.IsNullOrEmpty(site.OutgoingMailHost);
        }

        public async Task<bool> CanSendMailTo(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var site = await _siteRepository.GetByIdAsync(user.SiteId);

            return CanSendMailTo(site);
        }

        public async Task Send(int userId, string subject, string body, string htmlBody = null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var site = await _siteRepository.GetByIdAsync(user.SiteId);

            if (!string.IsNullOrEmpty(user.Email))
            {
                await SendEmailAsync(site,
                    user.Email,
                    subject,
                    body,
                    htmlBody,
                    user.FullName);
            }
            else
            {
                _logger.LogError("Unable to send email to user {UserId} with subject {Subject}: no email address configured.",
                    userId,
                    subject);
            }
        }

        public async Task SendBulkAsync(User user, int emailTemplateId)
        {
            var site = await _siteRepository.GetByIdAsync(user.SiteId);
            var template = await _emailTemplateRepository.GetByIdAsync(emailTemplateId);

            if (user.SiteId != template.SiteId)
            {
                _logger.LogError("Site ID mismatch: user {UserId} is in site {UserSiteId} and template {TemplateId} is in site {TemplateSiteId}",
                    user.Id,
                    user.SiteId,
                    emailTemplateId,
                    template.SiteId);
                return;
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                await SendEmailAsync(site,
                    user.Email,
                    template.Subject,
                    template.BodyText,
                    template.BodyHtml,
                    emailName: user.FullName,
                    providedFromName: template.FromName,
                    providedFromEmail: template.FromAddress);

                template.EmailsSent++;
                await _emailTemplateRepository.UpdateSaveNoAuditAsync(template);
            }
            else
            {
                _logger.LogError("Unable to send email to user {UserId} for template id {TemplateId}: no email address configured.",
                    user.Id,
                    emailTemplateId);
            }
        }

        public async Task SendEmailToAddressAsync(int siteId, string emailAddress, string subject,
            string body, string htmlBody = null)
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
            if (!CanSendMailTo(site))
            {
                throw new GraException("Sending email is not configured.");
            }

            var message = new MimeMessage();

            string fromName = providedFromName ?? site.FromEmailName;
            string fromEmail = providedFromEmail ?? site.FromEmailAddress;

            message.From.Add(new MailboxAddress(fromName, fromEmail));

            if (!string.IsNullOrWhiteSpace(_config[ConfigurationKey.EmailOverride]))
            {
                message.To.Add(new MailboxAddress(_config[ConfigurationKey.EmailOverride]));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(emailName))
                {
                    message.To.Add(new MailboxAddress(emailName, emailAddress));
                }
                else
                {
                    message.To.Add(new MailboxAddress(emailAddress));
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

            using (var client = new SmtpClient())
            {
                // accept any STARTTLS certificate
                client.ServerCertificateValidationCallback = (_, __, ___, ____) => true;

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
                        "Unable to send email to {emailAddress} with subject {subject}: {Message}",
                        emailAddress,
                        subject,
                        ex.Message);
                    throw new GraException("Unable to send email.", ex);
                }
                await client.DisconnectAsync(true);
            }
        }
    }
}
