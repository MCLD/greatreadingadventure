using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Model;
using GRA.Domain.Service.Abstract;
using GRA.Domain.Repository;
using System.Text;

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
            var authResult = await _userRepository.AuthenticateUserAsync(trimmedUsername, password);

            if (!authResult.FoundUser)
            {
                authResult.AuthenticationMessage = $"Could not find username '{trimmedUsername}'";
            }
            else if (!authResult.PasswordIsValid)
            {
                authResult.AuthenticationMessage = "The provided password is incorrect.";
            }
            else
            {
                authResult.PermissionNames
                    = await _roleRepository.GetPermisisonNamesForUserAsync(authResult.User.Id);

                if (authResult.PermissionNames.Contains(Permission.ManageSites.ToString())) {
                    _logger.LogInformation("Site manager {username} authenticated", username);
                }

                if (!authResult.PermissionNames.Contains(Permission.AccessMissionControl.ToString())
                    && !authResult.PermissionNames.Contains(Permission.AccessPerformerRegistration.ToString())
                    && !allowDuringCloseProgram)
                {
                    var userContext = GetUserContext();
                    if (userContext.SiteStage == SiteStage.BeforeRegistration
                        || userContext.SiteStage == SiteStage.AccessClosed)
                    {
                        throw new GraException("The program is not accepting sign-ins at this time.");
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
                _logger.LogError($"User {authUserId} doesn't have permission to reset password for {userIdToReset}.");
                throw new GraException("Permission denied.");
            }
        }

        public async Task ResetPassword(string username, string password, string token)
        {
            string trimmedUsername = username.Trim();
            _passwordValidator.Validate(password);
            var fixedToken = token.Trim().ToLower();

            var user = await _userRepository.GetByUsernameAsync(trimmedUsername);
            if (user == null)
            {
                throw new GraException($"User {trimmedUsername} does not exist.");
            }

            var tokens = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            var validTokens = tokens
                .Where(_ => _.Token == fixedToken)
                .OrderByDescending(_ => _.CreatedBy);
            if (validTokens.Count() > 0)
            {
                if ((validTokens.First().CreatedAt - _dateTimeProvider.Now).TotalHours > 24)
                {
                    throw new GraException($"Token {token} has expired.");
                }

                foreach (var request in tokens)
                {
                    await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
                }

                await _userRepository.SetUserPasswordAsync(user.Id, user.Id, password);
            }
            else
            {
                throw new GraException($"Token {token} is not valid.");
            }
        }

        public async Task GenerateTokenAndEmail(string username, string recoveryUrl)
        {
            string trimmedUsername = username.Trim();
            var user = await _userRepository.GetByUsernameAsync(trimmedUsername);

            if (user == null)
            {
                _logger.LogInformation($"Username '{trimmedUsername}' doesn't exist so can't create a recovery token.");
                throw new GraException($"User '{trimmedUsername}' not found.");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                _logger.LogInformation($"User {user.Id} doesn't have an email address configured so cannot send a recovery token.");
                throw new GraException($"User '{trimmedUsername}' doesn't have an email address configured.");
            }

            // clear any existing tokens
            var existingRequests = await _recoveryTokenRepository.GetByUserIdAsync(user.Id);
            _logger.LogInformation($"Found {existingRequests.Count()} existing recovery tokens for user {user.Id}.");
            foreach (var request in existingRequests)
            {
                await _recoveryTokenRepository.RemoveSaveAsync(-1, request.Id);
            }

            string tokenString = _tokenGenerator.Generate().ToUpper().Trim();

            // insert new token
            var token = await _recoveryTokenRepository.AddSaveAsync(-1, new RecoveryToken
            {
                Token = tokenString.ToLower(),
                UserId = user.Id
            });
            _logger.LogInformation($"Inserted token for user {user.Id}.");

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
        }

        public async Task EmailAllUsernames(string email)
        {
            var site = await _siteRepository.GetByIdAsync(GetCurrentSiteId());

            var lookupEmail = email.Trim();
            var usernames = await _userRepository.GetUserIdAndUsernames(lookupEmail);

            if (usernames == null || usernames.Data.Count() == 0)
            {
                throw new GraException($"There are no usernames associated with email address: '{lookupEmail}'.");
            }

            var sb = new StringBuilder($"{site.Name} has received a request for usernames associated with this email address.");
            var sbH = new StringBuilder($"<p>{site.Name} has received a request for usernames associated with this email address.</p>");
            sbH.AppendLine($"<p>The following usernames are associated with <strong>{lookupEmail}</strong>:<ul>");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"The following usernames are associated with '{lookupEmail}':");
            sb.AppendLine();

            foreach (string username in usernames.Data)
            {
                sb.AppendLine($"- {username}");
                sbH.AppendLine($"<li>{username}</li>");
            }
            sbH.AppendLine("</ul></p>");
            string subject = $"{site.Name} usernames associated with your email address";
            await _emailService.Send(usernames.Id, subject, sb.ToString(), sbH.ToString());
        }
    }
}