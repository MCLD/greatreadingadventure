﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRA.Domain.Model;
using GRA.Domain.Repository;
using GRA.Domain.Service.Abstract;
using Microsoft.Extensions.Logging;

namespace GRA.Domain.Service
{
    public class AuthenticationService : Abstract.BaseUserService<AuthenticationService>
    {
        private readonly GRA.Abstract.ITokenGenerator _tokenGenerator;
        private readonly GRA.Abstract.IPasswordValidator _passwordValidator;
        private readonly IUserRepository _userRepository;
        private readonly IRecoveryTokenRepository _recoveryTokenRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly EmailService _emailService;

        public AuthenticationService(ILogger<AuthenticationService> logger,
            GRA.Abstract.IDateTimeProvider dateTimeProvider,
            GRA.Abstract.ITokenGenerator tokenGenerator,
            GRA.Abstract.IPasswordValidator passwordValidator,
            IUserContextProvider userContextProvider,
            IUserRepository userRepository,
            IRecoveryTokenRepository recoveryTokenRepository,
            IRoleRepository roleRepository,
            ISiteRepository siteRepository,
            EmailService emailService) : base(logger, dateTimeProvider, userContextProvider)
        {
            _tokenGenerator = Require.IsNotNull(tokenGenerator, nameof(tokenGenerator));
            _passwordValidator = Require.IsNotNull(passwordValidator, nameof(passwordValidator));
            _userRepository = Require.IsNotNull(userRepository, nameof(userRepository));
            _recoveryTokenRepository = Require.IsNotNull(recoveryTokenRepository,
                nameof(recoveryTokenRepository));
            _roleRepository = Require.IsNotNull(roleRepository, nameof(roleRepository));
            _siteRepository = Require.IsNotNull(siteRepository, nameof(siteRepository));
            _emailService = Require.IsNotNull(emailService, nameof(emailService));
        }

        public async Task<AuthenticationResult> AuthenticateUserAsync(string username,
            string password, bool allowDuringCloseProgram = false)
        {
            string trimmedUsername = username.Trim();
            var authResult = await _userRepository.AuthenticateUserAsync(
                trimmedUsername,
                password,
                _userContextProvider.GetCurrentCulture().Name);

            if (!authResult.FoundUser)
            {
                authResult.Message = Annotations.Validate.Username;
                authResult.Arguments = new[] { trimmedUsername };
            }
            else if (!authResult.PasswordIsValid)
            {
                authResult.Message = Annotations.Validate.Password;
            }
            else
            {
                authResult.PermissionNames
                    = await _roleRepository.GetPermisisonNamesForUserAsync(authResult.User.Id);

                if (!authResult.PermissionNames.Contains(nameof(Permission.AccessMissionControl))
                    && !authResult.PermissionNames.Contains(nameof(Permission.AccessPerformerRegistration))
                    && !allowDuringCloseProgram)
                {
                    var userContext = GetUserContext();
                    if (userContext.SiteStage == SiteStage.BeforeRegistration
                        || userContext.SiteStage == SiteStage.AccessClosed)
                    {
                        throw new GraException(Annotations.Validate.NotOpenSignins);
                    }
                }
            }
            return authResult;
        }

        public async Task<AuthenticationResult> RevalidateUserAsync(int userId)
        {
            return new AuthenticationResult
            {
                FoundUser = true,
                PasswordIsValid = true,
                PermissionNames = await _roleRepository.GetPermisisonNamesForUserAsync(userId),
                User = await _userRepository.GetByIdAsync(userId)
            };
        }

        public async Task ResetPassword(int userIdToReset, string password)
        {
            _passwordValidator.Validate(password);
            int authUserId = GetClaimId(ClaimType.UserId);
            int activeUserId = GetActiveUserId();
            if (activeUserId == userIdToReset
                || HasPermission(Permission.EditParticipants))
            {
                await _userRepository.SetUserPasswordAsync(authUserId, userIdToReset, password);
            }
            else
            {
                _logger.LogError("User {UserId} doesn't have permission to reset password for {ResetPasswordUserId}.",
                    authUserId,
                    userIdToReset);
                throw new GraException(Annotations.Validate.Permission);
            }
        }

        public async Task<Models.ServiceResult> ResetPassword(string username, string password, string token)
        {
            string trimmedUsername = username.Trim();
            _passwordValidator.Validate(password);
            var fixedToken = token.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByUsernameAsync(trimmedUsername);
            if (user == null)
            {
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.Username,
                    Arguments = new[] { trimmedUsername }
                };
            }

            var tokens = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            var validTokens = tokens
                .Where(_ => _.Token == fixedToken)
                .OrderByDescending(_ => _.CreatedBy);
            if (validTokens.Any())
            {
                if ((validTokens.First().CreatedAt - _dateTimeProvider.Now).TotalHours > 24)
                {
                    return new Models.ServiceResult
                    {
                        Status = Models.ServiceResultStatus.Error,
                        Message = Annotations.Validate.TokenExpired,
                        Arguments = new[] { token }
                    };
                }

                foreach (var request in tokens)
                {
                    await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
                }

                await _userRepository.SetUserPasswordAsync(user.Id, user.Id, password);
            }
            else
            {
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.TokenExpired,
                    Arguments = new[] { token }
                };
            }

            return new Models.ServiceResult(Models.ServiceResultStatus.Success);
        }

        public async Task<Models.ServiceResult> GenerateTokenAndEmail(string username, string recoveryUrl)
        {
            string trimmedUsername = username.Trim();
            var user = await _userRepository.GetByUsernameAsync(trimmedUsername);

            if (user == null)
            {
                _logger.LogInformation("Username {Username} doesn't exist so can't create a recovery token.",
                    trimmedUsername);
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.Username,
                    Arguments = new[] { trimmedUsername }
                };
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogInformation("User {Username} ({UserId}) doesn't have an email address configured so cannot send a recovery token.",
                    user?.Username,
                    user?.Id);
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = Annotations.Validate.EmailConfigured,
                    Arguments = new[] { trimmedUsername }
                };
            }

            // clear any existing tokens
            var existingRequests = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            foreach (var request in existingRequests)
            {
                await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
            }

            string tokenString = _tokenGenerator.Generate().ToUpperInvariant().Trim();

            // insert new token
            await _recoveryTokenRepository.AddSaveAsync(-1, new RecoveryToken
            {
                Token = tokenString.ToLowerInvariant(),
                UserId = user.Id
            });

            _logger.LogInformation("Cleared {Existing} existing recovery tokens and inserted a new one for {Username} ({UserId})",
                existingRequests.Count(),
                user?.Username,
                user.Id);

            var site = await _siteRepository.GetByIdAsync(GetCurrentSiteId());

            string subject = $"{site.Name} password recovery";
            string mailBody = $"{site.Name} has received a request for a password recovery."
                + "\n\rAccess the password recovery page in order to set a new password:"
                + $"\n\r  {recoveryUrl}?username={trimmedUsername}&token={tokenString}"

                + $"\n\r\n\rIf that link does not work work, please visit:"
                + $"\n\r  {recoveryUrl}"
                + $"\n\rand enter the following:"
                + $"\n\r  Username: {trimmedUsername}"
                + $"\n\r  Token: {tokenString}";

            string htmlBody = $"<p>{site.Name} has received a request for a password recovery.</p>"
                + "<p>Access the "
                + $"<a href=\"{recoveryUrl}?username={trimmedUsername}&token={tokenString}\">"
                + "password recovery page</a> in order to set a new password.</p>"
                + "<p>If that link does not work, please visit: "
                + $"<a href=\"{recoveryUrl}\">{recoveryUrl}</a> "
                + "and enter the following:<ul>"
                + $"<li>Username:{trimmedUsername}</li>"
                + $"<li>Token: {tokenString}</li></ul></p>";

            await _emailService.Send(user.Id, subject, mailBody, htmlBody);

            return new Models.ServiceResult(Models.ServiceResultStatus.Success);
        }

        public async Task<Models.ServiceResult> EmailAllUsernames(string email)
        {
            var site = await _siteRepository.GetByIdAsync(GetCurrentSiteId());

            var lookupEmail = email.Trim();
            var usernames = await _userRepository.GetUserIdAndUsernames(lookupEmail);

            if (usernames?.Data.Any() != true)
            {
                return new Models.ServiceResult
                {
                    Status = Models.ServiceResultStatus.Error,
                    Message = "There are no usernames associated with the email address: {0}.",
                    Arguments = new[] { lookupEmail }
                };
            }

            var sb = new StringBuilder($"{site.Name} has received a request for usernames associated with this email address.");
            var sbH = new StringBuilder($"<p>{site.Name} has received a request for usernames associated with this email address.</p>");
            sbH.Append("<p>The following usernames are associated with <strong>")
                .Append(lookupEmail)
                .AppendLine("</strong>:<ul>")
                .AppendLine()
                .AppendLine()
                .Append("The following usernames are associated with '")
                .Append(lookupEmail)
                .AppendLine("':")
                .AppendLine();

            foreach (string username in usernames.Data)
            {
                sb.Append("- ").AppendLine(username);
                sbH.Append("<li>")
                    .Append(username)
                    .AppendLine("</li>");
            }
            sbH.AppendLine("</ul></p>");
            string subject = $"{site.Name} usernames associated with your email address";
            await _emailService.Send(usernames.Id, subject, sb.ToString(), sbH.ToString());
            return new Models.ServiceResult(Models.ServiceResultStatus.Success);
        }
    }
}