using GRA.Domain.Service.Abstract;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MimeKit;
using GRA.Domain.Repository;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using GRA.Domain.Model;
using System;

namespace GRA.Domain.Service
{
    public class EmailService : BaseService<EmailService>
    {
        private readonly IConfigurationRoot _config;
        private readonly IProgramRepository _programRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;
        public EmailService(ILogger<EmailService> logger,
            IConfigurationRoot config,
            IProgramRepository programRepository,
            ISiteRepository siteRepository,
            IUserRepository userRepository) : base(logger)
        {
            _config = Require.IsNotNull(config, nameof(config));
            _programRepository = Require.IsNotNull(programRepository, nameof(programRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
        }

        private bool CanSendMailTo(Site site)
        {
            return !string.IsNullOrEmpty(site.FromEmailAddress)
                && !string.IsNullOrEmpty(site.FromEmailName)
                && !string.IsNullOrEmpty(site.OutgoingMailHost)
                && site.OutgoingMailPort != null;
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
            var message = new MimeMessage();

            if(!CanSendMailTo(site))
            {
                throw new GraException("Sending email is not configured.");
            }

            message.From.Add(new MailboxAddress(site.FromEmailName, site.FromEmailAddress));

            if (!string.IsNullOrWhiteSpace(_config[ConfigurationKey.EmailOverride]))
            {
                message.To.Add(new MailboxAddress(_config[ConfigurationKey.EmailOverride]));
            }
            else
            {
                message.To.Add(new MailboxAddress(user.FullName, user.Email));
            }
            message.Subject = subject;

            var builder = new BodyBuilder();
            builder.TextBody = body;
            if (!string.IsNullOrWhiteSpace(htmlBody))
            {
                builder.HtmlBody = htmlBody;
            }
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // accept any STARTTLS certificate
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // TODO store mail server in site
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
                } catch (Exception ex)
                {
                    _logger.LogError($"Unable to send email: {ex.Message}");
                    throw new GraException("Unable to send email.", ex);
                }
                await client.DisconnectAsync(true);
            }
        }
    }
}
